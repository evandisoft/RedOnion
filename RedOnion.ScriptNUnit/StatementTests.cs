using System;
using System.Linq;
using NUnit.Framework;
using RedOnion.Script;

namespace RedOnion.ScriptNUnit
{
	public class StatementTestsBase : EngineTestsBase
	{
		public void Test(OpCode code, object value, string script)
		{
			Test(script);
			Assert.AreEqual(code, Exit, "Test: <{0}>", script);
			Assert.AreEqual(value, Result.Native, "Test: <{0}>", script);
		}
	}

	[TestFixture]
	public class ROS_StatementTests : StatementTestsBase
	{
		[Test]
		public void ROS_EStts01_Return()
		{
			Test(OpCode.Return, null, "return");
			Test(OpCode.Return, 1234, "return 1234");
			Test(OpCode.Return, 12/5, "return 12/5");
		}

		[Test]
		public void ROS_EStts02_For()
		{
			Test(OpCode.Return, "321",
				"var s = \"\"\r\n" +
				"for var i = 3; i; i -= 1; s += i\r\n" +
				"return s");
		}

		[Test]
		public void ROS_EStts03_If()
		{
			Test(OpCode.Return, true, "if true then return true");
			Test(OpCode.Return, false, "if false: return true else: return false");
		}

		[Test]
		public void ROS_EStts04_FunctionFromStr()
		{
			// lowercase `function` may mean delegate or lambda
			// => either `Function` or `new function` for this
			Test("var sum1 = Function \"a,b\", \"return a+b\"");
			Test(3, "sum1 1,2");
		}

		[Test]
		public void ROS_EStts05_FunctionInScript()
		{
			Test("function sum2 a,b\r\n\treturn a+b");
			Test(3, "sum2 1,2");
		}
	}
}
