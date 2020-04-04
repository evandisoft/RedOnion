using RedOnion.ROS.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
			public static MemberInfo[] GetMembers(Type type, string name = null, bool instance = true)
			{
				var flags = BindingFlags.IgnoreCase|BindingFlags.Public
				| (instance ? BindingFlags.Instance : BindingFlags.Static|BindingFlags.FlattenHierarchy);
				var members = name == null
				? type.GetMembers(flags)
				: type.GetMember(name, flags);
				if (members.Length > 1)
					Array.Sort(members, MemberComparer.Instance);
				return members;
			}

			public static string GetName(ICustomAttributeProvider provider)
			{
				string name = provider is MemberInfo member ? member is ConstructorInfo ci
					? ci.DeclaringType.Name : member.Name
					: provider is Type type ? type.Name : null;
				var displayName = provider.GetCustomAttributes(typeof(DisplayNameAttribute), !(provider is Type));
				if (displayName.Length == 1)
				{
					var display = ((DisplayNameAttribute)displayName[0]).DisplayName;
					if (display?.Length > 0	&& (char.IsLetter(display, 0) || display[0] == '_'))
					{
						foreach (char c in display)
							if (c != '_' && !char.IsLetterOrDigit(c))
								goto mangle;
						return display;
					}
				}
			mangle:
				if (LowerFirstLetter
					&& name?.Length > 0
					&& char.IsUpper(name, 0)
					&& (name.Length == 1
					|| char.IsLower(name, 1)))
					name = char.ToLowerInvariant(name[0]) + name.Substring(1);
				return name;
			}

			protected virtual void ProcessMember(
				MemberInfo member, bool instance, ref Dictionary<string, int> dict)
			{
				var browsable = member.GetCustomAttributes(typeof(BrowsableAttribute), true);
				if (browsable.Length == 1 && !((BrowsableAttribute)browsable[0]).Browsable)
					return;
				string name = GetName(member);

				if (instance && member is ConstructorInfo c)
				{
					ProcessCtor(c);
					return;
				}

				if (dict == null || !dict.TryGetValue(name, out var idx))
					idx = -1;

				if (member is FieldInfo f)
				{
					if (idx >= 0)
						Value.DebugLog("Conflicting name: {0}.{1} [instace: {2}; field]", Type.Name, name, instance);
					else ProcessField(name, f, instance, ref dict);
				}
				else if (member is PropertyInfo p)
				{
					if (idx >= 0)
						Value.DebugLog("Conflicting name: {0}.{1} [instace: {2}; property]", Type.Name, name, instance);
					else ProcessProperty(name, p, instance, ref dict);
				}
				else if (member is EventInfo e)
				{
					if (idx >= 0)
						Value.DebugLog("Conflicting name: {0}.{1} [instace: {2}; event]", Type.Name, name, instance);
					else ProcessEvent(name, e, instance, ref dict);
				}
				else if (member is MethodInfo m)
				{
					if (idx >= 0)
					{
						ref var slot = ref prop.items[idx];
						if (slot.kind != Prop.Kind.Method && slot.kind != Prop.Kind.MethodGroup)
							Value.DebugLog("Conflicting name: {0}.{1} [instace: {2}; method]", Type.Name, name, instance);
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
					ProcessMethod(name, m, instance, ref dict, idx);
				}
			}

			protected virtual void ProcessNested(Type nested)
			{
				var browsable = nested.GetCustomAttributes(typeof(BrowsableAttribute), true);
				if (browsable.Length == 1 && !((BrowsableAttribute)browsable[0]).Browsable)
					return;

				Value.ExtraLog("Processing nested type {0}.{1}", Type.Name, nested.Name);

				if (sdict == null)
					sdict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				sdict[nested.Name] = prop.size;
				ref var it = ref prop.Add();
				it.name = nested.Name;
				it.kind = Prop.Kind.Type;
				it.read = Expression.Lambda<Func<object, Value>>(
					GetNewValueExpression(nested,
						Expression.Constant(nested)),
					SelfParameter
				).Compile();
			}

			protected virtual void ProcessField(
				string name, FieldInfo f, bool instance, ref Dictionary<string, int> dict)
			{
				Type convert = null;
				var convertAttrs = f.GetCustomAttributes(typeof(ConvertAttribute), true);
				if (convertAttrs.Length == 1)
					convert = ((ConvertAttribute)convertAttrs[0]).Type;

				Value.ExtraLog("Processing field {0}.{1} [instace: {2}, convert: {3}]",
					Type.Name, f.Name, instance, convert?.Name ?? "False");

				if (dict == null)
					dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				dict[name] = prop.size;
				ref var it = ref prop.Add();
				it.name = name;
				it.kind = Prop.Kind.Field;
				var type = f.FieldType;
				it.read = Expression.Lambda<Func<object, Value>>(
					// WARNING: Mono may crash if const field is fed into Expression.Field!
					GetNewValueExpression(convert ?? type,
					GetConvertExpression(f.IsLiteral ? (Expression)
						// self => new Value(T.name)
						Expression.Constant(f.GetValue(null), f.FieldType) :
						Expression.Field(instance
							// self => new Value(((T)self).name)
							? GetConvertExpression(SelfParameter, f.DeclaringType)
							// self => new Value(T.name)
							: null,
						f),	convert)),
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
			}

			protected virtual void ProcessProperty(
				string name, PropertyInfo p, bool instance, ref Dictionary<string, int> dict)
			{
				Type convert = null;
				var convertAttrs = p.GetCustomAttributes(typeof(ConvertAttribute), true);
				if (convertAttrs.Length == 1)
					convert = ((ConvertAttribute)convertAttrs[0]).Type;

				Value.ExtraLog("Processing property {0}.{1} [instace: {2}, convert: {3}]",
					Type.Name, p.Name, instance, convert?.Name ?? "False");

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
									GetNewValueExpression(convert ?? p.PropertyType,
									GetConvertExpression(Expression.Call(
										GetConvertExpression(SelfParameter, p.DeclaringType),
										read, IntIndexParameter), convert)),
									SelfParameter, IntIndexParameter
								).Compile();
						}
						if (p.CanWrite)
						{
							var write = p.GetSetMethod(false);
							if (write != null)
								intIndexSet = Expression.Lambda<Action<object, int, Value>>(
									Expression.Call(
										GetConvertExpression(SelfParameter, p.DeclaringType),
										write, IntIndexParameter,
										GetConvertExpression(GetValueConvertExpression(
											convert ?? p.PropertyType, ValueParameter),
											convert == null ? null : p.PropertyType)
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
									GetNewValueExpression(convert ?? p.PropertyType,
									GetConvertExpression(Expression.Call(
										GetConvertExpression(SelfParameter, p.DeclaringType),
										read, StrIndexParameter), convert)),
									SelfParameter, StrIndexParameter
								).Compile();
						}
						if (p.CanWrite)
						{
							var write = p.GetSetMethod();
							if (write != null)
								strIndexSet = Expression.Lambda<Action<object, string, Value>>(
									Expression.Call(
										GetConvertExpression(SelfParameter, p.DeclaringType),
										write, StrIndexParameter,
										GetConvertExpression(GetValueConvertExpression(
											convert ?? p.PropertyType, ValueParameter),
											convert == null ? null : p.PropertyType)
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
				dict[name] = prop.size;
				ref var it = ref prop.Add();
				it.name = name;
				it.kind = Prop.Kind.Property;
				var type = p.PropertyType;
				if (p.CanRead)
				{
					if (p.GetGetMethod() != null)
						it.read = Expression.Lambda<Func<object, Value>>(
							GetNewValueExpression(convert ?? type,
							GetConvertExpression(Expression.Property(instance
								? GetConvertExpression(SelfParameter, p.DeclaringType)
								: null,
							p), convert)),
							SelfParameter
						).Compile();
				}
				if (p.CanWrite)
				{
					var write = p.GetSetMethod();
					if (write != null)
						it.write = Expression.Lambda<Action<object, Value>>(
							Expression.Call(instance
								? GetConvertExpression(SelfParameter, p.DeclaringType)
								: null,
								write,
								GetConvertExpression(GetValueConvertExpression(
									convert ?? type, ValueParameter),
									convert == null ? null : type)),
							SelfParameter, ValueParameter
						).Compile();
				}
			}

			protected virtual void ProcessEvent(
				string name, EventInfo e, bool instance, ref Dictionary<string, int> dict)
			{
				if (dict == null)
					dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				dict[name] = prop.size;
				ref var it = ref prop.Add();
				it.name = name;
				it.kind = Prop.Kind.Event;
				// TODO: cache these (by type - e.DeclaringType/null, e.EventHandlerType pair)
				it.read = instance
					// self => new Value(new EventProxy<T,E>(self, e))
					? Expression.Lambda<Func<object, Value>>(
						GetNewValueExpression(typeof(Descriptor),
						Expression.New(typeof(EventProxy<,>)
							.MakeGenericType(e.DeclaringType, e.EventHandlerType)
							.GetConstructor(new Type[] { e.DeclaringType, typeof(EventInfo) }),
							GetConvertExpression(SelfParameter, e.DeclaringType),
							Expression.Constant(e, typeof(EventInfo)))),
					SelfParameter).Compile()
					// self => new Value(new EventProxy<E>(e))
					: Expression.Lambda<Func<object, Value>>(
						GetNewValueExpression(typeof(Descriptor),
						Expression.New(typeof(EventProxy<>)
							.MakeGenericType(e.EventHandlerType)
							.GetConstructor(new Type[] { typeof(EventInfo) }),
							Expression.Constant(e, typeof(EventInfo)))),
					SelfParameter).Compile();
			}


			protected virtual void ProcessMethod(
				string name, MethodInfo m, bool instance, ref Dictionary<string, int> dict, int idx)
			{
#if DEBUG
				if (!m.IsSpecialName && !m.IsGenericMethod && !m.ReturnType.IsByRef)
					Value.ExtraLog("Processing method {0}.{1} [instace: {2}, args: {3}]",
						Type.Name, m.Name, instance, m.GetParameters().Length);
#endif
				if (m.Name == "op_Implicit")
				{
					implConvert.Add(new KeyValuePair<Type, Func<object, object>>(
						m.ReturnType, Expression.Lambda<Func<object, object>>(
							Expression.Convert(Expression.Convert(
								Expression.Convert(SelfParameter, Type),
								m.ReturnType, m), typeof(object)),
							SelfParameter).Compile()));
					return;
				}
				var value = ReflectMethod(m);
				if (value.IsVoid)
					return;
				if (dict == null)
					dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				if (idx >= 0)
					((MethodGroup)prop.items[idx].read(null).desc).list.Add(ref value);
				else
				{
					dict[name] = prop.size;
					ref var it = ref prop.Add();
					it.name = name;
					it.kind = Prop.Kind.Method;
					it.read = obj => value;
				}
			}

			protected virtual void ProcessCtor(ConstructorInfo c)
			{
				var args = c.GetParameters();
				if (args.Length == 0 || args[0].RawDefaultValue != DBNull.Value)
				{
					Value.ExtraLog("Found default {0}#ctor [args: {1}]",
						Type.Name, args.Length);
					if (defaultCtor == null || c.GetParameters().Length < defaultCtor.GetParameters().Length)
						defaultCtor = c;
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
								typeof(Procedure0<>).MakeGenericType(m.DeclaringType), m),
								Expression.Lambda(Expression.Call(self, m),
								self).Compile());
						}
						case 1:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							//TODO: cache and reuse the descriptors
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Procedure1<>).MakeGenericType(m.DeclaringType), m),
								Expression.Lambda(Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter)),
								self, ValueArg0Parameter).Compile());
						}
						case 2:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Procedure2<>).MakeGenericType(m.DeclaringType), m),
								Expression.Lambda(Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
								GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter)),
								self, ValueArg0Parameter, ValueArg1Parameter).Compile());
						}
						case 3:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Procedure3<>).MakeGenericType(m.DeclaringType), m),
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
						Type convert = null;
						var convertAttrs = m.ReturnTypeCustomAttributes
							.GetCustomAttributes(typeof(ConvertAttribute), true);
						if (convertAttrs.Length == 1)
							convert = ((ConvertAttribute)convertAttrs[0]).Type;
						switch (argc)
						{
						case 0:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Method0<>).MakeGenericType(m.DeclaringType), m),
								Expression.Lambda(GetNewValueExpression(convert ?? m.ReturnType,
								GetConvertExpression(Expression.Call(self, m),
								convert)), self).Compile());
						}
						case 1:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Method1<>).MakeGenericType(m.DeclaringType), m),
								Expression.Lambda(GetNewValueExpression(convert ?? m.ReturnType,
								GetConvertExpression(Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter)),
								convert)), self, ValueArg0Parameter).Compile());
						}
						case 2:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Method2<>).MakeGenericType(m.DeclaringType), m),
								Expression.Lambda(GetNewValueExpression(convert ?? m.ReturnType,
								GetConvertExpression(Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
								GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter)),
								convert)), self, ValueArg0Parameter, ValueArg1Parameter).Compile());
						}
						case 3:
						{
							var self = Expression.Parameter(m.DeclaringType, "self");
							return new Value((Descriptor)Activator.CreateInstance(
								typeof(Method3<>).MakeGenericType(m.DeclaringType), m),
								Expression.Lambda(GetNewValueExpression(convert ?? m.ReturnType,
								GetConvertExpression(Expression.Call(self, m,
								GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
								GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter),
								GetValueConvertExpression(args[2].ParameterType, ValueArg2Parameter)),
								convert)), self, ValueArg0Parameter, ValueArg1Parameter, ValueArg2Parameter).Compile());
						}
						}
					}
				}
				return Value.Void;
			}
		}
	}
}
