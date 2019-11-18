using System;
using System.ComponentModel;
using MoonSharp.Interpreter;

namespace RedOnion.KSP.MoonSharp.MoonSharpAPI
{
	[Description("Global variables, objects and functions.")]
	public static class MoonSharpGlobals
	{
		//		commonAPI["new"] = new DelegateTypeNew(New);
		//		commonAPI["static"] = new Func<object, DynValue>((o) =>
		//			{
		//				if (o is Type t)
		//				{
		//					return UserData.CreateStatic(t);
		//				}
		//				return UserData.CreateStatic(o.GetType());
		//			});

		//			commonAPI["gettype"] = new Func<object, DynValue>((o) =>
		//			{
		//				if (o is DynValue d && d.Type==DataType.UserData)
		//				{
		//					return DynValue.FromObject(this, d.UserData.Descriptor.Type);
		//}
		//				if (o is Type t)
		//				{
		//					return DynValue.FromObject(this, t);
		//}
		//				return DynValue.FromObject(this, o.GetType());
		//});

		//delegate object DelegateTypeNew(object obj, params DynValue[] args);
		[Description("Function for creating new objects given a type or static in lua.")]
		public static object @new(object obj, params DynValue[] dynArgs)
		{
			Type type=obj as Type;
			var constructors = type.GetConstructors();
			foreach (var constructor in constructors)
			{
				var parinfos = constructor.GetParameters();
				if (parinfos.Length >= dynArgs.Length)
				{
					object[] args = new object[parinfos.Length];

					for (int i = 0; i < args.Length; i++)
					{
						var parinfo = parinfos[i];
						if (i>= dynArgs.Length)
						{
							if (!parinfo.IsOptional)
							{
								goto nextConstructor;
							}
							args[i] = parinfo.DefaultValue;
						}
						else
						{
							if (parinfo.ParameterType.IsValueType)
							{
								try
								{
									args[i] = System.Convert.ChangeType(dynArgs[i].ToObject(), parinfo.ParameterType);
								}
								catch (Exception)
								{
									goto nextConstructor;
								}
							}
							else
							{
								args[i] = dynArgs[i].ToObject();
							}
						}

					}

					return constructor.Invoke(args);
				}
			nextConstructor:;
			}

			if (dynArgs.Length == 0)
			{
				return Activator.CreateInstance(type);
			}
			throw new Exception("Could not find constructor accepting given args for type " + type);
		}
	}
}
