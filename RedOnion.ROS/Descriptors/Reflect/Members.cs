using RedOnion.Attributes;
using RedOnion.Debugging;
using RedOnion.ROS.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

//TODO: reuse props of base class (from it's descriptor)

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
					var xmt = x.MemberType;
					var ymt = y.MemberType;
					// Nested types first
					if (xmt == MemberTypes.NestedType && ymt != MemberTypes.NestedType)
						return -1;
					if (ymt == MemberTypes.NestedType && xmt != MemberTypes.NestedType)
						return +1;
					// Constructors next
					if (xmt == MemberTypes.Constructor && ymt != MemberTypes.Constructor)
						return -1;
					if (ymt == MemberTypes.Constructor && xmt != MemberTypes.Constructor)
						return +1;
					// Properties next (prefer over fields)
					if (xmt == MemberTypes.Property && ymt != MemberTypes.Property)
						return -1;
					if (ymt == MemberTypes.Property && xmt != MemberTypes.Property)
						return +1;
					// Methods last (need that for conflict resolution vs. method to group upgrade)
					if (xmt == MemberTypes.Method && ymt != MemberTypes.Method)
						return +1;
					if (ymt == MemberTypes.Method && xmt != MemberTypes.Method)
						return -1;

					// sort by name, BIG letters first
					int cmp = string.CompareOrdinal(x.Name, y.Name);
					if (cmp != 0)
						return cmp;

					// prefer member declared later (e.g. RosList[int] over List<Value>[int])
					int xic = InheritanceDepth(x.DeclaringType);
					int yic = InheritanceDepth(y.DeclaringType);
					cmp = yic - xic; // deeper means newer/recent (therefore descending order here)
					if (cmp != 0)
						return cmp;

					// sorting by MetadataToken ensures stability
					// (to avoid problems with reflection cache)
					// e.g. RosMath overloads get processed in declared order
					return x.MetadataToken.CompareTo(y.MetadataToken);
				}
				static int InheritanceDepth(Type type)
				{
					int i = 0;
					while (type != null)
					{
						i++;
						type = type.BaseType;
					}
					return i;
				}
			}
			public static MemberInfo[] GetMembers(Type type, string name = null, bool instance = true)
			{
				var flags = (name == null ? BindingFlags.Public : BindingFlags.IgnoreCase|BindingFlags.Public)
					| (instance ? BindingFlags.Instance : BindingFlags.Static|BindingFlags.FlattenHierarchy);
				var members = name == null
				? type.GetMembers(flags)
				: type.GetMember(name, flags);
				// we are getting nested types twice (for instance as well),
				// although BindingFlags.FlattenHierarchy states "Nested types are not returned" (clearly not true)
				// so we rather make sure we include nested along static and not for instance
				var nested = type.GetNestedTypes();
				if (nested.Length > 0)
				{
					var list = new List<MemberInfo>(members.Length);
					// 1. remove all nested types
					foreach (var m in members)
						if (m as Type == null)
							list.Add(m);
					// 2. add nested types if instance = false
					if (!instance)
						foreach (var m in nested)
							list.Add(m);
					members = list.ToArray();
				}
				if (members.Length > 1)
					Array.Sort(members, MemberComparer.Instance);
				return members;
			}

			public static string GetName(ICustomAttributeProvider provider, out string strict)
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
						return strict = display;
					}
				}
			mangle:
				strict = name;
				if (LowerFirstLetter
					&& name?.Length > 0
					&& char.IsUpper(name, 0)
					&& (name.Length == 1
					|| (char.IsLower(name, 1)
					&& (name.Length != 3
					|| char.IsLower(name, 2)))))
					name = char.ToLowerInvariant(name[0]) + name.Substring(1);
				return name;
			}

			protected virtual bool Conflict(string name, string strict, ref int idx)
			{
				int i = idx;
				while (i >= 0)
				{
					ref var it = ref prop.items[idx = i];
					if (it.strict == strict)
						return true;
					i = it.next;
				}
				return false;
			}

			protected virtual void ProcessMember(MemberInfo member, bool instance)
			{
				var browsable = member.GetCustomAttributes(typeof(BrowsableAttribute), true);
				if (browsable.Length == 1 && !((BrowsableAttribute)browsable[0]).Browsable)
					return;
				string name = GetName(member, out var strict);

				if (instance && member is ConstructorInfo c)
				{
					ProcessCtor(c);
					return;
				}

				var dict = instance ? idict : sdict;
				if (!dict.TryGetValue(name, out var idx))
					idx = -1;
				int first = idx;
				int size = prop.size;

				if (member is Type t)
				{
					Debug.Assert(!instance);
					if (idx >= 0 && Conflict(name, strict, ref idx))
						MainLogger.DebugLog("Conflicting name: {0}.{1} [nested type]", Type.Name, strict);
					else ProcessNested(t, name, idx);
				}
				else if (member is FieldInfo f)
				{
					if (idx >= 0 && Conflict(name, strict, ref idx))
						MainLogger.DebugLog("Conflicting name: {0}.{1} [instace: {2}; field]", Type.Name, strict, instance);
					else ProcessField(f, name, idx, dict);
				}
				else if (member is PropertyInfo p)
				{
					if (idx >= 0 && Conflict(name, strict, ref idx))
						MainLogger.DebugLog("Conflicting name: {0}.{1} [instace: {2}; property]", Type.Name, strict, instance);
					else ProcessProperty(p, name, idx, dict);
				}
				else if (member is EventInfo e)
				{
					if (idx >= 0 && Conflict(name, strict, ref idx))
						MainLogger.DebugLog("Conflicting name: {0}.{1} [instace: {2}; event]", Type.Name, strict, instance);
					else ProcessEvent(e, name, idx, dict);
				}
				else if (member is MethodInfo m)
				{
					if (idx >= 0)
					{
						ref var slot = ref prop.items[idx];
						if (slot.kind != Prop.Kind.Method && slot.kind != Prop.Kind.MethodGroup)
							MainLogger.DebugLog("Conflicting name: {0}.{1} [instace: {2}; method]", Type.Name, strict, instance);
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
					ProcessMethod(m, name, idx, dict);
				}

				// check if we added anything
				if (prop.size == size)
					return;
				ref var it = ref prop.items[size];
				it.strict = strict;
				it.next = -1; // not necessary as zero already is terminator, but to be nice
				it.instance = instance;
				// update the name to strict on conflicts
				if (first >= 0)
				{
					// current always needs renaming (on conflict)
					it.name = it.strict;
					// first needs updating when we add second (no harm in doing it repeatedly)
					it = ref prop.items[first];
					it.name = it.strict;
				}
			}

			protected virtual void ProcessNested(Type nested, string name, int idx)
			{
				MainLogger.ExtraLog("Processing nested type {0}.{1}", Type.Name, nested.Name);

				if (idx < 0) sdict[name] = prop.size;
				else prop.items[idx].next = prop.Count;
				ref var it = ref prop.Add();
				it.name = name;
				it.kind = Prop.Kind.Type;
				it.read = Expression.Lambda<Func<object, Value>>(
					GetNewValueExpression(nested,
						Expression.Constant(nested)),
					SelfParameter
				).Compile();
			}

			protected virtual void ProcessField(FieldInfo f, string name, int idx, Dictionary<string, int> dict)
			{
				Type convert = null;
				var convertAttrs = f.GetCustomAttributes(typeof(ConvertAttribute), true);
				if (convertAttrs.Length == 1)
					convert = ((ConvertAttribute)convertAttrs[0]).Type;

				MainLogger.ExtraLog("Processing field {0}.{1} [static: {2}, convert: {3}]",
					Type.Name, f.Name, f.IsStatic, convert?.Name ?? "False");

				if (idx < 0) dict[name] = prop.size;
				else prop.items[idx].next = prop.Count;
				ref var it = ref prop.Add();
				it.name = name;
				it.kind = Prop.Kind.Field;
				it.next = -1;
				var type = f.FieldType;
				it.read = Expression.Lambda<Func<object, Value>>(
					// WARNING: Mono may crash if const field is fed into Expression.Field!
					GetNewValueExpression(convert ?? type,
					GetConvertExpression(f.IsLiteral ? (Expression)
						Expression.Constant(f.GetValue(null), f.FieldType) :
						Expression.Field(f.IsStatic ? null :
							GetConvertExpression(SelfParameter, f.DeclaringType),
						f),	convert)),
					SelfParameter
				).Compile();
				if (f.IsInitOnly || f.IsLiteral)
					return;
				// maybe use Reflection.Emit: https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.dynamicmethod?view=netframework-3.5
				if (f.IsStatic)
				{
					it.write = (self, value) =>
					{
						var fv = value.Box();
						if (!type.IsAssignableFrom(fv.GetType()))
						{// warning: do not even try to pre-fetch Of(type), may not exist now!
							value.desc.Convert(ref value, Of(type));
							fv = value.Box();
						}
						f.SetValue(null, fv);
					};
				}
				else
				{
					it.write = (self, value) =>
					{
						var fv = value.Box();
						if (!type.IsAssignableFrom(fv.GetType()))
						{
							value.desc.Convert(ref value, Of(type));
							fv = value.Box();
						}
						f.SetValue(self, fv);
					};
				}
			}

			protected virtual void ProcessProperty(PropertyInfo p, string name, int idx, Dictionary<string, int> dict)
			{
				Type convert = null;
				var convertAttrs = p.GetCustomAttributes(typeof(ConvertAttribute), true);
				if (convertAttrs.Length == 1)
					convert = ((ConvertAttribute)convertAttrs[0]).Type;

				MainLogger.ExtraLog("Processing property {0}.{1} [static: {2}, convert: {3}]",
					Type.Name, p.Name, dict == sdict, convert?.Name ?? "False");

				var iargs = p.GetIndexParameters();
				if (p.IsSpecialName || iargs.Length > 0)
				{
					if (iargs.Length != 1)
					{
						MainLogger.DebugLog("Property with multiple indexers: {0}.{1} [{2}]", Type.Name, p.Name, iargs.Length);
						return;
					}
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
					//-------------------------------------------------------------- this[Value]
					if (itype == typeof(Value))
					{
						if (p.CanRead)
						{
							var read = p.GetGetMethod(false);
							if (read != null)
								valIndexGet = Expression.Lambda<Func<object, Value, Value>>(
									GetNewValueExpression(convert ?? p.PropertyType,
									GetConvertExpression(Expression.Call(
										GetConvertExpression(SelfParameter, p.DeclaringType),
										read, ValIndexParameter), convert)),
									SelfParameter, ValIndexParameter
								).Compile();
						}
						if (p.CanWrite)
						{
							var write = p.GetSetMethod();
							if (write != null)
								valIndexSet = Expression.Lambda<Action<object, Value, Value>>(
									Expression.Call(
										GetConvertExpression(SelfParameter, p.DeclaringType),
										write, ValIndexParameter,
										GetConvertExpression(GetValueConvertExpression(
											convert ?? p.PropertyType, ValueParameter),
											convert == null ? null : p.PropertyType)
									),
									SelfParameter, ValIndexParameter, ValueParameter
								).Compile();
						}
						return;
					}
					return;
				}
				//-------------------------------------------------------------- normal property
				if (idx < 0) dict[name] = prop.size;
				else prop.items[idx].next = prop.Count;
				ref var it = ref prop.Add();
				it.name = name;
				it.kind = Prop.Kind.Property;
				var type = p.PropertyType;
				if (p.CanRead)
				{
					var read = p.GetGetMethod();
					if (read != null)
					{
						it.read = Expression.Lambda<Func<object, Value>>(
							GetNewValueExpression(convert ?? type,
							GetConvertExpression(Expression.Property(dict == sdict ? null :
								GetConvertExpression(SelfParameter, p.DeclaringType),
							p), convert)),
							SelfParameter
						).Compile();
					}
				}
				if (p.CanWrite)
				{
					var write = p.GetSetMethod();
					if (write != null)
					{
						it.write = Expression.Lambda<Action<object, Value>>(
							Expression.Call(dict == sdict ? null :
								GetConvertExpression(SelfParameter, p.DeclaringType),
								write,
								GetConvertExpression(GetValueConvertExpression(
									convert ?? type, ValueParameter),
									convert == null ? null : type)),
							SelfParameter, ValueParameter
						).Compile();
					}
				}
			}

			protected virtual void ProcessEvent(EventInfo e, string name, int idx, Dictionary<string, int> dict)
			{
				MainLogger.ExtraLog("Processing event {0}.{1} [static: {2}]",
					Type.Name, e.Name, dict == sdict);

				if (idx < 0) dict[name] = prop.size;
				else prop.items[idx].next = prop.Count;
				ref var it = ref prop.Add();
				it.name = name;
				it.kind = Prop.Kind.Event;
				// TODO: cache these (by type - e.DeclaringType/null, e.EventHandlerType pair)
				it.read = dict == idict
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

			protected virtual void ProcessMethod(MethodInfo m, string name, int idx, Dictionary<string, int> dict)
			{
#if DEBUG
				if (!m.IsSpecialName && !m.IsGenericMethod && !m.ReturnType.IsByRef)
					MainLogger.ExtraLog("Processing method {0}.{1} [static: {2}, args: {3}]",
						Type.Name, m.Name, m.IsStatic, m.GetParameters().Length);
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
				if (idx >= 0)
					((MethodGroup)prop.items[idx].read(null).desc).list.Add(ref value);
				else
				{
					dict[name] = prop.size;
					ref var it = ref prop.Add();
					it.name = name;
					it.kind = Prop.Kind.Method;
					it.next = -1;
					it.read = obj => value;
				}
			}

			protected virtual void ProcessCtor(ConstructorInfo c)
			{
				var args = c.GetParameters();
				if (args.Length == 0 || args[0].RawDefaultValue != DBNull.Value)
				{
					MainLogger.ExtraLog("Found default {0}#ctor [args: {1}]",
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
						Type convert = ConvertAttribute.Get(m);
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
				//TODO: use inversal descriptor
				return new Value(new Callable(m), m);
			}
		}
	}
}
