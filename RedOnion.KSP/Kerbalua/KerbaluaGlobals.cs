using System;
using System.ComponentModel;
using System.Reflection;
using MunSharp.Interpreter;
using MunSharp.Interpreter.Interop;
using RedOnion.Attributes;

namespace RedOnion.KSP.Kerbalua
{
	/// <summary>
	/// These implementations are just dummies as the real implementations are tied
	/// to KerbaluaScript
	/// </summary>
	[Description("Functionality that is specific to Kerbalua.")]
	public static class KerbaluaGlobals
	{
		// this will be overriden in KerbaluaScript.cs
		[Description(
		"Create new objects given a type. For example: `new(ui.Window)`")]
		public static object @new(DynValue @static, params DynValue[] dynArgs)
		{
			return null;
		}

		//This will be overridden in KerbaluaScript.cs
		[Description("Causes the script to wait, for the given number of seconds. "
		+"This is not a precise timing mechanism. Sleeping for 0 seconds will cause the script "
			+"to wait until the next unity FixedUpdate")]
		public static void sleep(double seconds)
		{

		}

		//This will be overridden in KerbaluaScript.cs
		[Description("Executes the file with the given filename. The base directory is" +
			" KSPDir/GameData/RedOnion/Scripts.")]
		public static DynValue dofile(string filename)
		{
			return null;
		}

		[Unsafe,Description("Unsafe, Kerbalua specific reflection stuff.")]
		public static readonly Type reflection=typeof(Reflection);
	}

	[Description("Reflection functionality specific to Moonsharp.")]
	public static class Reflection
	{
		[Description("Returns true if the argument is a type.")]
		public static bool istype(DynValue possibleType)
		{
			return possibleType.UserData!=null && possibleType.UserData.Object==null;
		}

		[Description("Returns true if the argument is a runtime type. (which can be used for reflection).")]
		public static bool isruntimetype(DynValue possibleRuntimeType)
		{
			return possibleRuntimeType.UserData!=null && possibleRuntimeType.UserData.Object!=null && possibleRuntimeType.ToObject() is Type;
		}

		[Description("Returns a type given a runtime type or object.")]
		public static DynValue gettype(DynValue runtimeTypeOrObject)
		{
			object o=runtimeTypeOrObject.ToObject();
			if (o is Type t)
			{
				return UserData.CreateStatic(t);
			}
			return UserData.CreateStatic(o.GetType());
		}

		[Description("Returns the runtime type associated with this object or type. runtime types can be used for reflection.")]
		public static object getruntimetype(DynValue objectOrType)
		{
			//if (dynValue.Type==DataType.UserData)
			//{
			//	return DynValue.FromObject(this, dynValue.UserData.Descriptor.Type);
			//}
			object o=objectOrType.ToObject();
			if (o is Type t)
			{
				return t; //DynValue.FromObject(this, t);
			}
			return o.GetType();//DynValue.FromObject(this, o.GetType());
		}

		[Description("Returns the descriptor of the given object, null if one is not avaialble.")]
		public static IUserDataDescriptor getdescriptor(DynValue @object)
		{
			return @object.UserData?.Descriptor;
		}
	}
}
