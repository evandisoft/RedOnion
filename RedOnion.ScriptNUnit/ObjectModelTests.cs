using System;
using NUnit.Framework;
using RedOnion.Script;

namespace RedOnion.ScriptNUnit
{
	[TestFixture]
	public class ObjectModelTests : EngineTestsBase
	{
		[TearDown]
		public void ResetEngine() => Reset();

		[Test]
		public void ObjectModel_01_LazyCreate()
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
		public void ObjectModel_02_NumberConvert()
		{
			Test(3.14f, "float 3.14");
			Test(123, "int \"123\"");
		}
	}
}
