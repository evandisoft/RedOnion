using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace Bee.Run.Tests
{
	public class BeeStatementRunTestsBase: BeeExpressionRunTestsBase
	{
		public void Test( Opcode opcode, Object @object, string @string )
		{
			Test( @string );
			Assert.AreEqual( opcode, Exit, "Test: <{0}>", @string );
			Assert.AreEqual( @object, Result.Native, "Test: <{0}>", @string );
		}//Test
	}//BeeStatementRunTestsBase
	
	[TestClass]
	public class BeeStatementRunTests: BeeStatementRunTestsBase
	{
		[TestMethod]
		public void RS01_return(  )
		{
			Test( Opcode.Return, null, "return" );
			Test( Opcode.Return, 1234, "return 1234" );
			Test( Opcode.Return, 12 / 5, "return 12/5" );
		}//RS01_return
		
		[TestMethod]
		public void RS02_for(  )
		{
			Test( Opcode.Return, "321", ("var s = \"\"\r\n" + "for var i = 3; i; i -= 1; s += i\r\n") + "return s" );
		}//RS02_for
		
		[TestMethod]
		public void RS03_if(  )
		{
			Test( Opcode.Return, true, "if true then return true" );
			Test( Opcode.Return, false, "if false: return true else: return false" );
		}//RS03_if
		
		[TestMethod]
		public void RS04_rtfunc(  )
		{
			Test( "sum = function \"a,b\", \"return a+b\"" );
			Test( 3, "sum 1,2" );
		}//RS04_rtfunc
		
		[TestMethod]
		public void RS05_ptfunc(  )
		{
			Test( "function sum a,b\r\n\treturn a+b" );
			Test( 3, "sum 1,2" );
		}//RS05_ptfunc
	}//BeeStatementRunTests
}//Bee.Run.Tests
