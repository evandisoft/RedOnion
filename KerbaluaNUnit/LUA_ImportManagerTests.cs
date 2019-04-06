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
			var mappings = NamespaceMappings.ForAllAssemblies;
			var namespaceInstance = mappings.GetNamespace("System.Collections.Generic");
			foreach(var part in namespaceInstance.PossibleCompletions)
			{
				//Console.WriteLine(part);
			}

			Assert.AreEqual(typeof(HashSet<>), namespaceInstance.GetRawType("HashSet`1"));
		}

		[Test()]
		public void LUA_ImportManager_2()
		{
			var mappings = NamespaceMappings.ForAllAssemblies;
			var namespaceInstance = mappings.GetNamespace("")
				.GetSubNamespace("System")
				.GetSubNamespace("Collections")
				.GetSubNamespace("Generic");
			foreach (var part in namespaceInstance.PossibleCompletions)
			{
				//Console.WriteLine(part);
			}

			Assert.AreEqual(typeof(HashSet<>), namespaceInstance.GetRawType("HashSet`1"));
		}

		[Test()]
		public void LUA_ImportManager_3()
		{
			var mappings = NamespaceMappings.ForAllAssemblies;
			var namespaceInstance = mappings.GetNamespace("")
				.GetSubNamespace("System.Collections.Generic");
			foreach (var part in namespaceInstance.PossibleCompletions)
			{
				//Console.WriteLine(part);
			}

			Assert.AreEqual(typeof(HashSet<>), namespaceInstance.GetRawType("HashSet`1"));
		}

		[Test()]
		public void LUA_ImportManager_4()
		{
			var mappings = NamespaceMappings.ForAllAssemblies;
			var namespaceInstance = mappings.GetNamespace("")
				.GetSubNamespace("System.Collections")
				.GetSubNamespace("Generic");
			foreach (var part in namespaceInstance.PossibleCompletions)
			{
				//Console.WriteLine(part);
			}

			Assert.AreEqual(typeof(HashSet<>), namespaceInstance.GetRawType("HashSet`1"));
		}

		[Test()]
		public void LUA_ImportManager_5()
		{
			var mappings = NamespaceMappings.ForAllAssemblies;
			var namespaceInstance = mappings.GetNamespace("")
				.GetSubNamespace("System.Collections")
				.GetSubNamespace("Generic");
			foreach (var part in namespaceInstance.PossibleCompletions)
			{
				//Console.WriteLine(part);
			}

			Assert.AreEqual(typeof(HashSet<object>), namespaceInstance.GetType("HashSet"));
		}

		[Test()]
		public void LUA_ImportManager_6()
		{
			var mappings = NamespaceMappings.ForAllAssemblies;
			var namespaceInstance = mappings.GetNamespace("")
				.GetSubNamespace("System");
			foreach (var part in namespaceInstance.PossibleCompletions)
			{
				//Console.WriteLine(part);
			}

			Assert.AreEqual(typeof(Func<object,object>), namespaceInstance.GetType("Func",2));
		}

		[Test()]
		public void LUA_ImportManager_7()
		{
			var mappings = NamespaceMappings.ForAllAssemblies;
			var namespaceInstance = mappings.GetNamespace("System");
			foreach (var part in namespaceInstance.PossibleCompletions)
			{
				//Console.WriteLine(part);
			}

			Assert.AreEqual(typeof(Func<,,>), namespaceInstance.GetRawType("Func`3"));
		}

		[Test()]
		public void LUA_ImportManager_8()
		{
			var mappings = NamespaceMappings.ForAllAssemblies;
			var namespaceInstance = mappings.GetNamespace("System");
			foreach (var part in namespaceInstance.PossibleCompletions)
			{
			//Console.WriteLine(part);
			}

			Assert.AreEqual(typeof(Func<,,>), namespaceInstance.GetRawType("Func",3));
		}
	}
}
