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

		[Test]
		public void ROS_Catch02_Throw()
		{
			Lines(ExitCode.Exception, "thrown",
				"throw \"thrown\"");
			/* TBD
			Lines(ExitCode.Exception, "catch",
				"global.done = false",
				"try",
				"  throw \"catch\"",
				"  return false",
				"finally",
				"  global.done = true",
				"  return true", // must not override active exception
				"return false");
			Assert.IsTrue(Globals["done"].ToBool());
			*/
		}
	}
}
