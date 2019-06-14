using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public abstract class Callable : Descriptor
		{
			public bool IsMethod { get; protected set; }
			public int MinArgs { get; protected set; }
			public int MaxArgs { get; protected set; }
			public MethodInfo Info { get; protected set; }
			protected ParameterInfo[] Params { get; set; }

			protected Callable(string name, Type type, bool method)
				: base(name, type)
			{
				if (!type.IsAssignableFrom(typeof(Delegate)))
				{
					MaxArgs = int.MaxValue;
					return;
				}
				Info = type.GetMethod("Invoke");
				Params = Info.GetParameters();
				MaxArgs = Params.Length;
				if (IsMethod = method)
					MaxArgs--;
			}
			internal Callable(string name, Type type, bool method, MethodInfo invoke)
				: base(name, type)
			{
				Info = invoke;
				Params = invoke.GetParameters();
				MaxArgs = Params.Length;
				if (IsMethod = method)
					MaxArgs--;
			}

			public static Descriptor FromType(Type type)
			{
				if (type.IsAssignableFrom(typeof(Delegate)))
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
		}
	}
}
