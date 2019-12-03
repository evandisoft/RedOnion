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
				"  return true", // Java allows this, C# does not
				"return false");
			Lines(ExitCode.Return, true,
				"var ok = false",
				"try",
				"  ok = true",
				"catch string",
				"  ok = false",
				"finally",
				"  return ok"); // again: Java
		}

		public static void ThrowError()
			=> throw new InvalidOperationException("error");
		[Test]
		public void ROS_Catch02_Finally()
		{
			Lines(ExitCode.Exception, "thrown",
				"throw \"thrown\"");
			Lines(ExitCode.Exception, "thrown",
				"global.done = false",
				"throw \"thrown\"",
				"done = true");
			Assert.IsFalse(Globals["done"].ToBool());

			Lines(ExitCode.Exception, "catch",
				"global.done = false",
				"global.test = false",
				"try",
				"  throw \"catch\"",
				"  test = true",
				"  return false",
				"finally",
				"  done = true",
				"  return true", // must not override active exception
				"test = true",
				"return false");
			Assert.IsTrue(Globals["done"].ToBool());
			Assert.IsFalse(Globals["test"].ToBool());

			Lines(ExitCode.Exception, 3.14,
				"global.counter = 0",
				"global.test = false",
				"try",
				"  counter++",
				"  try",
				"    counter++",
				"    raise 3.14",
				"    test = true",
				"  finally",
				"    counter++",
				"  test = true",
				"  return 0",
				"finally",
				"  counter++",
				"test = true");
			Assert.AreEqual(4, Globals["counter"].ToInt());
			Assert.IsFalse(Globals["test"].ToBool());

			Globals["throwError"] = new Value(ThrowError);
			Expect<RuntimeError>("throwError");
			Assert.IsTrue((error.obj as RuntimeError)?.InnerException is InvalidOperationException);
			Assert.AreEqual("error", ((error.obj as RuntimeError)?.InnerException as InvalidOperationException)?.Message);

			Expect<RuntimeError>(
				"global.done = false",
				"global.test = false",
				"try",
				"  throwError",
				"  test = true",
				"finally",
				"  done = true",
				"test = true");
			Assert.IsTrue(Globals["done"].ToBool());
			Assert.IsFalse(Globals["test"].ToBool());
		}

		[Test]
		public void ROS_Catch03_FinallyWithDef()
		{
			Globals["throwError"] = new Value(ThrowError);
			Expect<RuntimeError>(
				"global.done = false",
				"global.test = false",
				"def throwIt",
				"  throwError",
				"  test = true",
				"try",
				"  throwIt",
				"  test = true",
				"finally",
				"  done = true",
				"test = true");
			Assert.IsTrue(Globals["done"].ToBool());
			Assert.IsFalse(Globals["test"].ToBool());
			Assert.IsTrue((error.obj as RuntimeError)?.InnerException is InvalidOperationException);
			Assert.AreEqual("error", ((error.obj as RuntimeError)?.InnerException as InvalidOperationException)?.Message);

			Expect<RuntimeError>(
				"global.counter = 0",
				"global.test = false",
				"def throwIt",
				"  throwError",
				"  test = true",
				"try",
				"  counter++",
				"  try",
				"    counter++",
				"    throwIt",
				"    test = true",
				"  finally",
				"    counter++",
				"  test = true",
				"  return 0",
				"finally",
				"  counter++",
				"test = true");
			Assert.IsFalse(Globals["test"].ToBool());
			Assert.AreEqual(4, Globals["counter"].ToInt());
			Assert.IsTrue((error.obj as RuntimeError)?.InnerException is InvalidOperationException);
			Assert.AreEqual("error", ((error.obj as RuntimeError)?.InnerException as InvalidOperationException)?.Message);

			Lines(ExitCode.Exception, 3.14,
				"global.counter = 0",
				"global.test = false",
				"def throwIt",
				"  raise 3.14",
				"  test = true",
				"try",
				"  counter++",
				"  try",
				"    counter++",
				"    throwIt",
				"    test = true",
				"  finally",
				"    counter++",
				"  test = true",
				"  return 0",
				"finally",
				"  counter++",
				"test = true");
			Assert.IsFalse(Globals["test"].ToBool());
			Assert.AreEqual(4, Globals["counter"].ToInt());
		}

		[Test]
		public void ROS_Catch04_SimpleCatch()
		{
			Lines(ExitCode.Return, true,
				"var result = false",
				"try",
				"  throw 1",
				"else",
				"  result = true",
				"return result");

			Globals["throwError"] = new Value(ThrowError);
			Lines(ExitCode.Return, true,
				"var result = false",
				"try",
				"  throwError",
				"catch",
				"  result = true",
				"return result");

			Lines(ExitCode.Return, true,
				"var result = false",
				"try",
				"  throw true",
				"catch var x",
				"  result = x",
				"return result");

			Lines(ExitCode.Return, "error",
				"var result = false",
				"try",
				"  throwError",
				"catch var x",
				"  result = x.message",
				"return result");
		}

		[Test]
		public void ROS_Catch05_Return()
		{
			// like empty finally, but we have to track the pending return
			Lines(ExitCode.Return, true,
				"try",
				"  return true");

			// Java allows that, C# does not
			Lines(ExitCode.Return, true,
				"try",
				"  return false",
				"finally",
				"  return true");

			// with the pending return implemented, we can just pretend the return is inside try and no exception happened
			Lines(ExitCode.Return, 2.7f,
				"try",
				"  throw 3.14",
				"catch",
				"  return 2.7f");

			Globals["throwError"] = new Value(ThrowError);
			Lines(ExitCode.Return, "error",
				"try",
				"  throwError",
				"catch var x",
				"  return x.message");
		}

		[Test]
		public void ROS_Catch06_Break()
		{
			Lines(ExitCode.Return, true,
				"do",
				"  try",
				"    throw false",
				"  catch",
				"    break",
				"while true",
				"return true");
		}

		[Test]
		public void ROS_Catch07_Continue()
		{
			Lines(ExitCode.Return, 3,
				"var x = 0",
				"for var i = 0; i < 3; i++",
				"  try",
				"    throw 1",
				"  catch var e",
				"    x += e",
				"    continue",
				"  return false",
				"return x");
		}
	}
}
