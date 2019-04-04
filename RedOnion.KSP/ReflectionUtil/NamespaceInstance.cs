using System;
using System.Collections.Generic;
using System.Linq;

namespace RedOnion.KSP.ReflectionUtil
{
	/// <summary>
	/// Represents a Namespace. Can return a type from this namespace or a child namespace.
	/// </summary>
	public class NamespaceInstance
	{
		public readonly string NamespaceString;
		/// <summary>
		/// Returns the current Mapping from Type Name to Type for this namespace
		/// </summary>
		public Dictionary<string, Type> NameTypeMap
		{
			get
			{
				if (NamespaceMappings
					.NamespaceToNameTypeMap
					.TryGetValue(NamespaceString, out Dictionary<string,Type> nameTypeMap))
				{
					return nameTypeMap;
				}

				throw new Exception("Types for namespace \"" + NamespaceString + "\" not found.");
			}
		}
		/// <summary>
		/// Possible continuations of the current namespace that when concatenated
		/// to the current namespace represent another existing namespace.
		/// </summary>
		public List<string> NamespaceContinuations
		{
			get
			{
				if (NamespaceMappings
					.NamespaceContinuationMap
					.TryGetValue(NamespaceString, out List<string> namespaceContinuations))
				{
					return namespaceContinuations;
				}

				throw new Exception("NamespaceContinuations for namespace \"" + NamespaceString+ "\" not found.");
			}
		}

		/// <summary>
		/// A list of the possible completions in the current context. All the typenames and namespaceContinuations
		/// for this namespace together in a sorted list.
		/// </summary>
		public List<string> PossibleCompletions
		{
			get
			{
				if (NamespaceMappings
					.NamespaceCompletionMap
					.TryGetValue(NamespaceString, out List<string> completions))
				{
					return completions;
				}

				throw new Exception("Possible completions for namespace \"" +NamespaceString + "\" not found.");
			}
		}

		public NamespaceInstance(string namespaceString)
		{
			NamespaceString=namespaceString;
			NamespaceMappings.Load();
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
