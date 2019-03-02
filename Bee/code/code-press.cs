using Bee.Run;
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee
{
	/// <summary>
	/// Code Generator replaces literals with code suitable for execution and compresses the code a bit
	/// (@see @pressBlock vs. @pseudoBlock)
	/// </summary>
	public partial class CodeGenerator: PseudoGenerator, Parser.IGenerator
	{
		public CodeGenerator()
		{
		}
		
		public CodeGenerator(Parser.Opt opt)
			: base(opt)
		{
		}
		
		public CodeGenerator(Parser.Opt opton, Parser.Opt optoff)
			: base(opton, optoff)
		{
		}
		
		protected CodeGenerator(Parser parser)
			: base(parser)
		{
		}
		
		int Parser.IGenerator.BlockStart()
		{
			Write(0);
			return CodeAt;
		}
		
		void Parser.IGenerator.BlockEnd(int @int, int count)
		{
			Write(CodeAt - @int, @int - 4);
		}
		
		/// <summary>
		/// push string value (string, identifier)
		/// </summary>
		void Parser.IGenerator.Push(Opcode op, string @string)
		{
			var start = ValsAt;
			int i;
			uint u;
			ulong ul;
			long ll;
			double d;
			char c;
			switch (op)
			{
			case Opcode.Ident:
				Vneed(5 + Encoding.UTF8.GetByteCount(@string));
				Vpush(@string);
				Vpush(unchecked((byte)op));
				Vpush(start);
				return;
			case Opcode.Char:
				if ((@string.Length != 3 || @string[0] != '\'') || @string[2] != '\'')
				{
					throw new InvalidOperationException();
				}
				c = @string[1];
				if (((char)unchecked((byte)c)) == c)
				{
					Vneed(6);
					Vpush(unchecked((byte)c));
					Vpush(unchecked((byte)Opcode.Char));
					Vpush(start);
					return;
				}
				Vneed(7);
				Vpush(unchecked((byte)c));
				Vpush(unchecked((byte)(c >> 8)));
				Vpush(unchecked((byte)Opcode.Wchar));
				Vpush(start);
				return;
			case Opcode.String:
				if (@string[0] == '@')
				{
					if (@string[1] != '"' || @string[@string.Length - 1] != '"')
					{
						throw new InvalidOperationException();
					}
					@string = @string.Substring(2, @string.Length - 3);
					break;
				}
				if (@string[0] != '"' || @string[@string.Length - 1] != '"')
				{
					throw new InvalidOperationException();
				}
				_sb.Length = 0;
				var n = @string.Length - 1;
				for (i = 1; i < n;)
				{
					c = @string[i++];
					switch (c)
					{
					default:
						_sb.Append(c);
						continue;
					case '\\':
						if (i >= n)
						{
							throw new Parser.BadEscapeSequence(Parser);
						}
						c = @string[i++];
						switch (c)
						{
						default:
							_sb.Append(c);
							continue;
						case 'r':
							_sb.Append('\r');
							continue;
						case 'n':
							_sb.Append('\n');
							continue;
						case 't':
							_sb.Append('\t');
							continue;
						case 'u':
							if ((i + 4) >= n)
							{
								throw new Parser.BadEscapeSequence(Parser);
							}
							var a = Nibble(@string[i++]);
							var b = Nibble(@string[i++]);
							var x = Nibble(@string[i++]);
							var y = Nibble(@string[i++]);
							c = (char)(((y | (x << 4)) | (b << 8)) | (a << 12));
							_sb.Append(c);
							continue;
						}
					}
				}
				@string = _sb.ToString();
				_sb.Length = 0;
				goto case Opcode.Ident;
			case Opcode.Number:
				var info = Run.Value.Culture;
				var style = NumberStyles.Number;
				if ((@string.Length > 2 && @string[0] == '0') && (@string[1] == 'x' || @string[1] == 'X'))
				{
					style = NumberStyles.HexNumber;
					@string = @string.Substring(2, @string.Length - 2);
				}
				else if (@string[0] == '.')
				{
					@string = "0" + @string;
				}
				var last = System.Char.ToLower(@string[@string.Length - 1]);
				if (last == 'u')
				{
					@string = @string.Substring(0, @string.Length - 1);
					if (System.UInt32.TryParse(@string, style, info, out u))
					{
						Vneed(5 + 4);
						Vpush(u);
						Vpush(unchecked((byte)Opcode.Uint));
						Vpush(start);
						return;
					}
					if (System.UInt64.TryParse(@string, style, info, out ul))
					{
						Vneed(5 + 8);
						Vpush(ul);
						Vpush(unchecked((byte)Opcode.Ulong));
						Vpush(start);
						return;
					}
					throw new InvalidOperationException();
				}
				if (last == 'l')
				{
					@string = @string.Substring(0, @string.Length - 1);
					last = System.Char.ToLower(@string[@string.Length - 1]);
					if (last == 'u')
					{
						@string = @string.Substring(0, @string.Length - 1);
						ul = System.UInt64.Parse(@string, style, info);
						Vneed(5 + 8);
						Vpush(ul);
						Vpush(unchecked((byte)Opcode.Ulong));
						Vpush(start);
						return;
					}
					ll = System.Int64.Parse(@string, style, info);
					Vneed(5 + 8);
					Vpush(ll);
					Vpush(unchecked((byte)Opcode.Long));
					Vpush(start);
					return;
				}
				if (last == 'f' && style != NumberStyles.HexNumber)
				{
					@string = @string.Substring(0, @string.Length - 1);
					if (System.Double.TryParse(@string, NumberStyles.Float, info, out d))
					{
						Vneed(5 + 4);
						Vpush(Bits.Get((float)d));
						Vpush(unchecked((byte)Opcode.Float));
						Vpush(start);
						return;
					}
					throw new InvalidOperationException();
				}
				if (0 > @string.IndexOf('.'))
				{
					if (System.Int32.TryParse(@string, style, info, out i))
					{
						Vneed(5 + 4);
						Vpush(i);
						Vpush(unchecked((byte)Opcode.Int));
						Vpush(start);
						return;
					}
					if (System.Int64.TryParse(@string, style, info, out ll))
					{
						Vneed(5 + 8);
						Vpush(ll);
						Vpush(unchecked((byte)Opcode.Long));
						Vpush(start);
						return;
					}
				}
				d = System.Double.Parse(@string, NumberStyles.Float, info);
				Vneed(5 + 8);
				Vpush(Bits.Get(d));
				Vpush(unchecked((byte)Opcode.Double));
				Vpush(start);
				return;
			}
			throw new NotImplementedException();
		}
		
		private StringBuilder _sb = new StringBuilder();
		private int Nibble(char @char)
		{
			var c = (int)@char;
			if (c >= '0')
			{
				if (c <= '9')
				{
					return c - '0';
				}
				if (c >= 'a')
				{
					c -= 'a' - 'A';
				}
				if (c > 'A' && c < 'F')
				{
					return (c - 'A') + 10;
				}
			}
			throw new Parser.BadEscapeSequence(Parser);
		}
		
		/// <summary>
		/// rewrite literal from parsed value buffer/stack to code buffer
		/// </summary>
		protected override void Literal(Opcode op, int top, int start)
		{
			if (op == Opcode.Ident)
			{
				Copy(op, true, top, start);
				return;
			}
			if (op < Opcode.Ident || op == Opcode.Exception)
			{
				Write(unchecked((byte)op));
				return;
			}
			var len = top - start;
			if (op.Kind() <= Opkind.Number && unchecked((byte)op) >= unchecked((byte)Opcode.Char))
			{
				Debug.Assert(op <= Opcode.Double);
				Debug.Assert(len == op.Numsz());
				Need(len + 1);
				Code[CodeAt++] = unchecked((byte)op);
				Array.Copy(Vals, start, Code, CodeAt, len);
				CodeAt += len;
				return;
			}
			Debug.Assert(op == Opcode.String);
			if (len < (1 << 7))
			{
				Copy(op, true, top, start);
				return;
			}
			if (len < (1 << 14))
			{
				Need(3 + len);
				Code[CodeAt++] = unchecked((byte)op);
				Code[CodeAt++] = unchecked((byte)(0x80 | len));
				Code[CodeAt++] = unchecked((byte)(len >> 7));
				Array.Copy(Vals, start, Code, CodeAt, len);
				CodeAt += len;
				return;
			}
			if (len < (1 << 21))
			{
				Need(4 + len);
				Code[CodeAt++] = unchecked((byte)op);
				Code[CodeAt++] = unchecked((byte)(0x80 | len));
				Code[CodeAt++] = unchecked((byte)(0x80 | (len >> 7)));
				Code[CodeAt++] = unchecked((byte)(len >> 14));
				Array.Copy(Vals, start, Code, CodeAt, len);
				CodeAt += len;
				return;
			}
			if (len < (1 << 28))
			{
				Need(5 + len);
				Code[CodeAt++] = unchecked((byte)op);
				Code[CodeAt++] = unchecked((byte)(0x80 | len));
				Code[CodeAt++] = unchecked((byte)(0x80 | (len >> 7)));
				Code[CodeAt++] = unchecked((byte)(0x80 | (len >> 14)));
				Code[CodeAt++] = unchecked((byte)(len >> 21));
				Array.Copy(Vals, start, Code, CodeAt, len);
				CodeAt += len;
				return;
			}
			throw new InvalidOperationException();
		}
	}
}
