using System;
using System.Collections;
using NUnit.Framework;
using RedOnion.ROS.Objects;

namespace RedOnion.ROS.Tests
{
	[TestFixture]
	public class ROS_Reflection : CoreTests
	{
		public static class StaticTest
		{
			public static int Integer { get; set; }
			public static void Increment() => Integer++;
			public static int GetInteger() => Integer;
			public static void SetInteger(int value) => Integer = value;
		}
		public class InstanceTest
		{
			public int Integer { get; set; }
			public void Increment() => Integer++;
			public int GetInteger() => Integer;
			public void SetInteger(int value) => Integer = value;
		}

		[Test]
		public void ROS_Refl01_Properties()
		{
			Globals = new Globals();
			Globals.Add("test", typeof(StaticTest));
			Test(1, "test.integer = 1");
			Test(1, "test.integer");

			Globals.Add("it", new InstanceTest());
			Test(2, "it.integer = 2");
			Test(2, "it.integer");
		}

		[Test]
		public void ROS_Refl02_SimpleAction()
		{
			Globals = new Globals();
			Globals.Add("test", typeof(StaticTest));
			StaticTest.Integer = 1;
			Test("test.increment()");
			Assert.AreEqual(2, StaticTest.Integer);
			Test("test.increment");
			Assert.AreEqual(3, StaticTest.Integer);

			Globals.Add("it", new InstanceTest());
			Test("it.increment()");
			Test(1, "it.integer");
			Test("it.increment");
			Test(2, "it.integer");
		}

		[Test]
		public void ROS_Refl03_Methods()
		{
			Globals = new Globals();
			Globals.Add("test", typeof(StaticTest));
			Test("test.setInteger(10)");
			Test(10, "test.getInteger()");

			Globals.Add("it", new InstanceTest());
			Test("it.setInteger(10)");
			Test(10, "it.getInteger");
		}
	}
}
