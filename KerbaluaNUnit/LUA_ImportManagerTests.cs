using NUnit.Framework;
using Kerbalua.Completion;
using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.Interpreter.Interop;
using Kerbalua.Other;
using RedOnion.KSP.Lua;
using Kerbalua.MoonSharp;
using UnityEngine;
using System.Reflection;
using RedOnion.KSP.API;

namespace KerbaluaNUnit {
	[TestFixture()]
	public class LUA_ImportManagerTests 
	{
		[Test()]
		public void LUA_ImportManager_1()
		{
			var namespace1 = new NamespaceInstance("System.Collections.Generic");
			foreach(var part in namespace1.PossibleCompletions)
			{
				Console.WriteLine(part);
			}
			foreach(var type in namespace1.TypesMap.Values)
			{
				Console.WriteLine(type.FullName);
			}
			Assert.AreEqual(typeof(HashSet<object>), namespace1.GetTypeFromNamespace("HashSet`1"));
			Assert.IsTrue(false);
		}
	}
}
