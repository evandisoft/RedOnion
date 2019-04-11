using System;
using System.Collections;
using System.Collections.Generic;

namespace RedOnion.Script.Utilities
{
	/// <summary>
	/// Helper class for efficient list implementation
	/// </summary>
	public struct ListCore<T> : IList<T>
	{
		public T[] items;
		public int size;

		public void EnsureCapacity(int capacity)
		{
			int newSize = 4;
			if (items != null)
				newSize = items.Length;
			while (newSize < capacity)
				newSize <<= 1;
			Array.Resize(ref items, newSize);
		}

		public int Count
		{
			get => size;
			set
			{
				if (value > size)
				{
					EnsureCapacity(value);
					size = value;
				}
				else if (value < size)
				{
					Array.Clear(items, value, size-value);
					size = value;
				}
			}
		}

		public void Clear()
		{
			if (items != null)
			{
				Array.Clear(items, 0, size);
				size = 0;
			}
		}

		public void Add(T item)
		{
			if (items == null)
				items = new T[4];
			else if (size == items.Length)
				Array.Resize(ref items, items.Length << 1);

			items[size++] = item;
		}

		public void Insert(int index, T item)
		{
			if (index < 0)
				ThrowNegative(index);
			if (index >= size)
				ThrowGreater(index);

			if (items == null)
			{
				items = new T[4];
				items[0] = item;
				size = 1;
				return;
			}
			if (index == size)
			{
				items[size++] = item;
				return;
			}

			T[] array = new T[items.Length << 1];
			Array.Copy(items, 0, array, 0, index);
			array[index] = item;
			Array.Copy(items, index + 1, array, index + 1, size - index);
			items = array;
			size++;
		}

		public T this[int index]
		{
			get
			{
				if (index < 0)
					ThrowNegative(index);
				if (index >= size)
					ThrowGreater(index);
				return items[index];
			}
			set
			{
				if (index < 0)
					ThrowNegative(index);
				if (index >= size)
					ThrowGreater(index);
				items[index] = value;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < size; i++)
				yield return items[i];
		}

		public int IndexOf(T item)
			=> Array.IndexOf(items, item, 0, size);
		public bool Contains(T item)
			=> IndexOf(item) >= 0;
		public void CopyTo(T[] array, int index)
			=> Array.Copy(items, 0, array, index, size);

		public bool Remove(T item)
		{
			int index = IndexOf(item);
			if (index < 0)
				return false;
			size--;
			Array.Copy(items, index + 1, items, index, size - index);
			items[size] = default(T);
			return true;
		}

		public void RemoveAt(int index)
		{
			if (index < 0)
				ThrowNegative(index);
			if (index >= size)
				ThrowGreater(index);
			size--;
			Array.Copy(items, index + 1, items, index, size - index);
			items[size] = default(T);
		}

		bool ICollection<T>.IsReadOnly => false;

		static void ThrowNegative(int index)
			=> throw new ArgumentOutOfRangeException("index", index, "Index is negative");
		static void ThrowGreater(int index)
			=> throw new ArgumentOutOfRangeException("index", index, "Index is past the end of the list");
	}
}
