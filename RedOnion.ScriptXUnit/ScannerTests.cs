using System;
using System.Linq;
using Xunit;
using RedOnion.Script.Parsing;

namespace RedOnion.ScriptTests
{
	public class ScannerTests: Scanner
	{
		public void Test(string line)
			=> Line = line;

		[Fact]
		public void Scanner_01_Words()
		{
			Test("word");
			Assert.Equal("word", Word);
			Assert.Equal(0, At);
			Assert.Equal(4, End);
			Assert.Equal('w', CharAt(0));
			Assert.Equal('d', CharAt(3));
			Assert.True(White);
			Assert.True(PeekEol);
			Assert.Equal('\n', PeekAt(0));
			Assert.Equal('\0', PeekAt(1));

			Test("$small_under");
			Assert.Equal("$small_under", Word);

			Test("first second");
			Assert.Equal(Word, "first");
			Assert.Equal(' ', Peek);
			Assert.Equal('s', PeekAt(1));
			Assert.True(White);
			Assert.Equal(NextWord(), "second");
			Assert.True(White);
		}

		[Fact]
		public void Scanner_02_String()
		{
			Test("\"string\"");
			Assert.Equal('"', Curr);
			Assert.Equal(0, At);
			Assert.Equal(8, End);
			Assert.Equal('"', Read());
			Assert.Equal("string\"", Rest());
			Assert.True(Normal);

			Test("@\"verbatim\"");
			Assert.Equal("@\"verbatim\"", Rest());
			Assert.True(Normal);

			Test("$\"inter\"");
			Assert.Equal("$\"inter\"", Rest());
			Assert.True(Normal);

			Test("$@\"inter-verbatim\"");
			Assert.Equal("$@\"inter-verbatim\"", Rest());
			Assert.True(Normal);

			Test("\"esc\\\"\"");
			Assert.Equal("\"esc\\\"\"", Rest());
			Assert.True(Normal);

			Test("@\"esc\\\"");
			Assert.Equal("@\"esc\\\"", Rest());
			Assert.True(Normal);

			Test("\"blah");
			Assert.True(InString);
			Assert.Equal(5, End);
			Line = "end\"";
			Assert.True(Normal);
			Assert.Equal(4, End);
		}

		[Fact]
		public void Scanner_03_Number()
		{
			Test("12345");
			Assert.Equal('1', Curr);
			Assert.Equal('1', Read());
			Assert.Equal('2', Read());
			Assert.Equal("345", Rest());

			Test("0x1AB");
			Assert.Equal('0', Read());
			Assert.Equal('x', Read());
			Assert.Equal("1AB", Rest());

			Test("1.2");
			Assert.Equal("1.2", Rest());

			Test("1.2e+3");
			Assert.Equal("1.2e+3", Rest());
		}

		[Fact]
		public void Scanner_04_Comments()
		{
			Test("first /* comment */ second");
			Assert.Equal(Word, "first");
			Assert.Equal(NextWord(), "second");

			Test("/*");
			Assert.True(Comment);

			Test("comment");
			Assert.True(Comment);

			Test("*/");
			Assert.True(Normal);
		}
	}
}
