using Bee.Run;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee
{
	public partial class Parser
	{
		/// <summary>
		/// parse expression
		/// </summary>
		public virtual void Expression(Flag flags = Flag.None)
		{
			var bottom = OpsAt;
		unext:
			var unary = true;
		next:
			var code = Opcode;
			var kind = code.Kind();
			if (((((code == Opcode.Ident || kind == Opkind.Number) || code == Opcode.String) || code == Opcode.Char) || code == Opcode.Func) || code == Opcode.Object)
			{
				if (!unary)
				{
					goto autocall;
				}
				if (Word.Length > 127)
				{
					throw new ParseError(this, "Identifier name too long");
				}
				Cgen.Push(Opcode.Ident, Word);
				if (Peek == '!')
				{
					Op(Opcode.Cast, bottom);
					Next().Next();
					goto unext;
				}
				Next();
				unary = false;
				goto next;
			}
			switch (kind)
			{
			case Opkind.Literal:
				switch (code)
				{
				default:
					Debug.Assert((((((code == Opcode.Undef || code == Opcode.Null) || code == Opcode.False) || code == Opcode.True) || code == Opcode.This) || code == Opcode.Base) || code == Opcode.Exception);
					if (!unary)
					{
						goto autocall;
					}
					Cgen.Push(code);
					Next();
					unary = false;
					goto next;
				case Opcode.Default:
					throw new ParseError(this, "TODO");
				case Opcode.Number:
					if (Instr)
					{
						throw new ParseError(this, "Unterminated string (or char) literal");
					}
					if (Curr == '"')
					{
						code = Opcode.String;
					}
					else if (Curr == '\'')
					{
						code = Opcode.Char;
					}
					if (!unary)
					{
						goto autocall;
					}
					Cgen.Push(code, Rest());
					Next();
					unary = false;
					goto next;
				}
			case Opkind.Assign:
				CheckUnary(unary, false);
				Op(code);
				Next();
				goto unext;
			case Opkind.Binary:
				if (code != Opcode.Add && code != Opcode.Sub)
				{
					goto binary_check;
				}
				if (unary)
				{
					code = code == Opcode.Add ? Opcode.Plus : Opcode.Neg;
					if (Next().Opcode != Opcode.Number)
					{
						goto unary;
					}
					Cgen.Push(Opcode.Number, code.Text() + Rest());
					Next();
					unary = false;
					goto next;
				}
				if (White && (!PeekWhite))
				{
					goto autocall;
				}
				goto binary;
			case Opkind.Logic:
			binary_check:
				CheckUnary(unary, false);
			binary:
				Next();
				Op(code, bottom);
				goto unext;
			case Opkind.Unary:
				CheckUnary(unary, true);
				Next();
			unary:
				Op(code);
				goto next;
			case Opkind.Prepost:
				if (unary)
				{
					Op(code);
					Next();
					goto next;
				}
				switch (code)
				{
				case Opcode.Inc:
					Op(Opcode.PostInc, bottom);
					Next();
					goto next;
				case Opcode.Dec:
					Op(Opcode.PostDec, bottom);
					Next();
					goto next;
				default:
					throw new ExpectedUnary(this);
				}
			case Opkind.Special:
				switch (code)
				{
				case Opcode.Create:
					CheckUnary(unary, true);
					Next();
					Op(code);
					Type(flags);
					unary = false;
					goto next;
				case Opcode.Dot:
					if (unary || White)
					{
						if ((unary && Peek >= '0') && Peek <= '9')
						{
							Cgen.Push(Opcode.Number, code.Text() + Next().Rest());
							Next();
							unary = false;
							goto next;
						}
						if (unary || (Opts & Opt.DotThisAfterWhite) != 0)
						{
							if ((flags & (Flag.Static | Flag.With)) == Flag.Static)
							{
								throw new ParseError(this, "Dot-shortcut for 'this' not allowed in static method");
							}
							if (!unary)
							{
								goto autocall;
							}
							Cgen.Push((flags & Flag.With) != 0 ? Opcode.Ivalue : Opcode.This);
							unary = false;
						}
					}
					if (Next().Word == null)
					{
						throw new ParseError(this, "Expected word after '.'");
					}
					if (Word.Length > 127)
					{
						throw new ParseError(this, "Identifier name too long");
					}
					Cgen.Push(Opcode.Ident, Word);
					Cgen.Prepare(Opcode.Dot);
					if (Peek == '!')
					{
						Op(Opcode.Cast, bottom);
						Next().Next();
						goto unext;
					}
					Next();
					unary = false;
					goto next;
				case Opcode.Ternary:
					while (OpsAt > bottom)
					{
						Cgen.Prepare(Pop());
					}
					Next().Expression(flags & (~Flag.Limit));
					if (Eol)
					{
						NextLine();
					}
					if (Curr != ':')
					{
						throw new ParseError(this, "Expected matching ':' for ternary '?'");
					}
					Next().Expression(flags);
					Cgen.Prepare(Opcode.Ternary);
					unary = false;
					goto next;
				case Opcode.Var:
					if (Next().Word == null)
					{
						throw new ParseError(this, "Expected variable name");
					}
					if (Word.Length > 127)
					{
						throw new ParseError(this, "Variable name too long");
					}
					Cgen.Push(Opcode.Ident, Word);
					Next();
					if (Curr == ':')
					{
						if (Next().Word == null)
						{
							throw new ParseError(this, "Expected variable type");
						}
					}
					Type(flags);
					if (Opcode == Opcode.Assign)
					{
						Next().Expression(flags);
					}
					else
					{
						Cgen.Push(Opcode.Undef);
					}
					Cgen.Prepare(Opcode.Var);
					unary = false;
					goto next;
				}
				break;
			case Opkind.Meta:
				Debug.Assert(code == Opcode.Unknown);
				switch (Curr)
				{
				case '(':
					if (unary || White)
					{
						if (!unary)
						{
							goto autocall;
						}
						Next().Expression(flags & (~Flag.Limit));
						if (Curr != ')')
						{
							throw new ParseError(this, "Expected matching ')'");
						}
						Next();
						unary = false;
						goto next;
					}
					if (Next().Curr == ')')
					{
						Cgen.Prepare(Opcode.Ecall);
						Next();
						unary = false;
						goto next;
					}
					Expression(flags & (~Flag.Limit));
					if (Curr == ')')
					{
						Cgen.Prepare(Opcode.Call);
						Next();
						unary = false;
						goto next;
					}
					if (Curr != ',')
					{
						throw new ParseError(this, "Expected ',' or ')'");
					}
					do
					{
						Op(Opcode.Comma);
						Next().Expression(flags & (~Flag.Limit));
					}
					while (Curr == ',');
					if (Curr != ')')
					{
						throw new ParseError(this, "Expected matching ')'");
					}
					Next();
					Cgen.Prepare(Opcode.Mcall);
					unary = false;
					goto next;
				case '[':
					if (unary)
					{
						if ((Opts & Opt.BracketsAsParens) == 0)
						{
							throw new ParseError(this, "Unexpected '[' - nothing to index");
						}
						Next().Expression(flags & (~Flag.Limit));
						if (Curr != ']')
						{
							throw new ParseError(this, "Expected matching ']'");
						}
						Next();
						unary = false;
						goto next;
					}
					if (Next().Curr == ']')
					{
						if ((Opts & Opt.BracketsAsParens) == 0)
						{
							throw new ParseError(this, "Unexpected '[' - nothing to index");
						}
						Cgen.Prepare(Opcode.Ecall);
						Next();
						unary = false;
						goto next;
					}
					Expression(flags & (~Flag.Limit));
					if (Curr == ']')
					{
						Cgen.Prepare(Opcode.Index);
						Next();
						unary = false;
						goto next;
					}
					if (Curr != ',')
					{
						throw new ParseError(this, "Expected ',' or ']'");
					}
					do
					{
						Op(Opcode.Comma);
						Next().Expression(flags & (~Flag.Limit));
					}
					while (Curr == ',');
					if (Curr != ']')
					{
						throw new ParseError(this, "Expected matching ']'");
					}
					Next();
					Cgen.Prepare(Opcode.Mindex);
					unary = false;
					goto next;
				default:
					goto done;
				}
			default:
				if (code.Kind() >= Opkind.Statement)
				{
					goto done;
				}
				break;
			}
			throw new ParseError(this, "Unrecognised token: {0} / {1}", code, Word ?? Curr.ToString());
		done:
			if (unary)
			{
				if (Eol)
				{
					NextLine();
					goto next;
				}
				throw new ExpectedBinary(this);
			}
			while (OpsAt > bottom)
			{
				Cgen.Prepare(Pop());
			}
			return;
		autocall:
			Debug.Assert(!unary);
			Expression(flags);
			if (Curr != ',')
			{
				Cgen.Prepare(Opcode.Call);
				unary = false;
				goto next;
			}
			do
			{
				Op(Opcode.Comma);
				Next().Expression(flags);
			}
			while (Curr == ',');
			Cgen.Prepare(Opcode.Mcall);
			unary = false;
			goto next;
		}
	}
}
