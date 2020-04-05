using RedOnion.Attributes;
using RedOnion.Collections;
using RedOnion.Debugging;
using RedOnion.ROS.Objects;
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
						int at = Find(result.obj, callableMemberName, false);
						if (at < 0)
							throw InvalidOperation("{0}.{1} does not exist", Name, callableMemberName);
						ref var it = ref prop.items[at];
						if (it.read == null)
							throw InvalidOperation("{0}.{1} is not readable", Name, callableMemberName);
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
						throw InvalidOperation("{0}.{1} is of type {2} which is not callable", Name, callableMemberName, it.kind);
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
			public override bool Unary(ref Value self, OpCode op)
				=> self.obj is IOperators ops ? ops.Unary(ref self, op) : false;
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

			public override int Find(object self, string name, bool add)
			{
				if (self != null && idict != null && !(self is Type)
					&& idict.TryGetValue(name, out var idx))
					return idx;
				if (sdict != null && sdict.TryGetValue(name, out idx))
					return idx;
				return -1;
			}
			public override int IndexFind(ref Value self, Arguments args)
			{
				if (args.Length == 0)
					return -1;
				ref var index = ref args.GetRef(0);
				int at;
				if (index.IsNumberOrChar)
				{
					if (intIndexGet == null && intIndexSet == null)
						return -1;
					var proxy = new Value[1 + args.Length];
					proxy[0] = self;
					for (int i = 1; i < proxy.Length; i++)
						proxy[i] = args[i-1];
					self.obj = proxy;
					return int.MaxValue; // complex indexing
				}
				var strIndex = index;
				if (!strIndex.desc.Convert(ref strIndex, String))
					return -1;
				if (strIndexGet != null || strIndexSet != null)
				{
					var proxy = new Value[1 + args.Length];
					proxy[0] = self;
					for (int i = 1; i < proxy.Length; i++)
						proxy[i] = args[i-1];
					self.obj = proxy;
					return int.MaxValue; // complex indexing
				}
				var name = strIndex.obj.ToString();
				at = Find(self.obj, name, true);
				if (at < 0 || args.Length == 1)
					return at;
				if (!Get(ref self, at))
					return -1;
				return self.desc.IndexFind(ref self, new Arguments(args, args.Length-1));
			}
			public override string NameOf(object self, int at)
				=> at < 0 || at >= prop.size ? "[?]" : prop[at].name;
			public override bool Get(ref Value self, int at)
			{
				if (at == int.MaxValue)
				{
					var proxy = (Value[])self.obj;
					ref var index = ref proxy[1];
					if (index.IsNumberOrChar)
					{
						if (intIndexGet == null)
							throw InvalidOperation("{0}[{1}] is write only", Name, proxy[1]);
						self = intIndexGet(proxy[0].obj, index.num.Int);
						return true;
					}
					if (index.IsString)
					{
						if (strIndexGet == null)
							throw InvalidOperation("{0}[{1}] is write only", Name, proxy[1]);
						self = strIndexGet(proxy[0].obj, index.obj.ToString());
						return true;
					}
					return false;
				}
				if (at < 0 || at >= prop.size)
					return false;
				var read = prop.items[at].read;
				if (read == null) return false;
				self = read(self.obj);
				return true;
			}
#if DEBUG
			public override bool Set(ref Value self, int at, OpCode op, ref Value value)
			{
				try
				{
					return InternalSet(ref self, at, op, ref value);
				}
				catch
				{
					MainLogger.DebugLog($"Exception in {Name}.Set");
					MainLogger.DebugLog($"Self: {self}; at: {at}; op: {op}; value: {value}; prop: {NameOf(this, at)}");
					throw;
				}
			}
			private bool InternalSet(ref Value self, int at, OpCode op, ref Value value)
#else
			public override bool Set(ref Value self, int at, OpCode op, ref Value value)
#endif
			{
				if (at == int.MaxValue)
				{
					if (op != OpCode.Assign)
						return false;
					var proxy = (Value[])self.obj;
					ref var index = ref proxy[1];
					if (index.IsNumberOrChar)
					{
						if (intIndexSet == null)
							throw InvalidOperation("{0}[{1}] is read only", Name, proxy[1]);
						intIndexSet(proxy[0].obj, index.num.Int, value);
						return true;
					}
					if (index.IsString)
					{
						if (strIndexGet == null)
							throw InvalidOperation("{0}[{1}] is read only", Name, proxy[1]);
						strIndexSet(proxy[0].obj, index.obj.ToString(), value);
						return true;
					}
					return false;
				}
				if (at < 0 || at >= prop.size)
					return false;
				ref Prop p = ref prop.items[at];
				var write = p.write;
				if (write == null)
				{
					if (p.kind != Prop.Kind.Event)
						return false;
					if (op != OpCode.AddAssign && op != OpCode.SubAssign)
						return false;
					var evt = (IEventProxy)p.read(self.obj).obj;
					self = value;
					return op == OpCode.AddAssign ? evt.Add(ref value) : evt.Remove(ref value);
				}
				if (op == OpCode.Assign)
				{
					write(self.obj, value);
					return true;
				}
				var read = p.read;
				if (read == null) return false;
				var it = read(self.obj);
				if (op.Kind() == OpKind.Assign)
				{
					if (!it.desc.Binary(ref it, op + 0x10, ref value)
						&& !value.desc.Binary(ref it, op + 0x10, ref value))
						return false;
					write(self.obj, it);
					return true;
				}
				if (op.Kind() != OpKind.PreOrPost)
					return false;
				if (op >= OpCode.Inc)
				{
					if (!it.desc.Unary(ref it, op))
						return false;
					write(self.obj, it);
					return true;
				}
				var tmp = it;
				if (!it.desc.Unary(ref it, op + 0x08))
					return false;
				write(self.obj, it);
				self = tmp;
				return true;
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
