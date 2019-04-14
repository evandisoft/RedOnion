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

		public void Test(OpCode code, object value, string script, int countdown = 1000)
		{
			Test(script, countdown);
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

		[Test]
		public void ROS_Stts03_Loops()
		{
			Lines(OpCode.Return, 5,
				"var counter = 0",
				"while counter < 5",
				"  counter++",
				"return counter");
			Lines(OpCode.Return, 3,
				"counter = 0",
				"until counter > 2",
				"  counter++",
				"return counter");
			Lines(OpCode.Return, 2,
				"counter = 0",
				"do counter++ while counter < 2",
				"return counter");
			Lines(OpCode.Return, 6,
				"counter = 0",
				"do",
				"  if ++counter > 5; break",
				"  if counter > 3; continue",
				"  counter++",
				"  continue",
				"  counter = 10",
				"until counter > 10",
				"return counter");
		}

		[Test]
		public void ROS_Stts04_For()
		{
			Lines(OpCode.Return, "321",
				"var s = \"\"",
				"for var i = 3; i; i -= 1; s += i",
				"return s");
			Lines(OpCode.Return, "135",
				"s = \"\"",
				"for i = 0; i < 10; i++",
				"  if i&1 == 0; continue",
				"  s += i",
				"  if i == 5; break",
				"return s");
		}
	}
}
