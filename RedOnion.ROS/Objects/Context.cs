using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RedOnion.ROS.Objects
{
	[DebuggerDisplay("{BlockCount}/{BlockCode}; {prop.DebugString}")]
	public class Context : UserObject
	{
		protected struct Shadow
		{
			public int prev, next;
		}
		protected struct Block
		{
			public int start, end;
			public int at1, at2;
			public int propSize, lockSize1, lockSize2;
			public Dictionary<string, Shadow> shadow;
			public ListCore<string> added;
			public OpCode op;
		}
		protected ListCore<Block> blocks;
		public int BlockCount => blocks.size;
		public int BlockLock { get; protected set; }
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
		public Context(Context src, int rootStart, int rootEnd)
			: base(typeof(Context))
		{
			RootStart = rootStart;
			RootEnd = rootEnd;
			if (src == null)
				return;
			foreach (var p in src.prop)
				prop.Add() = p;
			if (src.dict != null)
				dict = new Dictionary<string, int>(src.dict);
			readOnlyTop = src.readOnlyTop;
			foreach (var b in src.blocks)
				blocks.Add() = b;
			BlockLock = blocks.size;
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
			block.propSize = prop.size;
			block.lockSize1 = prop.size;
			block.lockSize2 = 0;

			BlockCode = op;
			BlockStart = start;
			BlockEnd = end;
			BlockAt1 = at1;
			BlockAt2 = at2;
		}
		public void LockTop()
		{
			if (blocks.size <= BlockLock)
				return;
			ref var top = ref blocks.Top();
			top.lockSize1 = prop.size;
			top.lockSize2 = top.added.size;
		}
		public void ResetTop()
		{
			if (blocks.size <= BlockLock)
				return;
			ref var top = ref blocks.Top();
			prop.Count = top.lockSize1;
			for (var i = top.lockSize2; i < top.added.size; i++)
				dict.Remove(top.added.items[i]);
			var shadow = top.shadow;
			if (shadow == null)
				return;
			foreach (var pair in shadow)
			{
				if (pair.Value.next < prop.Count)
					continue;
				dict[pair.Key] = pair.Value.prev;
			}
			shadow.Clear();
		}
		public int Pop()
		{
			if (blocks.size <= BlockLock)
				throw InvalidOperation("No block left to remove");

			ref var top = ref blocks.Top();
			prop.Count = top.propSize;
			BlockStart = top.start;
			BlockEnd = top.end;
			BlockAt1 = top.at1;
			BlockAt2 = top.at2;
			BlockCode = top.op;
			var shadow = top.shadow;
			foreach (var name in top.added)
				dict.Remove(name);
			blocks.size--;
			if (shadow != null)
			{
				foreach (var pair in shadow)
					dict[pair.Key] = pair.Value.prev;
				shadow.Clear();
			}
			return BlockEnd;
		}
		public void PopAll()
		{
			BlockStart = RootStart;
			BlockEnd = RootEnd;
			BlockAt1 = 0;
			BlockAt2 = 0;
			var propSize = prop.size;
			while (blocks.size > BlockLock)
			{
				ref var top = ref blocks.Top();
				propSize = top.propSize;
				var shadow = top.shadow;
				foreach (var name in top.added)
					dict.Remove(name);
				if (shadow != null)
				{
					foreach (var pair in shadow)
						dict[pair.Key] = pair.Value.next;
					shadow.Clear();
				}
				blocks.size--;
			}
			blocks.size = 0;
			prop.Count = propSize;
		}

		public override int Add(string name, ref Value value)
		{
			var idx = prop.size;
			ref var it = ref prop.Add();
			it.name = name;
			it.value = value;
			if (name != null)
			{
				if (blocks.size > 0)
				{
					ref var top = ref blocks.Top();
					if (dict != null && dict.ContainsKey(name))
					{
						if (top.shadow == null)
							top.shadow = new Dictionary<string, Shadow>(StringComparer.OrdinalIgnoreCase);
						top.shadow[name] = new Shadow()
						{
							prev = dict[name],
							next = idx
						};
					}
					else
						top.added.Add(name);
				}
				if (dict == null)
					dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				dict[name] = idx;
			}
			return idx;
		}
	}
}
