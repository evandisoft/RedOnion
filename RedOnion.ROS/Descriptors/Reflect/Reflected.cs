using RedOnion.Attributes;
using RedOnion.Collections;
using RedOnion.Debugging;
using RedOnion.ROS.Objects;
using RedOnion.ROS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace RedOnion.ROS
{
	partial class Descriptor
	{
		public partial class Reflected : Descriptor
		{
			public static bool LowerFirstLetter = true;

			[DebuggerDisplay("{DebugString}")]
			public struct Prop
			{
				/// <summary>
				/// Name of the property (after mangling/DisplayName and conflict resolution)
				/// </summary>
				public string name;
				/// <summary>
				/// Index of next property with conflicting name
				/// (search the chain to find exact match or use the first).
				/// Note that `next` is either higher than current index (always growing)
				/// or shall be considered terminator of the chain
				/// (thus zero and negative numbers are terminators).
				/// Also note that static and instance chains do not overlap
				/// (static members are shadowed by instance members, not merged).
				/// </summary>
				public int next;
				/// <summary>
				/// Used to implement <see cref="Reflected.Get(ref Value)"/>
				/// </summary>
				public Func<object, Value> read;
				/// <summary>
				/// Used to implement <see cref="Reflected.Set(ref Value, OpCode, ref Value)"/>
				/// </summary>
				public Action<object, Value> write;

				// The following may not be necessary after the descriptor is created
				// and may be moved to some temporary (build time only) list in the future

				/// <summary>
				/// Original name of the property (used only for conflict resolution)
				/// </summary>
				public string strict;
				/// <summary>
				/// Type/kind/class of property (used either to upgrade methods to group or for <see cref="CallableAttribute"/> implementation).
				/// </summary>
				public Kind kind;
				public enum Kind
				{
					Unknown,
					Type,
					Field,
					Property,
					Event,
					Method,
					MethodGroup
				}
				public Flag flags;
				[Flags]
				public enum Flag
				{
					Instance = 1<<0,
				}
				public bool instance
				{
					get => (flags & Flag.Instance) != 0;
					set
					{
						if (value) flags |= Flag.Instance;
						else flags &=~Flag.Instance;
					}
				}

				public override string ToString()
					=> name;
				private string DebugString
					=> $"{name} ({strict}; {next}; {(instance ? "Instance" : "Static")} {kind})";
			}
			/// <summary>List of all properties (for enumeration and indexing given by <see cref="sdict"/> or <see cref="idict"/></summary>
			protected ListCore<Prop> prop;
			/// <summary>Map of (non-conflicting) static properties (case-insensitive, name-to-index in <see cref="prop"/>)</summary>
			protected readonly Dictionary<string, int> sdict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			/// <summary>Map of (non-conflicting) instance properties (case-insensitive, name-to-index in <see cref="prop"/>)</summary>
			protected readonly Dictionary<string, int> idict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			/// <summary>Map of all static properties (case-sensitive, name-to-index in <see cref="prop"/>)</summary>
			protected readonly Dictionary<string, int> scase = new Dictionary<string, int>();
			/// <summary>Map of all instance properties (case-sensitive, name-to-index in <see cref="prop"/>)</summary>
			protected readonly Dictionary<string, int> icase = new Dictionary<string, int>();

			/// <summary>Default constructor (if found)</summary>
			protected ConstructorInfo defaultCtor;
			/// <summary>Default constructor converted to action (done after discovery of final <see cref="defaultCtor"/>)</summary>
			protected Func<object> defaultConstruct;

			/// <summary>Reflected this[int].get (if present and public)</summary>
			protected Func<object, int, Value> intIndexGet;
			/// <summary>Reflected this[int].set (if present and public)</summary>
			protected Action<object, int, Value> intIndexSet;
			/// <summary>Reflected this[string].get (if present and public)</summary>
			protected Func<object, string, Value> strIndexGet;
			/// <summary>Reflected this[string].set (if present and public)</summary>
			protected Action<object, string, Value> strIndexSet;
			/// <summary>Reflected this[Value].get (if present and public)</summary>
			protected Func<object, Value, Value> valIndexGet;
			/// <summary>Reflected this[Value].set (if present and public)</summary>
			protected Action<object, Value, Value> valIndexSet;

			//TODO: store index to props instead
			protected string callableMemberName;
			//TODO: implement all operators in similar way (this is for "op_Implicit" only)
			protected ListCore<KeyValuePair<Type, Func<object, object>>> implConvert;

			public Reflected(Type type) : this(type.Name, type) { }
			public Reflected(string name, Type type) : base(name, type)
			{
				callableMemberName = type.GetCustomAttribute<CallableAttribute>()?.Name;

				// process static members and nested types
				foreach (var member in GetMembers(type, null, false))
				{
					try
					{
						ProcessMember(member, false);
					}
					catch (Exception ex)
					{
						MainLogger.Log("Exception {0} when processing static {1}.{2}: {3}",
							ex.GetType(), Type.Name, member.Name, ex.Message);
					}
				}
				// process instance members
				foreach (var member in GetMembers(type, null, true))
				{
					try
					{
						ProcessMember(member, true);
					}
					catch (Exception ex)
					{
						MainLogger.Log("Exception {0} when processing {1}.{2}: {3}",
							ex.GetType(), Type.Name, member.Name, ex.Message);
					}
				}
				// add all non-conflicting static/nested to instance members
				foreach (var pair in sdict)
				{
					if (!idict.ContainsKey(pair.Key))
						idict[pair.Key] = pair.Value;
				}
				// fill case-sensitive dictionaries (warning: the key may not match the selected casing)
				foreach (var pair in sdict)
				{
					ref var p = ref prop.items[pair.Value];
					scase[p.name] = pair.Value;
					int curr = pair.Value, next = p.next;
					while (next > curr)
					{
						p = ref prop.items[curr = next];
						scase[p.name] = curr;
						next = p.next;
					}
				}
				foreach (var pair in idict)
				{
					ref var p = ref prop.items[pair.Value];
					icase[p.name] = pair.Value;
					int curr = pair.Value, next = p.next;
					while (next > curr)
					{
						p = ref prop.items[curr = next];
						icase[p.name] = curr;
						next = p.next;
					}
				}
				// prepare default constructor
				if (defaultCtor != null)
				{
					try
					{
						var pars = defaultCtor.GetParameters();
						var args = new Expression[pars.Length];
						for (int i = 0; i < args.Length; i++)
						{
							var par = pars[i];
							var v = par.DefaultValue;
							// value types (e.g. int or enum) signal `default` using `DefaultValue == null`
							if (v == null && par.ParameterType.IsValueType)
								args[0] = Expression.Default(par.ParameterType);
							// check type before using constants
							else if (v == null || par.ParameterType.IsAssignableFrom(v.GetType()))
								args[0] = Expression.Constant(v);
							// convert
							else args[0] = Expression.Convert(Expression.Constant(v), par.ParameterType);
						}
						defaultConstruct = Expression.Lambda<Func<object>>(
							Expression.New(defaultCtor, args)).Compile();
					}
					catch (Exception ex)
					{
						MainLogger.DebugLog("Exception {0} when processing defaultCtor of {1}: {2}",
							ex.GetType(), Type.Name, ex.Message);
					}
				}
				if (defaultConstruct == null)
				{
					try
					{
						if (type.IsValueType)
						{
							defaultConstruct = Expression.Lambda<Func<object>>(
								Expression.Convert(Expression.Default(type), typeof(object))).Compile();
						}
						else
						{
							var construct = Constructor.MakeGenericMethod(type);
							defaultConstruct = (Func<object>)construct.CreateDelegate(typeof(Func<object>), construct);
						}
					}
					catch (Exception ex)
					{
						// this is quite normal, the MakeGenericMethod fails if its `where T:new()` is not satisfied
						MainLogger.DebugLog("Type {0} does not seem to be default constructible - {1}: {2}",
							Type.Name, ex.GetType(), ex.Message);
					}
				}
				// add int-indexers from list interface if none found yet
				if (intIndexGet == null && intIndexSet == null)
				{
					if (typeof(IList<Value>).IsAssignableFrom(type))
					{
						intIndexGet = (obj, idx) => ((IList<Value>)obj)[idx];
						intIndexSet = (obj, idx, v) => ((IList<Value>)obj)[idx] = v;
					}
					else if (typeof(IList).IsAssignableFrom(type))
					{
						intIndexGet = (obj, idx) => new Value(((IList)obj)[idx]);
						intIndexSet = (obj, idx, v) => ((IList)obj)[idx] = v.Box();
					}
				}
			}

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (result.obj is ICallable call)
					return call.Call(ref result, self, args, create);
				if (!create)
				{
					if (callableMemberName?.Length > 0)
					{
						int at = Find(result.obj, callableMemberName);
						if (at < 0)
							throw new InvalidOperation("{0}.{1} does not exist", Name, callableMemberName);
						ref var it = ref prop.items[at];
						if (it.read == null)
							throw new InvalidOperation("{0}.{1} is not readable", Name, callableMemberName);
						self = result.obj;
						result = it.read(result.obj);
						switch (it.kind)
						{
						case Prop.Kind.Field:
						case Prop.Kind.Property:
							return true;
						case Prop.Kind.Method:
						case Prop.Kind.MethodGroup:
							return result.desc.Call(ref result, self, args);
						}
						throw new InvalidOperation("{0}.{1} is of type {2} which is not callable", Name, callableMemberName, it.kind);
					}
					if (result.obj != null && !(result.obj is Type))
						return false;
				}
				if (args.Count == 0)
				{
					if (defaultConstruct != null)
					{
						result = new Value(this, defaultConstruct());
						return true;
					}
					// this is kept as fallback for situations we could not create defaultConstruct from defaultCtor
					// (which may no longer be required since we updated the creation of defaultConstruct using Expression.Default)
					if (defaultCtor != null
						&& Callable.TryCall(defaultCtor, ref result, self, args))
						return true;
					// try this last (unlikely to succeed but... another fallback to generic solution)
					result = new Value(Activator.CreateInstance(Type));
					return true;
				}
				//TODO: optimize (Delegate[][] - first index by number of arguments)
				foreach (var ctor in Type.GetConstructors())
				{
					if (Callable.TryCall(ctor, ref result, self, args))
						return true;
				}
				return false;
			}

			//TODO: reflect operators
			public override void Unary(ref Value self, OpCode op)
			{
				if (self.obj is IOperators ops
					&& ops.Unary(ref self, op))
					return;
				UnaryError(op);
			}
			public override bool Binary(ref Value lhs, OpCode op, ref Value rhs)
			{
				if (lhs.obj is IOperators ops && ops.Binary(ref lhs, op, ref rhs))
					return true;
				if (rhs.obj is IOperators ops2 && ops2.Binary(ref lhs, op, ref rhs))
					return true;
				if (lhs.desc != this)
					return false;
				switch (op)
				{
				case OpCode.Equals:
					lhs = lhs.obj?.Equals(rhs.Box()) == true;
					return true;
				case OpCode.Differ:
					lhs = lhs.obj?.Equals(rhs.Box()) != true;
					return true;
				}
				return false;
			}
			public override bool Convert(ref Value self, Descriptor to)
			{
				if (self.obj is IConvert cvt && cvt.Convert(ref self, to))
					return true;
				foreach (var mtd in implConvert)
				{
					if (to.Type == mtd.Key)
					{
						self = new Value(mtd.Value(self.Box()));
						return true;
					}
				}
				return base.Convert(ref self, to);
			}

			protected int Find(object self, string name)
				=> (self is Type ? sdict : idict).TryGetValue(name, out var idx) ? idx : -1;
			protected int FindStrict(object self, string name)
				=> (self is Type ? scase : icase).TryGetValue(name, out var idx) ? idx : -1;
			public override bool Has(ref Value self, string name)
				=> Find(self.obj, name) >= 0;
			public override void Get(ref Value self)
			{
				if (self.idx is string name)
				{
					int at = Find(self.obj, name);
					if (at < 0) goto fail;
					ref var p = ref prop.items[at];
					if (p.next > at)
					{// resolve case conflict
						int at2 = FindStrict(self.obj, name);
						if (at2 >= 0)
						{
							at = at2;
							p = ref prop.items[at];
						}
					}
					var read = p.read;
					if (read == null) goto fail;
					self = read(self.obj);
					return;
				}
				if (self.IsIntIndex)
				{
					if (intIndexGet == null)
						goto fail;
					self = intIndexGet(self.obj, self.num.Int);
					return;
				}
				if (self.idx is ValueBox box)
				{
					if (box.Value.IsStringOrChar && strIndexGet != null)
					{
						self = strIndexGet(self.obj, box.Value.obj.ToString());
						ValueBox.Return(box);
						return;
					}
					if (box.Value.IsNumberOrChar && intIndexGet != null)
					{
						self = intIndexGet(self.obj, box.Value.num.Int);
						ValueBox.Return(box);
						return;
					}
					if (valIndexGet != null)
					{
						self = valIndexGet(self.obj, box.Value);
						return;
					}
				}
			fail:
				GetError(ref self);
			}

			public override void Set(ref Value self, OpCode op, ref Value value)
			{
				if (self.idx is string name)
				{
					int at = Find(self.obj, name);
					if (at < 0) goto fail;
					ref var p = ref prop.items[at];
					if (p.next > at)
					{// resolve case conflict
						int at2 = FindStrict(self.obj, name);
						if (at2 >= 0)
						{
							at = at2;
							p = ref prop.items[at];
						}
					}
					var write = p.write;
					if (write == null)
					{
						if (p.kind != Prop.Kind.Event)
							goto fail;
						if (op != OpCode.AddAssign && op != OpCode.SubAssign)
							goto fail;
						var evt = (IEventProxy)p.read(self.obj).obj;
						self = value;
						if (op == OpCode.AddAssign ? evt.Add(ref value) : evt.Remove(ref value))
							return;
						goto fail;
					}
					if (op == OpCode.Assign)
					{
						write(self.obj, value);
						return;
					}
					var read = p.read;
					if (read == null) goto fail;
					var it = read(self.obj);
					if (op.Kind() == OpKind.Assign)
					{
						if (!it.desc.Binary(ref it, op + 0x10, ref value)
							&& !value.desc.Binary(ref it, op + 0x10, ref value))
							goto fail;
						write(self.obj, it);
						return;
					}
					if (op.Kind() != OpKind.PreOrPost)
						goto fail;
					if (op >= OpCode.Inc)
					{
						it.desc.Unary(ref it, op);
						write(self.obj, it);
						return;
					}
					var tmp = it;
					it.desc.Unary(ref it, op + 0x08);
					write(self.obj, it);
					self = tmp;
					return;
				}
				if (op != OpCode.Assign)
					goto fail;
				if (self.IsIntIndex)
				{
					if (intIndexSet == null)
						goto fail;
					intIndexSet(self.obj, self.num.Int, value);
					return;
				}
				if (self.idx is ValueBox box)
				{
					if (box.Value.IsStringOrChar && strIndexSet != null)
					{
						strIndexSet(self.obj, box.Value.obj.ToString(), value);
						return;
					}
					if (box.Value.IsNumberOrChar && intIndexSet != null)
					{
						intIndexSet(self.obj, box.Value.num.Int, value);
						return;
					}
					if (valIndexSet != null)
					{
						valIndexSet(self.obj, box.Value, value);
						return;
					}
				}
			fail:
				GetError(ref self);
			}
			public override IEnumerable<Value> Enumerate(object self)
			{
				if (self is IEnumerable<Value> ev)
					return ev;
				if (self is IEnumerable eo)
					return EnumerateNative(eo);
				return null;
			}
			private IEnumerable<Value> EnumerateNative(IEnumerable e)
			{
				foreach (var v in e)
					yield return new Value(v);
			}
			public override IEnumerable<string> EnumerateProperties(object self)
				=> self is Type ? scase.Keys : icase.Keys;
		}
	}
}
