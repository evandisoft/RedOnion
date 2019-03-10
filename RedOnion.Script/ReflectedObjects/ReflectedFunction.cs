using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace RedOnion.Script.ReflectedObjects
{
	public class ReflectedFunction : BasicObjects.SimpleObject
	{
		/// <summary>
		/// Reflected type this function belongs to (may be null)
		/// </summary>
		public ReflectedType Creator { get; }

		/// <summary>
		/// Class/type this function belongs to (may be null)
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Function name (static method)
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Discovered methods with same name
		/// </summary>
		protected MethodInfo[] Methods { get; }

		public ReflectedFunction(
			Engine engine, ReflectedType creator,
			string name, MethodInfo[] methods)
			: base(engine, null)
		{
			Creator = creator;
			Type = creator?.Type;
			Name = name;
			Methods = methods;
		}

		public override Value Call(IObject self, int argc)
		{
			var result = new Value();
			foreach (MethodInfo method in Methods)
				if (TryCall(Engine, method, null, argc, ref result))
					return result;
			if (!Engine.HasOption(Engine.Option.Silent))
				throw new InvalidOperationException(
					"Could not call " + (Type == null ? Name
					: Type.Name + "." + Name)
					+ ", " + Methods.Length + " candidates");
			return result;
		}

		protected internal static bool TryCall(
			Engine engine, MethodBase method,
			object self, int argc, ref Value result)
		{
			var pars = method.GetParameters();
			if (pars.Length != argc)
			{
				if (pars.Length < argc)
					return false;
				if (pars[argc].RawDefaultValue == DBNull.Value)
					return false;
			}
			for (int i = 0; i < argc; i++)
			{
				var param = pars[i];
				var arg = engine.Args.Arg(argc, i);
				var type = param.ParameterType;
				if (type == typeof(string))
				{
					if (arg.IsString)
						continue;
					return false;
				}
				if (type.IsPrimitive || type.IsEnum)
				{
					if (arg.IsNumber)
						continue;
					return false;
				}
				if (typeof(Delegate).IsAssignableFrom(type))
				{
					if (arg.RValue.Deref is BasicObjects.FunctionObj fn)
					{
						var invoke = type.GetMethod("Invoke");
						var mipars = invoke.GetParameters();
						if (mipars.Length != fn.ArgCount)
							return false;
						continue;
					}
					return false;
				}
				var val = arg.Native;
				if (val == null)
					continue;
				if (type.IsAssignableFrom(val.GetType()))
					continue;
				if (val is IObjectProxy proxy)
				{
					val = proxy.Target;
					if (val == null)
						continue;
					if (type.IsAssignableFrom(val.GetType()))
						continue;
				}
				return false;
			}
			var args = new object[pars.Length];
			for (int i = 0; i < args.Length; i++)
			{
				var param = pars[i];
				var arg = engine.Args.Arg(argc, i);
				var type = param.ParameterType;
				if (i >= argc)
					args[i] = param.DefaultValue;
				else if (type == typeof(string))
					args[i] = arg.String;
				else if (type.IsPrimitive || type.IsEnum)
					args[i] = Convert.ChangeType(arg.Native, type);
				else if (typeof(Delegate).IsAssignableFrom(type))
				{
					var fn = (BasicObjects.FunctionObj)arg.RValue.Deref;
					var invoke = type.GetMethod("Invoke");
					var mipars = invoke.GetParameters();
					var fnargs = new ParameterExpression[mipars.Length];
					for (int j = 0; j < mipars.Length; j++)
						fnargs[j] = Expression.Parameter(mipars[j].ParameterType, fn.ArgName(j));
					var chargs = new Expression[mipars.Length];
					for (int j = 0; j < mipars.Length; j++)
						chargs[j] = Expression.Convert(fnargs[j], typeof(object));
					// (x, y, ...) => FunctionCallHelper<T>(fn, new object[] { x, y, ... })
					var lambda = Expression.Call(
						invoke.ReturnType == typeof(void)
						? typeof(ReflectedFunction).GetMethod("ActionCallHelper")
						: typeof(ReflectedFunction).GetMethod("FunctionCallHelper")
						.MakeGenericMethod(invoke.ReturnType),
						Expression.Constant(fn),
						Expression.NewArrayInit(typeof(object), chargs));
					args[i] = Expression.Lambda(type, lambda, fnargs).Compile();
				}
				else
				{
					var val = arg.Native;
					if (val == null)
					{
						args[i] = null;
						continue;
					}
					if (type.IsAssignableFrom(val.GetType()))
					{
						args[i] = val;
						continue;
					}
					args[i] = ((IObjectProxy)val).Target;
				}
			}
			var ctor = method as ConstructorInfo;
			if (ctor != null)
			{
				var creator = self as ReflectedType;
				result = new Value(new ReflectedObject(engine, ctor.Invoke(args), creator, creator?.TypeProps));
				return true;
			}
			result = ReflectedType.Convert(
				engine, method.IsStatic
				? method.Invoke(null, args)
				: method.Invoke(self, args),
				((MethodInfo)method).ReturnType);
			return true;
		}

		public static void ActionCallHelper(BasicObjects.FunctionObj fn, params object[] args)
		{
			var engine = fn.Engine;
			var engargs = engine.Args;
			foreach (var arg in args)
				engargs.Add(ReflectedType.Convert(engine, arg));
			try
			{
				fn.Call(null, args.Length);
			}
			finally
			{
				engargs.Remove(args.Length);
			}
		}
		public static T FunctionCallHelper<T>(BasicObjects.FunctionObj fn, params object[] args)
		{
			var engine = fn.Engine;
			var engargs = engine.Args;
			foreach (var arg in args)
				engargs.Add(ReflectedType.Convert(engine, arg));
			try
			{
				return ReflectedType.Convert<T>(fn.Call(null, args.Length));
			}
			finally
			{
				engargs.Remove(args.Length);
			}
		}
	}
}
