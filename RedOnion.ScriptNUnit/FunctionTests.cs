using NUnit.Framework;
using RedOnion.Script;

namespace RedOnion.ScriptNUnit
{
	[TestFixture]
	public class ROS_FunctionTests : StatementTestsBase
	{
		[TearDown]
		public void ResetEngine() => Reset();

		[Test]
		public void ROS_Func01_FromStr()
		{
			// lowercase `function` may mean delegate or lambda
			// => either `Function` or `new function` for this
			Test("var sum1 = Function \"a,b\", \"return a+b\"");
			Test(3, "sum1 1,2");
		}

		[Test]
		public void ROS_Func02_InScript()
		{
			Test("function sum2 a,b\r\n\treturn a+b");
			Test(3, "sum2 1,2");
		}
	}
}
