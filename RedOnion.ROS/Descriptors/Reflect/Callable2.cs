using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		/// <summary>
		/// static void Action(Value arg0, Value arg1)
		/// </summary>
		internal class Action2 : Descriptor
		{
			internal static Value CreateValue(MethodInfo m, ParameterInfo[] args)
				=> new Value(new Action2(m.Name), CreateDelegate(m, args));
			internal static Delegate CreateDelegate(MethodInfo m, ParameterInfo[] args)
				=> Expression.Lambda<Action<Value, Value>>(Expression.Call(m,
					GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
					GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter)),
					ValueArg0Parameter, ValueArg1Parameter).Compile();

			public Action2(string name)
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
		internal class Function2 : Descriptor
		{
			internal static Value CreateValue(MethodInfo m, ParameterInfo[] args)
				=> new Value(new Function2(m.Name), CreateDelegate(m, args));
			internal static Delegate CreateDelegate(MethodInfo m, ParameterInfo[] args)
				=> Expression.Lambda<Func<Value, Value, Value>>(GetNewValueExpression(
					m.ReturnType, Expression.Call(m,
					GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
					GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter))),
					ValueArg0Parameter, ValueArg1Parameter).Compile();

			public Function2(string name)
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
		public class Procedure2<T> : Descriptor
		{
			public Procedure2(string name) : base(name, typeof(Action<T, Value, Value>)) { }

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
		public class Method2<T> : Descriptor
		{
			public Method2(string name) : base(name, typeof(Func<T, Value, Value>)) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 2)
					return false;
				result = ((Func<T, Value, Value, Value>)result.obj)((T)self, args[0], args[1]);
				return true;
			}
		}
	}
}
