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

		public override Value Call(IObject self, Arguments args)
		{
			// TODO: better matching of float vs. int
			var result = new Value();
			foreach (MethodInfo method in Methods)
				if (TryCall(Engine, method, null, args, ref result))
					return result;
			if (!Engine.HasOption(EngineOption.Silent))
			{
#if DEBUG
				Engine.DebugLog("{0}(argc:{1})", Type == null ? Name : Type.Name + "." + Name, args.Length);
				for (int i = 0; i < args.Length; i++)
				{
					var arg = args[i];
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
			object self, Arguments args, ref Value result)
		{
			var pars = method.GetParameters();
			if (pars.Length != args.Length)
			{
				if (pars.Length < args.Length)
					return false;
				if (pars[args.Length].RawDefaultValue == DBNull.Value)
					return false;
			}
			for (int i = 0; i < args.Length; i++)
			{
				var param = pars[i];
				var arg = args[i];
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
			var call = new object[pars.Length];
			for (int i = 0; i < call.Length; i++)
			{
				var param = pars[i];
				var arg = args[i];
				var type = param.ParameterType;
				if (i >= args.Length)
					call[i] = param.DefaultValue;
				else if (type == typeof(string))
					call[i] = arg.String;
				else if (type.IsPrimitive || type.IsEnum)
					call[i] = System.Convert.ChangeType(arg.Native, type);
				else if (typeof(Delegate).IsAssignableFrom(type))
					call[i] = ((BasicObjects.FunctionObj)arg.Object).GetDelegate(type);
				else
				{
					var val = arg.Native;
					if (val == null)
					{
						call[i] = null;
						continue;
					}
					if (type.IsAssignableFrom(val.GetType()))
					{
						call[i] = val;
						continue;
					}
					call[i] = ((IObject)val).Target;
				}
			}
			var ctor = method as ConstructorInfo;
			if (ctor != null)
			{
				var native = ctor.Invoke(call);
				var creator = engine.Root[native.GetType()];
				result = new Value(creator.Convert(native));
				return true;
			}
			result = ReflectedType.Convert(
				engine, method.IsStatic
				? method.Invoke(null, call)
				: method.Invoke(self, call),
				((MethodInfo)method).ReturnType);
			return true;
		}
	}
}
