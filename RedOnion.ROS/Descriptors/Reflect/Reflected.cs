using RedOnion.ROS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
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

			[DebuggerDisplay("{name}")]
			protected struct Prop
			{
				public enum Kind
				{
					Unknown,
					Type,
					Field,
					Property,
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
			protected ConstructorInfo processorCtor;

			public Reflected(Type type) : this(type.Name, type) { }
			public Reflected(string name, Type type) : base(name, type)
			{
				//TODO: ignore members with [Browsable(false)]
				foreach (var member in GetMembers(type, null, false))
				{
					try
					{
						ProcessMember(member, false, ref sdict);
					}
					catch (Exception ex)
					{
						Value.Log("Exception {0} when processing static {1}.{2}: {3}",
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
						Value.Log("Exception {0} when processing {1}.{2}: {3}",
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
						Value.Log("Exception {0} when processing {1}.{2}: {3}",
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
				Value.DebugLog("{0}/{1}.{2} {3}", Name, Type.FullName, create ? "Create" : "Call", result);
				if (result.obj is ICallable call)
					return call.Call(ref result, self, args, create);
				if (!create && result.obj != null)
					return false;
				if (args.Count == 0)
				{
					if (processorCtor != null
						&& Callable.TryCall(processorCtor, ref result, self, args))
						return true;
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
				=> lhs.obj is IOperators ops ? ops.Binary(ref lhs, op, ref rhs)
				: rhs.obj is IOperators ops2 ? ops2.Binary(ref lhs, op, ref rhs)
				: false;
			public override bool Convert(ref Value self, Descriptor to)
				=> self.obj is IConvert cvt && cvt.Convert(ref self, to)
				|| base.Convert(ref self, to);

			public override int Find(object self, string name, bool add)
			{
				if (self != null && idict != null && idict.TryGetValue(name, out var idx))
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
				if (index.IsNumerOrChar)
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
				if (!index.desc.Convert(ref index, String))
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
				var name = index.obj.ToString();
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
					if (index.IsNumerOrChar)
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
			public override bool Set(ref Value self, int at, OpCode op, ref Value value)
			{
				if (at == int.MaxValue)
				{
					if (op != OpCode.Assign)
						return false;
					var proxy = (Value[])self.obj;
					ref var index = ref proxy[1];
					if (index.IsNumerOrChar)
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
				if (op == OpCode.Assign)
				{
					var write = prop.items[at].write;
					if (write == null) return false;
					write(self.obj, value);
					return true;
				}
				//TODO: other operations
				return false;
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
					yield return new Value(e);
			}
			public override IEnumerable<string> EnumerateProperties(object self)
			{
				foreach (var p in prop)
					yield return p.name;
			}
		}
	}
}
