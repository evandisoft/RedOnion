using NUnit.Framework;
using System;
using System.Collections.Generic;
using RedOnion.KSP.ReflectionUtil;
using MunSharp.Interpreter.Interop;
using MunSharp.Interpreter;

namespace KerbaluaNUnit {
	[TestFixture()]
	public class MLUA_ImportManagerTests 
	{
		[Test()]
		public void MLUA_ImportManager_1()
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
		public void MLUA_ImportManager_2()
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
		public void MLUA_ImportManager_3()
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
		public void MLUA_ImportManager_4()
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
		public void MLUA_ImportManager_5()
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
		public void MLUA_ImportManager_6()
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
		public void MLUA_ImportManager_7()
		{
			var mappings = NamespaceMappings.ForAllAssemblies;
			var namespaceInstance = mappings.GetNamespace("System");
			//foreach (var assembly in mappings.GetAssemblies())
			//{
			//	//Console.WriteLine(assembly.GetName().Name);
			//}

			Assert.AreEqual(typeof(Func<,,>), namespaceInstance.GetRawType("Func`3"));
		}

		[Test()]
		public void MLUA_ImportManager_8()
		{
			var mappings = NamespaceMappings.ForAllAssemblies;
			var namespaceInstance = mappings.GetNamespace("System");
			foreach (var part in namespaceInstance.PossibleCompletions)
			{
			//Console.WriteLine(part);
			}

			Assert.AreEqual(typeof(Func<,,>), namespaceInstance.GetRawType("Func",3));
		}

		[Test()]
		public void MLUA_ImportManager_9_CaseInsensitivity()
		{
			var mappings = NamespaceMappings.ForAllAssemblies;
			var namespaceInstance = mappings.GetNamespace("system");
			foreach (var part in namespaceInstance.PossibleCompletions)
			{
				//Console.WriteLine(part);
			}

			Assert.AreEqual(typeof(Func<,,>), namespaceInstance.GetRawType("func", 3));
		}

		// this test doesn't work properly
		[Test()]
		public void MLUA_ImportManager_10_LuaInterface()
		{
			var mappings = NamespaceMappings.ForAllAssemblies;
			var namespaceInstance = mappings.GetNamespace("system.collections.generic");
			var luaInterface = namespaceInstance as IUserDataType;
			foreach (var part in namespaceInstance.PossibleCompletions)
			{
				//Console.WriteLine(part);
			}
			Script script = new Script();
			//Assert.AreEqual(typeof(Func<object>), luaInterface.Index(script,DynValue.NewString("list"),false));
		}
	}
}
