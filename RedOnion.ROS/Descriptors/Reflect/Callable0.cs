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
		public class Action0 : Descriptor
		{
			internal static Value CreateValue(MethodInfo m)
				=> new Value(new Action0(m.Name), CreateDelegate(m));
			internal static Delegate CreateDelegate(MethodInfo m)
				=> Delegate.CreateDelegate(typeof(Action), m);

			public Action0(string name)
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
		public class Function0 : Descriptor
		{
			internal static Value CreateValue(MethodInfo m)
				=> new Value(new Function0(m.Name), CreateDelegate(m));
			internal static Delegate CreateDelegate(MethodInfo m)
				=> Expression.Lambda<Func<Value>>(GetNewValueExpression(
					m.ReturnType, Expression.Call(m))).Compile();

			public Function0(string name)
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
		public class Procedure0<T> : Descriptor
		{
			public Procedure0(string name) : base(name, typeof(Action<T>)) { }

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
		public class Method0<T> : Descriptor
		{
			public Method0(string name) : base(name, typeof(Func<T, Value>)) { }

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
