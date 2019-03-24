using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using RedOnion.Script;
using RedOnion.Script.ReflectedObjects;

namespace RedOnion.ScriptNUnit
{
	[TestFixture]
	public class ROS_ObjectReflectionTests : EngineTestsBase
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
		public void ROS_DRefl01_Methods()
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
		public void ROS_DRefl02_FieldsAndProps()
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
		public void ROS_DRefl03_Structure()
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
		public void ROS_DRefl04_ComplexArguments()
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
		public void ROS_DRefl05_CtorWithDefaultArgs()
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
		public void ROS_DRefl06_GenericFunction()
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
		public void ROS_DRefl07_Events()
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

		[Test]
		public void ROS_DRefl08_Dictionary()
		{
			var creator = new ReflectedType(this, typeof(Dictionary<string, string>));
			Root[typeof(Dictionary<string, string>)] = creator;
			Root.Set("dict", creator);
			Test("var test = new dict");
			var dict = Result.Native as Dictionary<string, string>;
			dict["test"] = "it";
			Test("it", "test[\"test\"]");
		}

		public struct MyVector
		{
			public float x, y, z;
			public MyVector(float x, float y, float z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}
			public static MyVector operator +(MyVector a, MyVector b)
				=> new MyVector(a.x + b.x, a.y + b.y, a.z + b.z);
			public static MyVector operator *(MyVector v, float f)
				=> new MyVector(v.x * f, v.y * f, v.z * f);
			public static MyVector operator *(float f, MyVector v)
				=> new MyVector(v.x * f, v.y * f, v.z * f);
		}
		[Test]
		public void ROS_DRefl09_Operators()
		{
			Root.AddType("vector", typeof(MyVector));
			Test("var v = new vector 1,2f,3.0");
			var v = (MyVector)Result.Native;
			Assert.AreEqual(3f, v.z);
			Test("var u = v * 2");
			var u = (MyVector)Result.Native;
			Assert.AreEqual(2f, u.x);
			Test("var w = 3 * v");
			var w = (MyVector)Result.Native;
			Assert.AreEqual(6f, w.y);
		}

		public struct MyEvent
		{
			public List<Action> it;
			public MyEvent(bool dummy) => it = new List<Action>();
			public void Add(Action call) => it.Add(call);
			public void Remove(Action call) => it.Remove(call);
			public void Clear() => it.Clear();
			public void Invoke()
			{
				foreach (var action in it)
					action();
			}
			public void Set(Action call)
			{
				Clear();
				Add(call);
			}

			public struct AddProxy
			{
				internal readonly MyEvent Event;
				internal readonly Action Action;
				public AddProxy(MyEvent e, Action a)
				{
					Event = e;
					Action = a;
				}
			}
			public struct RemoveProxy
			{
				internal readonly MyEvent Event;
				internal readonly Action Action;
				public RemoveProxy(MyEvent e, Action a)
				{
					Event = e;
					Action = a;
				}
			}
			public static AddProxy operator +(MyEvent e, Action a)
				=> new AddProxy(e, a);
			public static RemoveProxy operator -(MyEvent e, Action a)
				=> new RemoveProxy(e, a);
			public MyEvent(AddProxy add)
			{
				it = add.Event.it;
				Add(add.Action);
			}
			public MyEvent(RemoveProxy remove)
			{
				it = remove.Event.it;
				Remove(remove.Action);
			}
			public static implicit operator MyEvent(AddProxy add)
				=> new MyEvent(add);
			public static implicit operator MyEvent(RemoveProxy remove)
				=> new MyEvent(remove);

			public MyEvent Click
			{
				get => this;
				set { }
			}
		}
		[Test]
		public void ROS_DRefl10_CustomEvent()
		{
			var myEvent = new MyEvent(true);
			int counter = 0;
			myEvent += () => counter++;
			Assert.AreEqual(1, myEvent.it.Count);
			myEvent.Invoke();
			Assert.AreEqual(1, counter);

			Root.AddType(typeof(MyEvent));
			Test("var e = new myEvent true");
			Test("var c = 0");
			Test("e.click += def => c++");
			Test("e.click.invoke");
			Test(1, "c");
		}
	}
}
