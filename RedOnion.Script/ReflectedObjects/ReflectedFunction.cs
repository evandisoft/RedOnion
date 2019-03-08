using System;
using System.Reflection;

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
			return result;
		}

		protected internal static bool TryCall(
			Engine engine, MethodInfo method,
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
			result = ReflectedType.Convert(
				engine, method.IsStatic
				? method.Invoke(null, args)
				: method.Invoke(self, args),
				method.ReturnType);
			return true;
		}
	}
}
