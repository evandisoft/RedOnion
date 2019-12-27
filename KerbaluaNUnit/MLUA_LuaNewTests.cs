using System;
using System.Collections.Generic;
using Kerbalua.Scripting;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using NUnit.Framework;

namespace KerbaluaNUnit
{
	[TestFixture()]
	public class MLUA_LuaNewTests
	{
		Script script;
		Table globals;

		void Setup()
		{
			script = new Script(CoreModules.Preset_Complete);
			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
			globals=script.Globals;
			globals["new"]=script.DoString(KerbaluaScript.LuaNew);
		}

		[Test]
		public void MLUA_LuaNewTests_01_HashSetNoArgs()
		{
			Setup();
			globals["HashSet"]=UserData.CreateStatic(typeof(HashSet<object>));
			Assert.IsInstanceOf(typeof(HashSet<object>), script.DoString("return new(HashSet)").ToObject());
		}

		[Test]
		public void MLUA_LuaNewTests_02_HashSetOneArg()
		{
			Setup();
			globals["HashSet"]=UserData.CreateStatic(typeof(HashSet<object>));
			Assert.IsInstanceOf(typeof(HashSet<object>), script.DoString("return new(HashSet)").ToObject());
		}
	}
}
