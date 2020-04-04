using RedOnion.Attributes;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		/// <summary>
		/// static void Action()
		/// </summary>
		public class Action0 : Callable
		{
			internal static Value CreateValue(MethodInfo m)
				=> new Value(new Action0(m), CreateDelegate(m));
			internal static Delegate CreateDelegate(MethodInfo m)
				=> Delegate.CreateDelegate(typeof(Action), m);

			public Action0(MethodInfo m)
				: this(m.Name, m) { }
			public Action0(string name, MethodInfo m = null)
				: base(name, typeof(Action), false, m) { }

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
		public class Function0 : Callable
		{
			internal static Value CreateValue(MethodInfo m)
				=> new Value(new Function0(m), CreateDelegate(m));
			internal static Delegate CreateDelegate(MethodInfo m)
			{
				Type convert = null;
				var convertAttrs = m.ReturnTypeCustomAttributes
					.GetCustomAttributes(typeof(ConvertAttribute), true);
				if (convertAttrs.Length == 1)
					convert = ((ConvertAttribute)convertAttrs[0]).Type;
				return Expression.Lambda<Func<Value>>(GetNewValueExpression(
					convert ?? m.ReturnType, GetConvertExpression(
						Expression.Call(m), convert))).Compile();
			}

			public Function0(MethodInfo m)
				: this(m.Name, m) { }
			public Function0(string name, MethodInfo m = null)
				: base(name, typeof(Func<Value>), false, m) { }

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
		public class Procedure0<T> : Callable
		{
			public Procedure0(MethodInfo m)
				: this(m.Name, m) { }
			public Procedure0(string name, MethodInfo m = null)
				: base(name, typeof(Action<T>), true, m) { }

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
		public class Method0<T> : Callable
		{
			public Method0(MethodInfo m)
				: this(m.Name, m) { }
			public Method0(string name, MethodInfo m = null)
				: base(name, typeof(Func<T, Value>), true, m) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create || args.Length != 0)
					return false;
				result = ((Func<T, Value>)result.obj)((T)self);
				return true;
			}
		}
	}
}
