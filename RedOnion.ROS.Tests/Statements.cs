using System;
using System.Collections;
using NUnit.Framework;
using RedOnion.ROS.Objects;

namespace RedOnion.ROS.Tests
{
	[TestFixture]
	public class ROS_Statements : CoreTests
	{
		[TearDown]
		public void ResetGlobals() => Globals = null;

		public void Test(OpCode code, object value, string script, int countdown = 100)
		{
			Test(script);
			Assert.AreEqual(code, Exit, "Test: <{0}>", script);
			var result = Result.Object;
			Assert.AreEqual(value, result, "Different result: <{0}>", script);
			Assert.AreEqual(value?.GetType(), result?.GetType(), "Different type: <{0}>", script);
		}
		public void Lines(OpCode code, object value, params string[] lines)
			=> Test(code, value, string.Join(Environment.NewLine, lines));
		public void Lines(OpCode code, object value, int countdown, params string[] lines)
			=> Test(code, value, string.Join(Environment.NewLine, lines), countdown);

		[Test]
		public void ROS_Stts01_Return()
		{
			Test(OpCode.Return, null, "return");
			Test(OpCode.Return, 1234, "return 1234");
			Test(OpCode.Return, 12/5, "return 12/5");
		}

		[Test]
		public void ROS_Stts02_IfElse()
		{
			Test(OpCode.Return, true, "if true then return true");
			Test(OpCode.Return, false, "if false: return true else: return false");
		}
	}
}
