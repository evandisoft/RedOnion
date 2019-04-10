using RedOnion.ROS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public class Reflected : Descriptor
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
			protected Action<object, int, Value> strIndexSet;

			public class MemberComparer : IComparer<MemberInfo>
			{
				public static MemberComparer Instance { get; } = new MemberComparer();
				public int Compare(MemberInfo x, MemberInfo y)
				{
					// BIG letters first to prefer properties over fields
					int cmp = string.CompareOrdinal(x.Name, y.Name);
					// sorting by MetadataToken ensures stability
					// (to avoid problems with reflection cache)
					return cmp != 0 ? cmp
						: x.MetadataToken.CompareTo(y.MetadataToken);
				}
			}
			public static MemberInfo[] GetMembers(Type type, string name, bool instance)
			{
				var flags = BindingFlags.IgnoreCase|BindingFlags.Public
				| (instance ? BindingFlags.Instance : BindingFlags.Static);
				var members = name == null
				? type.GetMembers(flags)
				: type.GetMember(name, flags);
				if (members.Length > 1)
					Array.Sort(members, MemberComparer.Instance);
				return members;
			}

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

			protected static Dictionary<Type, ConstructorInfo>
				PrimitiveValueConstructors = GetPrimitiveValueConstructors();
			private static Dictionary<Type, ConstructorInfo> GetPrimitiveValueConstructors()
			{
				var dict = new Dictionary<Type,ConstructorInfo>();
				foreach (var ctor in typeof(Value).GetConstructors())
				{
					var args = ctor.GetParameters();
					if (args.Length != 1)
						continue;
					var type = args[0].ParameterType;
					if (!type.IsPrimitive)
						continue;
					dict[type] = ctor;
				}
				return dict;
			}
			protected static ConstructorInfo DefaultValueConstructor
				= typeof(Value).GetConstructor(new Type[] { typeof(object) });
			protected static ConstructorInfo IntValueConstructor
				= typeof(Value).GetConstructor(new Type[] { typeof(int) });
			protected static ConstructorInfo StrValueConstructor
				= typeof(Value).GetConstructor(new Type[] { typeof(string) });

			protected virtual void ProcessMember(
				MemberInfo member, bool instance, ref Dictionary<string, int> dict)
			{
				if (dict != null && dict.ContainsKey(member.Name))
					return; // conflict
				if (member is PropertyInfo p)
				{
					if (p.IsSpecialName)
					{
						var iargs = p.GetIndexParameters();
						if (iargs.Length != 1)
							return;
						var itype = iargs[0].ParameterType;
						if (itype == typeof(int))
						{
							if (p.CanRead)
							{
								var obj = Expression.Parameter(typeof(object), "obj");
								var idx = Expression.Parameter(typeof(int), "idx");
								intIndexGet = Expression.Lambda<Func<object, int, Value>>(
									// (obj, idx) => new Value(((T)obj)[idx])
									Expression.New(IntValueConstructor, new Expression[] {
										Expression.Call(
											Expression.Convert(obj, p.DeclaringType),
											p.GetGetMethod(),
											idx)
									}),
									obj, idx
								).Compile();
							}
							if (p.CanWrite)
							{
								var obj = Expression.Parameter(typeof(object), "obj");
								var idx = Expression.Parameter(typeof(int), "idx");
								var val = Expression.Parameter(typeof(Value), "val");
								intIndexSet = Expression.Lambda<Action<object, int, Value>>(
									// (obj, idx, val) => ((T)obj)[idx] = val.Object
									Expression.New(IntValueConstructor, new Expression[] {
										Expression.Call(
											Expression.Convert(obj, p.DeclaringType),
											p.GetSetMethod(),
											idx,
											Expression.Property(val, "Object"))
									}),
									obj, idx
								).Compile();
							}
							return;
						}
						return;
					}
					if (dict == null)
						dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
					dict[p.Name] = prop.size;
					ref var it = ref prop.Add();
					it.name = p.Name;
					var type = p.PropertyType;
					if (p.CanRead)
					{
						if (!PrimitiveValueConstructors.TryGetValue(type, out var vctor))
							vctor = DefaultValueConstructor;
						var obj = Expression.Parameter(typeof(object), "obj");
						it.read = Expression.Lambda<Func<object, Value>>(
							// obj => new Value(((T)obj).name)
							Expression.New(vctor, new Expression[] {
								Expression.Property(instance
									? Expression.Convert(obj, p.DeclaringType)
									: null, p)
							}),
							obj
						).Compile();
					}
					if (p.CanWrite)
					{
						var obj = Expression.Parameter(typeof(object), "obj");
						var val = Expression.Parameter(typeof(Value), "val");
						it.write = Expression.Lambda<Action<object, Value>>(
							// (obj, val) => ((T)obj).name = (P)val.Object
							Expression.Call(instance
								? Expression.Convert(obj, p.DeclaringType)
								: null,
								p.GetSetMethod(),
								Expression.Convert(
									Expression.Property(val, "Object"),
									type)),
							obj, val
						).Compile();
					}
				}
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
				if (index.IsNumber)
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
					if (index.IsNumber)
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
		}
	}
}
