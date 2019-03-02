using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace Bee.Tests
{
	public class BeeFullSourceGeneratorTestsBase: BeeSourceGeneratorTestsBase
	{
		private static readonly char[] Nlchars = new char[] { '\r', '\n' };
		public string Display( string @string )
		{
			return ("\n" + String.Join( "¶\n", @string.Split( Nlchars, StringSplitOptions.RemoveEmptyEntries ) )) + "\n";
		}//Display
		
		public new void Test( string @string, string cs = null, string bs = null, Parser.Opt opts = Parser.Opt.None )
		{
			string bsg;
			string csg;
			var step = "Parse";
			Parser.Opts = opts;
			try
			{
				Unit( @string );
				step = "BsGen";
				bsg = Bsgen.Eval( Code, 0, CodeAt ).ToString();
				step = "CsGen";
				csg = Csgen.Eval( Code, 0, CodeAt ).ToString();
			}
			catch( Exception e )
			{
				throw new Exception( String.Format( "{0} in {1}: {2}; IN: <{3}>", e.GetType().ToString(), step, e.Message, @string ), e );
			}
			bs = Display( bs ?? @string );
			cs = Display( cs ?? @string );
			bsg = Display( bsg );
			csg = Display( csg );
			Assert.AreEqual( bs, bsg, "\n--- B# ---\nIN:\n{0}\nC#:{1}\nCode: {2}", @string, csg, this );
			Assert.AreEqual( cs, csg, "\n--- C# ---\nIN:\n{0}\nB#:{1}\nCode: {2}", @string, bsg, this );
		}//Test
	}//BeeFullSourceGeneratorTestsBase
	
	[TestClass]
	public class BeeSourceGeneratorTests: BeeFullSourceGeneratorTestsBase
	{
		[TestMethod]
		public void GC01_for(  )
		{
			Test( "for var i = 0; i < n; i++; x += i", (("for (var i = 0; i < n; i++)\n" + "{\n") + "	x += i;\n") + "}", "for var i = 0; i < n; i++\n" + "	x += i" );
		}//GC01_for
		
		[TestMethod]
		public void GC02_while(  )
		{
			Test( "while true; break", (("while (true)\n" + "{\n") + "	break;\n") + "}", "while true\n" + "	break" );
		}//GC02_while
		
		[TestMethod]
		public void GC03_dowhile(  )
		{
			Test( "do x = false; while x\n", ((("do\n" + "{\n") + "	x = false;\n") + "}\n") + "while (x);", ("do\n" + "	x = false\n") + "while x" );
		}//GC03_dowhile
		
		[TestMethod]
		public void GC04_ifelse(  )
		{
			Test( "if false; continue; else return", (((((("if (false)\n" + "{\n") + "	continue;\n") + "}\n") + "else\n") + "{\n") + "	return;\n") + "}", (("if false\n" + "	continue\n") + "else\n") + "	return" );
		}//GC04_ifelse
		
		[TestMethod]
		public void GC05_elseif(  )
		{
			Test( ("if false; continue\n" + "else if true; return\n") + "else break", (((((((((("if (false)\n" + "{\n") + "	continue;\n") + "}\n") + "else if (true)\n") + "{\n") + "	return;\n") + "}\n") + "else\n") + "{\n") + "	break;\n") + "}", (((("if false\n" + "	continue\n") + "else if true\n") + "	return\n") + "else\n") + "	break" );
		}//GC05_elseif
		
		[TestMethod]
		public void GC06_nested(  )
		{
			Test( "if true; for\n" + "	do x = false; while x", ((((((((("if (true)\n" + "{\n") + "	for (;;)\n") + "	{\n") + "		do\n") + "		{\n") + "			x = false;\n") + "		}\n") + "		while (x);\n") + "	}\n") + "}", ((("if true\n" + "	for\n") + "		do\n") + "			x = false\n") + "		while x" );
		}//GC06_nested
		
		[TestMethod]
		public void GC07_array(  )
		{
			Test( "var a = new byte[]\n" + "for e in a; print e", ((("var a = new byte[];\n" + "foreach (var e in a)\n") + "{\n") + "	print(e);\n") + "}", ("var a = new byte[]\n" + "for e in a\n") + "	print e" );
		}//GC07_array
	}//BeeSourceGeneratorTests
}//Bee.Tests
