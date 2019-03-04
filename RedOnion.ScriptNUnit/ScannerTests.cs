using System;
using System.Linq;
using NUnit.Framework;
using RedOnion.Script.Parsing;

namespace RedOnion.ScriptNUnit
{
	[TestFixture]
	public class ScannerTests: Scanner
	{
		public void Test(string line)
			=> Line = line;

		[Test]
		public void Scanner_01_Words()
		{
			Test("word");
			Assert.AreEqual("word", Word);
			Assert.AreEqual(0, At);
			Assert.AreEqual(4, End);
			Assert.AreEqual('w', CharAt(0));
			Assert.AreEqual('d', CharAt(3));
			Assert.True(White);
			Assert.True(PeekEol);
			Assert.AreEqual('\n', PeekAt(0));
			Assert.AreEqual('\0', PeekAt(1));

			Test("$small_under");
			Assert.AreEqual("$small_under", Word);

			Test("first second");
			Assert.AreEqual(Word, "first");
			Assert.AreEqual(' ', Peek);
			Assert.AreEqual('s', PeekAt(1));
			Assert.True(White);
			Assert.AreEqual(NextWord(), "second");
			Assert.True(White);
		}

		[Test]
		public void Scanner_02_String()
		{
			Test("\"string\"");
			Assert.AreEqual('"', Curr);
			Assert.AreEqual(0, At);
			Assert.AreEqual(8, End);
			Assert.AreEqual('"', Read());
			Assert.AreEqual("string\"", Rest());
			Assert.True(Normal);

			Test("@\"verbatim\"");
			Assert.AreEqual("@\"verbatim\"", Rest());
			Assert.True(Normal);

			Test("$\"inter\"");
			Assert.AreEqual("$\"inter\"", Rest());
			Assert.True(Normal);

			Test("$@\"inter-verbatim\"");
			Assert.AreEqual("$@\"inter-verbatim\"", Rest());
			Assert.True(Normal);

			Test("\"esc\\\"\"");
			Assert.AreEqual("\"esc\\\"\"", Rest());
			Assert.True(Normal);

			Test("@\"esc\\\"");
			Assert.AreEqual("@\"esc\\\"", Rest());
			Assert.True(Normal);

			Test("\"blah");
			Assert.True(InString);
			Assert.AreEqual(5, End);
			Line = "end\"";
			Assert.True(Normal);
			Assert.AreEqual(4, End);
		}

		[Test]
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

		[Test]
		public void Scanner_04_Comments()
		{
			Test("first /* comment */ second");
			Assert.AreEqual(Word, "first");
			Assert.AreEqual(NextWord(), "second");

			Test("/*");
			Assert.True(Comment);

			Test("comment");
			Assert.True(Comment);

			Test("*/");
			Assert.True(Normal);
		}
	}
}
