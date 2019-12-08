using System;
using System.Collections.Generic;
using System.Diagnostics;
using RedOnion.Collections;

namespace RedOnion.ROS.Parsing
{
	partial class Parser
	{
		/// <summary>
		/// Function/script execution context.
		/// Simpler version of <see cref="Objects.Context"/>
		/// </summary>
		public class Context
		{
			/// <summary>
			/// Variables added/shadowed by current/inner block (blockStack.Top())
			/// and index of first variable in current/inner block (top in outer).
			/// Simpler version of <see cref="Objects.Context.Block"/>
			/// </summary>
			public struct Block
			{
				/// <summary>
				/// Variables added by inner block
				/// (outermost block does not need to track this)
				/// </summary>
				public ListCore<string> added;
				/// <summary>
				/// Previous indexes of shadowed variables (by the outer block)
				/// </summary>
				public Dictionary<string, int> shadow;
				/// <summary>
				/// Starting varsCount (index of first variable in current/inner block)
				/// </summary>
				public int varsFrom;
			}

			/// <summary>
			/// Number of all variables (includes shadowed)
			/// </summary>
			public int varsCount;
			/// <summary>
			/// Index of first variable in current block
			/// </summary>
			public int blockStart;
			/// <summary>
			/// Map of active variables to their indexes
			/// </summary>
			public Dictionary<string, int> vars;
			/// <summary>
			/// Stack of outer blocks
			/// </summary>
			public ListCore<Block> blockStack;
			/// <summary>
			/// Number of outer blocks
			/// </summary>
			public int BlockCount => blockStack.size;

			/// <summary>
			/// Variables captured by current function/lambda
			/// </summary>
			public HashSet<string> captured;

			/// <summary>
			/// Clear all variables and blocks
			/// </summary>
			public void Clear()
			{
				varsCount = 0;
				vars?.Clear();
				blockStack.Clear();
				captured?.Clear();
			}
			/// <summary>
			/// Add new variable (or overwrite or shadow existing)
			/// </summary>
			public void Add(string name)
			{
				if (vars == null)
					vars = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
				else if (vars.TryGetValue(name, out var idx))
				{
					// second declaration of the same variable sets it to void (none is created)
					if (idx >= blockStart)
						return;
					// variable of same name as in some outer block (shadowing)
					ref var top = ref blockStack.Top();
					if (top.shadow == null)
						top.shadow = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
					top.shadow[name] = idx;
				}
				vars[name] = varsCount++;
				if (BlockCount > 0)
					blockStack.Top().added.Add(name);
			}
			/// <summary>
			/// Enter new block
			/// </summary>
			public void Push()
				=> blockStack.Add().varsFrom = varsCount;
			/// <summary>
			/// Exit block
			/// </summary>
			public void Pop()
			{
				ref var top = ref blockStack.Top();
				varsCount = top.varsFrom;
				var shadow = top.shadow;
				foreach (var name in top.added)
					vars.Remove(name);
				blockStack.size--;
				if (shadow != null)
				{
					foreach (var pair in shadow)
						vars[pair.Key] = pair.Value;
					shadow.Clear();
				}
			}
		}
		public Context ctx = new Context();
		public ListCore<Context> ctxPool;

		/// <summary>
		/// Map of label to code-point
		/// </summary>
		public Dictionary<string, int> labelTable;
		/// <summary>
		/// Map of goto instructions and label names
		/// </summary>
		public Dictionary<int, string> gotoTable;

		protected virtual void Label(Flag flags)
		{
			if (Word.Length > 127)
				throw new ParseError(this, "Label too long");
			if (labelTable == null)
				labelTable = new Dictionary<string, int>();
			if (labelTable.ContainsKey(Word))
				throw new ParseError(this, "Duplicit label name: " + Word);
			labelTable[Word] = code.size;
			Next().Next();
		}
		protected struct StoredLabels
		{
			public Dictionary<string, int> LabelTable { get; }
			public Dictionary<int, string> GotoTable { get; }
			public StoredLabels(Dictionary<string, int> labelTable, Dictionary<int, string> gotoTable)
			{
				LabelTable = labelTable;
				GotoTable = gotoTable;
			}
		}
		protected StoredLabels StoreLabels()
		{
			var labelTable = this.labelTable;
			var gotoTable = this.gotoTable;
			this.labelTable = null;
			this.gotoTable = null;
			return new StoredLabels(labelTable, gotoTable);
		}
		protected void RestoreLabels(StoredLabels labels)
		{
			//TODO: save line, column and character index for each goto
			//..... for better exception reporting
			if (gotoTable != null)
			{
				if (labelTable == null)
					throw new ParseError(this, "No labels but goto");
				foreach (var pair in gotoTable)
				{
					var label = pair.Value;
					if (!labelTable.TryGetValue(label, out var at))
						throw new ParseError(this, "Missing label: " + label);
					Write(at - (pair.Key+4), pair.Key); // relative to after goto
				}
			}
			labelTable = labels.LabelTable;
			gotoTable = labels.GotoTable;
		}

