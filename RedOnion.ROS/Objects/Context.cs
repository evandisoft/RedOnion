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
	[DebuggerDisplay("{Name}: {BlockCount}/{BlockCode}; {prop.DebugString}")]
	public class Context : Namespace
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

			/// <summary>
			/// Code/type of the block
			/// </summary>
			public BlockCode op;
		}
		/// <summary>
		/// Stack of blocks
		/// </summary>
		protected ListCore<Block> blockStack;
		/// <summary>
		/// Number of outer blocks
		/// </summary>
		public int BlockCount => blockStack.size;

		/// <summary>
		/// Starting position of the block
		/// </summary>
		public int BlockStart { get; set; }
		/// <summary>
		/// Position after the block
		/// </summary>
		public int BlockEnd { get; set; }
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
		public BlockCode BlockCode { get; set; }
		/// <summary>
		/// Number of try..catch..finally blocks in current context
		/// </summary>
		public int CatchBlocks { get; set; }

		/// <summary>
		/// Indicator of closure (fully or partially separated from live context)
		/// </summary>
		public bool Closure { get; protected set; }
		/// <summary>
		/// Captured variables that we cannot copy yet.
		/// Value is either -1 or index to props (already reserved slot)
		/// </summary>
		public HashSet<string> Captured { get; protected set; }
		/// <summary>
		/// Inner closures (functions) that need to be separated
		/// </summary>
		public ListCore<Context> closures;
		/// <summary>
		/// Link from function-context to its separating closure-context
		/// </summary>
		public Context Cousin { get; protected set; }

		private int rootStart;
		public int RootStart
		{
			get => rootStart;
			set
			{
				rootStart = value;
				if (BlockCount == 0)
					BlockStart = value;
				else blockStack.items[0].start = value;
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
				else blockStack.items[0].start = value;
			}
		}

		public Context() : base("context", typeof(Context)) { }

		public Context(Function fn, Context parent, HashSet<string> cvars)
			: base("Context of " + fn.Name, typeof(Context))
		{
			BlockCode = BlockCode.Function;
			RootStart = fn.CodeAt;
			RootEnd = fn.CodeAt + fn.CodeSize;
			this.parent = parent;
			if (cvars != null)
			{
				Captured = cvars;
				parent.closures.Add(this);
			}
		}
		protected Context(Context cousin)
			: base(cousin.Name.StartsWith("Context of ")
				  ? "Cousin of " + cousin.Name.Substring(11)
				  : "Cousin Contex", typeof(Context))
		{
			RootStart = cousin.RootStart;
			RootEnd = cousin.RootEnd;
			Closure = true;
			parent = cousin;
		}

		public override void Reset()
		{
			PopAll();
			base.Reset();
		}

		public void Push(int start, int end, BlockCode op = BlockCode.Block, int at1 = 0, int at2 = 0)
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
		/// <summary>
		/// Lock current variables
		/// so that they are not removed by <see cref="ResetTop"/>.
		/// (Used after initialization of a loop)
		/// </summary>
		public void LockTop()
		{
			if (blockStack.size == 0)
				return;
			ref var top = ref blockStack.Top();
			top.varsLock = prop.size;
			top.addedLock = top.added.size;
		}
		/// <summary>
		/// Reset to block.
		/// (Called when loop is continued)
		/// </summary>
		public void ResetTop()
		{
			if (blockStack.size == 0)
				return;
			ref var top = ref blockStack.Top();
			if (closures.size > 0)
				SeparateVars(ref top.added, top.addedLock);
			else
				for (var i = top.addedLock; i < top.added.size; i++)
					dict.Remove(top.added.items[i]);
			prop.Count = top.varsLock;
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
		/// <summary>
		/// Pop/remove one block.
		/// (Called on block exit)
		/// </summary>
		/// <returns><see cref="BlockEnd"/> of reactivated block</returns>
		public int Pop()
		{
			if (blockStack.size == 0)
				throw InvalidOperation("No block left to remove");

			ref var top = ref blockStack.Top();
			BlockStart = top.start;
			BlockEnd = top.end;
			BlockAt1 = top.at1;
			BlockAt2 = top.at2;
			BlockCode = top.op;
			var shadow = top.shadow;
			if (closures.size > 0)
				SeparateVars(ref top.added);
			else
				foreach (var name in top.added)
					dict.Remove(name);
			prop.Count = top.varsFrom;
			blockStack.size--;
			if (shadow != null)
			{
				foreach (var pair in shadow)
					dict[pair.Key] = pair.Value;
				shadow.Clear();
			}
			return BlockEnd;
		}
		/// <summary>
		/// Pop/remove/exit all blocks.
		/// (Called on script/function exit/return)
		/// </summary>
		public void PopAll()
		{
			BlockStart = RootStart;
			BlockEnd = RootEnd;
			BlockAt1 = 0;
			BlockAt2 = 0;
			CatchBlocks = 0;
			var propSize = prop.size;
			while (blockStack.size > 0)
			{
				ref var top = ref blockStack.Top();
				BlockCode = top.op;
				propSize = top.varsFrom;
				var shadow = top.shadow;
				if (closures.size > 0)
					SeparateVars(ref top.added);
				else
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
		/// <summary>
		/// Copy removed variables (on block-exit or loop)
		/// to cousin context used by closures
		/// </summary>
		/// <param name="added">List of added and now removed variables</param>
		/// <param name="from">Index of first variable to remove</param>
		protected void SeparateVars(ref ListCore<string> added, int from = 0)
		{
			Debug.Assert(closures.size > 0);
			Debug.Assert(!Closure);
			for (var i = from; i < added.size; i++)
			{
				var name = added.items[i];
				foreach (var closure in closures)
				{
					if (closure.Captured?.Contains(name) != true)
						continue;
					closure.Captured.Remove(name);
					if (closure.Captured.Count == 0)
						closure.Captured = null;

					var cousin = Cousin;
					if (cousin == null)
						Cousin = cousin = new Context(this);
					closure.parent = Cousin;
					if (cousin.dict == null || !cousin.dict.TryGetValue(name, out var newIdx))
					{
						newIdx = cousin.prop.size;
						cousin.prop.Add() = prop[dict[name]];
						if (cousin.dict == null)
							cousin.dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
						cousin.dict[name] = newIdx;
					}
					int idx = closure.Find(name);
					Debug.Assert(idx >= 0);
					closure.prop.items[idx].value.SetRef(cousin, newIdx);
				}
				dict.Remove(name);
			}
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

		public override int Find(string name)
		{
			if (dict != null && dict.TryGetValue(name, out var idx))
				return idx;
			if (parent == null)
				return -1;

			if (Captured?.Contains(name) == true)
			{
				for (var obj = parent; ;)
				{
					if (obj.dict != null && obj.dict.TryGetValue(name, out var refIdx))
					{
						idx = prop.size;
						ref var link = ref prop.Add();
						link.name = name;
						link.value.SetRef(obj, refIdx);
						if (blockStack.size > 0)
							blockStack.Top().added.Add(name);
						if (dict == null)
							dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
						dict[name] = idx;
						return idx;
					}
					obj = obj.parent;
					if (obj == null)
						return -1;
				}
			}

			idx = parent.Find(name);
			if (idx < 0)
				return idx;
			return Add(name, ref parent.prop.items[idx].value);
		}
		public override bool Get(ref Value self, int at)
		{
			if (at < 0 || at >= prop.size)
				return false;
			self = prop.items[at].value;
			if (self.IsReference)
				return self.desc.Get(ref self, self.num.Int);
			return true;
		}
		public override bool Set(ref Value self, int at, OpCode op, ref Value value)
		{
			if (at < readOnlyTop || at >= prop.size)
				return false;
			ref var it = ref prop.items[at].value;
			if (it.IsReference)
				return it.desc.Set(ref self, it.num.Int, op, ref value);
			if (op == OpCode.Assign)
			{
				it = value;
				return true;
			}
			if (op.Kind() == OpKind.Assign)
				return it.desc.Binary(ref it, op + 0x10, ref value)
					|| value.desc.Binary(ref it, op + 0x10, ref value);
			if (op.Kind() != OpKind.PreOrPost)
				return false;
			if (op >= OpCode.Inc)
				return it.desc.Unary(ref it, op);
			self = it;
			return it.desc.Unary(ref it, op + 0x08);
		}
	}
}
