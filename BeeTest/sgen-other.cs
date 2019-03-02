using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace Bee.Tests
{
	[TestClass]
	public class BeeFullSourceGeneratorTests: BeeFullSourceGeneratorTestsBase
	{
		public BeeFullSourceGeneratorTests(  )
		{
			Csgen.Aliasing = CsGenerator.Alias.ExceptLocal;
		}//.ctor
		
		[TestInitialize]
		public void Init(  )
		{
			Csgen.DefClassAccess = Run.Tflag.None;
			Csgen.DefMethodAccess = Run.Tflag.None;
		}//Init
		
		[TestMethod]
		public void GU01_import(  )
		{
			Test( "import sys", "using System;", "use sys" );
			Test( "use sys, ms", "using System;\n" + "using Microsoft;", "use sys\n" + "use ms" );
			Test( "using sys: io", "using System;\n" + "using System.IO;", "use sys\n" + "use sys.io" );
			Test( "import sys: io, text;", ("using System;\n" + "using System.IO;\n") + "using System.Text;", ("use sys\n" + "use sys.io\n") + "use sys.text" );
			Test( "use sys: lang, cols: gen;;", (("using System;\n" + "using System.Globalization;\n") + "using System.Collections;\n") + "using System.Collections.Generic;", (("use sys\n" + "use sys.lang\n") + "use sys.cols\n") + "use sys.cols.gen" );
			Test( "using\nsys:\ndiag,\nrun:\ninter", (("using System;\n" + "using System.Diagnostics;\n") + "using System.Runtime;\n") + "using System.Runtime.InteropServices;", (("use sys\n" + "use sys.diag\n") + "use sys.run\n") + "use sys.run.inter" );
			Test( "use bee; using eto.forms;\nimport ms.vs:\ntest", (("using Bee;\n" + "using Eto.Forms;\n") + "using Microsoft.VisualStudio;\n") + "using Microsoft.VisualStudio.TestTools.UnitTesting;", (("use bee\n" + "use eto.forms\n") + "use ms.vs\n") + "use ms.vs.test" );
		}//GU01_import
		
		[TestMethod]
		public void GU02_class(  )
		{
			Test( "def type", "class Type {}" );
			Test( "def type: struct", "struct Type {}", "def type struct" );
			Test( "interface type", "interface Type {}", "def type interface" );
			Test( "type enum", "enum Type {}", "def type enum" );
			Test( "program static class", "static class Program {}", "def program static" );
			Test( "public static unsafe partial class program", "public static unsafe partial class Program {}", "def program public static partial unsafe" );
			Test( "public, static,, unsafe, partial,, class, program", "public static unsafe partial class Program {}", "def program public static partial unsafe" );
			Test( "program class: public, static:,:, unsafe,:, partial", "public static unsafe partial class Program {}", "def program public static partial unsafe" );
			Test( "class:\nprogram: public: static,\n:unsafe, partial", "public static unsafe partial class Program {}", "def program public static partial unsafe" );
		}//GU02_class
		
		[TestMethod]
		public void GU03_inherit(  )
		{
			Test( "def a b", "class A: B {}" );
			Test( "enum e: byte", "enum E: byte {}", "def e enum byte" );
			Test( "interface a:b,c", "interface A: B, C {}", "def a interface b c" );
			Test( "class a:,\n\n,\n:b:,\n:,:c", "class A: B, C {}", "def a b c" );
		}//GU03_inherit
		
		[TestMethod]
		public void GU04_field(  )
		{
			Test( "def a\n" + "	var x int", (("class A\n" + "{\n") + "	int X;\n") + "}" );
			Test( (("def test\n" + "	static var data\n") + "	var static data2\n") + "	var data3 static", (((("class Test\n" + "{\n") + "	static Data Data;\n") + "	static Data2 Data2;\n") + "	static Data3 Data3;\n") + "}", (("def test\n" + "	var data static\n") + "	var data2 static\n") + "	var data3 static" );
			Test( (("def test\n" + "	event handler eventHandler.[eventArgs]\n") + "	readonly static data\n") + "	const c int = 1", (((("class Test\n" + "{\n") + "	event EventHandler<EventArgs> Handler;\n") + "	static readonly Data Data;\n") + "	const int C = 1;\n") + "}", (("def test\n" + "	event handler eventHandler.[eventArgs]\n") + "	var data static readonly\n") + "	const c int = 1" );
		}//GU04_field
		
		[TestMethod]
		public void GU05_method(  )
		{
			Csgen.DefMethodAccess = Run.Tflag.Public;
			Test( ("def program static\n" + "	main\n") + "		return", ((((("static class Program\n" + "{\n") + "	public void Main()\n") + "	{\n") + "		return;\n") + "	}\n") + "}" );
			Test( (((((((("def mystery\n" + "	secret int, input int private\n") + "		if input & 1 != 0\n") + "			input = 3 * input + 1\n") + "		return input >> 1\n") + "	steps int, input int\n") + "		var n = 0\n") + "		while input > 1\n") + "			input = secret input\n") + "		return n", (((((((((((((((((("class Mystery\n" + "{\n") + "	private int Secret(int input)\n") + "	{\n") + "		if ((input & 1) != 0)\n") + "		{\n") + "			input = 3 * input + 1;\n") + "		}\n") + "		return input >> 1;\n") + "	}\n") + "	public int Steps(int input)\n") + "	{\n") + "		var n = 0;\n") + "		while (input > 1)\n") + "		{\n") + "			input = Secret(input);\n") + "		}\n") + "		return n;\n") + "	}\n") + "}" );
			Test( "def short\n" + "	nothing() return\n", ((((("class Short\n" + "{\n") + "	public void Nothing()\n") + "	{\n") + "		return;\n") + "	}\n") + "}", ("def short\n" + "	nothing\n") + "		return" );
		}//GU05_method
		
		[TestMethod]
		public void GU06_property(  )
		{
			Test( ((((("def p\n" + "	var _name string\n") + "	name string\n") + "		get\n") + "			return _name\n") + "		set\n") + "			_name = value\n", ((((((((((((("class P\n" + "{\n") + "	string _name;\n") + "	public string Name\n") + "	{\n") + "		get\n") + "		{\n") + "			return _name;\n") + "		}\n") + "		set\n") + "		{\n") + "			_name = value;\n") + "		}\n") + "	}\n") + "}" );
		}//GU06_property
	}//BeeFullSourceGeneratorTests
}//Bee.Tests
