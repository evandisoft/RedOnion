using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.API
{
	public class NamespaceMappings
	{
		public readonly string CurrentNamespace;

		static public Assembly[] assemblies;
		static public Dictionary<string, Dictionary<string,Type>> NamespaceToNameTypeMap = new Dictionary<string, Dictionary<string, Type>>();
		static public Dictionary<string, List<string>> NamespaceContinuationMap = new Dictionary<string, List<string>>();
		static public Dictionary<string, List<string>> NamespaceCompletionMap = new Dictionary<string, List<string>>();

		static public void Load()
		{
			if (NamespaceToNameTypeMap.Count == 0)
			{
				Reload();
			}
		}

		static public void Reload()
		{
			assemblies = AppDomain.CurrentDomain.GetAssemblies();
			HashSet<string> namespaces = new HashSet<string>();
			foreach(var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes())
				{
					string namespace1 = type.Namespace ?? "";
					if (!NamespaceToNameTypeMap.ContainsKey(namespace1))
					{
						NamespaceToNameTypeMap[namespace1] = new Dictionary<string, Type>();
					}

					NamespaceToNameTypeMap[namespace1][type.Name] = type;

					namespaces.Add(namespace1);
				}
			}

			foreach(var namespace1 in namespaces)
			{
				if (NamespaceParent(namespace1,out string parent))
				{
					if (!NamespaceContinuationMap.ContainsKey(parent))
					{
						NamespaceContinuationMap[parent] = new List<string>();
					}
					NamespaceContinuationMap[parent].Add(LastNamespacePart(namespace1));
				}

				if (!NamespaceContinuationMap.ContainsKey(namespace1))
				{
					NamespaceContinuationMap[namespace1] = new List<string>();
				}
			}

			foreach(var namespace1 in NamespaceToNameTypeMap.Keys)
			{
				if (!NamespaceCompletionMap.ContainsKey(namespace1))
				{
					NamespaceCompletionMap[namespace1] = new List<string>();
				}

				NamespaceCompletionMap[namespace1].AddRange(NamespaceToNameTypeMap[namespace1].Keys);
				NamespaceCompletionMap[namespace1].AddRange(NamespaceContinuationMap[namespace1]);
				NamespaceCompletionMap[namespace1].Sort();
			}
		}

		static public bool NamespaceParent(string namespace1,out string parent)
		{
			if (namespace1 == "")
			{
				parent = "";
				return false;
			}

			if (!namespace1.Contains("."))
			{
				parent = "";
				return true;
			}

			parent = namespace1.Substring(0,namespace1.LastIndexOf('.'));
			return true;
		}

		static public string LastNamespacePart(string namespace1)
		{
			if (namespace1 == "")
			{
				return "";
			}

			if (!namespace1.Contains("."))
			{
				return namespace1;
			}

			return namespace1.Substring(namespace1.LastIndexOf('.')+1);
		}
	}
}
