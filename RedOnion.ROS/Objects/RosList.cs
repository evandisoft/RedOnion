using System;
using System.Collections;
using System.Collections.Generic;

namespace RedOnion.ROS.Objects
{
	public class RosList : List<Value>
	{
		public RosList() { }
		public RosList(int capacity) : base(capacity) { }
		public RosList(IEnumerable<Value> collection) : base(collection) { }
		public RosList(IEnumerable collection)
		{
			foreach (var it in collection)
				Add(new Value(it));
		}

		public int Length => Count;
		public bool Empty => Count == 0;
		public new Value this[int index]
		{
			get => base[index];
			set
			{
				if (index == Count)
					Add(value);
				else base[index] = value;
			}
		}
	}
}
