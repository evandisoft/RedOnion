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
	[Description("Functionality that is specific to Kerbalua.")]
	public static class MoonSharpGlobals
	{
		[Unsafe, Description("Create new objects given a type, static, or object, in lua followed by the arguments to the constructor.")]
		public static object @new(object typeStaticOrObject, params DynValue[] constructorArgs)
		{
			Type type=typeStaticOrObject as Type;
			if (type==null)
			{
				type=typeStaticOrObject.GetType();
			}
			var constructors = type.GetConstructors();
			foreach (var constructor in constructors)
			{
				var parinfos = constructor.GetParameters();
				if (parinfos.Length >= constructorArgs.Length)
				{
					object[] objArgs = new object[parinfos.Length];

					for (int i = 0; i < objArgs.Length; i++)
					{
						var parinfo = parinfos[i];
						if (i>= constructorArgs.Length)
						{
							if (!parinfo.IsOptional)
							{
								goto nextConstructor;
							}
							objArgs[i] = parinfo.DefaultValue;
						}
						else
						{
							if (parinfo.ParameterType.IsValueType)
							{
								try
								{
									objArgs[i] = System.Convert.ChangeType(constructorArgs[i].ToObject(), parinfo.ParameterType);
								}
								catch (Exception)
								{
									goto nextConstructor;
								}
							}
							else
							{
								objArgs[i] = constructorArgs[i].ToObject();
							}
						}

					}

					return constructor.Invoke(objArgs);
				}
			nextConstructor:;
			}

			if (constructorArgs.Length == 0)
			{
				return Activator.CreateInstance(type);
			}
			throw new Exception("Could not find constructor accepting given args for type " + type);
		}

		[Unsafe,Description("Unsafe, reflection stuff.")]
		public static readonly Type reflection=typeof(Reflection);
	}

	[Description("Reflection functionality specific to Moonsharp.")]
	public static class Reflection
	{
		[Description("Returns true if the argument is a static.")]
		public static bool isstatic(DynValue possbileStatic)
		{
			return possbileStatic.UserData!=null && possbileStatic.UserData.Object==null;
		}

		[Description("Returns true if the argument is a Type.")]
		public static bool isclrtype(DynValue possibleType)
		{
			return possibleType.UserData!=null && possibleType.UserData.Object!=null && possibleType.ToObject() is Type;
		}

		[Description("Returns a static based on the given Type or object.")]
		public static DynValue getstatic(DynValue typeOrObject)
		{
			object o=typeOrObject.ToObject();
			if (o is Type t)
			{
				return UserData.CreateStatic(t);
			}
			return UserData.CreateStatic(o.GetType());
		}

		[Description("Returns the underyling clr type associated with this object.")]
		public static object getclrtype(DynValue @object)
		{
			//if (dynValue.Type==DataType.UserData)
			//{
			//	return DynValue.FromObject(this, dynValue.UserData.Descriptor.Type);
			//}
			object o=@object.ToObject();
			if (o is Type t)
			{
				return t; //DynValue.FromObject(this, t);
			}
			return o.GetType();//DynValue.FromObject(this, o.GetType());
		}
	}
}
