using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RedOnion.KSP.Completion;

namespace RedOnion.KSP.ReflectionUtil
{
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
