using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		/// <summary>
		/// static void Action(Value arg)
		/// </summary>
		internal class Action1 : Callable
		{
			internal static Value CreateValue(MethodInfo m, Type type0)
				=> new Value(new Action1(m), CreateDelegate(m, type0));
			internal static Delegate CreateDelegate(MethodInfo m, Type type0)
				=> Expression.Lambda<Action<Value>>(Expression.Call(m,
					GetValueConvertExpression(type0, ValueParameter)),
					ValueParameter).Compile();

			public Action1(MethodInfo m)
				: this(m.Name, m) { }
			public Action1(string name, MethodInfo m = null)
				: base(name, typeof(Action<Value>), false, m) { }

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
		internal class Function1 : Callable
		{
			internal static Value CreateValue(MethodInfo m, Type type0)
				=> new Value(new Function1(m), CreateDelegate(m, type0));
			internal static Delegate CreateDelegate(MethodInfo m, Type type0)
			{
				Type convert = null;
				var convertAttrs = m.ReturnTypeCustomAttributes
					.GetCustomAttributes(typeof(ConvertAttribute), true);
				if (convertAttrs.Length == 1)
					convert = ((ConvertAttribute)convertAttrs[0]).Type;
				return Expression.Lambda<Func<Value, Value>>(GetNewValueExpression(
					convert ?? m.ReturnType, GetConvertExpression(Expression.Call(m,
					GetValueConvertExpression(type0, ValueParameter)),
					convert)), ValueParameter).Compile();
			}

			public Function1(MethodInfo m)
				: this(m.Name, m) { }
			public Function1(string name, MethodInfo m = null)
				: base(name, typeof(Func<Value, Value>), false, m) { }

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
		public class Procedure1<T> : Callable
		{
			public Procedure1(string name)
				: base(name, typeof(Action<T, Value>), true) { }

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
		public class Method1<T> : Callable
		{
			public Method1(string name)
				: base(name, typeof(Func<T, Value, Value>), true) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 1)
					return false;
				result = ((Func<T, Value, Value>)result.obj)((T)self, args[0]);
				return true;
			}
		}
	}
}
