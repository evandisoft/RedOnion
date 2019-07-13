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
		internal class Action3 : Callable
		{
			internal static Value CreateValue(MethodInfo m, ParameterInfo[] args)
				=> new Value(new Action3(m), CreateDelegate(m, args));
			internal static Delegate CreateDelegate(MethodInfo m, ParameterInfo[] args)
				=> Expression.Lambda<Action<Value, Value, Value>>(Expression.Call(m,
					GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
					GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter),
					GetValueConvertExpression(args[2].ParameterType, ValueArg2Parameter)),
					ValueArg0Parameter, ValueArg1Parameter, ValueArg2Parameter).Compile();

			public Action3(MethodInfo m)
				: this(m.Name, m) { }
			public Action3(string name, MethodInfo m = null)
				: base(name, typeof(Action<Value, Value, Value>), false, m) { }

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
		internal class Function3 : Callable
		{
			internal static Value CreateValue(MethodInfo m, ParameterInfo[] args)
				=> new Value(new Function3(m), CreateDelegate(m, args));
			internal static Delegate CreateDelegate(MethodInfo m, ParameterInfo[] args)
			{
				Type convert = null;
				var convertAttrs = m.ReturnTypeCustomAttributes
					.GetCustomAttributes(typeof(ConvertAttribute), true);
				if (convertAttrs.Length == 1)
					convert = ((ConvertAttribute)convertAttrs[0]).Type;
				return Expression.Lambda<Func<Value, Value, Value, Value>>(GetNewValueExpression(
					convert ?? m.ReturnType, GetConvertExpression(Expression.Call(m,
					GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
					GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter),
					GetValueConvertExpression(args[2].ParameterType, ValueArg2Parameter)),
					convert)), ValueArg0Parameter, ValueArg1Parameter, ValueArg2Parameter).Compile();
			}

			public Function3(MethodInfo m)
				: this(m.Name, m) { }
			public Function3(string name, MethodInfo m = null)
				: base(name, typeof(Func<Value, Value, Value, Value>), false, m) { }

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
		public class Procedure3<T> : Callable
		{
			public Procedure3(string name)
				: base(name, typeof(Action<T, Value, Value, Value>), true) { }

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
		public class Method3<T> : Callable
		{
			public Method3(string name)
				: base(name, typeof(Func<T, Value, Value, Value, Value>), true) { }

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
