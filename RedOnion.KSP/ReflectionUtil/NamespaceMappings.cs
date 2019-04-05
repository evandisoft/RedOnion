using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.ReflectionUtil
{
	/// <summary>
	/// Stores mappings between namespaces and potential completions for those namespaces.
	/// </summary>
	public class NamespaceMappings
	{
		public readonly string CurrentNamespace;

		static public Assembly[] assemblies;
		static public Dictionary<string, NameTypeMap> NamespaceToNameTypeMap = new Dictionary<string, NameTypeMap>();
		static public Dictionary<string, List<string>> NamespaceContinuationMap = new Dictionary<string, List<string>>();
		/// <summary>
		/// Map of namespace to possible completions. The completions consist of all the
		/// possible namespace continuations from the given namespace as well as
		/// all the base type names. By base I mean "HashSet" instead of "HashSet`1"
		/// </summary>
		static public Dictionary<string, List<string>> NamespaceCompletionMap = new Dictionary<string, List<string>>();
		static public Dictionary<string, List<string>> NamespaceRawTypeNamesMap = new Dictionary<string, List<string>>();

		static public void Load()
		{
			if (NamespaceToNameTypeMap.Count == 0)
			{
				Reload();
			}
		}

		static public void Reload()
		{
			NamespaceToNameTypeMap = new Dictionary<string, NameTypeMap>();
			NamespaceContinuationMap = new Dictionary<string, List<string>>();
			NamespaceCompletionMap = new Dictionary<string, List<string>>();
			NamespaceRawTypeNamesMap = new Dictionary<string, List<string>>();

			assemblies = AppDomain.CurrentDomain.GetAssemblies();
			HashSet<string> namespaceStrings = new HashSet<string>();
			foreach(var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes())
				{
					string namespaceString = type.Namespace ?? "";
					if (!NamespaceToNameTypeMap.ContainsKey(namespaceString))
					{
						NamespaceToNameTypeMap[namespaceString] = new NameTypeMap();
					}


					NamespaceToNameTypeMap[namespaceString].Add(type);

					namespaceStrings.Add(namespaceString);
				}
			}

			foreach(var namespaceString in namespaceStrings)
			{
				if (NamespaceParent(namespaceString,out string parent))
				{
					if (!NamespaceContinuationMap.ContainsKey(parent))
					{
						NamespaceContinuationMap[parent] = new List<string>();
					}
					NamespaceContinuationMap[parent].Add(LastNamespacePart(namespaceString));
				}

				if (!NamespaceContinuationMap.ContainsKey(namespaceString))
				{
					NamespaceContinuationMap[namespaceString] = new List<string>();
				}
			}

			foreach(var namespaceString in NamespaceToNameTypeMap.Keys)
			{
				if (!NamespaceCompletionMap.ContainsKey(namespaceString))
				{
					NamespaceCompletionMap[namespaceString] = new List<string>();
				}

				NamespaceCompletionMap[namespaceString].AddRange(NamespaceToNameTypeMap[namespaceString].BaseTypeNames);
				NamespaceCompletionMap[namespaceString].AddRange(NamespaceContinuationMap[namespaceString]);
				NamespaceRawTypeNamesMap[namespaceString].AddRange(NamespaceToNameTypeMap[namespaceString].RawTypeNames);

				NamespaceCompletionMap[namespaceString].Sort();
				NamespaceRawTypeNamesMap[namespaceString].Sort();
			}
		}

		static public bool NamespaceParent(string maybeChild,out string parent)
		{
			if (maybeChild == "")
			{
				parent = "";
				return false;
			}

			if (!maybeChild.Contains("."))
			{
				parent = "";
				return true;
			}

			parent = maybeChild.Substring(0,maybeChild.LastIndexOf('.'));
			return true;
		}

		static public string LastNamespacePart(string namespaceString)
		{
			if (namespaceString == "")
			{
				return "";
			}

			if (!namespaceString.Contains("."))
			{
				return namespaceString;
			}

			return namespaceString.Substring(namespaceString.LastIndexOf('.')+1);
		}
	}
}
