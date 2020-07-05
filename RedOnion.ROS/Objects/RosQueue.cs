using System;
using System.Collections;
using System.Collections.Generic;

namespace RedOnion.ROS.Objects
{
	public class RosQueue : Queue<Value>
	{
		public RosQueue() { }
		public RosQueue(int capacity) : base(capacity) { }
		public RosQueue(IEnumerable<Value> collection) : base(collection) { }
		public RosQueue(IEnumerable collection)
		{
			foreach (var it in collection)
				Enqueue(new Value(it));
		}

		public void Push(Value item) => Enqueue(item);
		public Value Pop() => Dequeue();
		public bool Empty => Count == 0;
	}
}
