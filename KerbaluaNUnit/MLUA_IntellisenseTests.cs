using System;
using System.Collections.Generic;
using System.Linq;
using Kerbalua.Completion;
using Kerbalua.Scripting;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using NUnit.Framework;
using RedOnion.KSP.ReflectionUtil;
using static KerbaluaNUnit.TestingUtil;

namespace KerbaluaNUnit
{
	[TestFixture()]
	public class MLUA_IntellisenseTests
	{
		public MLUA_IntellisenseTests()
		{
		}

		public class Adf
		{
			public Adf asdf()
			{
				return this;
			}
			public string asd = "badsf";
			public delegate Adf blah2(int i);
			public blah2 blah;
			public int testy = 2;
			public List<blah2> adfs;
			public static int asdfg = 4;
		}

		private const int ADF_RUNTIME_MEMBERS = 7;
		private const int OS_DEFAULT_ENTRIES = 11;
		Script script;
		Table globals;

		public IList<string> GetCompletions(string source)
		{
			return MoonSharpIntellisense.GetCompletions(globals, source, source.Length, out int replaceStart, out int replaceEnd);
		}

		void Setup()
		{
			script = new Script(CoreModules.Preset_Complete);
			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
			globals = script.Globals;
		}



		[Test()]
		public void MLUA_IntellisenseTest_Basic()
		{
			Setup();
			string source =
				@"";

			var completions = GetCompletions(source);
			Assert.AreEqual(42, completions.Count);
		}

		[Test()]
		public void MLUA_IntellisenseTest_Static()
		{
			Setup();
			globals["Adf"] = UserData.CreateStatic(typeof(Adf));
			string source =
				@"Adf.asdfg.";

			var completions = GetCompletions(source);

			Assert.Less(1, completions.Count);
		}

		[Test()]
		public void MLUA_IntellisenseTest_Instance()
		{
			Setup();
			globals["Adf"] = new Adf();
			string source =
				@"Adf.blah().";

			var completions = GetCompletions(source);
			// since blah is null and we now are testing
			// dynamicly when possible, we do not get to the call
			// type.
			Assert.Less(1, completions.Count);

			//Assert.AreEqual(11, completions.Count);
		}

		[Test()]
		public void MLUA_IntellisenseTest_Namespace()
		{
			Setup();
			var allMappings = NamespaceMappings.ForAllAssemblies;

			globals["Native"] = allMappings.GetNamespace("");
			//globals["Adf"] = new Adf();
			string source =
				@"Native.";

			var completions = GetCompletions(source);

			Assert.Less(1, completions.Count);

			//Assert.AreEqual(11, completions.Count);
		}

		[Test()]
		public void MLUA_IntellisenseTest_NamespaceStatic()
		{
			Setup();
			var allMappings = NamespaceMappings.ForAllAssemblies;
			globals["Import"] = allMappings.GetNamespace("");
			//globals["Adf"] = new Adf();
			string source =
				@"Import.System.Collections.Generic.List";

			var completions = GetCompletions(source);
			PrintAll(completions);
			Assert.Less(1, completions.Count);

			//Assert.AreEqual(11, completions.Count);
		}

		[Test()]
		public void MLUA_IntellisenseTest_Action()
		{
			Setup();
			var allMappings = NamespaceMappings.ForAllAssemblies;
			globals["Import"] = allMappings.GetNamespace("");
			//globals["Adf"] = new Adf();
			string source =
				@"Import.System.";
			Console.WriteLine(allMappings.GetNamespace("")
				.GetSubNamespace("System").GetType("Action"));
			var completions = GetCompletions(source);
			//PrintAll(completions);
		
			//Console.WriteLine("expected was " + expected);
			Assert.Less(1, completions.Count);

			//Assert.AreEqual(11, completions.Count);
		}

