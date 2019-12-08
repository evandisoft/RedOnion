using System;
using System.Collections.Generic;

namespace RedOnion.Collections
{
	public class GenQueue<T> : Queue<T>
	{
		public GenQueue() { }
		public GenQueue(int capacity) : base(capacity) { }
		public GenQueue(IEnumerable<T> collection) : base(collection) { }

		public void Push(T item) => Enqueue(item);
		public T Pop() => Dequeue();
		public bool Empty => Count == 0;
	}
}
