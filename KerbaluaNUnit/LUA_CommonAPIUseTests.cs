using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using NUnit.Framework;
using RedOnion.KSP.MoonSharp.CommonAPI;
using static KerbaluaNUnit.CommonAPITestUtil;

namespace KerbaluaNUnit
{
	[TestFixture()]
	public class LUA_CommonAPIUseTests
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
			Table metatable=new Table(script);
			metatable["__index"]=apiTable;
			globals.MetaTable=metatable;
		}

		[Test()]
		public void LUA_CommonAPIInstantiationTests_Creation()
		{
			Setup();
		}

		[Test()]
		public void LUA_CommonAPIInstantiationTests_Field()
		{
			Setup();
			StaticCheck(globals, "pid");
		}

		[Test()]
		public void LUA_CommonAPIInstantiationTests_NamespaceField()
		{
			Setup();
			var t1=NamespaceCheck(globals,"ui");
			StaticCheck(t1, "Color");
		}

		[Test()]
		public void LUA_CommonAPIInstantiationTests_NamespaceMethod()
		{
			Setup();
			var t1=NamespaceCheck(globals,"autorun"); ;
			MemberCheck(t1, "save", typeof(CallbackFunction));
		}
	}
}
