using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedOnion.Script.Execution;

namespace RedOnion.Script.Parsing
{
	partial class Parser
	{
		/// <summary>
		/// Parse expression
		/// </summary>
		protected virtual void ParseExpression(Flag flags = Flag.None)
		{
			/* TODO!!

			var bottom = OperatorAt;
		unext:
			var unary = true;
		next:
			var code = lexer.Code;
			var kind = code.Kind();
			if (code == OpCode.Identifier   //------------------------------------------- identifier
				|| kind == OpKind.Number    // type specifier (byte, bool, int, ...)
				|| code == OpCode.String || code == OpCode.Char	// type names
				|| code == OpCode.Function || code == OpCode.Object)
			{
				if (!unary)
					goto autocall;
				if (lexer.Word.Length > 127)
					throw new ParseError(lexer, "Identifier name too long");
				Push(OpCode.Identifier, lexer.Word);
				if (lexer.Peek == '!')
				{
					PushOperator(OpCode.Cast, bottom);
					lexer.Next().Next();
					goto unext;
				}
				lexer.Next();
				unary = false;
				goto next;
			}
			switch (kind)
			{
			case Opkind.Literal:
				switch (code)
				{
				default:
					Debug.Assert((((((code == OpCode.Undef || code == OpCode.Null) || code == OpCode.False) || code == OpCode.True) || code == OpCode.This) || code == OpCode.Base) || code == OpCode.Exception);
					if (!unary)
					{
						goto autocall;
					}
					Cgen.Push(code);
					Next();
					unary = false;
					goto next;
				case OpCode.Default:
					throw new ParseError(this, "TODO");
				case OpCode.Number:
					if (Instr)
					{
						throw new ParseError(this, "Unterminated string (or char) literal");
					}
					if (Curr == '"')
					{
						code = OpCode.String;
					}
					else if (Curr == '\'')
					{
						code = OpCode.Char;
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
				if (code != OpCode.Add && code != OpCode.Sub)
				{
					goto binary_check;
				}
				if (unary)
				{
					code = code == OpCode.Add ? OpCode.Plus : OpCode.Neg;
					if (Next().OpCode != OpCode.Number)
					{
						goto unary;
					}
					Cgen.Push(OpCode.Number, code.Text() + Rest());
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
				case OpCode.Inc:
					Op(OpCode.PostInc, bottom);
					Next();
					goto next;
				case OpCode.Dec:
					Op(OpCode.PostDec, bottom);
					Next();
					goto next;
				default:
					throw new ExpectedUnary(this);
				}
			case Opkind.Special:
				switch (code)
				{
				case OpCode.Create:
					CheckUnary(unary, true);
					Next();
					Op(code);
					Type(flags);
					unary = false;
					goto next;
				case OpCode.Dot:
					if (unary || White)
					{
						if ((unary && Peek >= '0') && Peek <= '9')
						{
							Cgen.Push(OpCode.Number, code.Text() + Next().Rest());
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
							Cgen.Push((flags & Flag.With) != 0 ? OpCode.Ivalue : OpCode.This);
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
					Cgen.Push(OpCode.Ident, Word);
					Cgen.Prepare(OpCode.Dot);
					if (Peek == '!')
					{
						Op(OpCode.Cast, bottom);
						Next().Next();
						goto unext;
					}
					Next();
					unary = false;
					goto next;
				case OpCode.Ternary:
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
					Cgen.Prepare(OpCode.Ternary);
					unary = false;
					goto next;
				case OpCode.Var:
					if (Next().Word == null)
					{
						throw new ParseError(this, "Expected variable name");
					}
					if (Word.Length > 127)
					{
						throw new ParseError(this, "Variable name too long");
					}
					Cgen.Push(OpCode.Ident, Word);
					Next();
					if (Curr == ':')
					{
						if (Next().Word == null)
						{
							throw new ParseError(this, "Expected variable type");
						}
					}
					Type(flags);
					if (OpCode == OpCode.Assign)
					{
						Next().Expression(flags);
					}
					else
					{
						Cgen.Push(OpCode.Undef);
					}
					Cgen.Prepare(OpCode.Var);
					unary = false;
					goto next;
				}
				break;
			case Opkind.Meta:
				Debug.Assert(code == OpCode.Unknown);
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
						Cgen.Prepare(OpCode.Ecall);
						Next();
						unary = false;
						goto next;
					}
					Expression(flags & (~Flag.Limit));
					if (Curr == ')')
					{
						Cgen.Prepare(OpCode.Call);
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
						Op(OpCode.Comma);
						Next().Expression(flags & (~Flag.Limit));
					}
					while (Curr == ',');
					if (Curr != ')')
					{
						throw new ParseError(this, "Expected matching ')'");
					}
					Next();
					Cgen.Prepare(OpCode.Mcall);
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
						Cgen.Prepare(OpCode.Ecall);
						Next();
						unary = false;
						goto next;
					}
					Expression(flags & (~Flag.Limit));
					if (Curr == ']')
					{
						Cgen.Prepare(OpCode.Index);
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
						Op(OpCode.Comma);
						Next().Expression(flags & (~Flag.Limit));
					}
					while (Curr == ',');
					if (Curr != ']')
					{
						throw new ParseError(this, "Expected matching ']'");
					}
					Next();
					Cgen.Prepare(OpCode.Mindex);
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
				Cgen.Prepare(OpCode.Call);
				unary = false;
				goto next;
			}
			do
			{
				Op(OpCode.Comma);
				Next().Expression(flags);
			}
			while (Curr == ',');
			Cgen.Prepare(OpCode.Mcall);
			unary = false;
			goto next;
			*/
		}
	}
}
