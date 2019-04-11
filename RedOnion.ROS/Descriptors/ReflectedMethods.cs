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
				if (!type.IsPrimitive)
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
			=> type == typeof(Value) ? expr
			: Expression.New(GetValueConstructor(type), new Expression[] { expr });

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
				return Expression.Property(expr, "Object");
			return Expression.Convert(Expression.Property(expr, "Object"), type);
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

		//############################################################################## NO ARGUMENT

		/// <summary>
		/// static void Action()
		/// </summary>
		internal class ReflectedAction0 : Descriptor
		{
			internal static Value CreateValue(MethodInfo m)
				=> new Value(new ReflectedAction0(m.Name), CreateDelegate(m));
			internal static Delegate CreateDelegate(MethodInfo m)
				=> Delegate.CreateDelegate(typeof(Action), m);

			public ReflectedAction0(string name)
				: base(name, typeof(Action)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 0)
					return false;
				((Action)result.obj)();
				result = Value.Void;
				return true;
			}
		}
		/// <summary>
		/// static Value Function()
		/// </summary>
		internal class ReflectedFunction0 : Descriptor
		{
			internal static Value CreateValue(MethodInfo m)
				=> new Value(new ReflectedFunction0(m.Name), CreateDelegate(m));
			internal static Delegate CreateDelegate(MethodInfo m)
				=> Expression.Lambda<Func<Value>>(GetNewValueExpression(
					m.ReturnType, Expression.Call(m))).Compile();

			public ReflectedFunction0(string name)
				: base(name, typeof(Func<Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 0)
					return false;
				result = ((Func<Value>)result.obj)();
				return true;
			}
		}
		/// <summary>
		/// void Procedure()
		/// </summary>
		public class ReflectedProcedure0<T> : Descriptor
		{
			public ReflectedProcedure0(string name) : base(name, typeof(Action<T>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 0)
					return false;
				((Action<T>)result.obj)((T)self);
				result = Value.Void;
				return true;
			}
		}
		/// <summary>
		/// Value Method()
		/// </summary>
		public class ReflectedMethod0<T> : Descriptor
		{
			public ReflectedMethod0(string name) : base(name, typeof(Func<T, Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 0)
					return false;
				result = ((Func<T, Value>)result.obj)((T)self);
				return true;
			}
		}

		//############################################################################# ONE ARGUMENT

		/// <summary>
		/// static void Action(Value arg)
		/// </summary>
		internal class ReflectedAction1 : Descriptor
		{
			internal static Value CreateValue(MethodInfo m, Type type0)
				=> new Value(new ReflectedAction1(m.Name), CreateDelegate(m, type0));
			internal static Delegate CreateDelegate(MethodInfo m, Type type0)
				=> Expression.Lambda<Action<Value>>(Expression.Call(m,
					GetValueConvertExpression(type0, ValueParameter)),
					ValueParameter).Compile();

			public ReflectedAction1(string name)
				: base(name, typeof(Action<Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 1)
					return false;
				((Action<Value>)result.obj)(args[0]);
				result = Value.Void;
				return true;
			}
		}
		/// <summary>
		/// static Value Function(Value arg)
		/// </summary>
		internal class ReflectedFunction1 : Descriptor
		{
			internal static Value CreateValue(MethodInfo m, Type type0)
				=> new Value(new ReflectedFunction1(m.Name), CreateDelegate(m, type0));
			internal static Delegate CreateDelegate(MethodInfo m, Type type0)
				=> Expression.Lambda<Func<Value, Value>>(GetNewValueExpression(
					m.ReturnType, Expression.Call(m,
					GetValueConvertExpression(type0, ValueParameter))),
					ValueParameter).Compile();

			public ReflectedFunction1(string name)
				: base(name, typeof(Func<Value, Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 1)
					return false;
				result = ((Func<Value, Value>)result.obj)(args[0]);
				return true;
			}
		}
		/// <summary>
		/// void Procedure(Value arg)
		/// </summary>
		public class ReflectedProcedure1<T> : Descriptor
		{
			public ReflectedProcedure1(string name) : base(name, typeof(Action<T, Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 1)
					return false;
				((Action<T, Value>)result.obj)((T)self, args[0]);
				result = Value.Void;
				return true;
			}
		}
		/// <summary>
		/// Value Method(Value arg)
		/// </summary>
		public class ReflectedMethod1<T> : Descriptor
		{
			public ReflectedMethod1(string name) : base(name, typeof(Func<T, Value, Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 1)
					return false;
				result = ((Func<T, Value, Value>)result.obj)((T)self, args[0]);
				return true;
			}
		}

		//############################################################################ TWO ARGUMENTS

		/// <summary>
		/// static void Action(Value arg0, Value arg1)
		/// </summary>
		internal class ReflectedAction2 : Descriptor
		{
			internal static Value CreateValue(MethodInfo m, ParameterInfo[] args)
				=> new Value(new ReflectedAction2(m.Name), CreateDelegate(m, args));
			internal static Delegate CreateDelegate(MethodInfo m, ParameterInfo[] args)
				=> Expression.Lambda<Action<Value, Value>>(Expression.Call(m,
					GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
					GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter)),
					ValueArg0Parameter, ValueArg1Parameter).Compile();

			public ReflectedAction2(string name)
				: base(name, typeof(Action<Value, Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 2)
					return false;
				((Action<Value, Value>)result.obj)(args[0], args[1]);
				result = Value.Void;
				return true;
			}
		}
		/// <summary>
		/// static Value Function(Value arg0, Value arg1)
		/// </summary>
		internal class ReflectedFunction2 : Descriptor
		{
			internal static Value CreateValue(MethodInfo m, ParameterInfo[] args)
				=> new Value(new ReflectedFunction2(m.Name), CreateDelegate(m, args));
			internal static Delegate CreateDelegate(MethodInfo m, ParameterInfo[] args)
				=> Expression.Lambda<Func<Value, Value, Value>>(GetNewValueExpression(
					m.ReturnType, Expression.Call(m,
					GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
					GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter))),
					ValueArg0Parameter, ValueArg1Parameter).Compile();

			public ReflectedFunction2(string name)
				: base(name, typeof(Func<Value, Value, Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 2)
					return false;
				result = ((Func<Value, Value, Value>)result.obj)(args[0], args[1]);
				return true;
			}
		}
		/// <summary>
		/// void Procedure(Value arg0, Value arg1)
		/// </summary>
		public class ReflectedProcedure2<T> : Descriptor
		{
			public ReflectedProcedure2(string name) : base(name, typeof(Action<T, Value, Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 2)
					return false;
				((Action<T, Value, Value>)result.obj)((T)self, args[0], args[1]);
				result = Value.Void;
				return true;
			}
		}
		/// <summary>
		/// Value Method(Value arg0, Value arg1)
		/// </summary>
		public class ReflectedMethod2<T> : Descriptor
		{
			public ReflectedMethod2(string name) : base(name, typeof(Func<T, Value, Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 2)
					return false;
				result = ((Func<T, Value, Value, Value>)result.obj)((T)self, args[0], args[1]);
				return true;
			}
		}

		//########################################################################## THREE ARGUMENTS

		/// <summary>
		/// static void Action(Value arg0, Value arg1, Value arg2)
		/// </summary>
		internal class ReflectedAction3 : Descriptor
		{
			internal static Value CreateValue(MethodInfo m, ParameterInfo[] args)
				=> new Value(new ReflectedAction3(m.Name), CreateDelegate(m, args));
			internal static Delegate CreateDelegate(MethodInfo m, ParameterInfo[] args)
				=> Expression.Lambda<Action<Value, Value, Value>>(Expression.Call(m,
					GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
					GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter),
					GetValueConvertExpression(args[2].ParameterType, ValueArg2Parameter)),
					ValueArg0Parameter, ValueArg1Parameter, ValueArg2Parameter).Compile();

			public ReflectedAction3(string name)
				: base(name, typeof(Action<Value, Value, Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 3)
					return false;
				((Action<Value, Value, Value>)result.obj)(args[0], args[1], args[2]);
				result = Value.Void;
				return true;
			}
		}
		/// <summary>
		/// static Value Function(Value arg0, Value arg1, Value arg2)
		/// </summary>
		internal class ReflectedFunction3 : Descriptor
		{
			internal static Value CreateValue(MethodInfo m, ParameterInfo[] args)
				=> new Value(new ReflectedFunction3(m.Name), CreateDelegate(m, args));
			internal static Delegate CreateDelegate(MethodInfo m, ParameterInfo[] args)
				=> Expression.Lambda<Func<Value, Value, Value, Value>>(GetNewValueExpression(
					m.ReturnType, Expression.Call(m,
					GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
					GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter),
					GetValueConvertExpression(args[2].ParameterType, ValueArg2Parameter))),
					ValueArg0Parameter, ValueArg1Parameter, ValueArg2Parameter).Compile();

			public ReflectedFunction3(string name)
				: base(name, typeof(Func<Value, Value, Value, Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 3)
					return false;
				result = ((Func<Value, Value, Value, Value>)result.obj)(args[0], args[1], args[2]);
				return true;
			}
		}
		/// <summary>
		/// void Procedure(Value arg0, Value arg1, Value arg2)
		/// </summary>
		public class ReflectedProcedure3<T> : Descriptor
		{
			public ReflectedProcedure3(string name) : base(name, typeof(Action<T, Value, Value, Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 3)
					return false;
				((Action<T, Value, Value, Value>)result.obj)((T)self, args[0], args[1], args[2]);
				result = Value.Void;
				return true;
			}
		}
		/// <summary>
		/// Value Method(Value arg0, Value arg1, Value arg2)
		/// </summary>
		public class ReflectedMethod3<T> : Descriptor
		{
			public ReflectedMethod3(string name) : base(name, typeof(Func<T, Value, Value, Value, Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 3)
					return false;
				result = ((Func<T, Value, Value, Value, Value>)result.obj)((T)self, args[0], args[1], args[2]);
				return true;
			}
		}
	}
}
