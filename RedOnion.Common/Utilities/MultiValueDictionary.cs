using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Common.Utilities
{
	/// <summary>
	/// A Dictionary where multiple values can be associated to the same key
	/// </summary>
	/// <typeparam name="K">The key type</typeparam>
	/// <typeparam name="V">The value type</typeparam>
	public class MultiValueDictionary<K, V>
	{
		protected Dictionary<K, List<V>> core;
		protected static readonly V[] empty = new V[0];

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiValueDictionary{K, V}"/> class.
		/// </summary>
		public MultiValueDictionary()
		{
			core = new Dictionary<K, List<V>>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiValueDictionary{K, V}"/> class.
		/// </summary>
		/// <param name="eqComparer">The equality comparer to use in the underlying dictionary.</param>
		public MultiValueDictionary(IEqualityComparer<K> eqComparer)
		{
			core = new Dictionary<K, List<V>>(eqComparer);
		}


		/// <summary>
		/// Adds the specified key. Returns true if this is the first value for a given key
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>True if it was the first and only value, false if the key existed</returns>
		public bool Add(K key, V value)
		{
			if (core.TryGetValue(key, out var list))
			{
				list.Add(value);
				return false;
			}
			else
			{
				core.Add(key, new List<V>
				{
					value
				});
				return true;
			}
		}

		/// <summary>
		/// Finds all the values associated with the specified key. 
		/// An empty collection is returned if not found.
		/// </summary>
		/// <param name="key">The key.</param>
		public IEnumerable<V> Find(K key)
		{
			if (core.TryGetValue(key, out var list))
				return list;
			else
				return empty;
		}

		/// <summary>
		/// Determines whether this contains the specified key 
		/// </summary>
		/// <param name="key">The key.</param>
		public bool ContainsKey(K key) => core.ContainsKey(key);

		/// <summary>
		/// Gets the keys.
		/// </summary>
		public IEnumerable<K> Keys => core.Keys;

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear() => core.Clear();

		/// <summary>
		/// Removes the specified key and all its associated values from the multidictionary
		/// </summary>
		/// <param name="key">The key.</param>
		public void Remove(K key) => core.Remove(key);

		/// <summary>
		/// Removes the value. Returns true if the removed value was the last of a given key
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>True if it was the only value, false if no such key or it had more values.</returns>
		public bool RemoveValue(K key, V value)
		{

			if (core.TryGetValue(key, out var list))
			{
				list.Remove(value);

				if (list.Count == 0)
				{
					Remove(key);
					return true;
				}
			}

			return false;
		}

	}
}
