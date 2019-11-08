using System;
using System.Collections.Generic;
using System.Linq;
using Kerbalua.Completion;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using NUnit.Framework;
using RedOnion.KSP.ReflectionUtil;
using static KerbaluaNUnit.TestingUtil;

namespace KerbaluaNUnit
{
	[TestFixture()]
	public class IntellisenseTests
	{
		public IntellisenseTests()
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

		Script script;
		Table globals;

		public IList<string> GetCompletions(string source)
		{
			return LuaIntellisense.GetCompletions(globals, source, source.Length, out int replaceStart, out int replaceEnd);
		}

		void Setup()
		{
			script = new Script();
			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
			globals = script.Globals;
		}

		[Test()]
		public void LUA_IntellisenseTest_Basic()
		{
			Setup();
			string source =
				@"";

			var completions = GetCompletions(source);
			Assert.AreEqual(41, completions.Count);
		}

		[Test()]
		public void LUA_IntellisenseTest_Static()
		{
			Setup();
			globals["Adf"] = UserData.CreateStatic(typeof(Adf));
			string source =
				@"Adf.asdfg.";

			var completions = GetCompletions(source);

			Assert.AreEqual(11, completions.Count);
		}

		[Test()]
		public void LUA_IntellisenseTest_Instance()
		{
			Setup();
			globals["Adf"] = new Adf();
			string source =
				@"Adf.blah().";

			var completions = GetCompletions(source);
			Assert.AreEqual(12, completions.Count);

			//Assert.AreEqual(11, completions.Count);
		}

		[Test()]
		public void LUA_IntellisenseTest_Namespace()
		{
			Setup();
			var allMappings = NamespaceMappings.ForAllAssemblies;

			globals["Native"] = allMappings.GetNamespace("");
			//globals["Adf"] = new Adf();
			string source =
				@"Native.";

			var completions = GetCompletions(source);

			Assert.AreEqual(107, completions.Count);

			//Assert.AreEqual(11, completions.Count);
		}

		[Test()]
		public void LUA_IntellisenseTest_NamespaceStatic()
		{
			Setup();
			var allMappings = NamespaceMappings.ForAllAssemblies;
			globals["Import"] = allMappings.GetNamespace("");
			//globals["Adf"] = new Adf();
			string source =
				@"Import.System.Collections.Generic.List.";

			var completions = GetCompletions(source);
			//PrintAll(completions);
			Assert.AreEqual(1, completions.Count);

			//Assert.AreEqual(11, completions.Count);
		}

		[Test()]
		public void LUA_IntellisenseTest_Action()
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
			Assert.AreEqual(454, completions.Count);

			//Assert.AreEqual(11, completions.Count);
		}

		[Test()]
		public void LUA_IntellisenseTest_CaseInsensitive()
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
			Assert.AreEqual(450, completions.Count);

			//Assert.AreEqual(11, completions.Count);
		}

		[Test()]
		public void LUA_IntellisenseTest_Interop()
		{
			Setup();
			globals.MetaTable = RedOnion.KSP.API.LuaGlobals.Instance;
			var allMappings = NamespaceMappings.ForAllAssemblies;
			//globals[""] = allMappings.GetNamespace("");
			//globals["Adf"] = new Adf();
			string source =
				@"reflect";
			//Console.WriteLine(allMappings.GetNamespace("")
			//.GetSubNamespace("System").GetType("Action"));
			var completions = GetCompletions(source);
			//PrintAll(completions);
			Assert.AreEqual(1, completions.Count);

			//Assert.AreEqual(11, completions.Count);
		}



		[Test()]
		public void LUA_IntellisenseTest_Interop2()
		{
			Setup();
			globals.MetaTable = RedOnion.KSP.API.LuaGlobals.Instance;
			var allMappings = NamespaceMappings.ForAllAssemblies;
			//globals[""] = allMappings.GetNamespace("");
			//globals["Adf"] = new Adf();
			string source =
				@"reflect.";
			//Console.WriteLine(allMappings.GetNamespace("")
			//.GetSubNamespace("System").GetType("Action"));
			var completions = GetCompletions(source);
			//PrintAll(completions);
			Assert.AreEqual(1, completions.Count);

			//Assert.AreEqual(11, completions.Count);
		}

		[Test()]
		public void LUA_IntellisenseTest_Bodies()
		{
			Setup();
			globals.MetaTable = RedOnion.KSP.API.LuaGlobals.Instance;
			string source = "bodies";
			var completions = GetCompletions(source);
			Assert.AreEqual(1, completions.Count);
		}

		[Test()]
		public void LUA_IntellisenseTest_Bodies2()
		{
			Setup();
			globals.MetaTable = RedOnion.KSP.API.LuaGlobals.Instance;
			string source = "bodies.";
			var completions = GetCompletions(source);
			Assert.AreEqual("Sun", completions[0]);
		}
	}
}
