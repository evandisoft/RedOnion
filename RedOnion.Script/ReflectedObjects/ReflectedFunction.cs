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
		public override Type Type => _type;
		private Type _type;

		public override ObjectFeatures Features
			=> ObjectFeatures.Function;

		/// <summary>
		/// Function name (static method)
		/// </summary>
		public override string Name { get; }

		/// <summary>
		/// Discovered methods with same name
		/// </summary>
		protected internal MethodInfo[] Methods { get; }

		public ReflectedFunction(
			IEngine engine, ReflectedType creator,
			string name, params MethodInfo[] methods)
			: base(engine, null)
		{
			Creator = creator;
			_type = creator?.Type;
			Name = name;
			Methods = methods;
		}

		public override Value Call(IObject self, int argc)
		{
			var result = new Value();
			foreach (MethodInfo method in Methods)
				if (TryCall(Engine, method, null, argc, ref result))
					return result;
			if (!Engine.HasOption(EngineOption.Silent))
			{
#if DEBUG
				Engine.DebugLog("{0}(argc:{1})", Type == null ? Name : Type.Name + "." + Name, argc);
				for (int i = 0; i < argc; i++)
				{
					var arg = Arg(argc, i);
					var native = arg.Native;
					Engine.DebugLog("#{0} {1} -> {2} {4}", i, arg.Kind, arg.String,
						native?.GetType().FullName ?? "null", native?.ToString() ?? "null");
				}
				var sb = new System.Text.StringBuilder();
				foreach (MethodInfo method in Methods)
				{
					foreach(ParameterInfo pi in method.GetParameters())
					{
						if (sb.Length > 0) sb.Append(", ");
						sb.AppendFormat("{0} {1}", pi.Name, pi.ParameterType.FullName);
						var def = pi.RawDefaultValue;
						if (def != DBNull.Value)
						{
							sb.Append(" = ");
							sb.Append(def == null ? "null" : def.ToString());
						}
					}
					Engine.DebugLog(sb.ToString());
					sb.Length = 0;
				}
#endif
				throw new InvalidOperationException(
					"Could not call " + (Type == null ? Name
					: Type.Name + "." + Name)
					+ ", " + Methods.Length + " candidates");
			}
			return result;
		}

		protected internal static bool TryCall(
			IEngine engine, MethodBase method,
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
				var arg = engine.GetArgument(argc, i);
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
					if (arg.Object is BasicObjects.FunctionObj fn)
					{
						var invoke = type.GetMethod("Invoke");
						var mipars = invoke.GetParameters();
						if (mipars.Length != fn.ArgumentCount)
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
				if (type.IsGenericParameter)
				{
					if (!method.IsGenericMethod)
						return false;
					// TODO: generic methods with multiple parameters
					if (method.GetGenericArguments().Length != 1)
						return false;
					try
					{
						method = ((MethodInfo)method).MakeGenericMethod(val.GetType());
						pars = method.GetParameters();
						continue;
					}
					catch
					{
						return false;
					}
				}
				return false;
			}
			var args = new object[pars.Length];
			for (int i = 0; i < args.Length; i++)
			{
				var param = pars[i];
				var arg = engine.GetArgument(argc, i);
				var type = param.ParameterType;
				if (i >= argc)
					args[i] = param.DefaultValue;
				else if (type == typeof(string))
					args[i] = arg.String;
				else if (type.IsPrimitive || type.IsEnum)
					args[i] = System.Convert.ChangeType(arg.Native, type);
				else if (typeof(Delegate).IsAssignableFrom(type))
					args[i] = ((BasicObjects.FunctionObj)arg.Object).GetDelegate(type);
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
					args[i] = ((IObject)val).Target;
				}
			}
			var ctor = method as ConstructorInfo;
			if (ctor != null)
			{
				var creator = self as ReflectedType;
				result = new Value(creator != null ? creator.Convert(ctor.Invoke(args))
					: new ReflectedObject(engine, ctor.Invoke(args), creator, creator?.TypeProps));
				return true;
			}
			result = ReflectedType.Convert(
				engine, method.IsStatic
				? method.Invoke(null, args)
				: method.Invoke(self, args),
				((MethodInfo)method).ReturnType);
			return true;
		}
	}
}
