using Bee.Run;
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Bee
{
	public partial class PseudoGenerator
	{
		/// <summary>
		/// ensure code buffer capacity
		/// </summary>
		protected void Need(int size)
		{
			var need = CodeAt + size;
			var cap = Code.Length;
			if (need < cap)
			{
				return;
			}
			do
			{
				cap <<= 1;
			}
			while (need > cap);
			Array.Resize(ref _code, cap);
		}
		
		/// <summary>
		/// write single byte to code buffer
		/// </summary>
		protected void Write(byte @byte)
		{
			Need(1);
			Code[CodeAt++] = @byte;
		}
		
		/// <summary>
		/// write single byte to code buffer at specified position
		/// </summary>
		protected void Write(byte @byte, int codeAt)
		{
			Code[codeAt++] = @byte;
		}
		
		/// <summary>
		/// write two bytes to code buffer
		/// </summary>
		protected void Write(ushort @ushort)
		{
			Need(2);
			Code[CodeAt++] = unchecked((byte)@ushort);
			Code[CodeAt++] = unchecked((byte)(@ushort >> 8));
		}
		
		/// <summary>
		/// write two bytes to code buffer at specified position
		/// </summary>
		protected void Write(ushort @ushort, int codeAt)
		{
			Code[codeAt++] = unchecked((byte)@ushort);
			Code[codeAt++] = unchecked((byte)(@ushort >> 8));
		}
		
		/// <summary>
		/// write integer (4B LE) to code buffer
		/// </summary>
		protected void Write(int @int)
		{
			Need(4);
			Code[CodeAt++] = unchecked((byte)@int);
			Code[CodeAt++] = unchecked((byte)(@int >> 8));
			Code[CodeAt++] = unchecked((byte)(@int >> 16));
			Code[CodeAt++] = unchecked((byte)(@int >> 24));
		}
		
		/// <summary>
		/// write integer (4B LE) to code buffer at specified position
		/// </summary>
		protected void Write(int @int, int codeAt)
		{
			Code[codeAt++] = unchecked((byte)@int);
			Code[codeAt++] = unchecked((byte)(@int >> 8));
			Code[codeAt++] = unchecked((byte)(@int >> 16));
			Code[codeAt++] = unchecked((byte)(@int >> 24));
		}
		
		/// <summary>
		/// write string (UTF8, no terminator, no size mark) to code buffer
		/// </summary>
		protected void Write(string @string)
		{
			Write(Encoding.UTF8.GetBytes(@string));
		}
		
		/// <summary>
		/// write byte array to code buffer
		/// </summary>
		protected void Write(byte[] bytes)
		{
			Need(bytes.Length);
			Array.Copy(bytes, 0, Code, CodeAt, bytes.Length);
			CodeAt += bytes.Length;
		}
		
		/// <summary>
		/// write string literal or identifier to code buffer
		/// @opcode	leading code (type of literal or identifier)
		/// @blen	true for byte-length (e.g. identifier), false for short/int length (string literal)
		/// @string	value to write to code buffer
		/// </summary>
		protected void Write(Opcode opcode, bool blen, string @string)
		{
			Write(opcode, blen, Encoding.UTF8.GetBytes(@string));
		}
		
		/// <summary>
		/// write string literal or identifier to code buffer
		/// @opcode	leading code (type of literal or identifier)
		/// @blen	true for byte-length (e.g. identifier), false for short/int length (string literal)
		/// @bytes	array of bytes to write to code buffer
		/// </summary>
		protected void Write(Opcode opcode, bool blen, byte[] bytes)
		{
			if (blen)
			{
				if (bytes.Length > 255)
				{
					throw new InvalidOperationException("Byte array too long");
				}
				Need(2 + bytes.Length);
				Code[CodeAt++] = unchecked((byte)opcode);
				Code[CodeAt++] = unchecked((byte)bytes.Length);
			}
			else
			{
				Need(5 + bytes.Length);
				Code[CodeAt++] = unchecked((byte)opcode);
				Code[CodeAt++] = unchecked((byte)bytes.Length);
				Code[CodeAt++] = unchecked((byte)(bytes.Length >> 8));
				Code[CodeAt++] = unchecked((byte)(bytes.Length >> 16));
				Code[CodeAt++] = unchecked((byte)(bytes.Length >> 24));
			}
			Array.Copy(bytes, 0, Code, CodeAt, bytes.Length);
			CodeAt += bytes.Length;
		}
		
		/// <summary>
		/// copy string literal or identifier from vals to code buffer
		/// @opcode	leading code (type of literal or identifier)
		/// @blen	true for byte-length (e.g. identifier), false for int length (string literal)
		/// @string	value to write to code buffer
		/// </summary>
		protected void Copy(Opcode opcode, bool blen, int top, int start)
		{
			var len = top - start;
			if (blen)
			{
				if (len > 255)
				{
					throw new InvalidOperationException("Byte array too long");
				}
				Need(2 + len);
				Code[CodeAt++] = unchecked((byte)opcode);
				Code[CodeAt++] = unchecked((byte)len);
			}
			else
			{
				Need(5 + len);
				Code[CodeAt++] = unchecked((byte)opcode);
				Code[CodeAt++] = unchecked((byte)len);
				Code[CodeAt++] = unchecked((byte)(len >> 8));
				Code[CodeAt++] = unchecked((byte)(len >> 16));
				Code[CodeAt++] = unchecked((byte)(len >> 24));
			}
			Array.Copy(Vals, start, Code, CodeAt, len);
			CodeAt += len;
		}
		
		/// <summary>
		/// copy block from parsed values to code buffer
		/// </summary>
		protected void Copy(int top, int start)
		{
			var len = top - start;
			Need(len);
			Array.Copy(Vals, start, Code, CodeAt, len);
			CodeAt += len;
		}
		
		/// <summary>
		/// rewrite code from parsed value buffer/stack to code buffer
		/// </summary>
		protected virtual void Rewrite(int top, bool type = false)
		{
		full:
			var start = TopInt(top);
			top -= 4;
			var create = false;
		next:
			var op = ((Opcode)Vals[--top]).Extend();
			Debug.Assert(op.Kind() < Opkind.Statement);
			if (op.Kind() <= Opkind.Number)
			{
				if ((!type) && (!create))
				{
					Literal(op, top, start);
					return;
				}
				if (op == Opcode.Ident)
				{
					Copy(op, true, top, start);
					return;
				}
				Debug.Assert(((op > Opcode.Ident || op == Opcode.Undef) || op == Opcode.This) || op == Opcode.Null);
				Write(unchecked((byte)op));
				return;
			}
			Write(unchecked((byte)op));
			if (op.Unary())
			{
				create |= op == Opcode.Create;
				goto next;
			}
			if (op.Binary())
			{
				Rewrite(TopInt(top), type || create);
				if (op == Opcode.Dot)
				{
					start = TopInt(top);
					top -= 4;
					op = ((Opcode)Vals[--top]).Extend();
					if (op != Opcode.Ident)
					{
						throw new InvalidOperationException();
					}
					var len = top - start;
					if (len > 127)
					{
						throw new InvalidOperationException("Identifier too long");
					}
					Need(1 + len);
					Code[CodeAt++] = unchecked((byte)len);
					Array.Copy(Vals, start, Code, CodeAt, len);
					CodeAt += len;
					return;
				}
				if (op != Opcode.LogicAnd && op != Opcode.LogicOr)
				{
					goto full;
				}
				var second = CodeAt;
				Write(0);
				Rewrite(top);
				Write((CodeAt - second) - 4, second);
				return;
			}
			if (op.Ternary())
			{
				var mtop = TopInt(top);
				if (op == Opcode.Var)
				{
					Debug.Assert(!create);
					var varat = --CodeAt;
					Rewrite(TopInt(mtop));
					Code[varat] = unchecked((byte)Opcode.Var);
					Rewrite(mtop, true);
					goto full;
				}
				Rewrite(TopInt(mtop), type || create);
				if (op == Opcode.Ternary)
				{
					var second = CodeAt;
					Write(0);
					Rewrite(mtop);
					Write((CodeAt - second) - 4, second);
					var third = CodeAt;
					Write(0);
					Rewrite(top);
					Write((CodeAt - third) - 4, third);
					return;
				}
				Rewrite(mtop);
				goto full;
			}
			switch (op)
			{
			case Opcode.Array:
				type = false;
				create = true;
				break;
			case Opcode.Generic:
				type = true;
				break;
			default:
				Debug.Assert(op == Opcode.Mcall || op == Opcode.Mindex);
				break;
			}
			var n = Vals[--top];
			Write(unchecked((byte)n));
			Rewrite(top, n, type, create);
		}
		
		/// <summary>
		/// rewrite n-expressions/values from parsed value buffer/stack to code buffer
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
		/// rewrite literal from parsed value buffer/stack to code buffer
		/// </summary>
		protected virtual void Literal(Opcode op, int top, int start)
		{
			if (op < Opcode.Ident || op == Opcode.Exception)
			{
				Write(unchecked((byte)op));
				return;
			}
			Debug.Assert(((op == Opcode.Ident || op == Opcode.Number) || op == Opcode.String) || op == Opcode.Char);
			Copy(op, op != Opcode.String && op != Opcode.Char, top, start);
		}
		
		private string _debuggerDisplay_code
		{
			get
			{
				if (CodeAt == 0)
				{
					return System.String.Empty;
				}
				var sb = new StringBuilder();
				var i = 0;
				while (i < CodeAt && i < 32)
				{
					sb.AppendFormat("{0:X2}", Code[i++]);
				}
				if (i < CodeAt)
				{
					sb.Append("...");
				}
				return sb.ToString();
			}
		}
	}
}
