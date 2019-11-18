using System;
using System.Collections.Generic;
using Kerbalua.Completion;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using NUnit.Framework;
using RedOnion.KSP.MoonSharp.CommonAPI;
using static KerbaluaNUnit.CommonAPITestUtil;

namespace KerbaluaNUnit
{
	[TestFixture()]
	public class MLUA_DummyAPITests
	{
		Script script;
		Table globals;
		//CommonAPICreator creator;
		CommonAPITable apiTable;
		ScriptTester st;

		void Setup()
		{
			script = new Script(CoreModules.Preset_Complete);
			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
			globals = script.Globals;
			//creator=new CommonAPICreator(script);
			apiTable=new CommonAPITable(script);
			apiTable.AddAPI(typeof(DummyAPI));
			Table metatable=new Table(script);
			metatable["__index"]=apiTable;
			globals.MetaTable=metatable;
			st=new ScriptTester(script);
		}

		[Test()]
		public void MLUA_DummyAPITests1_Creation()
		{
			Setup();
		}

		[Test()]
		public void MLUA_DummyAPITests2_Field()
		{
			Setup();

			st.StaticCheck(globals, "pid");
		}

		[Test()]
		public void MLUA_DummyAPITests3_Property()
		{
			Setup();
			double o=st.MemberCheck(globals, "a", typeof(double)).Number;
			Assert.AreEqual(DummyAPI.a, o);
		}

		[Test()]
		public void MLUA_DummyAPITests3_Method()
		{
			Setup();
			var f=st.MethodCheck(globals,"getint");
			object[] args={2};
			object[] returns={2,3};
			st.CallCheck(f, args, returns);
			//var d=c.Call(1);
			//Assert.AreEqual(DummyAPI.a, o);
		}

		[Test()]
		public void MLUA_DummyAPITests4_NamespaceField()
		{
			Setup();
			var t1=st.NamespaceCheck(globals,"ui");
			st.StaticCheck(t1, "Color");
		}

		[Test()]
		public void MLUA_DummyAPITests5_NamespaceMethod()
		{
			Setup();
			var t1=st.NamespaceCheck(globals,"autorun"); ;
			st.MemberCheck(t1, "save", typeof(CallbackFunction));
		}

		public IList<string> GetCompletions(string source,Table table)
		{
			return MoonSharpIntellisense.GetCompletions(table, source, source.Length, out int replaceStart, out int replaceEnd);
		}

		[Test()]
		public void MLUA_DummyAPITests6_Intellisense()
		{
			Setup();
			string source="";
			var completions=GetCompletions(source,apiTable);
			Assert.AreEqual(5, completions.Count);
		}
	}
}
