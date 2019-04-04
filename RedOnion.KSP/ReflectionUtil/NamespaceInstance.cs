using System;
using System.Collections.Generic;
using System.Linq;

namespace RedOnion.KSP.API
{
	public class NamespaceInstance
	{
		public string NamespaceString;
		/// <summary>
		/// Mapping from Type Name to Type
		/// </summary>
		public Dictionary<string,Type> NameTypeMap;
		/// <summary>
		/// Possible continuations of the current namespace that when concatenated
		/// to the current namespace represent another existing namespace.
		/// </summary>
		public List<string> NamespaceContinuations;

		/// <summary>
		/// A list of the possible completions in the current context.
		/// </summary>
		public List<string> PossibleCompletions;

		public NamespaceInstance(string namespace1)
		{
			NamespaceString=namespace1;
			NamespaceMappings.Load();
			if (NamespaceMappings
				.NamespaceToNameTypeMap
				.TryGetValue(namespace1, out Dictionary<string, Type> typesMap))
			{
				NameTypeMap = typesMap;
			}
			else
			{
				throw new Exception("Types for namespace \"" + namespace1 + "\" not found.");
			}

			if (NamespaceMappings
				.NamespaceContinuationMap
				.TryGetValue(namespace1,out List<string> namespaceContinuations))
			{
				NamespaceContinuations = namespaceContinuations;
			}
			else
			{
				throw new Exception("NamespaceContinuations for namespace \"" + namespace1 + "\" not found.");
			}

			if (NamespaceMappings
				.NamespaceCompletionMap
				.TryGetValue(namespace1,out List<string> possibleCompletions))
			{
				PossibleCompletions = possibleCompletions;
			}
			else
			{
				throw new Exception("Possible completions for namespace \"" + namespace1 + "\" not found.");
			}
		}

		/// <summary>
		/// Is <paramref name="maybeTypeName"/> the name of a type contained in the current namespace.
		/// </summary>
		/// <returns><c>true</c>, if type was ised, <c>false</c> otherwise.</returns>
		/// <param name="maybeTypeName">Maybe type name.</param>
		public bool IsType(string maybeTypeName)
		{
			return NameTypeMap.ContainsKey(maybeTypeName);
		}

		/// <summary>
		/// Does <paramref name="maybeNamespaceContinuation"/> represent an existing continuation to the
		/// current namespace.
		/// </summary>
		/// <returns><c>true</c>, if namespace continuation was ised, <c>false</c> otherwise.</returns>
		/// <param name="maybeNamespaceContinuation">Maybe namespace continuation.</param>
		public bool IsNamespaceContinuation(string maybeNamespaceContinuation)
		{
			return NamespaceContinuations.Contains(maybeNamespaceContinuation);
		}

		/// <summary>
		/// Gets the type in this namespace of name typeName.
		/// Makes any generic parameters of types be of type object.
		/// </summary>
		/// <returns>The type.</returns>
		/// <param name="typeName">Type name.</param>
		public Type GetType(string typeName)
		{
			if(NameTypeMap.TryGetValue(typeName,out Type t))
			{
				if (t.IsGenericType)
				{
					Type[] typeArgs = new Type[t.GetGenericArguments().Length];
					for (int i = 0; i < typeArgs.Length; i++)
					{
						typeArgs[i] = typeof(object);
					}
					return t.MakeGenericType(typeArgs);
				}

				return t;
			}

			throw new Exception("Type " + typeName + " not found in namespace " + NamespaceString);
		}

		/// <summary>
		/// Returns a raw unparameterized type, in contrast to GetType(string typeName) (which returns
		/// a type that has all its type parameters set to typeof(object)).
		/// 
		/// For types that do not have type parameters, this returns the same type as GetType(string typeName)
		/// </summary>
		/// <returns>The raw type.</returns>
		/// <param name="typeName">Type name.</param>
		public Type GetRawType(string typeName)
		{
			if (NameTypeMap.TryGetValue(typeName, out Type t))
			{
				return t;
			}

			throw new Exception("Type " + typeName + " not found in namespace " + NamespaceString);
		}

		/// <summary>
		/// Gets the namespace created by adding the given namespaceContinuation to the current namespace.
		/// </summary>
		/// <returns>The namespace.</returns>
		/// <param name="namespaceContinuation">Namespace continuation.</param>
		public NamespaceInstance GetNamespace(string namespaceContinuation)
		{
			return new NamespaceInstance(NamespaceString + "." + namespaceContinuation);
		}

		public override string ToString()
		{
			return NamespaceString;
		}
	}
}
