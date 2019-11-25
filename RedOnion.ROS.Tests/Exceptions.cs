using System;
using System.Collections;
using NUnit.Framework;
using RedOnion.ROS.Objects;

namespace RedOnion.ROS.Tests
{
	[TestFixture]
	public class ROS_Exceptions : StatementTests
	{
		[TearDown]
		public override void Reset() => base.Reset();

		[Test]
		public void ROS_Catch01_NoThrow()
		{
			Lines(ExitCode.Return, true,
				"try",
				"return true");
			Lines(ExitCode.Return, true,
				"var ok = false",
				"try",
				"  ok = true",
				"return ok");
			Lines(ExitCode.Return, true,
				"try",
				"finally",
				"  return true",
				"return false");
			Lines(ExitCode.Return, true,
				"var ok = false",
				"try",
				"  ok = true",
				"catch string",
				"  ok = false",
				"finally",
				"  return ok");
		}
	}
}
