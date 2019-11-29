using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using RedOnion.KSP.Completion;
using RedOnion.Attributes;

namespace RedOnion.KSP.ReflectionUtil
{
	[DisplayName("GetMappings"), DocBuild("RedOnion.KSP/ReflectionUtil/GetMappings")]
	[Description(@"
`native` is restricted to a default list of assemblies.

To access other assemblies, you can use `assembly.assemblyname`.

`assembly.assemblyname` returns a new `NamespaceInstance` which acts like `native`
and allows you to search through the namespaces available in the assembly and retrieves
types therein.

When the assemblyname has a space in it or another special character that would not work
well with `assembly.assemblyname`, like `assembly.abc def`, you have to use
assembly\['abc def'\] instead, since the interpreter sees `assembly.abc` and `def` as two
different tokens.
"
)]
	public partial class GetMappings : ICompletable
	{
		IList<string> ICompletable.PossibleCompletions
			=> AppDomain.CurrentDomain.GetAssemblies()
			.Select(a => a.GetName().Name).Distinct().ToList();


		bool ICompletable.TryGetCompletion(string completionName, out object completion)
		{
			if(TryGetAssemblyNamespaceInstance(completionName,out NamespaceInstance namespaceInstance))
			{
				completion = namespaceInstance;
				return true;
			}
			completion = null;
			return false;
		}

		public NamespaceInstance Get(string assemblyName)
		{
			if(TryGetAssemblyNamespaceInstance(assemblyName,out NamespaceInstance namespaceInstance))
			{
				return namespaceInstance;
			}

			throw new Exception("Assembly name " + assemblyName + " not found.");
		}

		public NamespaceInstance Get(params Assembly[] assemblies)
		{
			return new NamespaceMappings(assemblies).GetNamespace("");
		}

		public bool TryGetAssemblyNamespaceInstance(string name,out NamespaceInstance namespaceInstance)
		{
			var assemblies = new List<Assembly>();
			foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.GetName().Name.ToLower() == name.ToLower())
				{
					assemblies.Add(assembly);
				}
			}
			if (assemblies.Count > 0)
			{
				namespaceInstance=new NamespaceMappings(assemblies.ToArray()).GetNamespace("");
				return true;
			}

			namespaceInstance = null;
			return false;
		}
	}
}
