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
			[DebuggerDisplay("{name}")]
			protected struct Prop
			{
				public string name;
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

			public Reflected(Type type) : this(type.Name, type) { }
			public Reflected(string name, Type type) : base(name, type)
			{
				foreach (var member in GetMembers(type, null, false))
					ProcessMember(member, false, ref sdict);
				foreach (var member in GetMembers(type, null, true))
					ProcessMember(member, true, ref idict);
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
						intIndexSet = (obj, idx, v) => ((IList)obj)[idx] = v.Object;
					}
				}
			}

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (args.Count == 0)
				{
					result = new Value(Activator.CreateInstance(Type));
					return true;
				}
				return false;
			}

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
						//TODO: multi-index
						return true;
					}
					if (index.IsString)
					{
						if (strIndexGet == null)
							throw InvalidOperation("{0}[{1}] is write only", Name, proxy[1]);
						self = strIndexGet(proxy[0].obj, index.obj.ToString());
						//TODO: multi-index
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
					//TODO: index-set
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
			public override IEnumerator<Value> Enumerate(ref Value self)
			{
				var it = self.obj;
				if (it is IEnumerable<Value> ev)
					return ev.GetEnumerator();
				if (it is IEnumerable eo)
					return EnumerateNative(eo);
				return null;
			}
			private IEnumerator<Value> EnumerateNative(IEnumerable e)
			{
				foreach (var v in e)
					yield return new Value(e);
			}
		}
	}
}
