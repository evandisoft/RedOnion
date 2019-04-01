using NUnit.Framework;
using Kerbalua.Completion;
using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.Interpreter.Interop;
using Kerbalua.Other;
using RedOnion.KSP.Lua;

namespace KerbaluaNUnit {
	[TestFixture()]
	public class LUA_CompletionObjectTests {
		public CompletionObject GetCompletionObject(Table globals,string source)
		{
			var processed = LuaIntellisense.Parse(source);

			return new CompletionObject(globals, processed.Segments);
		}

		Script script = new Script(CoreModules.Preset_Complete);
		public LUA_CompletionObjectTests()
		{
			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
		}


		[Test()]
		public void LUA_TestCase()
		{
			script= new Script();
			var completion= GetCompletionObject(script.Globals,
				@"os."
				);
			var completions = completion.GetCurrentCompletions();
			foreach(var c in completions)
			{
				Console.WriteLine(c);
			}
			Assert.AreEqual(2, completions.Count);
			Assert.True(completion.ProcessNextSegment());
			completions = completion.GetCurrentCompletions();
			foreach (var c in completions)
			{
				Console.WriteLine(c);
			}
			Assert.AreEqual(11, completions.Count);
		}

		public class Adf {
			public Adf asdf()
			{
				return this;
			}
			public string asd = "badsf";
			public delegate Adf blah2(int i);
			public blah2 blah;
			public List<blah2> adfs;
		}

		[Test()]
		public void LUA_TestCase_2()
		{
			script = new Script(CoreModules.Preset_Complete);
			script.Globals["ADF"] = new Adf();

			var completion = GetCompletionObject(script.Globals,
				@"ADF."
				);
			var completions = completion.GetCurrentCompletions();
			Assert.AreEqual(3, completions.Count);
			Assert.True(completion.ProcessNextSegment());
			completions = completion.GetCurrentCompletions();
			foreach (var c in completions)
			{
				Console.WriteLine(c);
			}
			Assert.AreEqual(10, completions.Count);
		}

		[Test()]
		public void LUA_TestCase_3_Interop()
		{
			script = new KerbaluaScript();
			//script.Globals["ship"] = new Adf();

			var completion = GetCompletionObject(script.Globals,
@"
	a=ship.
"
				);
			var completions = completion.GetCurrentCompletions();
			foreach (var c in completions)
			{
				Console.WriteLine(c);
			}
			Assert.AreEqual(1, completions.Count);
			Assert.Throws<LuaIntellisenseException>(() => completion.ProcessNextSegment());
		}

		[Test()]
		public void LUA_TestCase_4_Interop_2()
		{
			script = new KerbaluaScript();
			//script.Globals["ship"] = new Adf();

			// I think this is not quite right, but I'm done for now.
			var completion = GetCompletionObject(script.Globals,
@"
	a=Vector.abs
"
				);
			var completions = completion.GetCurrentCompletions();
			foreach (var c in completions)
			{
				Console.WriteLine(c);
			}
			Assert.AreEqual(1, completions.Count);
			completion.ProcessNextSegment();
			completions = completion.GetCurrentCompletions();
			foreach (var c in completions)
			{
				Console.WriteLine(c);
			}
			Assert.AreEqual(1, completions.Count);
		}
	}
}
