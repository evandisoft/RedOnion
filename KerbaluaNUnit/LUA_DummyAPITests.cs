using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using NUnit.Framework;
using RedOnion.KSP.MoonSharp.CommonAPI;
using static KerbaluaNUnit.CommonAPITestUtil;

namespace KerbaluaNUnit
{
	[TestFixture()]
	public class LUA_DummyAPITests
	{
		Script script;
		Table globals;
		CommonAPICreator creator;
		Table apiTable;
		ScriptTester st;

		void Setup()
		{
			script = new Script(CoreModules.Preset_Complete);
			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
			globals = script.Globals;
			creator=new CommonAPICreator(script);
			apiTable=creator.Create(typeof(DummyAPI));
			Table metatable=new Table(script);
			metatable["__index"]=apiTable;
			globals.MetaTable=metatable;
			st=new ScriptTester(script);
		}

		[Test()]
		public void LUA_DummyAPITests1_Creation()
		{
			Setup();
		}

		[Test()]
		public void LUA_DummyAPITests2_BasicField()
		{
			Setup();

			st.StaticCheck(apiTable, "pid");
		}

		[Test()]
		public void LUA_DummyAPITests3_BasicProperty()
		{
			Setup();
			double o=st.MemberCheck(apiTable, "a", typeof(double)).Number;
			Assert.AreEqual(DummyAPI.a, o);
		}

		[Test()]
		public void LUA_DummyAPITests3_BasicMethod()
		{
			Setup();
			var d=st.MethodCheck(apiTable,"getint");

			//var d=c.Call(1);
			//Assert.AreEqual(DummyAPI.a, o);
		}

		[Test()]
		public void LUA_DummyAPITests4_BasicNamespaceField()
		{
			Setup();
			var t1=st.NamespaceCheck(apiTable,"ui");
			st.StaticCheck(t1, "Color");
		}

		[Test()]
		public void LUA_DummyAPITests5_BasicNamespaceMethod()
		{
			Setup();
			var t1=st.NamespaceCheck(apiTable,"autorun"); ;
			st.MemberCheck(t1, "save", typeof(CallbackFunction));
		}

		[Test()]
		public void LUA_DummyAPITests_Field()
		{
			Setup();
			st.StaticCheck(globals, "pid");
		}

		[Test()]
		public void LUA_DummyAPITests_NamespaceField()
		{
			Setup();
			var t1=st.NamespaceCheck(globals,"ui");
			st.StaticCheck(t1, "Color");
		}

		[Test()]
		public void LUA_DummyAPITests_NamespaceMethod()
		{
			Setup();
			var t1=st.NamespaceCheck(globals,"autorun"); ;
			st.MemberCheck(t1, "save", typeof(CallbackFunction));
		}
	}
}
