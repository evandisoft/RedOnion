using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedOnion.ROS.Objects
{
	public class RosDictionary : Dictionary<Value, Value>
	{
		public RosDictionary() { }
		public RosDictionary(int capacity) : base(capacity) { }
		public RosDictionary(IDictionary<Value, Value> dictionary) : base(dictionary) { }
		public RosDictionary(IEnumerable<Value> pairs)
		{
			var e = pairs.GetEnumerator();
			while (e.MoveNext())
			{
				var key = e.Current;
				if (!e.MoveNext())
					throw new InvalidOperationException("Odd number of elements");
				Add(key, e.Current);
			}
		}

		public new Value this[Value key]
		{
			get => TryGetValue(key, out var value) ? value : Value.Null;
			set => base[key] = value;
		}

		public bool Contains(Value key) => ContainsKey(key);
		public int Length => Count;
		public bool Empty => Count == 0;
	}
}
