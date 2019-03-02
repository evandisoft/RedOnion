using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace Bee.Run.Tests
{
	[TestClass]
	public class BeeScopeRunTests: BeeStatementRunTestsBase
	{
		[TestCleanup]
		public void Cleanup(  )
		{
			Opts = Opt.None;
		}//Cleanup
		
		[TestMethod]
		public void RV01_global(  )
		{
			Test( 1, (("var x = 1\r\n" + "function f\r\n") + "	return x\r\n") + "f()" );
			Test( 1, (("var x = 1\r\n" + "function f\r\n") + "	return this.x\r\n") + "f()" );
		}//RV01_global
		
		[TestMethod]
		public void RV02_local(  )
		{
			Test( 2, ((("var x = 1\r\n" + "function f\r\n") + "	var x = 2\r\n") + "	return x\r\n") + "f()" );
		}//RV02_local
		
		[TestMethod]
		public void RV03_block(  )
		{
			var s = (((("var x = 1\r\n" + "function f\r\n") + "	if true\r\n") + "		var x = 2\r\n") + "	return x\r\n") + "f()";
			Test( 2, s );
			Opts = Opt.BlockScope;
			Test( 1, s );
		}//RV03_block
		
		[TestMethod]
		public void RV04_closure(  )
		{
			Test( 2, ((((("var x = 1\r\n" + "function f\r\n") + "	var x = 2\r\n") + "	functon g\r\n") + "		return x\r\n") + "	return g()\r\n") + "f()" );
		}//RV04_closure
		
		[TestMethod]
		public void RV05_prototype(  )
		{
			Test( 2, (((("var x = 1\r\n" + "function f\r\n") + "	this.x = 2\r\n") + "f.prototype = new object\r\n") + "f.prototype.x = 3\r\n") + "(new f).x" );
			Test( 3, (((("var x = 1\r\n" + "function f\r\n") + "	x = 2\r\n") + "f.prototype = new object\r\n") + "f.prototype.x = 3\r\n") + "(new f).x" );
		}//RV05_prototype
	}//BeeScopeRunTests
}//Bee.Run.Tests
