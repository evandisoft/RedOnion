using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
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
