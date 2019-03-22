using System;
using System.Linq;
using NUnit.Framework;
using RedOnion.Script;

//TODO: goto

namespace RedOnion.ScriptNUnit
{
	[TestFixture]
	public class ROS_ExecutionCountdownTests : EngineTestsBase
	{
		public void Test(string script)
		{
			try
			{
				ExecutionCountdown = 10;
				Execute(script);
				Assert.Fail("Should never reach here");
			}
			catch (TookTooLong)
			{
				// this is expected
			}
			catch (Exception e)
			{
				throw new Exception(String.Format("{0} in Eval: {1}; IN: <{2}>",
					e.GetType().ToString(), e.Message, script), e);
			}
		}

		[Test]
		public void ROS_Loop01_WhileTrue()
		{
			Test("while true");
			Test("while true continue");
		}

		[Test]
		public void ROS_Loop02_UntilFalse()
		{
			Test("until false");
			Test("until false continue");
		}

		[Test]
		public void ROS_Loop03_DoWhileTrue()
		{
			Test("do while true");
			Test("do continue while true");
		}

		[Test]
		public void ROS_Loop04_DoUntilFalse()
		{
			Test("do until false");
			Test("do continue until false");
		}

		[Test]
		public void ROS_Loop05_ForLoop()
		{
			Test("for\r\n\tcontinue");
			Test("for var i = 1; i > 0; i++; continue");
		}
	}
}
