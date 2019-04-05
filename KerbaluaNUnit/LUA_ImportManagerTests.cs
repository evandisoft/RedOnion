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
			var mappings = NamespaceMappings.All;
			var namespace1 = mappings.GetNamespace("System.Collections.Generic");
			foreach(var part in namespace1.PossibleCompletions)
			{
				Console.WriteLine(part);
			}
			//namespace1.TryGetType("HashSet",out Type rawType);

			Assert.AreEqual(typeof(HashSet<>), namespace1.GetRawType("HashSet`1"));
			//Assert.IsTrue(false);
		}

		[Test()]
		public void LUA_ImportManager_2()
		{
			var mappings = NamespaceMappings.All;
			var namespace1 = mappings.GetNamespace("")
				.GetSubNamespace("System")
				.GetSubNamespace("Collections")
				.GetSubNamespace("Generic");
			foreach (var part in namespace1.PossibleCompletions)
			{
				Console.WriteLine(part);
			}
			//namespace1.TryGetType("HashSet",out Type rawType);

			Assert.AreEqual(typeof(HashSet<>), namespace1.GetRawType("HashSet`1"));
			//Assert.IsTrue(false);
		}

		[Test()]
		public void LUA_ImportManager_3()
		{
			var mappings = NamespaceMappings.All;
			var namespace1 = mappings.GetNamespace("")
				.GetSubNamespace("System.Collections.Generic");
			foreach (var part in namespace1.PossibleCompletions)
			{
				Console.WriteLine(part);
			}
			//namespace1.TryGetType("HashSet",out Type rawType);

			Assert.AreEqual(typeof(HashSet<>), namespace1.GetRawType("HashSet`1"));
			//Assert.IsTrue(false);
		}

		[Test()]
		public void LUA_ImportManager_4()
		{
			var mappings = NamespaceMappings.All;
			var namespace1 = mappings.GetNamespace("")
				.GetSubNamespace("System.Collections")
				.GetSubNamespace("Generic");
			foreach (var part in namespace1.PossibleCompletions)
			{
				Console.WriteLine(part);
			}
			//namespace1.TryGetType("HashSet",out Type rawType);

			Assert.AreEqual(typeof(HashSet<>), namespace1.GetRawType("HashSet`1"));
			//Assert.IsTrue(false);
		}

		[Test()]
		public void LUA_ImportManager_5()
		{
			var mappings = NamespaceMappings.All;
			var namespace1 = mappings.GetNamespace("")
				.GetSubNamespace("System.Collections")
				.GetSubNamespace("Generic");
			foreach (var part in namespace1.PossibleCompletions)
			{
				Console.WriteLine(part);
			}
			//namespace1.TryGetType("HashSet",out Type rawType);

			Assert.AreEqual(typeof(HashSet<object>), namespace1.GetType("HashSet"));
			//Assert.IsTrue(false);
		}

		[Test()]
		public void LUA_ImportManager_6()
		{
			var mappings = NamespaceMappings.All;
			var namespace1 = mappings.GetNamespace("")
				.GetSubNamespace("System");
			foreach (var part in namespace1.PossibleCompletions)
			{
				Console.WriteLine(part);
			}
			//namespace1.TryGetType("HashSet",out Type rawType);

			Assert.AreEqual(typeof(Func<object,object>), namespace1.GetType("Func",2));
			//Assert.IsTrue(false);
		}

		[Test()]
		public void LUA_ImportManager_7()
		{
			var mappings = NamespaceMappings.All;
			var namespace1 = mappings.GetNamespace("System");
			foreach (var part in namespace1.PossibleCompletions)
			{
				Console.WriteLine(part);
			}
			//namespace1.TryGetType("HashSet",out Type rawType);

			Assert.AreEqual(typeof(Func<,,>), namespace1.GetRawType("Func`3"));
			//Assert.IsTrue(false);
		}
	}
}
