using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace Bee.Run.Tests
{
	public class BeeExpressionRunTestsBase: Engine
	{
		public void Test( string @string )
		{
			try
			{
				Eval( @string );
			}
			catch( Exception e )
			{
				throw new Exception( String.Format( "{0} in Eval: {1}; IN: <{2}>", e.GetType().ToString(), e.Message, @string ), e );
			}
		}//Test
		
		public void Test( Object @object, string @string )
		{
			Test( @string );
			Assert.AreEqual( @object, Result.Native, "Test: <{0}>", @string );
		}//Test
	}//BeeExpressionRunTestsBase
	
	[TestClass]
	public class BeeExpressionRunTests: BeeExpressionRunTestsBase
	{
		[TestMethod]
		public void RX01_simple(  )
		{
			Test( "bee", "\"bee\"" );
			Test( 1, "1" );
			Test( 3.14, "3.14" );
			Test( 5.5f, "5.5f" );
			Test( 10, "0xA" );
			Test( -7, "-7" );
			Test( 0.1, ".1" );
			Test( 7u, "7u" );
		}//RX01_simple
		
		[TestMethod]
		public void RX02_addmul(  )
		{
			Test( 3, "1+2" );
			Test( 12, "3*4" );
			Test( 1.1, "1+0.1" );
			Test( 7, "1+2*3" );
			Test( 8.5, "1+2.5*3" );
			Test( ((long)2), "1+1u" );
			Test( ((ulong)2), "1u+1" );
		}//RX02_addmul
		
		[TestMethod]
		public void RX03_divnan(  )
		{
			Test( 12 / 5, "12/5" );
			Test( 12f / 5, "12f/5" );
			Test( null, "0/0" );
			Test( System.Double.NaN, "0/.0" );
		}//RX03_divnan
		
		[TestMethod]
		public void RX04_vars(  )
		{
			Test( 10, "x = 2*3+4" );
			Test( 10, "x" );
			Test( "x10", "\"x\" + x" );
			Test( 10, "x++" );
			Test( 12, "++x" );
		}//RX04_vars
		
		[TestMethod]
		public void RX05_props(  )
		{
			Test( "obj = new object" );
			Assert.AreEqual( Vtype.Object, Result.Type, "not object type" );
			Assert.IsNotNull( Result.Refobj );
			Test( "obj.x = 3.14" );
			Test( 3.14, "obj.x" );
			Test( "obj.s = \"xyz\"" );
			Test( "xyz", "obj.s" );
			Test( 3, "obj.s.length" );
			Test( 3.14, "obj.s.bad = obj.x" );
			Test( null, "obj.s.bad" );
			Test( "s = new string \"hello\"" );
			Test( "s.e = 2.7" );
			Test( 2.7, "s.e" );
			Test( 5, "s.length" );
			Test( 2.7, "s[\"e\"]" );
			Test( 5, "s[\"length\"]" );
			Test( 3, "obj[\"s\"][\"length\"]" );
			Test( 3, "obj[\"s\", \"length\"]" );
		}//RX05_props
		
		[TestMethod]
		public void RX06_compare(  )
		{
			Test( true, "1 < 2" );
			Test( true, "1 > .1" );
			Test( true, "1 == 1.0" );
		}//RX06_compare
		
		public void RX07_ternary(  )
		{
			Test( true, "true ? true : false" );
			Test( false, "false ? true : false" );
			Test( 2, "0 != 0 ? 1 : 2" );
		}//RX07_ternary
	}//BeeExpressionRunTests
}//Bee.Run.Tests
