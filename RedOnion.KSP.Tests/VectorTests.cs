using System;
using NUnit.Framework;
using RedOnion.KSP.API;
using RedOnion.KSP.ROS;
using RedOnion.ROS;

namespace RedOnion.KSP.Tests
{
	public class ApiTestsBase : RosCore
	{
		public ApiTestsBase()
		{
			Descriptor.Create = CustomCreateDescriptor;
		}
		public static Descriptor CustomCreateDescriptor(Type type)
		{
			if (typeof(Delegate).IsAssignableFrom(type))
				return Descriptor.Callable.FromType(type);
			// to avoid those secirity exceptions mentioning ECall (e.g. on Vector3d.Slerp)
			if (type.Assembly.FullName.StartsWith("UnityEngine")
				|| type.Assembly.FullName.StartsWith("Assembly-CSharp"))
				return new DummyDescriptor(type);
			return new Descriptor.Reflected(type);
		}
		public class DummyDescriptor : Descriptor
		{
			public DummyDescriptor(Type type) : this(type.Name, type) { }
			public DummyDescriptor(string name, Type type) : base(name, type) { }
		}

		public void Test(string script, int countdown = 100)
		{
			try
			{
				Execute(script, null, countdown);
			}
			catch (Exception e)
			{
				throw new Exception(string.Format("{0} in Eval: {1}; IN: <{2}>",
					e.GetType().ToString(), e.Message, script), e);
			}
		}
		public void Test(object value, string script, int countdown = 100)
		{
			Test(script);
			Assert.True(Result.Equals(value),
				"Value: {0}\nExpected: {1}\nTest: <{2}>",
				Result, value, script);
		}
		public void Expect<Ex>(string script, int countdown = 100) where Ex : Exception
		{
			try
			{
				Execute(script, null, countdown);
				Assert.Fail("Should throw " + typeof(Ex).Name);
			}
			catch (Ex)
			{
			}
			catch (Exception e)
			{
				if (e is RuntimeError re && re.InnerException is Ex)
					return;
				throw new Exception(string.Format("{0} in Eval: {1}; IN: <{2}>",
					e.GetType().ToString(), e.Message, script), e);
			}
		}
		public void Lines(params string[] lines)
			=> Test(string.Join(Environment.NewLine, lines));
		public void Lines(object value, params string[] lines)
			=> Test(value, string.Join(Environment.NewLine, lines));
		public void Expect<Ex>(params string[] lines) where Ex : Exception
			=> Expect<Ex>(string.Join(Environment.NewLine, lines));
	}
	[TestFixture]
	public class API_VectorTests : ApiTestsBase
	{
		[Test]
		public void API_Vec01_Create()
		{
			Test("v(1)");
			var v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0, v.X);
			Assert.AreEqual(1.0, v.Y);
			Assert.AreEqual(1.0, v.Z);
			Test(1.0, "v(1).x");

			Test("var a = v 1,2u");
			v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0, v.X);
			Assert.AreEqual(2.0, v.Y);
			Assert.AreEqual(0.0, v.Z);
			Test(2.0, "a.y");

			Test("var b = v 1,2f,3.0");
			v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0, v.X);
			Assert.AreEqual(2.0, v.Y);
			Assert.AreEqual(3.0, v.Z);
			Test(3.0, "b.z");
		}
		[Test]
		public void API_Vec02_Unary()
		{
			Test("-v(1)");
			var v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(-1.0, v.X);
			Assert.AreEqual(-1.0, v.Y);
			Assert.AreEqual(-1.0, v.Z);

			Test("+v(1)");
			v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0, v.X);
			Assert.AreEqual(1.0, v.Y);
			Assert.AreEqual(1.0, v.Z);
		}
		[Test]
		public void API_Vec03_Binary()
		{
			Test("var a = v 1,2,3");
			Test("var b = v 4,5,6");
			Test("a + b");
			var v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0+4.0, v.X);
			Assert.AreEqual(2.0+5.0, v.Y);
			Assert.AreEqual(3.0+6.0, v.Z);

			Test("a - b");
			v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0-4.0, v.X);
			Assert.AreEqual(2.0-5.0, v.Y);
			Assert.AreEqual(3.0-6.0, v.Z);

			Test("a * b");
			v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0*4.0, v.X);
			Assert.AreEqual(2.0*5.0, v.Y);
			Assert.AreEqual(3.0*6.0, v.Z);

			Test("a / b");
			v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0/4.0, v.X);
			Assert.AreEqual(2.0/5.0, v.Y);
			Assert.AreEqual(3.0/6.0, v.Z);
		}

		public static class ConvertTest
		{
			public static Vector3d v3d;
			public static UnityEngine.Vector3 v3;
		}
		[Test]
		public void API_Vec04_Convert()
		{
			Globals.Add("test", typeof(ConvertTest));
			ConvertTest.v3d = Vector3d.zero;
			Test("test.v3d = v.one");
			Assert.AreEqual(1.0, ConvertTest.v3d.x);
			ConvertTest.v3 = UnityEngine.Vector3.zero;
			Test("test.v3 = v.one");
			Assert.AreEqual(1f, ConvertTest.v3.x);
			Test("v.abs -v test.v3");
			var v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0, v.X);
			Assert.AreEqual(1.0, v.Y);
			Assert.AreEqual(1.0, v.Z);
			Assert.AreEqual(Math.Sqrt(3.0), v.Size);
		}

		[Test]
		public void API_Vec05_Methods()
		{
			Lines(
				"var a = v.one",
				"a.scale 2",
				"return a");
			var v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(2.0, v.X);
			Assert.AreEqual(2.0, v.Y);
			Assert.AreEqual(2.0, v.Z);

			Lines(
				"a.shrink 3",
				"return a");
			Assert.AreEqual(2.0/3.0, v.X);
			Assert.AreEqual(2.0/3.0, v.Y);
			Assert.AreEqual(2.0/3.0, v.Z);
		}

		[Test]
		public void API_Vec06_Index()
		{
			Lines(1.0,
				"var a = v 1,2",
				"return a[0]");
			Test(2.0, "a[1]");
			Test(0.0, "a[2]");
			Test("a[2] = 3");
			Test(3.0, "a[2]");
		}
	}
}
