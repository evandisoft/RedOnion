using System;
using System.ComponentModel;
using MoonSharp.Interpreter;
using RedOnion.KSP.Attributes;

namespace RedOnion.KSP.MoonSharp.MoonSharpAPI
{
	/// <summary>
	/// These implementations are just dummies as the real implementations are tied
	/// to KerbaluaScript
	/// </summary>
	[Description("List of functions that are specific to KerbaluaScript.")]
	public static class MoonSharpGlobals
	{
		[Unsafe, Description("Create new objects given a type or static in lua followed by the arguments to the constructor.")]
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

		[Unsafe,Description("Unsafe, reflection stuff.")]
		public static Type reflection=typeof(MoonSharpReflection);
	}

	[Description("List of functions that are specific to KerbaluaScript.")]
	public static class MoonSharpReflection
	{
		[Description("Returns true if the argument is a static.")]
		public static bool isstatic(DynValue dynValue)
		{
			return dynValue.UserData!=null && dynValue.UserData.Object==null;
		}

		[Description("Returns true if the argument is a Type.")]
		public static bool isclrtype(DynValue dynValue)
		{
			return dynValue.UserData!=null && dynValue.UserData.Object!=null && dynValue.ToObject() is Type;
		}

		[Description("Returns a static based on the given Type or object.")]
		public static DynValue getstatic(DynValue dynValue)
		{
			object o=dynValue.ToObject();
			if (o is Type t)
			{
				return UserData.CreateStatic(t);
			}
			return UserData.CreateStatic(o.GetType());
		}

		[Description("Returns the underyling clr type associated with this DynValue.")]
		public static object getclrtype(DynValue dynValue)
		{
			//if (dynValue.Type==DataType.UserData)
			//{
			//	return DynValue.FromObject(this, dynValue.UserData.Descriptor.Type);
			//}
			object o=dynValue.ToObject();
			if (o is Type t)
			{
				return t; //DynValue.FromObject(this, t);
			}
			return o.GetType();//DynValue.FromObject(this, o.GetType());
		}
	}
}
