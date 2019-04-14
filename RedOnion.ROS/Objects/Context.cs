using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RedOnion.ROS.Objects
{
	public class Context : UserObject
	{
		protected struct Block
		{
			public int start, end;
			public int at1, at2;
			public Dictionary<string, int> shadow;
			public OpCode op;
		}
		protected ListCore<Block> blocks;
		public int BlockCount => blocks.size;
		public int BlockStart { get; protected set; }
		public int BlockEnd { get; protected set; }
		public int BlockAt1 { get; protected set; }
		public int BlockAt2 { get; protected set; }
		public OpCode BlockCode { get; set; }

		private int rootStart;
		public int RootStart
		{
			get => rootStart;
			set
			{
				rootStart = value;
				if (BlockCount == 0)
					BlockStart = value;
			}
		}
		private int rootEnd;
		public int RootEnd
		{
			get => rootEnd;
			set
			{
				rootEnd = value;
				if (BlockCount == 0)
					BlockEnd = value;
			}
		}

		public Context(int rootStart, int rootEnd)
			: base(typeof(Context))
		{
			RootStart = rootStart;
			RootEnd = rootEnd;
		}

		public override void Reset()
		{
			PopAll();
			base.Reset();
		}

		public void Push(int start, int end, OpCode op = OpCode.Block, int at1 = 0, int at2 = 0)
		{
			ref var block = ref blocks.Add();
			block.op = BlockCode;
			block.start = BlockStart;
			block.end = BlockEnd;
			block.at1 = BlockAt1;
			block.at2 = BlockAt2;

			BlockCode = op;
			BlockStart = start;
			BlockEnd = end;
			BlockAt1 = at1;
			BlockAt2 = at2;
		}
		public void ResetTop()
		{
			if (blocks.size == 0)
				return;
			var shadow = blocks.Top().shadow;
			if (shadow == null)
				return;
			foreach (var pair in shadow)
				dict[pair.Key] = pair.Value;
			shadow.Clear();
		}
		public int Pop()
		{
			if (blocks.size <= 0)
				throw InvalidOperation("No block left to remove");
			ResetTop();

			ref var block = ref blocks.Top();
			BlockStart = block.start;
			BlockEnd = block.end;
			BlockAt1 = block.at1;
			BlockAt2 = block.at2;
			BlockCode = block.op;
			blocks.size--;
			return BlockEnd;
		}
		public void PopAll()
		{
			BlockStart = RootStart;
			BlockEnd = RootEnd;
			BlockAt1 = 0;
			BlockAt2 = 0;
			while (blocks.size > 0)
			{
				var shadow = blocks.Top().shadow;
				if (shadow != null)
				{
					foreach (var pair in shadow)
						dict[pair.Key] = pair.Value;
					shadow.Clear();
				}
				blocks.size--;
			}
			blocks.size = 0;
		}

		public override int Add(string name, ref Value value)
		{
			var idx = prop.size;
			ref var it = ref prop.Add();
			it.name = name;
			it.value = value;
			if (name != null)
			{
				if (blocks.size > 0 && dict != null && dict.ContainsKey(name))
				{
					ref var block = ref blocks.Top();
					if (block.shadow == null)
						block.shadow = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
					block.shadow[name] = dict[name];
				}
				if (dict == null)
					dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				dict[name] = idx;
			}
			return idx;
		}
	}
}
