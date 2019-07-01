using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RedOnion.ROS.Utilities
{
	/// <summary>
	/// Helper class for efficient list implementation
	/// </summary>
	[DebuggerDisplay("{DebugString}")]
	public struct ListCore<T> : IList<T>
	{
		public T[] items;
		public int size;

		public ListCore(int capacity)
		{
			if (capacity < 4)
				capacity = 4;
			items = new T[capacity];
			size = 0;
		}

		public void EnsureCapacity(int capacity)
		{
			int newSize = 4;
			if (items != null)
				newSize = items.Length;
			while (newSize < capacity)
				newSize <<= 1;
			Array.Resize(ref items, newSize);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
		public void Push(T item)
		{
			if (items == null)
				items = new T[4];
			else if (size == items.Length)
				Array.Resize(ref items, items.Length << 1);

			items[size++] = item;
		}
		public void Add(ref T item)
		{
			if (items == null)
				items = new T[4];
			else if (size == items.Length)
				Array.Resize(ref items, items.Length << 1);

			items[size++] = item;
		}
		public void Push(ref T item)
		{
			if (items == null)
				items = new T[4];
			else if (size == items.Length)
				Array.Resize(ref items, items.Length << 1);

			items[size++] = item;
		}
		public ref T Add()
		{
			if (items == null)
				items = new T[4];
			else if (size == items.Length)
				Array.Resize(ref items, items.Length << 1);

			return ref items[size++];
		}
		public ref T Push()
		{
			if (items == null)
				items = new T[4];
			else if (size == items.Length)
				Array.Resize(ref items, items.Length << 1);

			return ref items[size++];
		}
		public void AddRange(IEnumerable<T> range)
		{
			if (range is ICollection<T> col)
			{
				AddRange(col);
				return;
			}
			foreach (var e in range)
				Add(e);
		}
		public void AddRange(ICollection<T> range)
		{
			EnsureCapacity(size + range.Count);
			foreach (var e in range)
				items[size++] = e;
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
		public T[] ToArray()
		{
			T[] array = new T[size];
			Array.Copy(items, 0, array, 0, size);
			return array;
		}

		public bool Remove(T item)
		{
			int index = IndexOf(item);
			if (index < 0)
				return false;
			size--;
			Array.Copy(items, index + 1, items, index, size - index);
			items[size] = default;
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
			items[size] = default;
		}
		public T Pop()
		{
			if (size <= 0)
				ThrowEmpty();
			var it = items[--size];
			items[size] = default;
			return it;
		}
		public ref T Top()
			=> ref items[size-1];
		public ref T Top(int fromEnd)
			=> ref items[size+fromEnd];

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection<T>.IsReadOnly => false;

		static void ThrowNegative(int index)
			=> throw new ArgumentOutOfRangeException("index", index, "Index is negative");
		static void ThrowGreater(int index)
			=> throw new ArgumentOutOfRangeException("index", index, "Index is past the end of the list");
		static void ThrowEmpty()
			=> throw new InvalidOperationException("The list is empty");

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebugString
		{
			get
			{
				if (size == 0)
					return "empty";
				var sb = new StringBuilder();
				sb.Append("[");
				if (typeof(T) == typeof(byte))
				{
					for (int i = 0; i < size; i++)
					{
						if (i >= 128)
						{
							sb.Append("...");
							break;
						}
						if ((i & 1) == 0 && i > 0)
							sb.Append((i & 2) == 0 ? (i & 4) == 0 ? '|' : ' ' : ':');
						sb.AppendFormat("{0:X2}", items[i]);
					}
				}
				else
				{
					for (int i = 0; i < size; i++)
					{
						if (sb.Length >= 256)
						{
							sb.Append(", ...");
							break;
						}
						if (i > 0)
							sb.Append(", ");
						sb.Append(items[i]);
					}
				}
				sb.Append("]");
				return sb.ToString();
			}
		}
	}
}
