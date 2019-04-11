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
			protected Action<object, string, Value> strIndexSet;

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

			protected virtual void ProcessMember(
				MemberInfo member, bool instance, ref Dictionary<string, int> dict)
			{
				if (dict != null && dict.ContainsKey(member.Name))
					return; // conflict

				//============================================================================ FIELD
				if (member is FieldInfo f)
				{
					if (dict == null)
						dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
					dict[f.Name] = prop.size;
					ref var it = ref prop.Add();
					it.name = f.Name;
					var type = f.FieldType;
					it.read = Expression.Lambda<Func<object, Value>>(
						// self => new Value(((T)self).name)
						GetNewValueExpression(type,
							Expression.Field(instance
								? Expression.Convert(SelfParameter, f.DeclaringType)
								: null, f)),
						SelfParameter
					).Compile();
					if (f.IsInitOnly)
						return;
					// maybe use Reflection.Emit: https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.dynamicmethod?view=netframework-3.5
					if (instance)
						it.write = (self, value) => f.SetValue(self, value.Object);
					else it.write = (self, value) => f.SetValue(null, value.Object);
					return;
				}

				//========================================================================= PROPERTY
				if (member is PropertyInfo p)
				{
					if (p.IsSpecialName)
					{
						var iargs = p.GetIndexParameters();
						if (iargs.Length != 1)
							return;
						var itype = iargs[0].ParameterType;

						//---------------------------------------------------------------- this[int]
						if (itype == typeof(int))
						{
							if (p.CanRead)
							{
								intIndexGet = Expression.Lambda<Func<object, int, Value>>(
									Expression.New(IntValueConstructor, new Expression[] {
										Expression.Call(
											Expression.Convert(SelfParameter, p.DeclaringType),
											p.GetGetMethod(), IntIndexParameter)
									}),
									SelfParameter, IntIndexParameter
								).Compile();
							}
							if (p.CanWrite)
							{
								intIndexSet = Expression.Lambda<Action<object, int, Value>>(
									Expression.New(IntValueConstructor, new Expression[] {
										Expression.Call(
											Expression.Convert(SelfParameter, p.DeclaringType),
											p.GetSetMethod(), IntIndexParameter,
											GetValueConvertExpression(p.PropertyType, ValueParameter))
									}),
									SelfParameter, IntIndexParameter, ValueParameter
								).Compile();
							}
							return;
						}
						//------------------------------------------------------------- this[string]
						if (itype == typeof(string))
						{
							if (p.CanRead)
							{
								strIndexGet = Expression.Lambda<Func<object, string, Value>>(
									Expression.New(StrValueConstructor, new Expression[] {
										Expression.Call(
											Expression.Convert(SelfParameter, p.DeclaringType),
											p.GetGetMethod(), StrIndexParameter)
									}),
									SelfParameter, StrIndexParameter
								).Compile();
							}
							if (p.CanWrite)
							{
								var obj = Expression.Parameter(typeof(object), "obj");
								var idx = Expression.Parameter(typeof(string), "idx");
								var val = Expression.Parameter(typeof(Value), "val");
								strIndexSet = Expression.Lambda<Action<object, string, Value>>(
									Expression.New(StrValueConstructor, new Expression[] {
										Expression.Call(
											Expression.Convert(obj, p.DeclaringType),
											p.GetSetMethod(), StrIndexParameter,
											GetValueConvertExpression(p.PropertyType, ValueParameter))
									}),
									SelfParameter, StrIndexParameter, ValueParameter
								).Compile();
							}
							return;
						}
						return;
					}
					//-------------------------------------------------------------- normal property
					if (dict == null)
						dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
					dict[p.Name] = prop.size;
					ref var it = ref prop.Add();
					it.name = p.Name;
					var type = p.PropertyType;
					if (p.CanRead)
					{
						it.read = Expression.Lambda<Func<object, Value>>(
							GetNewValueExpression(type,
								Expression.Property(instance
									? Expression.Convert(SelfParameter, p.DeclaringType)
									: null, p)),
							SelfParameter
						).Compile();
					}
					if (p.CanWrite)
					{
						it.write = Expression.Lambda<Action<object, Value>>(
							Expression.Call(instance
								? Expression.Convert(SelfParameter, p.DeclaringType)
								: null,
								p.GetSetMethod(),
								GetValueConvertExpression(type, ValueParameter)),
							SelfParameter, ValueParameter
						).Compile();
					}
				}

				//=========================================================================== METHOD
				if (member is MethodInfo m)
				{
					Func<object, Value> read = null;
#if DEBUG
					read = ReflectMethod(m);
#else
					try
					{
						read = ReflectMethod(m);
					}
					catch
					{
					}
#endif
					if (read == null)
						return;
					if (dict == null)
						dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
					dict[m.Name] = prop.size;
					ref var it = ref prop.Add();
					it.name = m.Name;
					it.read = read;
					return;
				}
			}

			protected static Func<object, Value> ReflectMethod(MethodInfo m)
			{
				if (m.IsSpecialName)
					return null;
				if (m.ReturnType.IsByRef)
					return null;
				var args = m.GetParameters();
				foreach (var arg in args)
				{
					if (arg.ParameterType.IsByRef)
						return null;
					if (arg.IsOut)
						return null;
				}
				var argc = args.Length;
				if (m.IsStatic)
				{
					if (m.ReturnType == typeof(void))
					{
						switch (argc)
						{
						case 0:
						{
							var value = ReflectedAction0.CreateValue(m);
							return obj => value;
						}
						case 1:
						{
							var value = ReflectedAction1.CreateValue(m, args[0].ParameterType);
							return obj => value;
						}
						case 2:
						{
							var value = ReflectedAction2.CreateValue(m, args);
							return obj => value;
						}
						case 3:
						{
							var value = ReflectedAction3.CreateValue(m, args);
							return obj => value;
						}
						}
					}
					else
					{
						switch (argc)
						{
						case 0:
						{
							var value = ReflectedFunction0.CreateValue(m);
							return obj => value;
						}
						case 1:
						{
							var value = ReflectedFunction1.CreateValue(m, args[0].ParameterType);
							return obj => value;
						}
						case 2:
						{
							var value = ReflectedFunction2.CreateValue(m, args);
							return obj => value;
						}
						case 3:
						{
							var value = ReflectedFunction3.CreateValue(m, args);
							return obj => value;
						}
						}
					}
				}
				else
				{
					if (m.ReturnType == typeof(void))
					{
						switch (argc)
						{
						case 0:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							var value = new Value((Descriptor)Activator.CreateInstance(
								typeof(ReflectedProcedure0<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(Expression.Call(self, m),
								self).Compile());
							return obj => value;
						}
						case 1:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							var value = new Value((Descriptor)Activator.CreateInstance(
								typeof(ReflectedProcedure1<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter)),
								self, ValueArg0Parameter).Compile());
							return obj => value;
						}
						case 2:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							var value = new Value((Descriptor)Activator.CreateInstance(
								typeof(ReflectedProcedure2<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
								GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter)),
								self, ValueArg0Parameter, ValueArg1Parameter).Compile());
							return obj => value;
						}
						case 3:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							var value = new Value((Descriptor)Activator.CreateInstance(
								typeof(ReflectedProcedure3<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
								GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter),
								GetValueConvertExpression(args[2].ParameterType, ValueArg2Parameter)),
								self, ValueArg0Parameter, ValueArg1Parameter, ValueArg2Parameter).Compile());
							return obj => value;
						}
						}
					}
					else
					{
						switch (argc)
						{
						case 0:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							var value = new Value((Descriptor)Activator.CreateInstance(
								typeof(ReflectedMethod0<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(GetNewValueExpression(m.ReturnType,
								Expression.Call(self, m)),
								self).Compile());
							return obj => value;
						}
						case 1:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							var value = new Value((Descriptor)Activator.CreateInstance(
								typeof(ReflectedMethod1<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(GetNewValueExpression(m.ReturnType,
								Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter))),
								self, ValueArg0Parameter).Compile());
							return obj => value;
						}
						case 2:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							var value = new Value((Descriptor)Activator.CreateInstance(
								typeof(ReflectedMethod2<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(GetNewValueExpression(m.ReturnType,
								Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
								GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter))),
								self, ValueArg0Parameter, ValueArg1Parameter).Compile());
							return obj => value;
						}
						case 3:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							var value = new Value((Descriptor)Activator.CreateInstance(
								typeof(ReflectedMethod3<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(GetNewValueExpression(m.ReturnType,
								Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
								GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter),
								GetValueConvertExpression(args[2].ParameterType, ValueArg2Parameter))),
								self, ValueArg0Parameter, ValueArg1Parameter, ValueArg2Parameter).Compile());
							return obj => value;
						}
						}
					}
				}
				return null;
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
