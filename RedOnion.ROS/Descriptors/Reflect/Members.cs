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
				if (instance && member is ConstructorInfo c)
				{
					ProcessCtor(c);
					return;
				}

				if (dict == null || !dict.TryGetValue(member.Name, out var idx))
					idx = -1;

				if (member is FieldInfo f)
				{
					if (idx >= 0)
						Value.DebugLog("Conflicting name: {0}.{1} [instace: {2}; field]", Type.Name, member.Name, instance);
					else ProcessField(f, instance, ref dict);
				}
				else if (member is PropertyInfo p)
				{
					if (idx >= 0)
						Value.DebugLog("Conflicting name: {0}.{1} [instace: {2}; property]", Type.Name, member.Name, instance);
					else ProcessProperty(p, instance, ref dict);
				}
				else if (member is MethodInfo m)
				{
					if (idx >= 0)
					{
						ref var slot = ref prop.items[idx];
						if (slot.kind != Prop.Kind.Method && slot.kind != Prop.Kind.MethodGroup)
							Value.DebugLog("Conflicting name: {0}.{1} [instace: {2}; method]", Type.Name, member.Name, instance);
						else if (slot.kind == Prop.Kind.Method)
						{
							var call = slot.read(null);
							var desc = (Callable)call.desc;
							var group = new MethodGroup(desc.Name, desc.Type, desc.IsMethod);
							group.list.Add(call);
							var  value = new Value(group);
							slot.read = obj => value;
							slot.kind = Prop.Kind.MethodGroup;
						}
					}
					ProcessMethod(m, instance, ref dict, idx);
				}
			}

			protected virtual void ProcessField(
				FieldInfo f, bool instance, ref Dictionary<string, int> dict)
			{
				Value.DebugLog("Processing field {0}.{1} [instace: {2}]", Type.Name, f.Name, instance);
				if (dict == null)
					dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				dict[f.Name] = prop.size;
				ref var it = ref prop.Add();
				it.name = f.Name;
				it.kind = Prop.Kind.Field;
				var type = f.FieldType;
				it.read = Expression.Lambda<Func<object, Value>>(
					// WARNING: Mono may crash if const field is fed into Expression.Field!
					GetNewValueExpression(type, f.IsLiteral ? (Expression)
						// self => new Value(T.name)
						Expression.Constant(f.GetValue(null), f.FieldType) :
						Expression.Field(instance
							// self => new Value(((T)self).name)
							? Expression.Convert(SelfParameter, f.DeclaringType)
							// self => new Value(T.name)
							: null, f)),
					SelfParameter
				).Compile();
				if (f.IsInitOnly || f.IsLiteral)
					return;
				// maybe use Reflection.Emit: https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.dynamicmethod?view=netframework-3.5
				if (instance)
				{
					it.write = (self, value) =>
					{
						var fv = value.Box();
						if (!f.FieldType.IsAssignableFrom(fv.GetType()))
						{
							value.desc.Convert(ref value, Of(f.FieldType));
							fv = value.Box();
						}
						f.SetValue(self, fv);
					};
				}
				else
				{
					it.write = (self, value) =>
					{
						var fv = value.Box();
						if (!f.FieldType.IsAssignableFrom(fv.GetType()))
						{
							value.desc.Convert(ref value, Of(f.FieldType));
							fv = value.Box();
						}
						f.SetValue(null, fv);
					};
				}
				return;
			}

			protected virtual void ProcessProperty(
				PropertyInfo p, bool instance, ref Dictionary<string, int> dict)
			{
				Value.DebugLog("Processing property {0}.{1} [instace: {2}]", Type.Name, p.Name, instance);
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
							var read = p.GetGetMethod(false);
							if (read != null)
								intIndexGet = Expression.Lambda<Func<object, int, Value>>(
									GetNewValueExpression(p.PropertyType, Expression.Call(
										Expression.Convert(SelfParameter, p.DeclaringType),
										read, IntIndexParameter)),
									SelfParameter, IntIndexParameter
								).Compile();
						}
						if (p.CanWrite)
						{
							var write = p.GetSetMethod(false);
							if (write != null)
								intIndexSet = Expression.Lambda<Action<object, int, Value>>(
									Expression.Call(
										Expression.Convert(SelfParameter, p.DeclaringType),
										write, IntIndexParameter,
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
							var read = p.GetGetMethod(false);
							if (read != null)
								strIndexGet = Expression.Lambda<Func<object, string, Value>>(
									GetNewValueExpression(p.PropertyType, Expression.Call(
										Expression.Convert(SelfParameter, p.DeclaringType),
										read, StrIndexParameter)),
									SelfParameter, StrIndexParameter
								).Compile();
						}
						if (p.CanWrite)
						{
							var write = p.GetSetMethod();
							if (write != null)
								strIndexSet = Expression.Lambda<Action<object, string, Value>>(
									Expression.Call(
										Expression.Convert(SelfParameter, p.DeclaringType),
										write, StrIndexParameter,
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
				it.kind = Prop.Kind.Property;
				var type = p.PropertyType;
				if (p.CanRead)
				{
					if (p.GetGetMethod() != null)
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
					var write = p.GetSetMethod();
					if (write != null)
						it.write = Expression.Lambda<Action<object, Value>>(
							Expression.Call(instance
								? Expression.Convert(SelfParameter, p.DeclaringType)
								: null,
								write,
								GetValueConvertExpression(type, ValueParameter)),
							SelfParameter, ValueParameter
						).Compile();
				}
			}

			protected virtual void ProcessMethod(
				MethodInfo m, bool instance, ref Dictionary<string, int> dict, int idx)
			{
#if DEBUG
				if (!m.IsSpecialName && !m.IsGenericMethod && !m.ReturnType.IsByRef)
					Value.DebugLog("Processing method {0}.{1} [instace: {2}, args: {3}]",
						Type.Name, m.Name, instance, m.GetParameters().Length);
#endif
				var value = ReflectMethod(m);
				if (value.IsVoid)
					return;
				if (dict == null)
					dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				if (idx >= 0)
					((MethodGroup)prop.items[idx].read(null).desc).list.Add(ref value);
				else
				{
					dict[m.Name] = prop.size;
					ref var it = ref prop.Add();
					it.name = m.Name;
					it.kind = Prop.Kind.Method;
					it.read = obj => value;
				}
			}

			protected virtual void ProcessCtor(ConstructorInfo c)
			{
				var args = c.GetParameters();
				if (args.Length == 0 || args[0].RawDefaultValue != DBNull.Value)
				{
					Value.DebugLog("Found default {0}#ctor [args: {1}]",
						Type.Name, args.Length);
					defaultCtor = c;
					return;
				}
				if (args[0].ParameterType == typeof(IProcessor)
					&& (args.Length == 1 || args[1].RawDefaultValue != DBNull.Value)
					&& !args[0].ParameterType.IsByRef && !args[0].IsOut)
				{
					Value.DebugLog("Found default {0}#ctor accepting processor [args: {1}]",
						Type.Name, args.Length);
					processorCtor = c;
					return;
				}
			}

			protected static Value ReflectMethod(MethodInfo m)
			{
				if (m.IsSpecialName || m.IsGenericMethod || m.ReturnType.IsByRef)
					return Value.Void;
				var args = m.GetParameters();
				foreach (var arg in args)
				{
					if (arg.ParameterType.IsByRef || arg.IsOut)
						return Value.Void;
				}
				var argc = args.Length;
				if (m.IsStatic)
				{
					if (m.ReturnType == typeof(void))
					{
						//TODO: more args
						switch (argc)
						{
						case 0:
							return Action0.CreateValue(m);
						case 1:
							return Action1.CreateValue(m, args[0].ParameterType);
						case 2:
							return Action2.CreateValue(m, args);
						case 3:
							return Action3.CreateValue(m, args);
						}
					}
					else
					{
						switch (argc)
						{
						case 0:
							return Function0.CreateValue(m);
						case 1:
							return Function1.CreateValue(m, args[0].ParameterType);
						case 2:
							return Function2.CreateValue(m, args);
						case 3:
							return Function3.CreateValue(m, args);
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
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Procedure0<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(Expression.Call(self, m),
								self).Compile());
						}
						case 1:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							//TODO: cache and reuse the descriptors
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Procedure1<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter)),
								self, ValueArg0Parameter).Compile());
						}
						case 2:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Procedure2<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
								GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter)),
								self, ValueArg0Parameter, ValueArg1Parameter).Compile());
						}
						case 3:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Procedure3<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
								GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter),
								GetValueConvertExpression(args[2].ParameterType, ValueArg2Parameter)),
								self, ValueArg0Parameter, ValueArg1Parameter, ValueArg2Parameter).Compile());
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
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Method0<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(GetNewValueExpression(m.ReturnType,
								Expression.Call(self, m)),
								self).Compile());
						}
						case 1:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Method1<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(GetNewValueExpression(m.ReturnType,
								Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter))),
								self, ValueArg0Parameter).Compile());
						}
						case 2:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Method2<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(GetNewValueExpression(m.ReturnType,
								Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
								GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter))),
								self, ValueArg0Parameter, ValueArg1Parameter).Compile());
						}
						case 3:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Method3<>).MakeGenericType(m.DeclaringType), m.Name),
								Expression.Lambda(GetNewValueExpression(m.ReturnType,
								Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
								GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter),
								GetValueConvertExpression(args[2].ParameterType, ValueArg2Parameter))),
								self, ValueArg0Parameter, ValueArg1Parameter, ValueArg2Parameter).Compile());
						}
						}
					}
				}
				return Value.Void;
			}
		}
	}
}
