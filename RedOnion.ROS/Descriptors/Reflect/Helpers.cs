using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		internal static Dictionary<Type, ConstructorInfo>
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
				if (!type.IsPrimitive && type != typeof(Type))
					continue;
				dict[type] = ctor;
			}
			return dict;
		}
		internal static readonly ConstructorInfo DefaultValueConstructor
			= typeof(Value).GetConstructor(new Type[] { typeof(object) });
		internal static readonly ConstructorInfo IntValueConstructor
			= typeof(Value).GetConstructor(new Type[] { typeof(int) });
		internal static readonly ConstructorInfo StrValueConstructor
			= typeof(Value).GetConstructor(new Type[] { typeof(string) });

		public static ConstructorInfo GetValueConstructor(Type type)
			=> PrimitiveValueConstructors.TryGetValue(type, out var vctor)
			? vctor : DefaultValueConstructor;
		public static Expression GetNewValueExpression(Type type, Expression expr)
		{
			if (type == typeof(Value))
				return expr;
			if (!PrimitiveValueConstructors.TryGetValue(type, out var vctor))
			{
				vctor = DefaultValueConstructor;
				if (type.IsValueType)
				{
					return Expression.New(vctor, new Expression[]
					{
						GetConvertExpression(expr, typeof(object))
					});
				}
			}
			return Expression.New(vctor, new Expression[] { expr });
		}
		public static Expression GetConvertExpression(Expression from, Type to)
		{
			if (to == null)
				return from;
			var type = from.Type;
			if (type == to)
				return from;
			if (IsPrimitiveConversion(type, to) || IsReferenceConversion(type, to))
				return Expression.Convert(from, to);
			var op = GetUnaryOperator("op_Explicit", type, type, to);
			if (op == null)
				op = GetUnaryOperator("op_Implicit", type, type, to);
			if (op == null)
				op = GetUnaryOperator("op_Explicit", to, type, to);
			if (op == null)
				op = GetUnaryOperator("op_Implicit", to, type, to);
			if (op != null)
				return Expression.Convert(from, to, op);
			var ctor = to.GetConstructor(new Type[] { type });
			if (ctor != null)
				return Expression.New(ctor, from);
			throw Value.InvalidOperation("Cannot convert from {0} to {1}", type.FullName, to.FullName);
		}
		public static bool IsNullable(Type type)
			=> type.IsValueType && IsGenericInstanceOf(type, typeof(Nullable<>));
		public static Type GetNotNullableType(Type type)
			=> !IsNullable(type) ? type : type.GetGenericArguments()[0];
		public static bool IsAssignableTo(Type from, Type to)
			=> to.IsAssignableFrom(from) || ArrayTypeIsAssignableTo(from, to);
		public static bool ArrayTypeIsAssignableTo(Type from, Type to)
		{
			if (!from.IsArray || !to.IsArray)
				return false;
			if (from.GetArrayRank() != to.GetArrayRank())
				return false;
			return IsAssignableTo(from.GetElementType(), to.GetElementType());
		}
		public static bool IsAssignableToParameterType(Type type, ParameterInfo param)
		{
			Type to = param.ParameterType;
			if (to.IsByRef)
				to = to.GetElementType();
			return IsAssignableTo(GetNotNullableType(type), to);
		}
		public static bool IsGenericInstanceOf(Type type, Type generic)
		{
			if (!type.IsGenericType)
				return false;
			return type.GetGenericTypeDefinition() == generic;
		}
		public static bool IsPrimitiveConversion(Type from, Type to)
		{
			if (from == to)
				return true;
			if (IsNullable(from) && to == GetNotNullableType(from))
				return true;
			if (IsNullable(to) && from == GetNotNullableType(to))
				return true;
			if (IsConvertiblePrimitive(from) && IsConvertiblePrimitive(to))
				return true;
			return false;
		}
		public static bool IsConvertiblePrimitive(Type type)
		{
			Type notNullableType = GetNotNullableType(type);
			if (notNullableType == typeof(bool))
				return false;
			if (notNullableType.IsEnum)
				return true;
			return notNullableType.IsPrimitive;
		}
		public static bool IsReferenceConversion(Type from, Type to)
		{
			if (from == to)
				return true;
			if (from == typeof(object) || to == typeof(object))
				return true;
			if (from.IsInterface || to.IsInterface)
				return true;
			if (from.IsValueType || to.IsValueType)
				return false;
			if (IsAssignableTo(from, to) || IsAssignableTo(to, from))
				return true;
			return false;
		}

		public static MethodInfo GetUnaryOperator(string name, Type type, Type param, Type ret)
		{
			foreach (var method in GetNotNullableType(type)
				.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy))
			{
				if (method.Name != name)
					continue;
				if (method.IsGenericMethod)
					continue;
				if (ret != null && method.ReturnType != ret && method.ReturnType != GetNotNullableType(ret))
					continue;
				var parameters = method.GetParameters();
				if (parameters.Length == 1
					&& IsAssignableToParameterType(GetNotNullableType(param), parameters[0]))
					return method;
			}
			return null;
		}
		public static MethodInfo GetBinaryOperator(string name, Type type, Expression left, Expression right, Type ret = null)
		{
			foreach (var method in type
				.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy))
			{
				if (method.Name != name)
					continue;
				if (method.IsGenericMethod)
					continue;
				if (ret != null && method.ReturnType != ret && method.ReturnType != GetNotNullableType(ret))
					continue;
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length == 2
					&& IsAssignableToParameterType(left.Type, parameters[0])
					&& IsAssignableToParameterType(right.Type, parameters[1]))
					return method;
			}
			return null;
		}

		internal static readonly MethodInfo ValueToObject
			= typeof(Value).GetMethod("Box");
		internal static readonly MethodInfo ValueToInt
			= typeof(Value).GetMethod("ToInt");
		internal static readonly MethodInfo ValueToUInt
			= typeof(Value).GetMethod("ToUInt");
		internal static readonly MethodInfo ValueToLong
			= typeof(Value).GetMethod("ToLong");
		internal static readonly MethodInfo ValueToULong
			= typeof(Value).GetMethod("ToULong");
		internal static readonly MethodInfo ValueToDouble
			= typeof(Value).GetMethod("ToDouble");
		internal static readonly MethodInfo ValueToStr
			= typeof(Value).GetMethod("ToStr");
		internal static readonly MethodInfo ValueToBool
			= typeof(Value).GetMethod("ToBool");
		internal static readonly MethodInfo ValueToChar
			= typeof(Value).GetMethod("ToChar");
		internal static readonly MethodInfo ValueToType
			= typeof(Value).GetMethod("ToType", new Type[] { typeof(Type) });
		public static Expression GetValueConvertExpression(Type type, Expression expr)
		{
			if (type == typeof(Value))
				return expr;
			if (type.IsPrimitive)
			{
				if (type == typeof(int))
					return Expression.Call(expr, ValueToInt);
				if (type == typeof(string))
					return Expression.Call(expr, ValueToStr);
				if (type == typeof(double))
					return Expression.Call(expr, ValueToDouble);
				if (type == typeof(float))
					return GetConvertExpression(Expression.Call(expr, ValueToDouble), type);
				if (type == typeof(uint))
					return Expression.Call(expr, ValueToUInt);
				if (type == typeof(long))
					return Expression.Call(expr, ValueToLong);
				if (type == typeof(ulong))
					return Expression.Call(expr, ValueToULong);
				if (type == typeof(bool))
					return Expression.Call(expr, ValueToBool);
				if (type == typeof(char))
					return Expression.Call(expr, ValueToChar);
				if (type == typeof(byte) || type == typeof(sbyte)
				|| type == typeof(short) || type == typeof(ushort))
					return GetConvertExpression(Expression.Call(expr, ValueToInt), type);
			}
			if (type == typeof(object))
				return Expression.Call(expr, ValueToObject);
			return GetConvertExpression(Expression.Call(expr, ValueToType, Expression.Constant(type)), type);
		}

		internal static readonly ParameterExpression SelfParameter
			= Expression.Parameter(typeof(object), "self");
		internal static readonly ParameterExpression ValueParameter
			= Expression.Parameter(typeof(Value), "value");
		internal static readonly ParameterExpression IntIndexParameter
			= Expression.Parameter(typeof(int), "index");
		internal static readonly ParameterExpression StrIndexParameter
			= Expression.Parameter(typeof(string), "index");
		internal static readonly ParameterExpression ValIndexParameter
			= Expression.Parameter(typeof(Value), "index");
		internal static readonly ParameterExpression ValueArg0Parameter
			= Expression.Parameter(typeof(Value), "arg0");
		internal static readonly ParameterExpression ValueArg1Parameter
			= Expression.Parameter(typeof(Value), "arg1");
		internal static readonly ParameterExpression ValueArg2Parameter
			= Expression.Parameter(typeof(Value), "arg2");

	}
}
