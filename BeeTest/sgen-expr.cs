using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace Bee.Tests
{
	public class BeeSourceGeneratorTestsBase: PseudoGenerator
	{
		protected BsGenerator Bsgen = new BsGenerator();
		protected CsGenerator Csgen = new CsGenerator() { Aliasing = CsGenerator.Alias.None };
		public override string ToString(  )
		{
			if( CodeAt == 0 )
			{
				return "empty";
			}
			var sb = new StringBuilder();
			var i = 0;
			while( (i < CodeAt) && (i < 32) ){
				sb.AppendFormat( "{0:X2}", Code[i++] );
			}
			if( i < CodeAt )
			{
				sb.Append( "..." );
			}
			return sb.ToString();
		}//ToString
		
		public void Test( string @string, string cs = null, string bs = null, Parser.Opt opts = Parser.Opt.None )
		{
			string bsg;
			string csg;
			var step = "Parse";
			Parser.Opts = opts;
			try
			{
				Reset().Expression( @string );
				step = "BsGen";
				bsg = Bsgen.Reset().Expression( Code ).ToString();
				step = "CsGen";
				csg = Csgen.Reset().Expression( Code ).ToString();
			}
			catch( Exception e )
			{
				throw new Exception( String.Format( "{0} in {1}: {2}; IN: <{3}>", e.GetType().ToString(), step, e.Message, @string ), e );
			}
			Assert.AreEqual( bs ?? @string, bsg, "B#; IN: <{0}>; C#: <{1}>; Code: {2}", @string, csg, this );
			Assert.AreEqual( cs ?? @string, csg, "C#; IN: <{0}>; B#: <{1}>; Code: {2}", @string, bsg, this );
		}//Test
	}//BeeSourceGeneratorTestsBase
	
	[TestClass]
	public class BeeExpressionSrcGenTests: BeeSourceGeneratorTestsBase
	{
		[TestMethod]
		public void GX01_addmul(  )
		{
			Test( "1 + 2" );
			Test( "1 + 2 + 3" );
			Test( "2 * 3 + 1" );
			Test( "(1 + 2) * 3" );
			Test( "1 + 2 * (3 + 4 + 5) * 6 + 7" );
		}//GX01_addmul
		
		[TestMethod]
		public void GX02_logic(  )
		{
			Test( "true && false" );
			Test( "x && y && z" );
			Test( "x || y && z" );
			Test( "(x || y) && z" );
		}//GX02_logic
		
		[TestMethod]
		public void GX03_ternary(  )
		{
			Test( "true ? true : false" );
			Test( "x ? a + 1 : n * 2" );
			Test( "x == y ? a : b" );
			Test( "(x = y) ? a : b" );
		}//GX03_ternary
		
		[TestMethod]
		public void GX04_binary(  )
		{
			Test( "x & 1 != 0", "(x & 1) != 0" );
			Test( "x | 1 == 3", "(x | 1) == 3" );
			Test( "x ^ y <= z", "(x ^ y) <= z" );
			Test( "x | 1 << 2", "x | (1 << 2)", "x | (1 << 2)" );
			Test( "x ^ 1 << 2", "(x ^ 1) << 2", "(x ^ 1) << 2" );
			Test( "x & 7 + 3 << 8", "((x & 7) + 3) << 8", "(x & 7 + 3) << 8" );
			Test( "3 * x & 3 << 4 + y", "(3 * (x & 3)) << (4 + y)", "(3 * (x & 3)) << (4 + y)" );
			Test( "1 + 2 | 3 * 4 << 1", "(1 + 2) | ((3 * 4) << 1)", "(1 + 2) | ((3 * 4) << 1)" );
		}//GX04_binary
		
		[TestMethod]
		public void GX05_funcs(  )
		{
			Test( "f()" );
			Test( "f x", "f(x)" );
			Test( "f(g x)", "f(g(x))", "f g(x)" );
			Test( "f g x, y", "f(g(x, y))", "f g(x, y)" );
			Test( "f g(x), h y", "f(g(x), h(y))", "f g(x), h(y)" );
			Test( "f (g x,y),z,h()", "f(g(x, y), z, h())", "f g(x, y), z, h()" );
			Test( "f -g(x)", "f(-g(x))", "f -g(x)" );
			Test( "f -g x", "f(-g(x))", "f -g(x)" );
		}//GX05_funcs
		
		[TestMethod]
		public void GX06_dot_idx(  )
		{
			Test( "base.x" );
			Test( ".x", "this.x", "this.x" );
			Test( "x .y", "x.y", "x.y" );
			Test( "f .y", "f(this.y)", "f this.y", Parser.Opt.DotThisAfterWhite );
			Test( "(x + y).toString()" );
			Test( "a[i + 1] = 0" );
			Test( "v = (x += y)[i, j + 1]" );
			Test( "f[] * [3 + 4]", "f() * (3 + 4)", "f() * (3 + 4)", Parser.Opt.BracketsAsParens );
			Test( "g f [x], y", "g(f[x], y)", "g f[x], y", Parser.Opt.BracketsAsParens );
		}//GX06_dot_idx
		
		[TestMethod]
		public void GX07_cast(  )
		{
			Test( "byte! x", "(byte)x" );
			Test( "byte! x + y", "(byte)(x + y)" );
			Test( "some.type! obj", "(some.type)obj" );
			Test( "obj.prop as some.type" );
			Test( "obj as! type", "(type)obj" );
		}//GX07_cast
	}//BeeExpressionSrcGenTests
}//Bee.Tests
