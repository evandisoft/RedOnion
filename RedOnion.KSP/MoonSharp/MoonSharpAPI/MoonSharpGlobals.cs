using System;
using System.ComponentModel;
using System.Reflection;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.Attributes;

namespace RedOnion.KSP.MoonSharp.MoonSharpAPI
{
	/// <summary>
	/// These implementations are just dummies as the real implementations are tied
	/// to KerbaluaScript
	/// </summary>
	[Description("Functionality that is specific to Kerbalua.")]
	public static class MoonSharpGlobals
	{
		// this will be overriden in KerbaluaScript.cs
		[Description(
		"Create new objects given a static. A static is a special MunSharp value that represents a CLR Class." +
		"You can access static members from them, including the __new member, which represents the CLR Class' constructor." +
		"We provide references to many of these. For example, ui.Window, or any CLR Class received via the " +
		"native global.")]
		public static object @new(object type, params DynValue[] dynArgs)
		{
			return null;
		}

		//This will be overriden in KerbaluaScript.cs
		[Description("Causes the script to wait, for the given number of seconds. "
		+"This is not a precise timing mechanism. Sleeping for 0 seconds will cause the script "
			+"to wait until the next unity FixedUpdate")]
		public static void sleep(double seconds)
		{

		}

		[Unsafe,Description("Unsafe, Kerbalua specific reflection stuff.")]
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

		[Description("Returns the descriptor of the given object, null if one is not avaialble.")]
		public static IUserDataDescriptor getdescriptor(DynValue @object)
		{
			return @object.UserData?.Descriptor;
		}
	}
}
