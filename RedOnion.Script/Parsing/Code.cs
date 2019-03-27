using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RedOnion.Script.Parsing
{
	partial class Parser
	{
		private byte[] _code = new byte[256];
		private string[] _stringTable = new string[64];
		private Dictionary<string, int> _stringMap = new Dictionary<string, int>();
		private int[] _lineMap = new int[64];

		/// <summary>
		/// Buffer for final code (statements and expressions)
		/// </summary>
		public byte[] Code => _code;
		/// <summary>
		/// Write position (top) for code buffer
		/// </summary>
		public int CodeAt { get; protected set; }

		/// <summary>
		/// Strings table for code
		/// </summary>
		public string[] Strings => _stringTable;
		/// <summary>
		/// Write position (top) for string table
		/// </summary>
		public int StringsAt { get; protected set; }

		/// <summary>
		/// Index into Code for each line
		/// </summary>
		public int[] LineMap => _lineMap;
		/// <summary>
		/// Write position (top) for line map
		/// </summary>
		public int LineMapAt { get; protected set; }

		/// <summary>
		/// Copy code to separate byte array
		/// </summary>
		public byte[] CodeToArray()
		{
			var arr = new byte[CodeAt];
			Array.Copy(Code, 0, arr, 0, arr.Length);
			return arr;
		}
		/// <summary>
		/// Copy string table to separate byte array
		/// </summary>
		public string[] StringsToArray()
		{
			var arr = new string[StringsAt];
			Array.Copy(Strings, 0, arr, 0, arr.Length);
			return arr;
		}
		public int[] LineMapToArray()
		{
			var arr = new int[LineMapAt];
			Array.Copy(LineMap, 0, arr, 0, arr.Length);
			return arr;
		}
		public CompiledCode.SourceLine[] LinesToArray()
			=> lexer.LinesToArray();

		internal void RecordLine()
		{
			if (LineMapAt == _lineMap.Length)
				Array.Resize(ref _lineMap, _lineMap.Length << 1);
			_lineMap[LineMapAt++] = CodeAt;
		}

		/// <summary>
		/// Ensure code buffer capacity
		/// </summary>
		protected void Reserve(int size)
		{
			var need = CodeAt + size;
			var cap = Code.Length;
			if (need < cap)
				return;
			do cap <<= 1; while (need > cap);
			Array.Resize(ref _code, cap);
		}

		/// <summary>
		/// Write single byte to code buffer
		/// </summary>
		protected void Write(byte value)
		{
			Reserve(1);
			Code[CodeAt++] = value;
		}
		/// <summary>
		/// Write instruction to code buffer
		/// </summary>
		protected int Write(OpCode value)
		{
			var at = CodeAt;
			Reserve(1);
			Code[CodeAt++] = value.Code();
			return at;
		}

		/// <summary>
		/// Write single byte to code buffer at specified position
		/// </summary>
		protected void Write(byte value, int codeAt)
		{
			Code[codeAt++] = value;
		}

		/// <summary>
		/// Write two bytes to code buffer
		/// </summary>
		protected void Write(ushort value)
		{
			Reserve(2);
			Code[CodeAt++] = (byte)value;
			Code[CodeAt++] = (byte)(value >> 8);
		}

		/// <summary>
		/// Write two bytes to code buffer at specified position
		/// </summary>
		protected void Write(ushort value, int codeAt)
		{
			Code[codeAt++] = (byte)value;
			Code[codeAt++] = (byte)(value >> 8);
		}

		/// <summary>
		/// Write integer (4B LE) to code buffer
		/// </summary>
		protected int Write(int value)
		{
			var codeAt = CodeAt;
			Reserve(4);
			Code[CodeAt++] = (byte)value;
			Code[CodeAt++] = (byte)(value >> 8);
			Code[CodeAt++] = (byte)(value >> 16);
			Code[CodeAt++] = (byte)(value >> 24);
			return codeAt;
		}

		/// <summary>
		/// Write integer (4B LE) to code buffer at specified position
		/// </summary>
		protected void Write(int value, int codeAt)
		{
			Code[codeAt++] = (byte)value;
			Code[codeAt++] = (byte)(value >> 8);
			Code[codeAt++] = (byte)(value >> 16);
			Code[codeAt++] = (byte)(value >> 24);
		}

		/// <summary>
		/// Write string (index to string table) to code buffer
		/// </summary>
		protected void Write(string value)
		{
			if (_stringMap.TryGetValue(value, out var index))
				Write(index);
			else
			{
				Write(StringsAt);
				_stringMap[value] = StringsAt;
				if (StringsAt == _stringTable.Length)
					Array.Resize(ref _stringTable, _stringTable.Length << 1);
				_stringTable[StringsAt++] = value;
			}
		}

		/// <summary>
		/// write byte array to code buffer
		/// </summary>
		protected void Write(byte[] bytes)
		{
			Reserve(bytes.Length);
			Array.Copy(bytes, 0, Code, CodeAt, bytes.Length);
			CodeAt += bytes.Length;
		}

		/// <summary>
		/// Write string literal or identifier to code buffer
		/// @string	value to write to code buffer
		/// </summary>
		/// <param name="code">Leading code (type of literal or identifier)</param>
		protected void Write(OpCode code, string value)
		{
			Write((byte)code);
			Write(value);
		}

		/// <summary>
		/// Copy string literal or identifier from vals to code buffer
		/// </summary>
		/// <param name="code">Leading code (type of literal or identifier)</param>
		protected void CopyString(OpCode code, int top, int start)
		{
			Debug.Assert(top - start == 4);
			Write((byte)code);
			var index = TopInt(top);
			Write(index == -1 ? "" : StringValues[index]);
		}
		/// <summary>
		/// Copy string literal or identifier from vals to code buffer
		/// </summary>
		protected void CopyString(int top, int start)
		{
			Debug.Assert(top - start == 4);
			var index = TopInt(top);
			Write(index == -1 ? "" : StringValues[index]);
		}

		/// <summary>
		/// Copy block from parsed values to code buffer
		/// </summary>
		protected void Copy(int top, int start)
		{
			var len = top - start;
			Reserve(len);
			Array.Copy(Values, start, Code, CodeAt, len);
			CodeAt += len;
		}

		/// <summary>
		/// Rewrite code from parsed value buffer/stack to code buffer
		/// </summary>
		protected virtual void Rewrite(int top, bool type = false)
		{
		full:
			var start = TopInt(top);
			top -= 4;
			var create = false;
		next:
			var op = ((OpCode)Values[--top]).Extend();
			var kind = op.Kind();
			if (kind >= OpKind.Statement)
			{
				Debug.Assert(op == OpCode.Function);
				Write(op);
				Copy(top, start);
				return;
			}
			if (kind <= OpKind.Number)
			{
				if (!type && !create)
				{
					Literal(op, top, start);
					return;
				}
				if (op == OpCode.Identifier)
				{
					CopyString(op, top, start);
					return;
				}
				Debug.Assert(op > OpCode.Identifier || op == OpCode.Undefined || op == OpCode.This || op == OpCode.Null);
				Write((byte)op);
				return;
			}
			Write((byte)op);
			if (op.Unary())
			{
				create |= op == OpCode.Create;
				goto next;
			}
			if (op.Binary())
			{
				// first/left argument
				Rewrite(TopInt(top), type || create);
				// second/right argument
				if (op == OpCode.Dot)
				{
					start = TopInt(top);
					top -= 4;
					op = ((OpCode)Values[--top]).Extend();
					if (op != OpCode.Identifier)
						throw new InvalidOperationException();
					CopyString(top, start);
					return;
				}
				if (op != OpCode.LogicAnd && op != OpCode.LogicOr)
					goto full;
				// prepare slot for size of second argument
				var second = CodeAt;
				Write(0);
				Rewrite(top);
				// update size of second argument (for skipping)
				Write(CodeAt - second - 4, second);
				return;
			}
			if (op.Ternary())
			{
				// top of middle/second argument
				var mtop = TopInt(top);

				if (op == OpCode.Var)
				{
					Debug.Assert(!create);
					var varat = --CodeAt; // remove our OpCode.Var
					Rewrite(TopInt(mtop));
					Code[varat] = OpCode.Var.Code(); // rewrite OpCode.Identifier with OpCode.Var
					Rewrite(mtop, true);
					goto full;
				}

				// rewrite first argument (condition, method or variable)
				Rewrite(TopInt(mtop), type || create);
				if (op == OpCode.Ternary)
				{
					// prepare slot for size of second argument (if true)
					var second = CodeAt;
					Write(0);
					Rewrite(mtop);
					// update size of second argument (for skipping)
					Write(CodeAt - second - 4, second);
					// prepare slot for size of third argument (if false)
					var third = CodeAt;
					Write(0);
					Rewrite(top);
					// update size of third argument (for skipping)
					Write(CodeAt - third - 4, third);
					return;
				}
				// rewrite second argument (first for method - call2)
				Rewrite(mtop);
				// rewrite third argument (second for method - call2)
				goto full;
			}

			switch (op)
			{
			case OpCode.Array:
				type = false;
				create = true;
				break;
			case OpCode.Generic:
				type = true;
				break;
			default:
				Debug.Assert(op == OpCode.CallN || op == OpCode.IndexN);
				break;
			}
			var n = Values[--top];
			Write((byte)n);
			Rewrite(top, n, type, create);
		}

		/// <summary>
		/// Rewrite n-expressions/values from parsed value buffer/stack to code buffer
		/// </summary>
		protected void Rewrite(int top, int n, bool type = false, bool create = false)
		{
			if (n > 1)
			{
				Rewrite(TopInt(top), n - 1, type, create);
				create = false;
			}
			Rewrite(top, type || create);
		}

		/// <summary>
		/// Rewrite literal from parsed value buffer/stack to code buffer
		/// </summary>
		protected virtual void Literal(OpCode code, int top, int start)
		{
			if (code == OpCode.Identifier || code == OpCode.String)
			{
				CopyString(code, top, start);
				return;
			}
			if (code < OpCode.Identifier || code == OpCode.Exception)
			{
				Write((byte)code);
				return;
			}
			var len = top - start;
			if (code.Kind() <= OpKind.Number && (byte)code >= OpCode.Char.Code())
			{
				Debug.Assert(code <= OpCode.Double);
				Debug.Assert(len == code.NumberSize());
				Reserve(len + 1);
				Code[CodeAt++] = (byte)code;
				Array.Copy(Values, start, Code, CodeAt, len);
				CodeAt += len;
				return;
			}
			throw new InvalidOperationException();
		}
	}
}
