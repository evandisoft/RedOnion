using System;
using System.Collections.Generic;

namespace RedOnion.KSP.ReflectionUtil
{
	/// <summary>
	/// Will search first in the case sensitive dict, then in the caseInsensitive
	/// dict. If when a value is added, the key is already in caseInsenstive,
	/// but not in caseSensitive, that will represent a collision and it will
	/// be stored in CollisionKeys
	/// </summary>
	public class DualCaseSensitivityDict<ValueType>
	{
		Dictionary<string, ValueType> caseSensitive = new Dictionary<string, ValueType>();
		Dictionary<string, ValueType> caseInsensitive = new Dictionary<string, ValueType>(StringComparer.OrdinalIgnoreCase);

		public ICollection<string> Keys => caseSensitive.Keys;

		public bool TryGetValue(string namespaceString, out ValueType value)
		{
			return caseSensitive.TryGetValue(namespaceString, out value)
				|| caseInsensitive.TryGetValue(namespaceString, out value);
		}

		public ICollection<string> InsensitiveKeys => caseInsensitive.Keys;

		/// <summary>
		/// This will contain keys for which the caseInsensitive dict
		/// encountered collisions.
		/// </summary>
		public HashSet<string> CollisionKeys = new HashSet<string>();

		public ValueType this[string key]
		{
			get
			{
				if (key == null) throw new ArgumentNullException();

				if (caseSensitive.ContainsKey(key))
				{
					return caseSensitive[key];
				}
				if (caseInsensitive.ContainsKey(key))
				{
					return caseInsensitive[key];
				}

				throw new KeyNotFoundException("key not found in " + nameof(DualCaseSensitivityDict<ValueType>));
			}
			set
			{
				if (key == null) throw new ArgumentNullException();

				if(caseInsensitive.ContainsKey(key) && !caseSensitive.ContainsKey(key))
				{
					CollisionKeys.Add(key);
				}

				caseSensitive[key] = value;
				caseInsensitive[key] = value;
			}
		}

		public bool ContainsKey(string key)
		{
			return caseSensitive.ContainsKey(key);
		}

		public bool ContainsInsensitiveKey(string key)
		{
			return caseInsensitive.ContainsKey(key);
		}
	}
}
