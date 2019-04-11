using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.ReflectionUtil
{
	/// <summary>
	/// Stores mappings between namespaces, types in those namespaces, possible namespaceContinuations
	/// and potential completions namespaces.
	/// </summary>
	public class NamespaceMappings
	{
		static NamespaceMappings forAllAssemblies;
		/// <summary>
		/// Namespace mappings that encompass all assemblies.
		/// If Reset is called, will rebuild mappings based on assemblies
		/// returned by AppDomain.CurrentDomain.GetAssemblies()
		/// </summary>
		/// <value>All.</value>
		static public NamespaceMappings ForAllAssemblies
		{
			get
			{
				if (forAllAssemblies == null)
				{
					forAllAssemblies = new NamespaceMappings(() => AppDomain.CurrentDomain.GetAssemblies());
				}
				return forAllAssemblies;
			}
		}

		public DualCaseSensitivityDict<NameTypeMap> NamespaceToNameTypeMap = new DualCaseSensitivityDict<NameTypeMap>();
		public DualCaseSensitivityDict<List<string>> NamespaceContinuationMap = new DualCaseSensitivityDict<List<string>>();
		/// <summary>
		/// Map of namespace to possible completions. The completions consist of all the
		/// possible namespace continuations from the given namespace as well as
		/// all the base type names. By base I mean "HashSet" instead of "HashSet`1"
		/// </summary>
		public DualCaseSensitivityDict<List<string>> NamespaceCompletionMap = new DualCaseSensitivityDict<List<string>>();
		//public Dictionary<string, List<string>> NamespaceRawTypeNamesMap = new Dictionary<string, List<string>>();

		public NamespaceInstance GetNamespace(string namespaceString)
		{
			if (TryGetNamespace(namespaceString, out NamespaceInstance namespaceInstance))
			{
				return namespaceInstance;
			}

			return null;
		}

		public bool TryGetNamespace(string namespaceString, out NamespaceInstance namespaceInstance)
		{
			if (NamespaceToNameTypeMap.ContainsKey(namespaceString))
			{
				namespaceInstance = new NamespaceInstance(namespaceString, this);
				return true;
			}

			namespaceInstance = null;
			return false;
		}

		public NamespaceMappings(Assembly[] assemblies)
		{
			SetAssemblies(assemblies);
		}

		public NamespaceMappings(Func<Assembly[]> getAssemblies)
		{
			GetAssemblies = getAssemblies;

			Reset();
		}

		public Func<Assembly[]> GetAssemblies;

		/// <summary>
		/// Rebuilds mappings based on current assemblies returned by GetAssemblies.
		/// </summary>
		public void Reset()
		{
			var assemblies = GetAssemblies();

			NamespaceToNameTypeMap = new DualCaseSensitivityDict<NameTypeMap>();
			NamespaceContinuationMap = new DualCaseSensitivityDict<List<string>>();
			NamespaceCompletionMap = new DualCaseSensitivityDict<List<string>>();
			//NamespaceRawTypeNamesMap = new Dictionary<string, List<string>>();

			HashSet<string> namespaceStrings = new HashSet<string>();
			foreach (var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes())
				{
					char startChar = type.Name[0];
					if(startChar == '<' || startChar == '$' || startChar == '_')
					{
						continue;
					}

					string namespaceString = type.Namespace ?? "";
					if (!NamespaceToNameTypeMap.ContainsSensitiveKey(namespaceString))
					{
						NamespaceToNameTypeMap[namespaceString] = new NameTypeMap();
					}


					NamespaceToNameTypeMap[namespaceString].Add(type);

					namespaceStrings.Add(namespaceString);
				}
			}

			foreach (var namespaceString in namespaceStrings)
			{
				if (NamespaceParent(namespaceString, out string parent))
				{
					if (!NamespaceContinuationMap.ContainsSensitiveKey(parent))
					{
						NamespaceContinuationMap[parent] = new List<string>();
					}
					NamespaceContinuationMap[parent].Add(LastNamespacePart(namespaceString));
				}

				if (!NamespaceContinuationMap.ContainsSensitiveKey(namespaceString))
				{
					NamespaceContinuationMap[namespaceString] = new List<string>();
				}
			}

			foreach (var namespaceString in NamespaceToNameTypeMap.Keys)
			{
				if (!NamespaceCompletionMap.ContainsSensitiveKey(namespaceString))
				{
					NamespaceCompletionMap[namespaceString] = new List<string>();
				}

				NamespaceCompletionMap[namespaceString].AddRange(NamespaceToNameTypeMap[namespaceString].BaseTypeNames);
				NamespaceCompletionMap[namespaceString].AddRange(NamespaceContinuationMap[namespaceString]);

				//NamespaceRawTypeNamesMap[namespaceString] = new List<string>();
				//NamespaceRawTypeNamesMap[namespaceString].AddRange(NamespaceToNameTypeMap[namespaceString].RawTypeNames);

				NamespaceCompletionMap[namespaceString].Sort();
				//NamespaceRawTypeNamesMap[namespaceString].Sort();
			}
		}

		/// <summary>
		/// Sets the assemblies and updates all mappings.
		/// 
		/// Sets GetAssemblies to be a function that returns the given assemblies.
		/// </summary>
		/// <param name="assemblies">Assemblies.</param>
		public void SetAssemblies(Assembly[] assemblies)
		{
			GetAssemblies = new Func<Assembly[]>(() => assemblies);

			Reset();
		}

		/// <summary>
		/// Sets the field GetAssemblies, with a function that will be called to 
		/// get the assemblies that will be used when Reset is again called. 
		/// Also calls reset.
		/// </summary>
		/// <param name="getAssemblies">Get assemblies.</param>
		public void SetAssemblies(Func<Assembly[]> getAssemblies)
		{
			GetAssemblies = getAssemblies;

			Reset();
		}

		public bool NamespaceParent(string maybeChild, out string parent)
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

			parent = maybeChild.Substring(0, maybeChild.LastIndexOf('.'));
			return true;
		}

		public string LastNamespacePart(string namespaceString)
		{
			if (namespaceString == "")
			{
				return "";
			}

			if (!namespaceString.Contains("."))
			{
				return namespaceString;
			}

			return namespaceString.Substring(namespaceString.LastIndexOf('.') + 1);
		}
	}
}
