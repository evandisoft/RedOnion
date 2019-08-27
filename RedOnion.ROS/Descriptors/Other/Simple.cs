using System;
using System.Collections.Generic;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public class Simple : Descriptor
		{
			protected readonly Values values;
			public class Values
			{
				protected Value[] items;
				protected Dictionary<string, int> index;
				public Values(Value[] items)
				{
					this.items = items;
					index = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
					for (int i = 0; i < items.Length; i++)
						index.Add(items[i].Name, i);
				}
				public int Find(string name)
					=> index.TryGetValue(name, out var it) ? it : -1;
				public bool Get(ref Value it, int at)
				{
					if (at < 0 || at >= items.Length)
						return false;
					it = items[at];
					return true;
				}
				public IEnumerable<string> EnumerateProperties()
				{
					foreach (var value in items)
						yield return value.Name;
				}

			}
			public Simple(string name, Values values)
				: this(name, typeof(Simple), values) {}
			public Simple(string name, Type type, Values values)
				: base(name, type)
				=> this.values = values;

			public override int Find(object self, string name, bool add = false)
				=> values.Find(name);
			public override bool Get(ref Value self, int at)
				=> values.Get(ref self, at);

			public override IEnumerable<string> EnumerateProperties(object self)
				=> values.EnumerateProperties();
		}
	}
}
