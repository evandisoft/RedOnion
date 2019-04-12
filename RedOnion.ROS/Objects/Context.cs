using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.ROS.Objects
{
	public class Context : UserObject
	{
		protected struct Layer
		{
			public int start, end;
			public Dictionary<string, int> shadow;
		}
		protected ListCore<Layer> layers;
		protected int start = int.MinValue;
		protected int end = int.MaxValue;
		protected int rootStart = int.MinValue;
		protected int rootEnd = int.MaxValue;

		public int LayerCount => layers.size;
		public int BlockStart => start;
		public int BlockEnd => end;
		public int RootStart
		{
			get => rootStart;
			set
			{
				rootStart = value;
				if (layers.size == 0)
					start = value;
			}
		}
		public int RootEnd
		{
			get => rootEnd;
			set
			{
				rootEnd = value;
				if (layers.size == 0)
					end = value;
			}
		}

		public Context(int start, int end)
			: base("Scope Context", typeof(Context))
		{
			RootStart = this.start = start;
			RootEnd = this.end = end;
		}

		public override void Reset()
		{
			PopAll();
			base.Reset();
		}

		public void Push(int start, int end)
		{
			ref var layer = ref layers.Add();
		}
		public int Pop()
		{
			var shadow = layers.Top().shadow;
			if (shadow != null)
			{
				foreach (var pair in shadow)
					dict[pair.Key] = pair.Value;
				shadow.Clear();
			}
			if (--layers.size == 0)
			{
				start = RootStart;
				return end = RootEnd;
			}
			else
			{
				ref var layer = ref layers.Top();
				start = layer.start;
				return end = layer.end;
			}
		}
		public void PopAll()
		{
			start = RootStart;
			end = RootEnd;
			for (int i = 0; i < layers.size; i++)
				layers[i].shadow?.Clear();
			layers.size = 0;
		}

		public override int Add(string name, ref Value value)
		{
			var idx = prop.size;
			ref var it = ref prop.Add();
			it.name = name;
			it.value = value;
			if (name != null)
			{
				if (layers.size > 0 && dict != null && dict.ContainsKey(name))
				{
					ref var layer = ref layers.Top();
					if (layer.shadow == null)
						layer.shadow = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
					layer.shadow[name] = dict[name];
				}
				if (dict == null)
					dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				dict[name] = idx;
			}
			return idx;
		}
	}
}
