using System;
using NUnit.Framework;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
using RedOnion.Script.ReflectedObjects;

namespace RedOnion.ScriptNUnit
{
	[TestFixture]
	public class ROS_ObjectModelTests : EngineTestsBase
	{
		[TearDown]
		public void ResetEngine() => Reset();

		[Test]
		public void ROS_Root01_LazyCreate()
		{
			var created = false;
			Root.Set("test", new Value(engine =>
			{
				var obj = new Script.BasicObjects.BasicObject(engine);
				obj.Set("value", 3.14);
				created = true;
				return obj;
			}));
			Assert.AreEqual(false, created);
			Test(3.14, "test.value");
			Assert.AreEqual(true, created);
		}

		[Test]
		public void ROS_Root02_NumberConvert()
		{
			Test(3.14f, "float 3.14");
			Test(123, "int \"123\"");
		}

		public class Thing { }
		[Test]
		public void ROS_Root03_NewWithNamespace()
		{
			Root.Set("space", new SimpleObject(this, new Properties()
			{
				{ "Thing", Root[typeof(Thing)] }
			}));
			Test("var it = new space.thing");
			Assert.AreEqual(typeof(Thing), Result.Native.GetType());
		}

		[Test]
		public void ROS_Root04_EasyProp()
		{
			var test = "hello";
			Root.Set("it", new SimpleObject(this, new Properties()
				.Set("one", dummy => 1)
				.Set("test", dummy => test, (dummy, value) => test = value.String)));
			Test(1, "it.one");
			Test("it.test = \"blah\"");
			Test("blah", "it.test");
			Expect<InvalidOperationException>("it.one = 2");
		}

		[Test]
		public void ROS_Root05_Enums()
		{
			Root.AddType("kind", typeof(ValueKind));
			Test(ValueKind.Long, "kind.long");
			Test(ValueKind.Int|ValueKind.fEnum, "kind.int | kind.fEnum");
		}
	}
}
