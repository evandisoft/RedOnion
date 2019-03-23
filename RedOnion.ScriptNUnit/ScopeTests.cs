using System;
using System.Linq;
using NUnit.Framework;
using RedOnion.Script;

namespace RedOnion.ScriptNUnit
{
	[TestFixture]
	public class ROS_ScopeTests: StatementTestsBase
	{
		[SetUp]
		public void SetUp()
		{
			Options = EngineOption.BlockScope;
		}

		[Test]
		public void ROS_Scope01_Global()
		{
			Test(1,
				"var x = 1\r\n" +
				"function f\r\n" +
				"	return x\r\n" +
				"f()");

			// this = global
			Options = EngineOption.BlockScope;
			Test(1,
				"var x = 1\r\n" +
				"function f\r\n" +
				"	return this.x\r\n" +
				"f()");

			// this = null
			Options = EngineOption.BlockScope | EngineOption.Strict | EngineOption.Silent;
			Test(null,
				"var x = 1\r\n" +
				"function f\r\n" +
				"	return this.x\r\n" +
				"f()");

			Options = EngineOption.BlockScope | EngineOption.Strict;
			try
			{
				ExecutionCountdown = 10;
				Execute(
					"var x = 1\r\n" +
					"function f\r\n" +
					"	return this.x\r\n" +
					"f()");
				Assert.Fail("Should throw exception");
			}
			catch(InvalidOperationException)
			{
			}

			try
			{
				ExecutionCountdown = 10;
				Execute("y = 2");
				Assert.Fail("Should throw exception");
			}
			catch (InvalidOperationException)
			{
			}
		}

		[Test]
		public void ROS_Scope02_Local()
		{
			Test(2,
				"var x = 1\r\n" +
				"function f\r\n" +
				"	var x = 2\r\n" +
				"	return x\r\n" +
				"f()");

			Test(1,
				"var x = 1\r\n" +
				"function f\r\n" +
				"	var y = x\r\n" +
				"	var x = 2\r\n" +
				"	return y\r\n" +
				"f()");
			/*
			JavaScript would actually return undefined
			because it moves all declarations to the top of the scopes
			effectively seeing it like this:

				var x
				x = 1
				function f
					var x,y
					y = x
					x = 2
					return y
			*/
		}

		[Test]
		public void ROS_Scope03_Block()
		{
			var s =
				"var x = 1\r\n" +
				"function f\r\n" +
				"	if true\r\n" +
				"		var x = 2\r\n" +
				"	return x\r\n" +
				"f()";
			Options = EngineOption.None;
			Test(2, s);
			Options = EngineOption.BlockScope;
			Test(1, s);
		}

		[Test]
		public void ROS_Scope04_Closure()
		{
			Test(2,
				"var x = 1\r\n" +
				"function f\r\n" +
				"	var x = 2\r\n" +
				"	function g\r\n" +
				"		return x\r\n" +
				"	return g()\r\n" +
				"f()");

			Test(2,
				"var x = 1\r\n" +
				"function f\r\n" +
				"	var x = 2\r\n" +
				"	function g\r\n" +
				"		return x\r\n" +
				"	return g\r\n" +
				"f()()");
		}

		[Test]
		public void ROS_Scope05_Prototype()
		{
			Test(2,
				"var x = 1\r\n" +
				"function f\r\n" +
				"	this.x = 2\r\n" +
				"f.prototype = new object\r\n" +
				"f.prototype.x = 3\r\n" +
				"(new f).x");
			Test(3,
				"var x = 1\r\n" +
				"function f\r\n" +
				"	x = 2\r\n" +
				"f.prototype = new object\r\n" +
				"f.prototype.x = 3\r\n" +
				"(new f).x");
		}
	}
}
