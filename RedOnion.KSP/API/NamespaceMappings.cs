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
		static public Dictionary<string, Dictionary<string,Type>> NamespaceTypes=new Dictionary<string, Dictionary<string, Type>>();
		static public Dictionary<string, List<string>> NextNamespaceParts = new Dictionary<string, List<string>>();

		static public void Load()
		{
			if (NamespaceTypes.Count == 0)
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
					if (!NamespaceTypes.ContainsKey(namespace1))
					{
						NamespaceTypes[namespace1] = new Dictionary<string, Type>();
					}

					NamespaceTypes[namespace1][type.Name] = type;


					namespaces.Add(namespace1);
				}
			}

			foreach(var namespace1 in namespaces)
			{
				if (NamespaceParent(namespace1,out string parent))
				{
					if (!NextNamespaceParts.ContainsKey(parent))
					{
						NextNamespaceParts[parent] = new List<string>();
					}
					NextNamespaceParts[parent].Add(LastNamespacePart(namespace1));
				}

				if (!NextNamespaceParts.ContainsKey(namespace1))
				{
					NextNamespaceParts[namespace1] = new List<string>();
				}
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
