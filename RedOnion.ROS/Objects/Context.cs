using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.ROS.Objects
{
	public class Context : UserObject
	{
		protected struct Block
		{
			public OpCode op;
			public int start, end;
			public int at1, at2;
			public Dictionary<string, int> shadow;
		}
		protected ListCore<Block> blocks;
		protected int start = int.MinValue;
		protected int end = int.MaxValue;
		protected int rootStart = int.MinValue;
		protected int rootEnd = int.MaxValue;
		protected int blockAt1, blockAt2;
		protected OpCode blockCode;

		public int BlockCount => blocks.size;
		public int BlockAt1 => blockAt1;
		public int BlockAt2 => blockAt2;
		public int BlockStart => start;
		public int BlockEnd => end;

		public OpCode BlockCode
		{
			get => blockCode;
			set
			{
				blockCode = value;
				if (blocks.size > 0)
					blocks.Top().op = value;
			}
		}

		public int RootStart
		{
			get => rootStart;
			set
			{
				rootStart = value;
				if (blocks.size == 0)
					start = value;
			}
		}
		public int RootEnd
		{
			get => rootEnd;
			set
			{
				rootEnd = value;
				if (blocks.size == 0)
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

		public void Push(int start, int end, OpCode op = OpCode.Block, int at1 = 0, int at2 = 0)
		{
			ref var block = ref blocks.Add();
			block.op = blockCode = op;
			block.start = this.start = start;
			block.end = this.end = end;
			block.at1 = blockAt1 = at1;
			block.at2 = blockAt2 = at2;
		}
		public void ResetTop()
		{
			var shadow = blocks.Top().shadow;
			if (shadow != null)
			{
				foreach (var pair in shadow)
					dict[pair.Key] = pair.Value;
				shadow.Clear();
			}
		}
		public int Pop()
		{
			if (blocks.size <= 0)
				throw InvalidOperation("No block left to remove");
			ResetTop();
			if (--blocks.size == 0)
			{
				blockCode = OpCode.Void;
				blockAt1 = 0;
				blockAt2 = 0;
				start = RootStart;
				return end = RootEnd;
			}
			else
			{
				ref var block = ref blocks.Top();
				blockCode = block.op;
				blockAt1 = block.at1;
				blockAt2 = block.at2;
				start = block.start;
				return end = block.end;
			}
		}
		public void PopAll()
		{
			start = RootStart;
			end = RootEnd;
			for (int i = 0; i < blocks.size; i++)
				blocks[i].shadow?.Clear();
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
