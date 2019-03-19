using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RedOnion.Script;
using RedOnion.Script.Completion;

namespace RedOnion.ScriptNUnit
{
	[TestFixture]
	public class CompletionTests : EngineTestsBase
	{
		CompletionEngine Hints { get; }
		public CompletionTests()
			=> Hints = new CompletionEngine(this);
		protected IList<string> Complete(string source, int at)
			=> Hints.Complete(source, at, out var from, out var to);
		protected void Same(IList<string> a, IList<string> b)
		{
			Assert.AreEqual(a.Count, b.Count);
			for (int i = 0; i < a.Count; i++)
				Assert.AreEqual(a[i], b[i]);
		}

		[Test]
		public void Completion_01_GlobalVar()
		{
			Test("var x");
			Assert.IsTrue(Complete("", 0).Contains("x"));
		}

		[Test]
		public void Completion_02_StringLength()
		{
			// easier - we create object
			Test("var s = new string \"hello\"");
			Assert.IsTrue(Complete("s.", 2).Contains("length"));

			// harder - the completion engine has to box it
			Test("var world = \"world\"");
			Assert.IsTrue(Complete("world.", 6).Contains("length"));
		}

		[Test]
		public void Completion_03_Nested()
		{
			Test("var space = new object");
			Test("space.thing = new object");
			Same(Complete("space.", 6), new[] { "thing" });

			Test("space.thing.x = 1");
			Test("space.thing.y = 2");
			Same(Complete("space.thing.", 12), new[] { "x", "y" });
		}

		[Test]
		public void Completion_04_Partial()
		{
			Test("var it = new object");
			Test("it.aaa = 1");
			Test("it.aab = 2");
			Test("it.abb = 3");
			Test("it.abc = 4");
			Test("it.bcd = 5");
			Same(Hints.Complete("it.aa", 5, out var from, out var to), new[] { "aaa", "aab", });
			Assert.AreEqual(3, from);
			Assert.AreEqual(5, to);
		}
	}
}
