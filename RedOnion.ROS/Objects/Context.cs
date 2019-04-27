using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RedOnion.ROS.Objects
{
	/// <summary>
	/// Function/script execution context.
	/// Enhanced version of <see cref="Parsing.Parser.Context"/>
	/// </summary>
	[DebuggerDisplay("{BlockCount}/{BlockCode}; {prop.DebugString}")]
	public class Context : UserObject
	{
		/// <summary>
		/// Variables added/shadowed by current/inner block (blockStack.Top())
		/// and stored state of outer block (blockStack.Push()/Pop()).
		/// Enhanced version of <see cref="Parsing.Parser.Context.Block"/>
		/// </summary>
		protected struct Block
		{
			// INNER: (used through blockStack.Top() for current block)

			/// <summary>
			/// Variables added by inner/current block
			/// (outermost block does not need to track this)
			/// </summary>
			public ListCore<string> added;
			/// <summary>
			/// Previous indexes of shadowed variables (by inner/current block)
			/// </summary>
			public Dictionary<string, int> shadow;
			/// <summary>
			/// Starting prop.size (index of first variable in current/inner block)
			/// </summary>
			public int varsFrom;
			/// <summary>
			/// Number of variables preserved on reset (varsLock >= varsFrom >= prop.size)
			/// </summary>
			public int varsLock;
			/// <summary>
			/// Number of added variables preserved on reset (addedLock <= added.size)
			/// </summary>
			public int addedLock;

			// OUTER: (stored and restored through blockStack.Push()/Pop())

			public int start, end;
			public int at1, at2;

			// NOTE: last for better object layout

			/// <summary>
			/// Code/type of the block
			/// </summary>
			public OpCode op;
		}
		/// <summary>
		/// Stack of blocks
		/// </summary>
		protected ListCore<Block> blockStack;
		/// <summary>
		/// Number of outer blocks
		/// </summary>
		public int BlockCount => blockStack.size;

		// temporary solution - will change
		public int BlockLock { get; protected set; }

		/// <summary>
		/// Starting position of the block
		/// </summary>
		public int BlockStart { get; protected set; }
		/// <summary>
		/// Position after the block
		/// </summary>
		public int BlockEnd { get; protected set; }
		/// <summary>
		/// First important position in the block (usualy condition of the loop)
		/// </summary>
		public int BlockAt1 { get; protected set; }
		/// <summary>
		/// Second important position (final expression of for-loop)
		/// </summary>
		public int BlockAt2 { get; protected set; }
		/// <summary>
		/// Code/type of current block
		/// </summary>
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
			foreach (var b in src.blockStack)
				blockStack.Add() = b;
			BlockLock = blockStack.size;
		}

		public override void Reset()
		{
			PopAll();
			base.Reset();
		}

		public void Push(int start, int end, OpCode op = OpCode.Block, int at1 = 0, int at2 = 0)
		{
			ref var block = ref blockStack.Add();
			block.op = BlockCode;
			block.start = BlockStart;
			block.end = BlockEnd;
			block.at1 = BlockAt1;
			block.at2 = BlockAt2;
			block.varsFrom = prop.size;
			block.varsLock = prop.size;
			block.addedLock = 0;

			BlockCode = op;
			BlockStart = start;
			BlockEnd = end;
			BlockAt1 = at1;
			BlockAt2 = at2;
		}
		public void LockTop()
		{
			if (blockStack.size <= BlockLock)
				return;
			ref var top = ref blockStack.Top();
			top.varsLock = prop.size;
			top.addedLock = top.added.size;
		}
		public void ResetTop()
		{
			if (blockStack.size <= BlockLock)
				return;
			ref var top = ref blockStack.Top();
			prop.Count = top.varsLock;
			for (var i = top.addedLock; i < top.added.size; i++)
				dict.Remove(top.added.items[i]);
			var shadow = top.shadow;
			if (shadow == null)
				return;
			foreach (var pair in shadow)
			{
				if (pair.Value < prop.Count)
					continue;
				dict[pair.Key] = pair.Value;
			}
			shadow.Clear();
		}
		public int Pop()
		{
			if (blockStack.size <= BlockLock)
				throw InvalidOperation("No block left to remove");

			ref var top = ref blockStack.Top();
			prop.Count = top.varsFrom;
			BlockStart = top.start;
			BlockEnd = top.end;
			BlockAt1 = top.at1;
			BlockAt2 = top.at2;
			BlockCode = top.op;
			var shadow = top.shadow;
			foreach (var name in top.added)
				dict.Remove(name);
			blockStack.size--;
			if (shadow != null)
			{
				foreach (var pair in shadow)
					dict[pair.Key] = pair.Value;
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
			while (blockStack.size > BlockLock)
			{
				ref var top = ref blockStack.Top();
				propSize = top.varsFrom;
				var shadow = top.shadow;
				foreach (var name in top.added)
					dict.Remove(name);
				if (shadow != null)
				{
					foreach (var pair in shadow)
						dict[pair.Key] = pair.Value;
					shadow.Clear();
				}
				blockStack.size--;
			}
			blockStack.size = 0;
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
				if (blockStack.size > 0)
				{
					ref var top = ref blockStack.Top();
					if (dict != null && dict.ContainsKey(name))
					{
						if (top.shadow == null)
							top.shadow = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
						top.shadow[name] = dict[name];
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