		[Test()]
		public void MLUA_IntellisenseTest_LuaGlobalsMetatableCompletion()
		{
			Setup();
			globals.MetaTable = RedOnion.KSP.API.LuaGlobals.Instance;
			string source = "native";
			var completions = GetCompletions(source);
			Assert.AreEqual(1, completions.Count);
		}

		[Test()]
		public void MLUA_IntellisenseTest_NestedTablesCompletion()
		{
			Setup();
			string source = "os.";
			var completions = GetCompletions(source);
			Assert.AreEqual(OS_DEFAULT_ENTRIES, completions.Count);
		}

		[Test()]
		public void MLUA_IntellisenseTest_StringKeyedTableCompletion()
		{
			Setup();
			string source = "_G['os'].";
			var completions = GetCompletions(source);
			Assert.AreEqual(OS_DEFAULT_ENTRIES, completions.Count);
		}

		[Test()]
		public void MLUA_IntellisenseTest_IntKeyedTableCompletion()
		{
			Setup();
			script.DoString("b={} b.c=3 a={b}");
			string source = "a[1].";
			var completions = GetCompletions(source);
			Assert.AreEqual(1, completions.Count);
		}

		[Test()]
		public void MLUA_IntellisenseTest_CaseInsensitive()
		{
			Setup();
			var allMappings = NamespaceMappings.ForAllAssemblies;
			globals["Import"] = allMappings.GetNamespace("");
			//globals["Adf"] = new Adf();
			string source =
				@"Import.System.";
			//Console.WriteLine(allMappings.GetNamespace("")
				//.GetSubNamespace("System").GetType("Action"));
			var completions = GetCompletions(source);
			//PrintAll(completions);
			Assert.Less(1, completions.Count);

			//Assert.AreEqual(11, completions.Count);
		}

		//[Test()]
		//public void MLUA_IntellisenseTest_Interop()
		//{
		//	Setup();
		//	globals.MetaTable = RedOnion.KSP.API.LuaGlobals.Instance;
		//	var allMappings = NamespaceMappings.ForAllAssemblies;
		//	//globals[""] = allMappings.GetNamespace("");
		//	//globals["Adf"] = new Adf();
		//	string source =
		//		@"reflect";
		//	//Console.WriteLine(allMappings.GetNamespace("")
		//	//.GetSubNamespace("System").GetType("Action"));
		//	var completions = GetCompletions(source);
		//	//PrintAll(completions);
		//	Assert.AreEqual(1, completions.Count);

		//	//Assert.AreEqual(11, completions.Count);
		//}



		//[Test()]
		//public void MLUA_IntellisenseTest_Interop2()
		//{
		//	Setup();
		//	globals.MetaTable = RedOnion.KSP.API.LuaGlobals.Instance;
		//	var allMappings = NamespaceMappings.ForAllAssemblies;
		//	//globals[""] = allMappings.GetNamespace("");
		//	//globals["Adf"] = new Adf();
		//	string source =
		//		@"reflect.";
		//	//Console.WriteLine(allMappings.GetNamespace("")
		//	//.GetSubNamespace("System").GetType("Action"));
		//	var completions = GetCompletions(source);
		//	//PrintAll(completions);
		//	Assert.AreEqual(1, completions.Count);

		//	//Assert.AreEqual(11, completions.Count);
		//}

		[Test()]
		public void MLUA_IntellisenseTest_Bodies()
		{
			Setup();
			globals.MetaTable = RedOnion.KSP.API.LuaGlobals.Instance;
			string source = "ksp.bodies";
			var completions = GetCompletions(source);
			Assert.AreEqual(1, completions.Count);
		}

		//[Test()]
		//public void MLUA_IntellisenseTest_Bodies2()
		//{
		//	Setup();
		//	globals.MetaTable = RedOnion.KSP.API.LuaGlobals.Instance;
		//	string source = "ksp.bodies.";
		//	var completions = GetCompletions(source);
		//	Assert.AreEqual("sun", completions[0]);
		//}
	}
}
