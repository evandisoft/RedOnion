using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using NUnit.Framework;
using RedOnion.KSP.MoonSharp.CommonAPI;
using UnityEngine;

namespace KerbaluaNUnit
{
	[TestFixture()]
	public class LUA_CommonAPIInstantiationTests
	{
		Script script;
		Table globals;
		CommonAPICreator creator;
		Table apiTable;

		void Setup()
		{
			script = new Script(CoreModules.Preset_Complete);
			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
			globals = script.Globals;
			creator=new CommonAPICreator(script);
			apiTable=creator.Create();
		}

		[Test()]
		public void LUA_CommonAPIInstantiationTests_Creation()
		{
			Setup();
		}

		[Test()]
		public void LUA_CommonAPIInstantiationTests_NamespaceField()
		{
			Setup();
			var t1=apiTable["ui"] as Table;
			Assert.NotNull(t1);
			var f=t1["Color"] as Type;
			Assert.NotNull(f);
			//Assert.IsInstanceOf<Color>(f);
		}

		[Test()]
		public void LUA_CommonAPIInstantiationTests_NamespaceMethod()
		{
			Setup();
			var t1=apiTable["autorun"] as Table;
			Assert.NotNull(t1);
			var c=t1["save"] as CallbackFunction;
			Assert.NotNull(c);
		}
	}
}
