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
			public static string name;
			public static int Integer { get; set; }
			public static void Increment() => Integer++;
			public static int GetInteger() => Integer;
			public static void SetInteger(int value) => Integer = value;
			public static string Combine(string a, string b) => a + " " + b;
			public static double Sum3(double a, float b, int c) => a + b + c;
		}
		public class InstanceTest
		{
			public string name;
			public int Integer { get; set; }
			public void Increment() => Integer++;
			public int GetInteger() => Integer;
			public void SetInteger(int value) => Integer = value;
			public string Combine(string a, string b) => a + " " + b;
			public double Sum3(double a, float b, long c) => a + b + c;
		}
		public class MixedTest
		{
			public static string DefaultName;
			public string Name;
			public static float DefaultValue { get; set; }
			public float Value { get; set; }

			public void Increment() => Value++;
			public float GetValue() => Value;
			public static void IncrementDefault() => DefaultValue++;
			public static float GetDefaultValue() => DefaultValue;

			public MixedTest()
			{
				Name = DefaultName;
				Value = DefaultValue;
			}
		}

		[Test]
		public void ROS_Refl01_FieldAndProp()
		{
			Globals = new Globals();
			Globals.Add("test", typeof(StaticTest));
			Test("me", "test.name = \"me\"");
			Test("me", "test.name");
			Test(1, "test.integer = 1");
			Test(1, "test.integer");

			Globals.Add("it", new InstanceTest());
			Test("me", "it.name = \"me\"");
			Test("me", "it.name");
			Test(2, "it.integer = 2");
			Test(2, "it.integer");

			Globals.Add("smix", typeof(MixedTest));
			Test("smix.defaultName = \"me\"");
			Test("smix.defaultValue = 3.14");
			Test("var mix = new smix");
			Test("me", "mix.name");
			Test(3.14f, "mix.value");

			Test("mix.defaultName = \"mix\"");
			Test("mix.defaultValue = \"1.414\"");
			Test("var m = new mix");
			Test("mix", "m.name");
			Test(1.414f, "m.value");
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

			Globals.Add("mix", new MixedTest());
			Test("mix.defaultValue = 9");
			Test("mix.value = 3.14");
			Test("mix.increment");
			Test("mix.incrementDefault");
			Test("var m = new mix");
			Test("m.increment");
			Test("m.increment");
			Test(10f, "m.defaultValue");
			Test(4.14f, "mix.value");
			Test(12f, "m.value");
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

			Test("hello world", @"test.combine ""hello"", ""world""");
			Test("hello world", @"it.combine ""hello"", ""world""");

			Test(1.0+2f+3, @"test.sum3 1.0, 2f, 3");
			Test(1.0+2f+3, @"it.sum3 1.0, 2f, 3");
		}
	}
}
