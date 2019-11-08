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
		internal class Action2 : Callable
		{
			internal static Value CreateValue(MethodInfo m, ParameterInfo[] args)
				=> new Value(new Action2(m), CreateDelegate(m, args));
			internal static Delegate CreateDelegate(MethodInfo m, ParameterInfo[] args)
				=> Expression.Lambda<Action<Value, Value>>(Expression.Call(m,
					GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
					GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter)),
					ValueArg0Parameter, ValueArg1Parameter).Compile();

			public Action2(MethodInfo m)
				: this(m.Name, m) { }
			public Action2(string name, MethodInfo m = null)
				: base(name, typeof(Action<Value, Value>), false, m) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create)
					return false;
				if (args.Length == 2)
				{
					((Action<Value, Value>)result.obj)(args[0], args[1]);
					result = Value.Void;
					return true;
				}
				if (MinArgs > args.Length || Params?.Length != 2 || args.Length > 2)
					return false;
				((Action<Value, Value>)result.obj)(
					args.Length == 0 ? new Value(Params[0].DefaultValue) : args[0],
					new Value(Params[1].DefaultValue));
				result = Value.Void;
				return true;
			}
		}
		/// <summary>
		/// static Value Function(Value arg0, Value arg1)
		/// </summary>
		internal class Function2 : Callable
		{
			internal static Value CreateValue(MethodInfo m, ParameterInfo[] args)
				=> new Value(new Function2(m), CreateDelegate(m, args));
			internal static Delegate CreateDelegate(MethodInfo m, ParameterInfo[] args)
			{
				Type convert = null;
				var convertAttrs = m.ReturnTypeCustomAttributes
					.GetCustomAttributes(typeof(ConvertAttribute), true);
				if (convertAttrs.Length == 1)
					convert = ((ConvertAttribute)convertAttrs[0]).Type;
				return Expression.Lambda<Func<Value, Value, Value>>(GetNewValueExpression(
					convert ?? m.ReturnType, GetConvertExpression(Expression.Call(m,
					GetValueConvertExpression(args[0].ParameterType, ValueArg0Parameter),
					GetValueConvertExpression(args[1].ParameterType, ValueArg1Parameter)),
					convert)), ValueArg0Parameter, ValueArg1Parameter).Compile();
			}

			public Function2(MethodInfo m)
				: this(m.Name, m) { }
			public Function2(string name, MethodInfo m = null)
				: base(name, typeof(Func<Value, Value, Value>), false, m) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create)
					return false;
				if (args.Length == 2)
				{
					result = ((Func<Value, Value, Value>)result.obj)(args[0], args[1]);
					return true;
				}
				if (MinArgs > args.Length || Params?.Length != 2 || args.Length > 2)
					return false;
				result = ((Func<Value, Value, Value>)result.obj)(
					args.Length == 0 ? new Value(Params[0].DefaultValue) : args[0],
					new Value(Params[1].DefaultValue));
				return true;
			}
		}
		/// <summary>
		/// void Procedure(Value arg0, Value arg1)
		/// </summary>
		public class Procedure2<T> : Callable
		{
			public Procedure2(MethodInfo m)
				: this(m.Name, m) { }
			public Procedure2(string name, MethodInfo m = null)
				: base(name, typeof(Action<T, Value, Value>), true, m) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create)
					return false;
				if (args.Length == 2)
				{
					((Action<T, Value, Value>)result.obj)((T)self, args[0], args[1]);
					result = Value.Void;
					return true;
				}
				if (MinArgs > args.Length || IsDelegate || Params?.Length != 2 || args.Length > 2)
					return false;
				((Action<T, Value, Value>)result.obj)((T)self,
					args.Length == 0 ? new Value(Params[0].DefaultValue) : args[0],
					new Value(Params[1].DefaultValue));
				result = Value.Void;
				return true;
			}
		}
		/// <summary>
		/// Value Method(Value arg0, Value arg1)
		/// </summary>
		public class Method2<T> : Callable
		{
			public Method2(MethodInfo m)
				: this(m.Name, m) { }
			public Method2(string name, MethodInfo m = null)
				: base(name, typeof(Func<T, Value, Value>), true, m) { }

			public override bool Call(ref Value result, object self, Arguments args, bool create)
			{
				if (create)
					return false;
				if (args.Length == 2)
				{
					result = ((Func<T, Value, Value, Value>)result.obj)((T)self, args[0], args[1]);
					return true;
				}
				if (MinArgs > args.Length || IsDelegate || Params?.Length != 2 || args.Length > 2)
					return false;
				result = ((Func<T, Value, Value, Value>)result.obj)((T)self,
					args.Length == 0 ? new Value(Params[0].DefaultValue) : args[0],
					new Value(Params[1].DefaultValue));
				return true;
			}
		}
	}
}
