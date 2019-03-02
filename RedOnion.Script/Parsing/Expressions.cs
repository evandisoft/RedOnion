using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
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
					Next().Next();
					goto unext;
				}
				Next();
				unary = false;
				goto next;
			}
			switch (kind)
			{
			case OpKind.Literal:	//###################################################### literal
				switch (code)
				{
				default:            //---------------------------------------------- null, this, ...
					Debug.Assert(code == OpCode.Undefined || code == OpCode.Null
						|| code == OpCode.False || code == OpCode.True
						|| code == OpCode.This || code == OpCode.Base
						|| code == OpCode.Exception);
					if (!unary)
						goto autocall;
					Push(code);
					Next();
					unary = false;
					goto next;
				case OpCode.Default://------------------------------------------------------ default
					throw new ParseError(lexer, "TODO");
				case OpCode.Number: //--------------------------------------- number, string or char
					if (lexer.InString)
						throw new ParseError(lexer, "Unterminated string (or char) literal");
					if (lexer.Curr == '"')
						code = OpCode.String;
					else if (lexer.Curr == '\'')
						code = OpCode.Char;
					if (!unary)
						goto autocall;
					Push(code, lexer.Rest());
					Next();
					unary = false;
					goto next;
				}

			case OpKind.Assign:		//##################################################### operator
				CheckUnary(unary, false);
				PushOperator(code);
				Next();
				goto unext;
			case OpKind.Binary:     //------------------------------------------------------- binary
				if (code != OpCode.Add && code != OpCode.Sub)
					goto binary_check;
				if (unary)
				{
					code = code == OpCode.Add ? OpCode.Plus : OpCode.Neg;
					if (Next().lexer.Code != OpCode.Number)
						goto unary;
					Push(OpCode.Number, code.Text() + lexer.Rest());
					Next();
					unary = false;
					goto next;
				}
				if (lexer.White && !lexer.PeekWhite)
					goto autocall;
				goto binary;
			case OpKind.Logic:
			binary_check:
				CheckUnary(unary, false);
			binary:
				Next();
				PushOperator(code, bottom);
				goto unext;
			case OpKind.Unary:      //-------------------------------------------------------- unary
				CheckUnary(unary, true);
				Next();
			unary:
				PushOperator(code);
				goto next;
			case OpKind.PreOrPost:
				if (unary)
				{
					PushOperator(code);
					Next();
					goto next;
				}
				switch (code)
				{
				case OpCode.Inc:
					PushOperator(OpCode.PostInc, bottom);
					Next();
					goto next;
				case OpCode.Dec:
					PushOperator(OpCode.PostDec, bottom);
					Next();
					goto next;
				default:
					throw new ExpectedUnary(lexer);
				}

			case OpKind.Special:	//###################################################### special
				switch (code)
				{
				case OpCode.Create:
					CheckUnary(unary, true);
					Next();
					PushOperator(code);
					ParseType(flags);
					unary = false;
					goto next;
				case OpCode.Dot:
					if (unary || lexer.White)
					{
						if (unary && lexer.Peek >= '0' && lexer.Peek <= '9')
						{
							Push(OpCode.Number, code.Text() + Next().lexer.Rest());
							Next();
							unary = false;
							goto next;
						}
						if (unary || (Options & Option.DotThisAfterWhite) != 0)
						{
							if ((flags & (Flag.Static | Flag.With)) == Flag.Static)
								throw new ParseError(lexer, "Dot-shortcut for 'this' not allowed in static method");
							if (!unary)
								goto autocall;
							Push((flags & Flag.With) != 0 ? OpCode.Implicit : OpCode.This);
							unary = false;
						}
					}
					if (Next().lexer.Word == null)
						throw new ParseError(lexer, "Expected word after '.'");
					if (lexer.Word.Length > 127)
						throw new ParseError(lexer, "Identifier name too long");
					Push(OpCode.Identifier, lexer.Word);
					PrepareOperator(OpCode.Dot);
					if (lexer.Peek == '!')
					{
						PushOperator(OpCode.Cast, bottom);
						Next().Next();
						goto unext;
					}
					Next();
					unary = false;
					goto next;
				case OpCode.Ternary://--------------------------------------------------- ternary ?:
					while (OperatorAt > bottom)
						PrepareOperator(PopOperator());
					Next().ParseExpression(flags &~Flag.LimitedContext);
					if (lexer.Eol)
						lexer.NextLine();
					if (lexer.Curr != ':')
						throw new ParseError(lexer, "Expected matching ':' for ternary '?'");
					Next().ParseExpression(flags);
					PrepareOperator(OpCode.Ternary);
					unary = false;
					goto next;
				case OpCode.Var:	//----------------------------------------- variable declaration
					if (Next().lexer.Word == null)
						throw new ParseError(lexer, "Expected variable name");
					if (lexer.Word.Length > 127)
						throw new ParseError(lexer, "Variable name too long");
					Push(OpCode.Identifier, lexer.Word);
					Next();
					if (lexer.Curr == ':' && Next().lexer.Word == null)
						throw new ParseError(lexer, "Expected variable type");
					ParseType(flags);
					if (lexer.Code == OpCode.Assign)
						Next().ParseExpression(flags);
					else
						Push(OpCode.Undefined);
					PrepareOperator(OpCode.Var);
					unary = false;
					goto next;
				}
				break;

			case OpKind.Meta:		//######################################################## other
				Debug.Assert(code == OpCode.Unknown);
				switch (lexer.Curr)
				{
				case '(':			//------------------------------------------------------------ (
					if (unary || lexer.White)
					{
						if (!unary)
							goto autocall;
						Next().ParseExpression(flags &~Flag.LimitedContext);
						if (lexer.Curr != ')')
							throw new ParseError(lexer, "Expected matching ')'");
						Next();
						unary = false;
						goto next;
					}
					if (Next().lexer.Curr == ')')
					{
						PrepareOperator(OpCode.Call0);
						Next();
						unary = false;
						goto next;
					}

					ParseExpression(flags &~Flag.LimitedContext);

					if (lexer.Curr == ')')
					{
						PrepareOperator(OpCode.Call1);
						Next();
						unary = false;
						goto next;
					}
					if (lexer.Curr != ',')
						throw new ParseError(lexer, "Expected ',' or ')'");
					do
					{
						PushOperator(OpCode.Comma);
						Next().ParseExpression(flags &~Flag.LimitedContext);
					}
					while (lexer.Curr == ',');
					if (lexer.Curr != ')')
						throw new ParseError(lexer, "Expected matching ')'");
					Next();
					PrepareOperator(OpCode.CallN);
					unary = false;
					goto next;
				case '[':			//------------------------------------------------------------ [
					if (unary)
						throw new ParseError(lexer, "Unexpected '[' - nothing to index");
					if (Next().lexer.Curr == ']')
						throw new ParseError(lexer, "Unexpected ']' - missing index");

					ParseExpression(flags &~Flag.LimitedContext);

					if (lexer.Curr == ']')
					{
						PrepareOperator(OpCode.Index);
						Next();
						unary = false;
						goto next;
					}
					if (lexer.Curr != ',')
						throw new ParseError(lexer, "Expected ',' or ']'");
					do
					{
						PushOperator(OpCode.Comma);
						Next().ParseExpression(flags &~Flag.LimitedContext);
					}
					while (lexer.Curr == ',');
					if (lexer.Curr != ']')
						throw new ParseError(lexer, "Expected matching ']'");
					Next();
					PrepareOperator(OpCode.IndexN);
					unary = false;
					goto next;
				default:
					goto done;
				}
			//################################################################################# TAIL
			default:
				if (code.Kind() >= OpKind.Statement)
					goto done;
				break;
			}
			throw new ParseError(lexer, "Unrecognised token: {0} / {1}",
				code, lexer.Word ?? lexer.Curr.ToString());

		done:
			if (unary)
			{
				if (lexer.Eol)
				{
					lexer.NextLine();
					goto next;
				}
				throw new ExpectedBinary(lexer);
			}
			while (OperatorAt > bottom)
				PrepareOperator(PopOperator());
			return;

		//################################################################################ auto call
		autocall:
			Debug.Assert(!unary);
			ParseExpression(flags);
			if (lexer.Curr != ',')
			{
				PrepareOperator(OpCode.Call1);
				unary = false;
				goto next;
			}
			do
			{
				PushOperator(OpCode.Comma);
				Next().ParseExpression(flags);
			}
			while (lexer.Curr == ',');
			PrepareOperator(OpCode.CallN);
			unary = false;
			goto next;
		}
	}
}
