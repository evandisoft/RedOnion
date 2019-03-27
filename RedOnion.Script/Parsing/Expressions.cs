using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RedOnion.Script.Parsing
{
	partial class Parser
	{
		/// <summary>
		/// Stored state of values (top markers)
		/// </summary>
		protected struct ValuesState
		{
			public int ValuesAt { get; }
			public int StringValuesAt { get; }
			public ValuesState(int valuesAt, int stringValuesAt)
			{
				ValuesAt = valuesAt;
				StringValuesAt = stringValuesAt;
			}
		}

		/// <summary>
		/// Start expression parsing and save state of values
		/// </summary>
		/// <returns>Current state of values</returns>
		protected ValuesState StartExpression()
			=> new ValuesState(ValuesAt, StringValuesAt);

		/// <summary>
		/// Finish expression parsing and restore state of values
		/// </summary>
		/// <param name="state">Stored state of values from StartExpression()</param>
		/// <returns>True if there was any expression</returns>
		protected bool FinishExpression(ValuesState state)
		{
			if (state.ValuesAt >= ValuesAt)
			{
				Debug.Assert(state.ValuesAt == ValuesAt);
				Debug.Assert(state.StringValuesAt == StringValuesAt);
				Write(OpCode.Undefined);
				return false;
			}
			Rewrite(ValuesAt);
			ValuesAt = state.ValuesAt;
			StringValuesAt = state.StringValuesAt;
			return true;
		}

		/// <summary>
		/// Parse expression
		/// </summary>
		protected virtual bool ParseExpression(Flag flags = Flag.None)
		{
			var bottom = OperatorAt;
			bool wasBlock = false;
		unext:
			var unary = true;
		next:
			var code = lexer.Code;
			var kind = code.Kind();
			if (code == OpCode.Identifier   //------------------------------------------- identifier
				|| kind == OpKind.Number    // type specifier (byte, bool, int, ...)
				|| code == OpCode.String || code == OpCode.Char	// type names
				|| code.Code() == OpCode.Function.Code() // function or def
				|| code == OpCode.Object
				|| code == OpCode.Get || code == OpCode.Set)
			{
				if (!unary)
					goto autocall;
				if (lexer.Word.Length > 127)
					throw new ParseError(lexer, "Identifier name too long");
				if (code.Code() == OpCode.Function.Code())
				{
					var fnat = CodeAt;
					ParseFunction(null, flags);
					var start = ValuesAt;
					ValuesReserve(5+CodeAt-fnat);
					ValuesPush(Code, fnat, CodeAt-fnat);
					ValuesPush((byte)code);
					ValuesPush(start);
					CodeAt = fnat;
					while (OperatorAt > bottom)
						PrepareOperator(PopOperator());
					return true;
				}
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
				if (HasOption(Option.AutocallWhenArgs) && lexer.White && !lexer.PeekWhite)
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
					if (!unary)
						goto autocall;
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
						if (unary || HasOption(Option.DotThisAfterWhite))
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
					if ((lexer.Curr == ':' || lexer.Code == OpCode.As)
						&& Next().lexer.Word == null)
						throw new ParseError(lexer, "Expected variable type");
					ParseType(flags);
					wasBlock = false;
					if (lexer.Code == OpCode.Assign)
						wasBlock = Next().ParseExpression(flags);
					else
						Push(OpCode.Undefined);
					PrepareOperator(OpCode.Var);
					if (wasBlock)
						goto blockend;
					unary = false;
					goto next;
				case OpCode.Generic:    //----------------------------------- generic type or method
					if (unary)
						throw new ParseError(lexer, "Unexpected '.[' - nothing to specialize");
					if (Next().lexer.Curr != ']')
					{
						for (; ; )
						{
							PushOperator(OpCode.Comma);
							ParseType(flags &~Flag.LimitedContext);
							if (lexer.Curr == ']')
								break;
							if (lexer.Curr != ',')
								throw new ParseError(lexer, "Expected ',' or ']'");
							Next();
						}
					}
					PrepareOperator(OpCode.Generic);
					Next();
					unary = false;
					goto next;
				}
				break;

			case OpKind.Meta:		//######################################################## other
				Debug.Assert(code == OpCode.Unknown);
				switch (lexer.Curr)
				{
				case '(':			//------------------------------------------------------------ (
					if (unary || (HasOption(Option.AutocallWhenArgs) && lexer.White))
					{
						if (!unary)
							goto autocall;
						Next(true).ParseExpression(flags &~Flag.LimitedContext);
						if (lexer.Eol)
							Next(true);
						if (lexer.Curr != ')')
							throw new ParseError(lexer, "Expected matching ')'");
						Next();
						unary = false;
						goto next;
					}
					if (Next(true).lexer.Curr == ')')
					{
						PrepareOperator(OpCode.Call0);
						Next();
						unary = false;
						goto next;
					}

					ParseExpression(flags &~Flag.LimitedContext);

					if (lexer.Eol)
						Next(true);
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
						Next(true).ParseExpression(flags &~Flag.LimitedContext);
						if (lexer.Eol)
							Next(true);
					}
					while (lexer.Curr == ',');
					if (lexer.Eol)
						Next(true);
					if (lexer.Curr != ')')
						throw new ParseError(lexer, "Expected matching ')'");
					Next();
					PrepareOperator(OpCode.CallN);
					unary = false;
					goto next;
				case '[':           //------------------------------------------------------------ [
					if (unary)
					{
						if (!HasOption(Option.ArrayLiteral))
							throw new ParseError(lexer, "Unexpected '[' - nothing to index");
						Push(OpCode.Undefined);
						if (Next(true).lexer.Curr != ']')
						{
							for (; ; )
							{
								PushOperator(OpCode.Comma);
								ParseExpression(flags &~Flag.LimitedContext);
								if (lexer.Eol)
									Next(true);
								if (lexer.Curr == ']')
									break;
								if (lexer.Curr != ',')
									throw new ParseError(lexer, "Expected ',' or ']'");
								Next();
							}
						}
						PrepareOperator(OpCode.Array);
						Next(true);
						unary = false;
						goto next;
					}
					if (Next(true).lexer.Curr == ']')
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
						Next(true).ParseExpression(flags &~Flag.LimitedContext);
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
			return false;

		blockend:
			while (OperatorAt > bottom)
				PrepareOperator(PopOperator());
			return true;

		//################################################################################ auto call
		autocall:
			if (!HasOption(Option.AutocallWhenArgs))
				throw new ParseError(lexer, "Unexpected literal (autocall is disabled)");
			Debug.Assert(!unary);
			wasBlock = ParseExpression(flags | Flag.LimitedContext);
			if (lexer.Curr != ',')
			{
				PrepareOperator(OpCode.Call1);
				if (wasBlock)
					goto blockend;
				unary = false;
				goto next;
			}
			do
			{
				PushOperator(OpCode.Comma);
				wasBlock = Next().ParseExpression(flags | Flag.LimitedContext);
			}
			while (lexer.Curr == ',');
			PrepareOperator(OpCode.CallN);
			if (wasBlock)
				goto blockend;
			unary = false;
			goto next;
		}
	}
}
