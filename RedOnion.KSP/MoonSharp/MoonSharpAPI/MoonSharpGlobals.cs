using System;
using System.ComponentModel;
using MoonSharp.Interpreter;

namespace RedOnion.KSP.MoonSharp.MoonSharpAPI
{
	/// <summary>
	/// These implementations are just dummies as the real implementations are tied
	/// to KerbaluaScript
	/// </summary>
	[Description("List of functions that are specific to KerbaluaScript.")]
	public static class MoonSharpGlobals
	{
		[Description("Function that acts as a constructor for ")]
#pragma warning disable IDE1006 // Naming Styles
		public static object @new(object obj, params DynValue[] dynArgs)
#pragma warning restore IDE1006 // Naming Styles
		{
			return null;
		}
	}
}
