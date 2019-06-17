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
						Expression.Convert(expr, typeof(object))
					});
				}
			}
			return Expression.New(vctor, new Expression[] { expr });
		}

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
		public static Expression GetValueConvertExpression(Type type, Expression expr)
		{
			if (type == typeof(Value)) return expr;
			if (type.IsPrimitive)
			{
				if (type == typeof(int))
					return Expression.Call(expr, ValueToInt);
				if (type == typeof(string))
					return Expression.Call(expr, ValueToStr);
				if (type == typeof(double))
					return Expression.Call(expr, ValueToDouble);
				if (type == typeof(float))
					return Expression.Convert(Expression.Call(expr, ValueToDouble), type);
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
					return Expression.Convert(Expression.Call(expr, ValueToInt), type);
			}
			if (type == typeof(object))
				return Expression.Field(expr, "obj");
			return Expression.Convert(Expression.Field(expr, "obj"), type);
		}

		internal static readonly ParameterExpression SelfParameter
			= Expression.Parameter(typeof(object), "self");
		internal static readonly ParameterExpression ValueParameter
			= Expression.Parameter(typeof(Value), "value");
		internal static readonly ParameterExpression IntIndexParameter
			= Expression.Parameter(typeof(int), "index");
		internal static readonly ParameterExpression StrIndexParameter
			= Expression.Parameter(typeof(string), "index");
		internal static readonly ParameterExpression ValueArg0Parameter
			= Expression.Parameter(typeof(Value), "arg0");
		internal static readonly ParameterExpression ValueArg1Parameter
			= Expression.Parameter(typeof(Value), "arg1");
		internal static readonly ParameterExpression ValueArg2Parameter
			= Expression.Parameter(typeof(Value), "arg2");

	}
}
