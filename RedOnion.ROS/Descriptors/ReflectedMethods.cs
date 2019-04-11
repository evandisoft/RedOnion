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
		internal static ConstructorInfo DefaultValueConstructor
			= typeof(Value).GetConstructor(new Type[] { typeof(object) });
		internal static ConstructorInfo IntValueConstructor
			= typeof(Value).GetConstructor(new Type[] { typeof(int) });
		internal static ConstructorInfo StrValueConstructor
			= typeof(Value).GetConstructor(new Type[] { typeof(string) });

		public static ConstructorInfo GetValueConstructor(Type type)
			=> PrimitiveValueConstructors.TryGetValue(type, out var vctor)
			? vctor : DefaultValueConstructor;
		public static Expression GetNewValueExpression(Type type, Expression expr)
			=> type == typeof(Value) ? expr
			: Expression.New(GetValueConstructor(type), new Expression[] { expr });
		public static Expression GetValueConvertExpression(Type type, Expression expr)
			=> type == typeof(Value) ? expr
			: Expression.Convert(Expression.Property(expr, "Object"), type);

		internal readonly static ParameterExpression SelfParameter
			= Expression.Parameter(typeof(object), "self");
		internal readonly static ParameterExpression ValueParameter
			= Expression.Parameter(typeof(Value), "value");
		internal readonly static ParameterExpression IntIndexParameter
			= Expression.Parameter(typeof(int), "index");
		internal readonly static ParameterExpression StrIndexParameter
			= Expression.Parameter(typeof(string), "index");
		internal readonly static ParameterExpression ValueArg0Parameter
			= Expression.Parameter(typeof(Value), "arg0");
		internal readonly static ParameterExpression ValueArg1Parameter
			= Expression.Parameter(typeof(Value), "arg1");
		internal readonly static ParameterExpression ValueArg2Parameter
			= Expression.Parameter(typeof(Value), "arg2");

		//############################################################################## NO ARGUMENT

		/// <summary>
		/// static void Action()
		/// </summary>
		internal class ReflectedAction : Descriptor
		{
			internal static Value CreateValue(MethodInfo m)
				=> new Value(new ReflectedAction(m.Name), CreateDelegate(m));
			internal static Delegate CreateDelegate(MethodInfo m)
				=> Delegate.CreateDelegate(typeof(Action), m);

			public ReflectedAction(string name)
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
		internal class ReflectedFunction : Descriptor
		{
			internal static Value CreateValue(MethodInfo m)
				=> new Value(new ReflectedFunction(m.Name), CreateDelegate(m));
			internal static Delegate CreateDelegate(MethodInfo m)
				=> Expression.Lambda<Func<Value>>(GetNewValueExpression(
					m.ReturnType, Expression.Call(m))).Compile();

			public ReflectedFunction(string name)
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
		public class ReflectedProcedure<T> : Descriptor
		{
			public ReflectedProcedure(string name) : base(name, typeof(Action<T>)) { }

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
		public class ReflectedMethod<T> : Descriptor
		{
			public ReflectedMethod(string name) : base(name, typeof(Func<T, Value>)) { }

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
				if (create || args.Length != 0)
					return false;
				result = ((Func<T, Value, Value>)result.obj)((T)self, args[0]);
				return true;
			}
		}
	}
}
