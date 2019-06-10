using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace RedOnion.ROS
{
	partial class Descriptor
	{
		partial class Reflected : Descriptor
		{
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
					var iargs = p.GetIndexParameters();
					if (p.IsSpecialName || iargs.Length > 0)
					{
						if (iargs.Length != 1)
							return;
						var itype = iargs[0].ParameterType;

						//---------------------------------------------------------------- this[int]
						if (itype == typeof(int))
						{
							if (p.CanRead)
							{
								intIndexGet = Expression.Lambda<Func<object, int, Value>>(
									GetNewValueExpression(p.PropertyType, Expression.Call(
										Expression.Convert(SelfParameter, p.DeclaringType),
										p.GetGetMethod(), IntIndexParameter)),
									SelfParameter, IntIndexParameter
								).Compile();
							}
							if (p.CanWrite)
							{
								intIndexSet = Expression.Lambda<Action<object, int, Value>>(
									Expression.Call(
										Expression.Convert(SelfParameter, p.DeclaringType),
										p.GetSetMethod(), IntIndexParameter,
										GetValueConvertExpression(p.PropertyType, ValueParameter)
									),
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
									GetNewValueExpression(p.PropertyType, Expression.Call(
										Expression.Convert(SelfParameter, p.DeclaringType),
										p.GetGetMethod(), StrIndexParameter)),
									SelfParameter, StrIndexParameter
								).Compile();
							}
							if (p.CanWrite)
							{
								strIndexSet = Expression.Lambda<Action<object, string, Value>>(
									Expression.Call(
										Expression.Convert(SelfParameter, p.DeclaringType),
										p.GetSetMethod(), StrIndexParameter,
										GetValueConvertExpression(p.PropertyType, ValueParameter)
									),
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
				if (m.IsGenericMethod)
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
							var value = Action0.CreateValue(m);
							return obj => value;
						}
						case 1:
						{
							var value = Action1.CreateValue(m, args[0].ParameterType);
							return obj => value;
						}
						case 2:
						{
							var value = Action2.CreateValue(m, args);
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
							var value = Function0.CreateValue(m);
							return obj => value;
						}
						case 1:
						{
							var value = Function1.CreateValue(m, args[0].ParameterType);
							return obj => value;
						}
						case 2:
						{
							var value = Function2.CreateValue(m, args);
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
								typeof(Procedure0<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(Expression.Call(self, m),
								self).Compile());
							return obj => value;
						}
						case 1:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							var value = new Value((Descriptor)Activator.CreateInstance(
								typeof(Procedure1<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter)),
								self, ValueArg0Parameter).Compile());
							return obj => value;
						}
						case 2:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							var value = new Value((Descriptor)Activator.CreateInstance(
								typeof(Procedure2<>).MakeGenericType(m.DeclaringType), m.Name),
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
								typeof(Method0<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(GetNewValueExpression(m.ReturnType,
								Expression.Call(self, m)),
								self).Compile());
							return obj => value;
						}
						case 1:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							var value = new Value((Descriptor)Activator.CreateInstance(
								typeof(Method1<>).MakeGenericType(m.DeclaringType), m.Name),
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
								typeof(Method2<>).MakeGenericType(m.DeclaringType), m.Name),
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
		}
	}
}
