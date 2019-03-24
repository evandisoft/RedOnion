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
			Lines(
				"function sum2 a,b",
				"	return a+b");
			Test(3, "sum2 1,2");
		}

		[Test]
		public void ROS_Func03_ArgsLength()
		{
			Lines(3,
				"def test", // alternative syntax from Python/Ruby
				"	return arguments.length",
				"test 0,1,2");
		}

		[Test]
		public void ROS_Func04_InlineFunc()
		{
			Lines(3.14,
				"var test = function",
				"	return 3.14",
				"test");
			Test("return test");
			Assert.IsTrue(Result.RefObj is Script.BasicObjects.FunctionObj);
			Lines(1f,
				"var v = 1f",
				"var test = def",
				"	return v",
				"test");
			Lines(3u,
				"var sum = def x, y",
				"	return x + y",
				"var a = 1u",
				"var b = 2u",
				"sum a, b");
		}

		[Test]
		public void ROS_Func05_ShortLambda()
		{
			Lines(3u,
				"var sum = def x, y => x + y",
				"var a = 1u",
				"var b = 2u",
				"sum a, b");
			Reset();
			Lines(3u,
				"var sum = (def x, y => x + y)",
				"var a = 1u",
				"var b = 2u",
				"sum a, b");
			Reset();
			Lines(3u,
				"var sum = (def x, y; return x + y)",
				"var a = 1u",
				"var b = 2u",
				"sum a, b");
			Reset();
			Lines(3u, // def/function is hungry for statements
				"var sum = def x, y; return x + y",
				"var a = 1u",
				"var b = 2u",
				"sum a, b");
			Reset();
			Lines(3u,
				"var sum = (def x, y",
				"	return x + y)",
				"var a = 1u",
				"var b = 2u",
				"sum a, b");
		}
	}
}
