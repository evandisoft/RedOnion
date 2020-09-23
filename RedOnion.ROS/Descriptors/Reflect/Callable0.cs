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

			public override bool Call(ref Value result, object self, in Arguments args)
			{
				if (args.Length != 0)
					return false;
				((Action)result.obj)();
				result = Value.Void;
				return true;
			}

			/// <summary>
			/// static void Action&lt;T&gt;() converted to void Action(Value type)
			/// </summary>
			public class Gen1 : Callable
			{
				public Gen1(MethodInfo m)
					: this(m.Name, m) { }
				public Gen1(string name, MethodInfo m = null)
					: base(name, typeof(MethodInfo), true, m) { }

				protected Type cachedType;
				protected MethodInfo cachedInfo;
				public override bool Call(ref Value result, object self, in Arguments args)
				{
					if (args.Length != 1)
						return false;
					if (!(args[0].obj is Type type))
						return false;
					if (type != cachedType)
						cachedInfo = Info.MakeGenericMethod(cachedType = type);
					cachedInfo.Invoke(null, new object[0]);
					result = Value.Void;
					return true;
				}
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
				Type convert = ConvertAttribute.Get(m);
				return Expression.Lambda<Func<Value>>(GetNewValueExpression(
					convert ?? m.ReturnType, GetConvertExpression(
						Expression.Call(m), convert))).Compile();
			}

			public Function0(MethodInfo m)
				: this(m.Name, m) { }
			public Function0(string name, MethodInfo m = null)
				: base(name, typeof(Func<Value>), false, m) { }

			public override bool Call(ref Value result, object self, in Arguments args)
			{
				if (args.Length != 0)
					return false;
				result = ((Func<Value>)result.obj)();
				return true;
			}

			/// <summary>
			/// static Value Function&lt;T&gt;() converted to Value Function(Value type)
			/// </summary>
			public class Gen1 : Callable
			{
				public Gen1(MethodInfo m)
					: this(m.Name, m) { }
				public Gen1(string name, MethodInfo m = null)
					: base(name, typeof(MethodInfo), true, m) { }

				protected Type cachedType;
				protected MethodInfo cachedInfo;
				public override bool Call(ref Value result, object self, in Arguments args)
				{
					if (args.Length != 1)
						return false;
					if (!(args[0].obj is Type type))
						return false;
					if (type != cachedType)
						cachedInfo = Info.MakeGenericMethod(cachedType = type);
					result = new Value(cachedInfo.Invoke(null, new object[0]));
					return true;
				}
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

			public override bool Call(ref Value result, object self, in Arguments args)
			{
				if (args.Length != 0)
					return false;
				((Action<T>)result.obj)((T)self);
				result = Value.Void;
				return true;
			}

			/// <summary>
			/// void Procedure&lt;T&gt;() converted to void Procedure(Value type)
			/// </summary>
			public class Gen1 : Callable
			{
				public Gen1(MethodInfo m)
					: this(m.Name, m) { }
				public Gen1(string name, MethodInfo m = null)
					: base(name, typeof(MethodInfo), true, m) { }

				protected Type cachedType;
				protected MethodInfo cachedInfo;
				public override bool Call(ref Value result, object self, in Arguments args)
				{
					if (args.Length != 1)
						return false;
					if (!(args[0].obj is Type type))
						return false;
					if (type != cachedType)
						cachedInfo = Info.MakeGenericMethod(cachedType = type);
					cachedInfo.Invoke(self, new object[0]);
					result = Value.Void;
					return true;
				}
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

			public override bool Call(ref Value result, object self, in Arguments args)
			{
				if (args.Length != 0)
					return false;
				result = ((Func<T, Value>)result.obj)((T)self);
				return true;
			}

			/// <summary>
			/// Value Method&lt;T&gt;() converted to Value Method(Value type)
			/// </summary>
			public class Gen1 : Callable
			{
				public Gen1(MethodInfo m)
					: this(m.Name, m) { }
				public Gen1(string name, MethodInfo m = null)
					: base(name, typeof(MethodInfo), true, m) { }

				protected Type cachedType;
				protected MethodInfo cachedInfo;
				public override bool Call(ref Value result, object self, in Arguments args)
				{
					if (args.Length != 1)
						return false;
					if (!(args[0].obj is Type type))
						return false;
					if (type != cachedType)
						cachedInfo = Info.MakeGenericMethod(cachedType = type);
					result = new Value(cachedInfo.Invoke(self, new object[0]));
					return true;
				}
			}
		}
	}
}
