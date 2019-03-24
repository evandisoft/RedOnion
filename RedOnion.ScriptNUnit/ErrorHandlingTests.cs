using System;
using NUnit.Framework;
using RedOnion.Script;

namespace RedOnion.ScriptNUnit
{
	[TestFixture]
	public class ROS_ErrorHandlingTests : EngineTestsBase
	{
		public ROS_ErrorHandlingTests() => Options &= ~EngineOption.Repl;

		[TearDown]
		public void ResetEngine() => Reset();

		public void Expect<Ex>(int lineNumber, string line, string script) where Ex : Exception
		{
			try
			{
				ExecutionCountdown = 100;
				Execute(script);
				Assert.Fail("Should throw " + typeof(Ex).Name);
			}
			catch (Exception e)
			{
				if (e is RuntimeError re && re.InnerException is Ex)
				{
					Assert.AreEqual(lineNumber, re.LineNumber);
					Assert.AreEqual(line, re.Line);
					return;
				}
				throw new Exception(string.Format("{0} in Eval: {1}; IN: <{2}>",
					e.GetType().ToString(), e.Message, script), e);
			}
		}
		public void Expect<Ex>(int lineNumber, string line, params string[] script) where Ex : Exception
			=> Expect<Ex>(lineNumber, line, string.Join(Environment.NewLine, script));

		[Test]
		public void ROS_Errs01_Simple()
		{
			Expect<InvalidOperationException>(
				0, "x",
				"x");
			Expect<InvalidOperationException>(
				1, "x",
				"var x = 1",
				"x");
			Expect<InvalidOperationException>(
				1, "x",
				"x = 2",
				"x",
				"y");
		}
	}
}
