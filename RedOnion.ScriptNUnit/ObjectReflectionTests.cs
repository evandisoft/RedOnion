using System;
using System.Reflection;
using NUnit.Framework;
using RedOnion.Script;
using RedOnion.Script.ReflectedObjects;

namespace RedOnion.ScriptNUnit
{
	[TestFixture]
	public class ObjectReflectionTests : EngineTestsBase
	{
		[TearDown]
		public void ResetEngine() => Reset();

		public class PointClass
		{
			public int x, y;
			// these should shadow the fields
			// thanks to ordinal compare in ReflectedType.MemberComparer
			public int X { get => x; set => x = value; }
			public int Y { get => y; set => y = value; }

			public void MakeZero() => x = y = 0;
			public void MakeSame(int v) => x = y = v;

			public string name;
		}

		[Test]
		public void ObjectReflection_01_Methods()
		{
			var creator = new ReflectedType(this, typeof(PointClass));
			Root[typeof(PointClass)] = creator;
			Root.Set("Point", creator);

			Test("var pt = new point");
			var pt = Result.Native as PointClass;
			Assert.NotNull(pt);
			Test("pt.makeSame 1");
			Assert.AreEqual(1, pt.x);
			Assert.AreEqual(1, pt.y);
			Test("pt.makeZero()");
			Assert.AreEqual(0, pt.x);
			Assert.AreEqual(0, pt.y);
		}

		[Test]
		public void ObjectReflection_02_FieldsAndProps()
		{
			var creator = new ReflectedType(this, typeof(PointClass));
			Root[typeof(PointClass)] = creator;
			Root.Set("Point", creator);
			Test("var pt = new point");
			var pt = Result.Native as PointClass;
			Assert.NotNull(pt);
			Test(0, "pt.x");
			Test(1, "pt.y = 1");
			Test(1, "pt.y");
			Test("point", "pt.name = \"point\"");
			Test("point", "pt.name");
		}

		public struct Rect
		{
			public float X, Y, Width, Height;
			public float Left { get => X; set => X = value; }
			public float Right { get => X+Width; set => Width = value-X; }
			public float Top { get => Y; set => Y = value; }
			public float Bottom { get => Y+Height; set => Height = value-Y; }

			public Rect(float x, float y, float w, float h)
			{
				X = x; Y = y; Width = w; Height = h;
			}
		}

		public static class RectFunctions
		{
			public static float Area(Rect rc) => rc.Width*rc.Height;
		}

		[Test]
		public void ObjectReflection_03_Structure()
		{
			var creator = new ReflectedType(this, typeof(Rect));
			Root[typeof(Rect)] = creator;
			Root.Set("Rect", creator);
			Test("var rc = new rect");
			Assert.True(Result.Native is Rect);
			Test("rc.x = 10");
			Test(10, "rc.x");
			Test("rc.right = 30");
			Test(30, "rc.right");
			Test(20, "rc.width");

			Root.Set("area", new ReflectedFunction(this, null, "area",
				new MethodInfo[] { typeof(RectFunctions).GetMethod("Area") }));
			Test("rc.height = 10");
			Test(200, "area rc");
		}

		public delegate Rect WindowFunction(int id);

		public static class GUITest
		{
			public static int counter;
			public static Rect Window(int id, Rect rc, WindowFunction fn, string title)
				=> fn(id);
		}

		[Test]
		public void ObjectReflection_04_ComplexArguments()
		{
			var creator = new ReflectedType(this, typeof(Rect));
			Root[typeof(Rect)] = creator;
			Root.Set("Rect", creator);
			Root.Set("GUI", new ReflectedType(this,
				typeof(GUITest)));

			Test("var rc = new rect 10,40,200,300");
			Test(10, "rc.x");
			Test(
				"var title = \"ROS Test Window\"\n" +
				"function onGUI\n" +
				" rc = GUI.window 0, rc, testWindow, title\n" +
				"function testWindow id\n" +
				" GUI.counter++\n" +
				" return rc");
			Test("onGUI()");
			Assert.AreEqual(1, GUITest.counter);
		}

		public class DefaultConstruct
		{
			public string Name { get; set; }
			public DefaultConstruct(string name = null)
				=> Name = name;
		}
		[Test]
		public void ObjectReflection_05_CtorWithDefaultArgs()
		{
			var creator = new ReflectedType(this, typeof(DefaultConstruct));
			Root[typeof(DefaultConstruct)] = creator;
			Root.Set("thing", creator);
			Test("new thing");
			Assert.IsNotNull(Result.Native);
			Assert.AreEqual(typeof(DefaultConstruct), Result.Native.GetType());
		}

		public class GenericTest
		{
			public T Pass<T>(T value) => value;
		}
		[Test]
		public void ObjectReflection_06_GenericFunction()
		{
			var creator = new ReflectedType(this, typeof(GenericTest));
			Root[typeof(GenericTest)] = creator;
			Root.Set("testClass", creator);
			Test("var test = new testClass");
			Test(1, "test.pass 1");
			Test(2u, "test.pass.[uint] 2");
		}

		public class EventTest
		{
			public event Action action;
			public void DoAction() => action?.Invoke();
			public void AddAction(Action a) => action += a;
			public void RemoveAction(Action a) => action -= a;
			public int NumberOfActions => action?.GetInvocationList().Length ?? 0;
		}
		[Test]
		public void ObjectReflection_07_Events()
		{
			var creator = new ReflectedType(this, typeof(EventTest));
			Root[typeof(EventTest)] = creator;
			Root.Set("testClass", creator);
			Test("var test = new testClass");
			Test("var counter = 0");
			Test("function action\n\tcounter++");
			Test(0, "test.numberOfActions");
			Test("test.addAction action");
			Test(1, "test.numberOfActions");
			Test("test.doAction()");
			Test(1, "counter");
			Test("test.removeAction action"); // see FunctionObj.GetDelegate/DelegateCache
			Test(0, "test.numberOfActions");
			Test("test.action += action");
			Test(1, "test.numberOfActions");
		}
	}
}
