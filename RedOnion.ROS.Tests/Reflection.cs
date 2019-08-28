using System;
using System.Collections;
using NUnit.Framework;
using RedOnion.ROS.Objects;

namespace RedOnion.ROS.Tests
{
	[TestFixture]
	public class ROS_Reflection : CoreTests
	{
		public delegate void MyAction();
		public delegate void MyAction2(string v);
		public static class StaticTest
		{
			public static string name;
			public static int Integer { get; set; }
			public static void Increment() => Integer++;
			public static int GetInteger() => Integer;
			public static void SetInteger(int value) => Integer = value;
			public static string Combine(string a, string b) => a + " " + b;
			public static double Sum3(double a, float b, int c) => a + b + c;
			public static MyAction action;
			public static void AddAction(MyAction add) => action += add;
			public static void CallAction() => action?.Invoke();
			public static MyAction2 action2;
			public static void AddAction2(MyAction2 add) => action2 += add;
			public static void CallAction2(string v) => action2?.Invoke(v);
			public static void DefaultArg(string v = "default") => name = v;
			public static void DefaultArg2(string v = "test", int i = 333)
			{
				name = v;
				Integer = i;
			}
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
			public MyAction action;
			public void AddAction(MyAction add) => action += add;
			public void CallAction() => action?.Invoke();
			public MyAction2 action2;
			public void AddAction2(MyAction2 add) => action2 += add;
			public void CallAction2(string v) => action2?.Invoke(v);
			public void DefaultArg(string v = "default") => name = v;
			public void DefaultArg2(string v = "test", int i = 333)
			{
				name = v;
				Integer = i;
			}
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

			Test(1.0+2f+3, "test.sum3 1.0, 2f, 3");
			Test(1.0+2f+3, "it.sum3 1.0, 2f, 3");

			Test("default", "test.defaultArg; test.name");
			Test("default", "it.defaultArg; it.name");

			Test("test", "test.defaultArg2; test.name");
			Test(333, "test.integer");
			Test("test", "it.defaultArg2; it.name");
			Test(333, "it.integer");
			Test("hello", "test.defaultArg2 \"hello\"; test.name");
			Test("hello", "it.defaultArg2 \"hello\"; it.name");
		}

		public struct Point
		{
			public float x, y;
			public Point(float x, float y)
			{
				this.x = x;
				this.y = y;
			}
		}
		public static class PointUtils
		{
			public static float Magnitude(Point pt)
				=> (float)Math.Sqrt(pt.x*pt.x+pt.y*pt.y);
		}
		[Test]
		public void ROS_Refl04_Struct()
		{
			Globals = new Globals();
			Globals.Add(typeof(Point));
			Test("var pt = new point 1,2");
			Test(1f, "pt.x");
			Globals.Add("ptu", typeof(PointUtils));
			Test((float)Math.Sqrt(5f), "ptu.magnitude pt");
		}

		public enum TestEnum : byte
		{
			Zero, One, Two, Three
		}
		public static class EnumUtils
		{
			public static int Enum2Int(TestEnum e)
				=> (int)e;
			public static TestEnum Or(TestEnum a, TestEnum b)
				=> (TestEnum)((int)a | (int)b);
		}
		[Test]
		public void ROS_Refl05_Enum()
		{
			Globals = new Globals();
			Globals.Add(typeof(TestEnum));
			Globals.Add(typeof(EnumUtils));
			Test("var zero = testEnum.zero");
			Test(TestEnum.Zero, "zero");
			Test(1, "enumUtils.enum2int testEnum.one");
			Test(TestEnum.Three, "enumUtils.or testEnum.one, testEnum.two");
		}

		[Test]
		public void ROS_Refl06_Math()
		{
			Globals = new Globals();
			Test(3.14, "math.abs -3.14");
			Test(2.7f, "math.abs -2.7f");
			Test(1.41, "math.max 1.0, 1.41");
			Test(30.0, "math.deg.asin 0.5");
		}

		[Test]
		public void ROS_Refl07_Action()
		{
			Globals = new Globals();
			Globals.Add(typeof(StaticTest));
			StaticTest.Integer = 0;
			Test("def action => staticTest.integer = 10");
			Test("staticTest.addAction action");
			Assert.AreEqual(0, StaticTest.Integer);
			StaticTest.CallAction();
			Assert.AreEqual(0, StaticTest.Integer);
			UpdatePhysics();
			Assert.AreEqual(10, StaticTest.Integer);

			var it = new InstanceTest();
			Globals.Add("it", it);
			Test("it.addAction def => it.name = \"done\"");
			Assert.IsNull(it.name);
			it.CallAction();
			Assert.IsNull(it.name);
			UpdatePhysics();
			Assert.AreEqual("done", it.name);
		}

		[Test]
		public void ROS_Refl08_ActionArgs()
		{
			Globals = new Globals();
			Globals.Add(typeof(StaticTest));
			StaticTest.name = null;
			Test("def setName name => staticTest.name = name");
			Test("staticTest.addAction2 setName");
			StaticTest.CallAction2("test");
			UpdatePhysics();
			Assert.AreEqual("test", StaticTest.name);
		}
	}
}
