using System;
using System.Collections.Generic;
using System.Diagnostics;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS.Parsing
{
	partial class Parser
	{
		public ListCore<byte> code = new ListCore<byte>(256);
		public ListCore<string> strings = new ListCore<string>(64);
		public Dictionary<string, int> stringMap = new Dictionary<string, int>();
		public ListCore<int> lineMap = new ListCore<int>(64);
		public ListCore<SourceLine> lines = new ListCore<SourceLine>(64);

		public override void SetLine(string value)
		{
			base.SetLine(value);
			if (Line == null)
				return;
			lines.Add(new SourceLine(CharCounter, Line));
			lineMap.Add(code.size);
		}

		/// <summary>
		/// Copy code to separate byte array
		/// </summary>
		public byte[] CodeToArray()
			=> code.ToArray();
		/// <summary>
		/// Copy string table to separate byte array
		/// </summary>
		public string[] StringsToArray()
			=> strings.ToArray();

		public int[] LineMapToArray()
			=> lineMap.ToArray();
		public SourceLine[] LinesToArray()
			=> lines.ToArray();

		/// <summary>
		/// Ensure code buffer capacity
		/// </summary>
		protected void Reserve(int size)
			=> code.EnsureCapacity(code.size + size);

		/// <summary>
		/// Write single byte to code buffer
		/// </summary>
		protected void Write(byte value)
			=> code.Push(value);

		// needed by lambda in postfix
		protected OpCode lastCode, prevCode;
		protected int lastCodeAt, prevCodeAt;

		/// <summary>
		/// Write instruction to code buffer
		/// </summary>
		protected int Write(OpCode value)
		{
			prevCode = lastCode;
			prevCodeAt = lastCodeAt;
			lastCodeAt = code.size;
			code.Push((lastCode = value).Code());
			return lastCodeAt;
		}
		/// <summary>
		/// Write instruction to code buffer
		/// </summary>
		protected int Write(ExCode value)
			=> Write((OpCode)value);

		/// <summary>
		/// Write single byte to code buffer at specified position
		/// </summary>
		protected void Write(byte value, int codeAt)
			=> code.items[codeAt] = value;
		/// <summary>
		/// Write single byte to code buffer at specified position
		/// </summary>
		protected void Write(OpCode value, int codeAt)
			=> code.items[codeAt] = (byte)value;

		/// <summary>
		/// Write two bytes to code buffer
		/// </summary>
		protected void Write(ushort value)
		{
			Reserve(2);
			code.items[code.size++] = (byte)value;
			code.items[code.size++] = (byte)(value >> 8);
		}

		/// <summary>
		/// Write two bytes to code buffer at specified position
		/// </summary>
		protected void Write(ushort value, int codeAt)
		{
			code.items[codeAt++] = (byte)value;
			code.items[codeAt++] = (byte)(value >> 8);
		}

		/// <summary>
		/// Write integer (4B LE) to code buffer
		/// </summary>
		protected int Write(int value)
		{
			var codeAt = code.size;
			Reserve(4);
			code.items[code.size++] = (byte)value;
			code.items[code.size++] = (byte)(value >> 8);
			code.items[code.size++] = (byte)(value >> 16);
			code.items[code.size++] = (byte)(value >> 24);
			return codeAt;
		}

		/// <summary>
		/// Write integer (4B LE) to code buffer at specified position
		/// </summary>
		protected void Write(int value, int codeAt)
		{
			code.items[codeAt++] = (byte)value;
			code.items[codeAt++] = (byte)(value >> 8);
			code.items[codeAt++] = (byte)(value >> 16);
			code.items[codeAt++] = (byte)(value >> 24);
		}

		/// <summary>
		/// Write string (index to string table) to code buffer
		/// </summary>
		protected void Write(string value)
		{
			if (stringMap.TryGetValue(value, out var index))
				Write(index);
			else
			{
				Write(strings.size);
				stringMap[value] = strings.size;
				strings.Push(value);
			}
		}

		/// <summary>
		/// write byte array to code buffer
		/// </summary>
		protected void Write(byte[] bytes)
		{
			Reserve(bytes.Length);
			Array.Copy(bytes, 0, code.items, code.size, bytes.Length);
			code.size += bytes.Length;
		}

		/// <summary>
		/// Write string literal or identifier to code buffer
		/// </summary>
		/// <param name="code">Leading code (type of literal or identifier)</param>
		/// <param name="value">Value to write to code buffer</param>
		protected void Write(OpCode code, string value)
		{
			Write((byte)code);
			Write(value);
		}
		/// <summary>
		/// Write string literal or identifier to code buffer
		/// </summary>
		/// <param name="code">Leading code (type of literal or identifier)</param>
		/// <param name="value">Value to write to code buffer</param>
		protected void Write(ExCode code, string value)
		{
			Write((byte)code);
			Write(value);
		}

		/// <summary>
		/// Copy string literal or identifier from vals to code buffer
		/// </summary>
		/// <param name="code">Leading code (type of literal or identifier)</param>
		protected string CopyString(OpCode code, int top, int start)
		{
			Debug.Assert(top - start == 4);
			Write((byte)code);
			var index = TopInt(top);
			var str = index == -1 ? "" : stringValues.items[index];
			Write(str);
			return str;
		}
		/// <summary>
		/// Copy string literal or identifier from vals to code buffer
		/// </summary>
		protected void CopyString(int top, int start)
		{
			Debug.Assert(top - start == 4);
			var index = TopInt(top);
			Write(index == -1 ? "" : stringValues.items[index]);
		}

		/// <summary>
		/// Copy block from parsed values to code buffer
		/// </summary>
		protected void Copy(int top, int start)
		{
			var len = top - start;
			Reserve(len);
			Array.Copy(values.items, start, code.items, code.size, len);
			code.size += len;
		}

		/// <summary>
		/// Rewrite code from parsed value buffer/stack to code buffer.
		/// Returns root op code.
		/// </summary>
		protected virtual OpCode Rewrite(int top, bool type = false, int start = -1, bool create = false)
		{
			if (start >= 0)
				goto next;
		full:
			start = TopInt(top);
			top -= 4;
			create = false;
		next:
			var op = (OpCode)values.items[--top];
			var kind = op.Kind();
			if (kind >= OpKind.Statement)
			{
				Debug.Assert(op == OpCode.Function);
				Write(op);
				Copy(top, start);
				return op;
			}
			if (kind <= OpKind.Number)
			{
				if (!type && !create)
				{
					Literal(op, top, start);
					return op;
				}
				if (op == OpCode.Identifier)
				{
					if (create && !HasOption(Option.Prefix))
						Write(OpCode.Create);
					CopyString(op, top, start);
					return op;
				}
				Debug.Assert(op > OpCode.Identifier || op == OpCode.Void || op == OpCode.This || op == OpCode.Null);
				Write(op);
				return op;
			}
			if (HasOption(Option.Prefix))
				Write(op);
			var ex = op.Extend();
			if (ex.Unary())
			{
				if (create && !HasOption(Option.Prefix))
					Write(OpCode.Create);
				create |= op == OpCode.Create;
				if (HasOption(Option.Prefix))
					goto next;
				Rewrite(top, type || create, start, create);
				if (op != OpCode.Create)
					Write(op);
				return op;
			}
			if (ex.Binary())
			{
				// first/left argument
				Rewrite(TopInt(top), type || create);
				// second/right argument
				if (op == OpCode.Dot)
				{
					start = TopInt(top);
					top -= 4;
					op = (OpCode)values.items[--top];
					if (op != OpCode.Identifier)
						throw new InvalidOperationException();
					if (!HasOption(Option.Prefix))
					{
						if (create)
							Write(OpCode.Create);
						Write(OpCode.Dot);
					}
					CopyString(top, start);
					return op;
				}
				if (op != OpCode.LogicAnd && op != OpCode.LogicOr && op != OpCode.NullCol)
				{
					if (HasOption(Option.Prefix))
						goto full;
					Rewrite(top, type || create);
					if (create && !HasOption(Option.Prefix))
						Write(OpCode.Create);
					Write(op);
					return op;
				}
				if (!HasOption(Option.Prefix))
					Write(op);
				// prepare slot for size of second argument
				var second = code.size;
				Write(0);
				Rewrite(top);
				// update size of second argument (for skipping)
				Write(code.size - second - 4, second);
				return op;
			}
			if (ex.Ternary())
			{
				// top of middle/second argument
				var mtop = TopInt(top);

				if (op == OpCode.Var)
				{
					Debug.Assert(!create);
					if (HasOption(Option.Prefix))
					{
						var varat = --code.size; // remove our OpCode.Var
						Rewrite(TopInt(mtop));
						Debug.Assert(code[varat] == OpCode.Identifier.Code());
						code[varat] = OpCode.Var.Code(); // rewrite OpCode.Identifier with OpCode.Var
						Rewrite(mtop, true);
						goto full;
					}
					// type
					var peek = (OpCode)values.items[mtop-5];
					if (peek >= OpCode.String && peek < OpCode.Create || peek == OpCode.Array)
						Write(OpCode.Type);
					Rewrite(mtop, true);
					// value
					Rewrite(top);
					// name - like `Rewrite(TopInt(mtop), type || create);`
					// ... but we need to avoid adding the variable to captured
					// ... and also change the code to OpCode.Var
					top = TopInt(mtop);
					start = TopInt(top);
					top -= 4;
					op = (OpCode)values.items[--top];
					Debug.Assert(op == OpCode.Identifier);
					var name = CopyString(OpCode.Var, top, start);
					ctx.Add(name);
					return OpCode.Var;
				}

				// rewrite first argument (condition, method or variable)
				Rewrite(TopInt(mtop), type || create);
				if (op == OpCode.Ternary)
				{
					if (!HasOption(Option.Prefix))
						Write(op);
					// prepare slot for size of second argument (if true)
					var second = code.size;
					Write(0);
					Rewrite(mtop);
					// update size of second argument (for skipping)
					Write(code.size - second - 4, second);
					// special skip instruction for third argument in postfix mode
					if (!HasOption(Option.Prefix))
						Write(OpCode.Else);
					// prepare slot for size of third argument (if false)
					var third = code.size;
					Write(0);
					Rewrite(top);
					// update size of third argument (for skipping)
					Write(code.size - third - 4, third);
					return op;
				}
				// rewrite second argument (first for method - call2)
				Rewrite(mtop);
				// rewrite third argument (second for method - call2)
				if (HasOption(Option.Prefix))
					goto full;
				Rewrite(top);
				if (create && !HasOption(Option.Prefix))
					Write(OpCode.Create);
				Write(op);
				return op;
			}

			var n = values.items[--top];
			if (HasOption(Option.Prefix))
			{
				Write(n);
				Rewrite(top, n, type, create);
				return op;
			}
			Rewrite(top, n, type, create);
			if (create)
				Write(OpCode.Create);
			else if (type)
				Write(OpCode.Type);
			Write(op);
			Write(n);
			return op;
		}

		/// <summary>
		/// Rewrite n-expressions/values from parsed value buffer/stack to code buffer
		/// </summary>
		protected void Rewrite(int top, int n, bool type = false, bool create = false)
		{
			if (n > 1)
			{
				var next = TopInt(top);
				if (type && !HasOption(Option.Prefix))
				{
					var peek = (OpCode)values.items[next-5];
					if (peek >= OpCode.String && peek < OpCode.Create || peek == OpCode.Array)
						Write(OpCode.Type);
				}
				Rewrite(next, n - 1, type, create);
				create = false;
			}
			Rewrite(top, type || create);
		}

		/// <summary>
		/// Rewrite literal from parsed value buffer/stack to code buffer
		/// </summary>
		protected virtual void Literal(OpCode op, int top, int start)
		{
			if (op == OpCode.Identifier || op == OpCode.String)
			{
				var name = CopyString(op, top, start);
				if (op != OpCode.Identifier)
					return;
				if (ctx.vars?.ContainsKey(name) == true)
					return;
				if (ctx.captured == null)
					ctx.captured = new HashSet<string>();
				ctx.captured.Add(name);
				return;
			}
			if (op < OpCode.Identifier || op == OpCode.Exception)
			{
				Write((byte)op);
				return;
			}
			var len = top - start;
			if (op.Kind() <= OpKind.Number && op >= OpCode.Char)
			{
				Debug.Assert(op <= OpCode.Double);
				Debug.Assert(len == op.Extend().NumberSize());
				Reserve(len + 1);
				code.items[code.size++] = (byte)op;
				Array.Copy(values.items, start, code.items, code.size, len);
				code.size += len;
				return;
			}
			throw new InvalidOperationException();
		}
	}
}
