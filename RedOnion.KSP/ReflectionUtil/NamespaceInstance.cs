using System;
using System.Collections.Generic;
using RedOnion.KSP.Completion;

namespace RedOnion.KSP.ReflectionUtil
{
	/// <summary>
	/// Represents a Namespace. Can return a type from this namespace or return a child namespace.
	/// </summary>
	public class NamespaceInstance : ICompletable
	{
		public string NamespaceString;

		public NamespaceMappings NamespaceMappings;
		/// <summary>
		/// Returns the current Mapping from Type Name to Type for this namespace
		/// </summary>
		public NameTypeMap NameTypeMap
		{
			get
			{
				if (NamespaceMappings
					.NamespaceToNameTypeMap
					.TryGetValue(NamespaceString, out NameTypeMap nameTypeMap))
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

				throw new Exception("NamespaceContinuations for namespace \"" + NamespaceString + "\" not found.");
			}
		}



		/// <summary>
		/// List of all the RawTypeNames in the current namespace. Raw typenames are of the form
		/// 'HashSet`n', where n is the number of type parameters. If ` is not present, number
		/// of type parameters is 0.
		/// </summary>
		/// <value>The raw type names.</value>
		public List<string> RawTypeNames
		{
			get
			{
				if (NamespaceMappings
					.NamespaceRawTypeNamesMap
					.TryGetValue(NamespaceString, out List<string> completions))
				{
					return completions;
				}

				throw new Exception("Possible completions for namespace \"" + NamespaceString + "\" not found.");
			}
		}


		public NamespaceInstance(string namespaceString, NamespaceMappings namespaceMappings)
		{
			NamespaceMappings = namespaceMappings;

			if (!NamespaceMappings.NamespaceToNameTypeMap.ContainsKey(namespaceString))
			{
				throw new Exception("Namespace " + namespaceString + " does not exist.");
			}

			NamespaceString = namespaceString;
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
		/// Returns a type with all type parameters set to typeof<object>
		/// </summary>
		/// <returns>The type.</returns>
		/// <param name="basename">Basename.</param>
		public Type GetType(string basename)
		{
			if (TryGetType(basename, out Type type))
			{
				return type;
			}

			return null;
		}

		/// <summary>
		/// Returns a type with all type parameters set to typeof<object>
		/// </summary>
		public Type GetType(string basename, int numTypeParameters)
		{
			if (TryGetType(basename, numTypeParameters, out Type type))
			{
				return type;
			}

			return null;
		}

		/// <summary>
		/// Returns a raw type with all type parameters unset.
		/// </summary>
		public Type GetRawType(string basename, int numTypeParameters)
		{
			if (TryGetRawType(basename, numTypeParameters, out Type rawType))
			{
				return rawType;
			}

			return null;
		}

		/// <summary>
		/// Returns a raw type with all type parameters unset.
		/// </summary>
		public Type GetRawType(string rawTypeName)
		{
			if (TryGetRawType(rawTypeName, out Type rawType))
			{
				return rawType;
			}

			return null;
		}

		/// <summary>
		/// Outputs a type with all type parameters set to typeof<object>
		/// </summary>
		public bool TryGetType(string basename, out Type type)
		{
			type = null;

			if (NameTypeMap.TryBasename(basename, out Type rawType))
			{
				type = MakeGeneric(rawType);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Outputs a type with all type parameters set to typeof<object>
		/// </summary>
		public bool TryGetType(string basename, int numTypeParameters, out Type type)
		{
			type = null;

			if (NameTypeMap.TryBasename(basename, numTypeParameters, out Type rawType))
			{
				type = MakeGeneric(rawType);
				return true;
			}

			return false;
		}


		/// <summary>
		/// Outputs a raw type with all type parameters unset.
		/// </summary>
		public bool TryGetRawType(string basename, int numTypeParameters, out Type rawType)
		{
			rawType = null;

			if (NameTypeMap.TryBasename(basename, numTypeParameters, out rawType))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Outputs a raw type with all type parameters unset.
		/// </summary>
		public bool TryGetRawType(string rawTypeName, out Type rawType)
		{
			rawType = null;

			if (NameTypeMap.TryRawTypeName(rawTypeName, out rawType))
			{
				return true;
			}

			return false;
		}

		Type MakeGeneric(Type t)
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

		public bool TryGetSubNamespace(string subNamespace, out NamespaceInstance namespaceInstance)
		{
			string namespaceString = GetNamespaceString(subNamespace);

			return NamespaceMappings.TryGetNamespace(namespaceString, out namespaceInstance);
		}

		/// <summary>
		/// Gets the namespace created by adding the given <paramref name="subNamespace"/>
		/// to the current namespace.
		/// </summary>
		/// <returns>The namespace.</returns>
		/// <param name="subNamespace">Namespace continuation.</param>
		public NamespaceInstance GetSubNamespace(string subNamespace)
		{
			if (TryGetSubNamespace(subNamespace, out NamespaceInstance namespaceInstance))
			{
				return namespaceInstance;
			}

			return null;
		}

		public string GetNamespaceString(string namespaceContinuation)
		{
			if (NamespaceString == "")
			{
				return namespaceContinuation;
			}

			return NamespaceString + "." + namespaceContinuation;
		}

		public override string ToString()
		{
			return NamespaceString;
		}

		/// <summary>
		/// Outputs a type or another NamespaceInstance, or null if no type or 
		/// namespace found.
		/// </summary>
		/// <returns>True if the completion is available, false otherwise.</returns>
		/// <param name="completionName">Completable name.</param>
		public bool TryGetCompletion(string completionName, out object completion)
		{
			if (NameTypeMap.TryBasename(completionName, out Type type))
			{
				completion = type;
				return true;
			}

			if (NamespaceMappings.NamespaceToNameTypeMap.ContainsKey(GetNamespaceString(completionName)))
			{
				completion = new NamespaceInstance(completionName, NamespaceMappings);
				return true;
			}

			completion = null;
			return false;
		}

		/// <summary>
		/// A list of the possible completions in the current context. All the base typenames and namespaceContinuations
		/// for this namespace together in a sorted list.
		/// 
		/// This will return type names like HashSet instead of HashSet`1.
		/// </summary>
		public IList<string> PossibleCompletions
		{
			get
			{
				if (NamespaceMappings
					.NamespaceCompletionMap
					.TryGetValue(NamespaceString, out List<string> completions))
				{
					return completions;
				}

				throw new Exception("Possible completions for namespace \"" + NamespaceString + "\" not found.");
			}
		}
	}
}
