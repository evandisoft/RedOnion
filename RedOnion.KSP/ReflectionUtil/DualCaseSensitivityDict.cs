using System;
using System.Collections.Generic;
using System.Text;

namespace RedOnion.KSP.ReflectionUtil
{
	public class CaseCollisionException : Exception
	{
		public CaseCollisionException(string message) : base(message)
		{
		}
	}
	/// <summary>
	/// Will search first in the case sensitive dict, then in the caseInsensitive
	/// dict. If when a value is added, the key is already in caseInsenstive,
	/// but not in caseSensitive, that will represent a collision and it will
	/// be stored in CollisionMap
	/// </summary>
	public class DualCaseSensitivityDict<TValue>
	{
		Dictionary<string, TValue> caseSensitive = new Dictionary<string, TValue>();
		/// <summary>
		/// This is a case insensitive mapping from case insensitive keys
		/// to case sensitive keys. In this[string key] set, this allows us to do
		/// CollisionMap[key].Add(caseInsensitive[key]), getting the case sensitive
		/// key that was part of the newly found collision.
		/// </summary>
		Dictionary<string, string> caseInsensitive = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		public Dictionary<string, List<string>> CollisionMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

		public ICollection<string> Keys => caseSensitive.Keys;

		public bool TryGetValue(string namespaceString, out TValue value)
		{
			if(caseSensitive.TryGetValue(namespaceString, out value))
			{
				return true;
			}
			if(caseInsensitive.TryGetValue(namespaceString, out string key))
			{
				if (CollisionMap.ContainsKey(namespaceString))
				{
					throw new CaseCollisionException(CollisionMessage(key, CollisionMap[key]));
				}

				if(caseSensitive.TryGetValue(key, out value))
				{
					return true;
				}

				// This should not ever occur. Every value in caseInsensitive should be a key in caseSensitive
				throw new KeyNotFoundException("key found in " + nameof(caseInsensitive)
					+ " but corresponding value was not a key in " + nameof(caseSensitive));
			}

			value = default(TValue);
			return false;
		}

		string CollisionMessage(string key, List<string> collisions)
		{
			var sb = new StringBuilder();
			sb.Append("Collision for key: " + key);
			sb.Append(", collisions: (");
			foreach (var collision in collisions)
			{
				sb.Append(collision+", ");
			}
			sb.Append(")");

			return sb.ToString();
		}

		public ICollection<string> InsensitiveKeys => caseInsensitive.Keys;

		public ICollection<TValue> Values => caseSensitive.Values;

		/// <summary>
		/// This will contain keys for which the caseInsensitive dict
		/// encountered collisions.
		/// </summary>

		public TValue this[string key]
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
					if (CollisionMap.ContainsKey(key))
					{
						throw new CaseCollisionException(CollisionMessage(key, CollisionMap[key]));
					}

					var newKey = caseInsensitive[key];

					if (!caseSensitive.ContainsKey(newKey))
					{
						// This should not ever occur. Every value in caseInsensitive should be a key in caseSensitive
						throw new KeyNotFoundException("key found in " + nameof(caseInsensitive)
							+ " but corresponding value was not a key in " + nameof(caseSensitive));
					}
					return caseSensitive[newKey];
				}

				throw new KeyNotFoundException("key not found in " + nameof(DualCaseSensitivityDict<TValue>));
			}
			set
			{
				if (key == null) throw new ArgumentNullException();

				if(caseInsensitive.ContainsKey(key) && !caseSensitive.ContainsKey(key))
				{
					if (!CollisionMap.ContainsKey(key))
					{
						CollisionMap[key] = new List<string>();
					}

					// Add case sensitive key that was previously associated with this case insensitive key
					CollisionMap[key].Add(caseInsensitive[key]);
				}

				caseSensitive[key] = value;
				caseInsensitive[key] = key;
			}
		}

		public bool ContainsKey(string key)
		{
			return caseInsensitive.ContainsKey(key);
		}

		public bool ContainsSensitiveKey(string key)
		{
			return caseSensitive.ContainsKey(key);
		}
	}
}