		/// <summary>
		/// Parse function or lambda
		/// </summary>
		/// <param name="name">Name of the function, null for lambda</param>
		/// <remarks><code>
		/// byte      OpCode.Function
		/// string    name (int index to strings)
		/// int       size of header (number of following bytes)
		/// ushort    type/access flags (reserved for methods)
		/// byte      number of generic parameters
		/// byte      number of arguments
		/// int       size of return type
		/// code      return type (e.g. OpCode.Void)
		/// arguments - array of
		///   string    argument name (int index to strings)
		///   int       argument type size
		///   code      argument type (OpCode.Void for universal)
		///   int       argument default value size
		///   code      argument default value (OpCode.Void for none)
		/// (end of header)
		/// 
		/// int       code size
		/// code      code
		/// int       tail size
		/// int       number of captured variables
		/// string[]  list of captured variables (indexes to strings)
		/// </code></remarks>
		protected virtual void ParseFunction(string name, Flag flags)
		{
			var ind = Indent;

			// swap context
			var prevCtx = ctx;
			ctx = ctxPool.size > 0 ? ctxPool.Pop() : new Context();

			// header
			if (name != null)
			{
				Write(name);    // function name (index to string table)
				prevCtx.Add(name);
			}
			else
			{
				flags |= Flag.Limited;
				if (!HasOption(Option.Prefix))
					Write(-1);	// lambda
			}
			Write(0);           // header size
			int mark = code.size;
			Write((ushort)0);   // type flags
			Write((byte)0);     // number of generic parameters
			Write((byte)0);     // number of arguments

			Write(0);           // return type size
			var typeMark = code.size;
			Next().OptionalType(flags);
			Write(code.size - typeMark, typeMark-4);

			var argc = 0;
			var paren = Curr == '(';
			if (paren || Curr == ',')
				Next(true);
			bool lambda = !paren && ExCode == ExCode.Lambda;
			while ((paren || (!Eol && Curr != ';')) && !Eof && !lambda)
			{
				if (Word == null)
					throw new ParseError(this, "Expected argument name");
				if (argc > 127)
					throw new ParseError(this, "Too many arguments");

				Write(Word);  // argument name (index to string table)

				Write(0);           // argument type size
				var argMark = code.size;
				Next().OptionalType(flags);
				Write(code.size - argMark, argMark-4);

				Write(0);           // argument default value size
				argMark = code.size;
				OptionalExpression(flags);
				Write(code.size - argMark, argMark-4);
				argc++;

				if (paren && Curr == ')')
				{
					Next();
					break;
				}
				if (!paren && ExCode == ExCode.Lambda)
				{
					lambda = true;
					break;
				}
				if (Curr == ',')
					Next(true);
			}
			if (lambda)
			{
				Next();
				flags |= Flag.Limited;
			}

			Write(code.size - mark, mark-4);   // header size
			code.items[mark + 3] = (byte)argc; // number of arguments

			// body
			var labels = StoreLabels();
			var blockAt = code.size;
			var count = ParseBlock(flags, ind);
			if (lambda && count == 1)
			{
				if (HasOption(Option.Prefix))
				{
					var op = (OpCode)code.items[blockAt+4];
					if (op == OpCode.Autocall)
						code.items[blockAt+4] = OpCode.Return.Code();
					else if (op.Kind() < OpKind.Statement)
					{
						code.EnsureCapacity(code.size+1);
						var sz = Core.Int(code.items, blockAt);
						Write(++sz, blockAt);
						Array.Copy(code.items, blockAt+4, code.items, blockAt+5, code.size-blockAt-5);
						Write(OpCode.Return.Code(), blockAt+4);
						code.size++;
					}
				}
				else if (lastCode == OpCode.Pop)
				{
					Debug.Assert(lastCodeAt == code.size-1);
					if (prevCode == OpCode.Autocall)
					{
						Debug.Assert(prevCodeAt == code.size-2);
						var sz = Core.Int(code.items, blockAt);
						Write(--sz, blockAt);
						code.size--;
					}
					code.items[code.size-1] = OpCode.Return.Code();
				}
			}
			RestoreLabels(labels);

			// tail
			var captured = ctx?.captured;
			if (captured == null)
				Write(0); // tail size
			else
			{
				Write(4+4*captured.Count);
				Write(captured.Count);
				foreach (var vname in captured)
				{
					Write(vname);
					if (prevCtx.vars?.ContainsKey(vname) == true)
						continue;
					if (prevCtx.captured == null)
						prevCtx.captured = new HashSet<string>();
					prevCtx.captured.Add(vname);
				}
			}

			// swap context
			ctx.Clear();
			ctxPool.Push(ctx);
			ctx = prevCtx;
		}
	}
}
