using NUnit.Framework;
using Kerbalua.Completion;
using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.Interpreter.Interop;
using Kerbalua.Other;

namespace KerbaluaNUnit {
	[TestFixture()]
	public class CompletionObjectTests {
		public CompletionObject GetCompletionObject(Table globals,string source)
		{
			var processed = LuaIntellisense.Parse(source);

			return new CompletionObject(globals, processed.Segments);
		}

		Script script = new Script(CoreModules.Preset_Complete);
		public CompletionObjectTests()
		{
			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
		}


		[Test()]
		public void TestCase()
		{
			script= new SimpleScript(CoreModules.Preset_Complete);
			var completion= GetCompletionObject(script.Globals,
				@"os."
				);
			var completions = completion.GetCurrentCompletions();
			Assert.AreEqual(1, completions.Count);
			Assert.True(completion.ProcessNextSegment());
			completions = completion.GetCurrentCompletions();
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
		public void TestCase_2()
		{
			script = new Script(CoreModules.Preset_Complete);
			script.Globals["ADF"] = new Adf();

			var completion = GetCompletionObject(script.Globals,
				@"ADF."
				);
			var completions = completion.GetCurrentCompletions();
			Assert.AreEqual(1, completions.Count);
			Assert.True(completion.ProcessNextSegment());
			completions = completion.GetCurrentCompletions();
			Assert.AreEqual(10, completions.Count);
		}

		[Test()]
		public void TestCase_3()
		{
			script = new Script(CoreModules.Preset_Complete);
			script.Globals["ADF"] = new Adf();

			var completion = GetCompletionObject(script.Globals,
@"
	ADF.a
"
				);
			var completions = completion.GetCurrentCompletions();
			Assert.AreEqual(1, completions.Count);
			Assert.True(completion.ProcessNextSegment());
			completions = completion.GetCurrentCompletions();
			Assert.AreEqual(3, completions.Count);
		}
	}
}
