using System;
using System.Collections;
using System.Collections.Generic;

namespace RedOnion.ROS.Objects
{
	public class RosStack : Stack<Value>
	{
		public RosStack() { }
		public RosStack(int capacity) : base(capacity) { }
		public RosStack(IEnumerable<Value> collection) : base(collection) { }
		public RosStack(IEnumerable collection)
		{
			foreach (var it in collection)
				Push(new Value(it));
		}
	}
}
