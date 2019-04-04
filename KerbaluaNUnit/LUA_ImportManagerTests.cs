using NUnit.Framework;
using System;
using System.Collections.Generic;
using RedOnion.KSP.ReflectionUtil;

namespace KerbaluaNUnit {
	[TestFixture()]
	public class LUA_ImportManagerTests 
	{
		[Test()]
		public void LUA_ImportManager_1()
		{
			var namespace1 = new NamespaceInstance("System");
			foreach(var part in namespace1.PossibleCompletions)
			{
				Console.WriteLine(part);
			}
			Assert.AreEqual(typeof(HashSet<object>), namespace1.GetType("HashSet`1"));
			Assert.IsTrue(false);
		}
	}
}
