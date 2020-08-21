using RedOnion.Attributes;
using RedOnion.Collections;
using RedOnion.Debugging;
using RedOnion.ROS.Objects;
using RedOnion.ROS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

// TODO: make it case semi-sensitive = prefer insensitive (present as camelCase),
// but use the case on collisions (requiring exact match to use the non-default).

namespace RedOnion.ROS
{
	partial class Descriptor
	{
		public partial class Reflected : Descriptor
		{
			public static bool LowerFirstLetter = true;

			[DebuggerDisplay("{name}")]
			public struct Prop
			{
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
				public string name;
				public Kind kind;
				public Func<object, Value> read;
				public Action<object, Value> write;
				public override string ToString()
					=> name;
			}
			protected ListCore<Prop> prop;
			protected Dictionary<string, int> sdict;
			protected Dictionary<string, int> idict;
			protected Func<object, int, Value> intIndexGet;
			protected Action<object, int, Value> intIndexSet;
			protected Func<object, string, Value> strIndexGet;
			protected Action<object, string, Value> strIndexSet;
			protected Func<object, Value, Value> valIndexGet;
			protected Action<object, Value, Value> valIndexSet;
			protected ConstructorInfo defaultCtor;
			protected string callableMemberName;
			protected ListCore<KeyValuePair<Type, Func<object, object>>> implConvert;

			public Reflected(Type type) : this(type.Name, type) { }
			public Reflected(string name, Type type) : base(name, type)
			{
				callableMemberName = type.GetCustomAttribute<CallableAttribute>()?.Name;
				foreach (var member in GetMembers(type, null, false))
				{
					try
					{
						ProcessMember(member, false, ref sdict);
					}
					catch (Exception ex)
					{
						MainLogger.Log("Exception {0} when processing static {1}.{2}: {3}",
							ex.GetType(), Type.Name, member.Name, ex.Message);
					}
				}
				foreach (var member in GetMembers(type, null, true))
				{
					try
					{
						ProcessMember(member, true, ref idict);
					}
					catch (Exception ex)
					{
						MainLogger.Log("Exception {0} when processing {1}.{2}: {3}",
							ex.GetType(), Type.Name, member.Name, ex.Message);
					}
				}
				foreach (var nested in type.GetNestedTypes())
				{
					try
					{
						ProcessNested(nested);
					}
					catch (Exception ex)
					{
						MainLogger.Log("Exception {0} when processing {1}.{2}: {3}",
							ex.GetType(), Type.Name, nested.Name, ex.Message);
					}
				}
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
					if (defaultCtor != null
						&& Callable.TryCall(defaultCtor, ref result, self, args))
						return true;
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
			{
				if (self != null && idict != null && !(self is Type)
					&& idict.TryGetValue(name, out var idx))
					return idx;
				if (sdict != null && sdict.TryGetValue(name, out idx))
					return idx;
				return -1;
			}
			public override bool Has(ref Value self, string name)
				=> Find(self.obj, name) >= 0;
			public override void Get(ref Value self)
			{
				if (self.idx is string name)
				{
					int at = Find(self.obj, name);
					if (at < 0) goto fail;
					var read = prop.items[at].read;
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
					ref Prop p = ref prop.items[at];
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
			{
				// static only
				if (self == null || self is Type)
				{
					if (sdict != null)
					{
						foreach (var name in sdict.Keys)
							yield return name;
					}
					yield break;
				}
				foreach (var p in prop)
					yield return p.name;
			}
		}
	}
}
