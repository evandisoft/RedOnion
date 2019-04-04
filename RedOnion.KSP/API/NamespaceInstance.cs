using System;
using System.Collections.Generic;
using System.Linq;

namespace RedOnion.KSP.API
{
	public class NamespaceInstance
	{

		public string NamespaceString;
		public Dictionary<string,Type> TypesMap;
		public List<string> NextNamespaceParts;
		public List<string> PossibleCompletions;

		public NamespaceInstance(string namespace1)
		{
			NamespaceString=namespace1;
			NamespaceMappings.Load();
			if (NamespaceMappings
				.NamespaceTypes
				.TryGetValue(namespace1, out Dictionary<string, Type> typesMap))
			{
				TypesMap = typesMap;
			}
			else
			{
				throw new Exception("Types for namespace \"" + namespace1 + "\" not found.");
			}

			if (NamespaceMappings
				.NextNamespaceParts
				.TryGetValue(namespace1,out List<string> namespaces))
			{
				NextNamespaceParts = namespaces;
			}
			else
			{
				throw new Exception("Namespace \"" + namespace1 + "\" not found.");
			}

			PossibleCompletions = new List<string>();
			PossibleCompletions.AddRange(TypesMap.Keys);
			PossibleCompletions.AddRange(NextNamespaceParts);
			PossibleCompletions.Sort();
		}

		public bool IsType(string maybeTypeName)
		{
			return TypesMap.ContainsKey(maybeTypeName);
		}

		public bool IsNextNamespacePart(string maybeNextNamespacePart)
		{
			return NextNamespaceParts.Contains(maybeNextNamespacePart);
		}

		/// <summary>
		/// Gets the type in this namespace of name typeName
		/// </summary>
		/// <returns>The type.</returns>
		/// <param name="typeName">Type name.</param>
		public Type GetTypeFromNamespace(string typeName)
		{
			Type t= TypesMap[typeName];
			if (t.IsGenericType)
			{
				Type[] typeArgs = new Type[t.GetGenericArguments().Length];
				for(int i = 0; i < typeArgs.Length; i++)
				{
					typeArgs[i] = typeof(object);
				}
				return t.MakeGenericType(typeArgs);
			}
			return t;
		}

		public NamespaceInstance NextNamespace(string namespacePart)
		{
			return new NamespaceInstance(NamespaceString + "." + namespacePart);
		}

		public override string ToString()
		{
			return NamespaceString;
		}
	}
}
