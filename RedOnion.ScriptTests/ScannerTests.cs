using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedOnion.Script.Parsing;

namespace RedOnion.ScriptTests
{
	[TestClass]
	public class ScannerTests: Scanner
	{
		public void Test(string line)
			=> Line = line;

		[TestMethod]
		public void Scanner_01_Words()
		{
			Test("word");
			Assert.AreEqual("word", Word);
			Assert.AreEqual(0, At);
			Assert.AreEqual(4, End);
			Assert.AreEqual('w', CharAt(0));
			Assert.AreEqual('d', CharAt(3));
			Assert.IsTrue(White);
			Assert.IsTrue(PeekEol);
			Assert.AreEqual('\n', PeekAt(0));
			Assert.AreEqual('\0', PeekAt(1));

			Test("$small_under");
			Assert.AreEqual("$small_under", Word);

			Test("first second");
			Assert.AreEqual(Word, "first");
			Assert.AreEqual(' ', Peek);
			Assert.AreEqual('s', PeekAt(1));
			Assert.IsTrue(White);
			Assert.AreEqual(NextWord(), "second");
			Assert.IsTrue(White);
		}

		[TestMethod]
		public void Scanner_02_String()
		{
			Test("\"string\"");
			Assert.AreEqual('"', Curr);
			Assert.AreEqual(0, At);
			Assert.AreEqual(8, End);
			Assert.AreEqual('"', Read());
			Assert.AreEqual("string\"", Rest());
			Assert.IsTrue(Normal);

			Test("@\"verbatim\"");
			Assert.AreEqual("@\"verbatim\"", Rest());
			Assert.IsTrue(Normal);

			Test("$\"inter\"");
			Assert.AreEqual("$\"inter\"", Rest());
			Assert.IsTrue(Normal);

			Test("$@\"inter-verbatim\"");
			Assert.AreEqual("$@\"inter-verbatim\"", Rest());
			Assert.IsTrue(Normal);

			Test("\"esc\\\"\"");
			Assert.AreEqual("\"esc\\\"\"", Rest());
			Assert.IsTrue(Normal);

			Test("@\"esc\\\"");
			Assert.AreEqual("@\"esc\\\"", Rest());
			Assert.IsTrue(Normal);

			Test("\"blah");
			Assert.IsTrue(InString);
			Assert.AreEqual(5, End);
			Line = "end\"";
			Assert.IsTrue(Normal);
			Assert.AreEqual(4, End);
		}

		[TestMethod]
		public void Scanner_03_Number()
		{
			Test("12345");
			Assert.AreEqual('1', Curr);
			Assert.AreEqual('1', Read());
			Assert.AreEqual('2', Read());
			Assert.AreEqual("345", Rest());

			Test("0x1AB");
			Assert.AreEqual('0', Read());
			Assert.AreEqual('x', Read());
			Assert.AreEqual("1AB", Rest());

			Test("1.2");
			Assert.AreEqual("1.2", Rest());

			Test("1.2e+3");
			Assert.AreEqual("1.2e+3", Rest());
		}

		[TestMethod]
		public void Scanner_04_Comments()
		{
			Test("first /* comment */ second");
			Assert.AreEqual(Word, "first");
			Assert.AreEqual(NextWord(), "second");

			Test("/*");
			Assert.IsTrue(Comment);

			Test("comment");
			Assert.IsTrue(Comment);

			Test("*/");
			Assert.IsTrue(Normal);
		}
	}
}
