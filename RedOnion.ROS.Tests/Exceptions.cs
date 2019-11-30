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

		public static void ThrowError()
			=> throw new InvalidOperationException("error");
		[Test]
		public void ROS_Catch02_Finally()
		{
			Lines(ExitCode.Exception, "thrown",
				"throw \"thrown\"");

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

			Lines(ExitCode.Exception, 3.14,
				"global.counter = 0",
				"try",
				"  counter++",
				"  try",
				"    counter++",
				"    raise 3.14",
				"  finally",
				"    counter++",
				"  return 0",
				"finally",
				"  counter++");
			Assert.AreEqual(Globals["counter"].ToInt(), 4);

			Globals["throwError"] = new Value(ThrowError);
			Expect<RuntimeError>("throwError");
			Assert.IsTrue((error.obj as RuntimeError)?.InnerException is InvalidOperationException);
			Assert.AreEqual("error", ((error.obj as RuntimeError)?.InnerException as InvalidOperationException)?.Message);

			Expect<RuntimeError>(
				"global.done = false",
				"try",
				"  throwError",
				"finally",
				"  global.done = true");
			Assert.IsTrue(Globals["done"].ToBool());
		}
	}
}
