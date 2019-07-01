using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public abstract class Callable : Descriptor
		{
			/// <summary>
			/// Instance method vs. static function
			/// </summary>
			public bool IsMethod { get; protected set; }
			/// <summary>
			/// Minimal number of arguments (number of required arguments)
			/// </summary>
			public int MinArgs { get; protected set; }
			/// <summary>
			/// Maximal number of arguments (number of all possible arguments)
			/// </summary>
			public int MaxArgs { get; protected set; }
			/// <summary>
			/// Method info (optional - may be null)
			/// </summary>
			public MethodInfo Info { get; protected set; }
			/// <summary>
			/// Parameter list (optional - may be null)
			/// </summary>
			public ParameterInfo[] Params { get; protected set; }

			protected Callable(string name, Type type, bool method, MethodInfo invoke = null)
				: base(name, type)
			{
				if (invoke == null)
				{
					if (!type.IsSubclassOf(typeof(Delegate)))
					{
						MaxArgs = int.MaxValue;
						return;
					}
					Info = type.GetMethod("Invoke");
					Params = Info.GetParameters();
					MaxArgs = Params.Length;
					if (IsMethod = method)
						MaxArgs--;
					MinArgs = MaxArgs;
					return;
				}
				Info = invoke;
				Params = invoke.GetParameters();
				IsMethod = method;
				MaxArgs = Params.Length;
				while (MinArgs < MaxArgs && Params[MinArgs].RawDefaultValue == DBNull.Value)
					MinArgs++;
				if (method)
				{
					MaxArgs--;
					if (MinArgs > 0)
						MinArgs--;
				}
			}

			public static Descriptor FromType(Type type)
			{
				if (type.IsSubclassOf(typeof(Delegate)))
				{
					var info = type.GetMethod("Invoke");
					var pars = info.GetParameters();
					if (pars.Length <= 3)
					{
						bool valueArgs = true;
						foreach (var pnfo in pars)
						{
							if (pnfo.ParameterType != typeof(Value))
							{
								valueArgs = false;
								break;
							}
						}
						if (valueArgs)
						{
							if (info.ReturnType == typeof(void))
								return Actions[pars.Length];
							if (info.ReturnType == typeof(Value))
								return Functions[pars.Length];
						}
					}
				}
				return new Reflected(type);
			}

			public static bool TryCall(
				MethodBase method,
				ref Value result, object self, Arguments args)
			{
				try
				{
					var pars = method.GetParameters();
					int parsFrom = 0;
					if (pars.Length > 0 && pars[0].ParameterType == typeof(IProcessor))
						parsFrom = 1;
					if (pars.Length-parsFrom != args.Length)
					{
						if (pars.Length-parsFrom < args.Length)
							return false;
						if (pars[parsFrom+args.Length].RawDefaultValue == DBNull.Value)
							return false;
					}

					for (int i = 0; i < args.Length; i++)
					{
						var param = pars[parsFrom+i];
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
						/* TODO: delegates / functions
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
						*/
						var val = arg.obj;
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
					if (parsFrom > 0)
						call[0] = args.Processor;
					for (int i = parsFrom; i < call.Length; i++)
					{
						var param = pars[i];
						if (i >= args.Length)
							call[i] = param.DefaultValue;
						else
						{
							var arg = args[i-parsFrom];
							var type = param.ParameterType;
							if (type == typeof(string))
								call[i] = arg.ToStr();
							else if (type.IsPrimitive || type.IsEnum)
								call[i] = System.Convert.ChangeType(arg.Box(), type);
							//else if (typeof(Delegate).IsAssignableFrom(type))
							//	call[i] = ((BasicObjects.FunctionObj)arg.Object).GetDelegate(type);
							else
								call[i] = arg.obj;
						}
					}

					if (method is ConstructorInfo ctor)
					{
						result = new Value(ctor.Invoke(call));
						return true;
					}
					result = new Value(method.IsStatic
						? method.Invoke(null, call)
						: method.Invoke(self, call));
					return true;
				}
				catch (Exception ex)
				{
					Value.DebugLog("Exception {0} when trying to call {1}.{2} ({3} v.s. {4} args): {5}",
						ex.GetType().Name, method.DeclaringType.Name, method.Name,
						method.GetParameters().Length, args.Length, ex.Message);
				}
				return false;
			}
		}
	}
}
