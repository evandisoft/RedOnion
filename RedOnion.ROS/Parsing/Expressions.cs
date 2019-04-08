using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RedOnion.ROS.Parsing
{
	partial class Parser
	{
		/// <summary>
		/// Stored state of values (top markers)
		/// </summary>
		protected struct ValuesState
		{
			public int values { get; }
			public int strings { get; }
			public ValuesState(int values, int strings)
			{
				this.values = values;
				this.strings = strings;
			}
		}

		/// <summary>
		/// Start expression parsing and save state of values
		/// </summary>
		/// <returns>Current state of values</returns>
		protected ValuesState StartExpression()
			=> new ValuesState(values.size, stringValues.size);

		/// <summary>
		/// Finish expression parsing and restore state of values
		/// </summary>
		/// <param name="state">Stored state of values from StartExpression()</param>
		/// <returns>True if there was any expression</returns>
		protected OpCode FinishExpression(ValuesState state)
		{
			if (state.values >= values.size)
			{
				Debug.Assert(state.values == values.size);
				Debug.Assert(state.strings == stringValues.size);
				Write(OpCode.Void);
				return OpCode.Void;
			}
			var op = Rewrite(values.size);
			values.size = state.values;
			stringValues.size = state.strings;
			return op;
		}

		/// <summary>
		/// Parse expression (returns true if block was parsed)
		/// </summary>
		protected virtual bool ParseExpression(Flag flags = Flag.None)
		{
			var bottom = operators.size;
			bool wasBlock = false;
		unext:
			var unary = true;
		next:
			var op = ExCode;
			var kind = op.Kind();
			if (op == ExCode.Identifier   //------------------------------------------- identifier
				|| kind == OpKind.Number    // type specifier (byte, bool, int, ...)
				|| op == ExCode.String || op == ExCode.Char	// type names
				|| op.Code() == ExCode.Function.Code() // function or def
				|| op == ExCode.Get || op == ExCode.Set)
			{
				if (!unary)
					goto autocall;
				if (Word.Length > 127)
					throw new ParseError(this, "Identifier name too long");
				if (op.Code() == OpCode.Function.Code())
				{
					var fnat = code.size;
					ParseFunction(null, flags);
					var start = values.size;
					ValuesReserve(5+code.size-fnat);
					ValuesPush(code.items, fnat, code.size-fnat);
					ValuesPush((byte)op);
					ValuesPush(start);
					code.size = fnat;
					while (operators.size > bottom)
						PrepareOperator(PopOperator());
					return true;
				}
				Push(ExCode.Identifier, Word);
				if (Peek == '!')
				{
					PushOperator(ExCode.Cast, bottom);
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
				switch (op)
				{
				default:            //---------------------------------------------- null, this, ...
					Debug.Assert(op == ExCode.Null
						|| op == ExCode.False || op == ExCode.True
						|| op == ExCode.This || op == ExCode.Base
						|| op == ExCode.Exception);
					if (!unary)
						goto autocall;
					Push(op);
					Next();
					unary = false;
					goto next;
				case ExCode.Default://------------------------------------------------------ default
					throw new ParseError(this, "TODO");
				case ExCode.Number: //--------------------------------------- number, string or char
					if (InString)
						throw new ParseError(this, "Unterminated string (or char) literal");
					if (Curr == '"')
						op = ExCode.String;
					else if (Curr == '\'')
						op = ExCode.Char;
					if (!unary)
						goto autocall;
					Push(op, Rest());
					Next();
					unary = false;
					goto next;
				}

			case OpKind.Assign:		//##################################################### operator
				CheckUnary(unary, false);
				PushOperator(op);
				Next();
				goto unext;
			case OpKind.Binary:     //------------------------------------------------------- binary
				if (op != ExCode.Add && op != ExCode.Sub)
					goto binary_check;
				if (unary)
				{
					op = op == ExCode.Add ? ExCode.Plus : ExCode.Neg;
					if (Next().ExCode != ExCode.Number)
						goto unary;
					Push(ExCode.Number, op.Text() + Rest());
					Next();
					unary = false;
					goto next;
				}
				if (HasOption(Option.AutocallWhenArgs) && White && !PeekWhite)
					goto autocall;
				goto binary;
			case OpKind.Logic:
			binary_check:
				CheckUnary(unary, false);
			binary:
				Next();
				PushOperator(op, bottom);
				goto unext;
			case OpKind.Unary:      //-------------------------------------------------------- unary
				CheckUnary(unary, true);
				Next();
			unary:
				PushOperator(op);
				goto next;
			case OpKind.PreOrPost:
				if (unary)
				{
					PushOperator(op);
					Next();
					goto next;
				}
				switch (op)
				{
				case ExCode.Inc:
					PushOperator(ExCode.PostInc, bottom);
					Next();
					goto next;
				case ExCode.Dec:
					PushOperator(ExCode.PostDec, bottom);
					Next();
					goto next;
				default:
					throw new ExpectedUnary(this);
				}

			case OpKind.Special:	//###################################################### special
				switch (op)
				{
				case ExCode.Create:
					if (!unary)
						goto autocall;
					Next();
					PushOperator(op);
					ParseType(flags);
					unary = false;
					goto next;
				case ExCode.Dot:
					if (unary || this.White)
					{
						if (unary && this.Peek >= '0' && this.Peek <= '9')
						{
							Push(ExCode.Number, op.Text() + Next().Rest());
							Next();
							unary = false;
							goto next;
						}
						if (unary)
						{
							Push((flags & Flag.With) != 0 ? ExCode.Implicit : ExCode.This);
							unary = false;
						}
					}
					if (Next().Word == null)
						throw new ParseError(this, "Expected word after '.'");
					if (Word.Length > 127)
						throw new ParseError(this, "Identifier name too long");
					Push(ExCode.Identifier, Word);
					PrepareOperator(ExCode.Dot);
					if (this.Peek == '!')
					{
						PushOperator(ExCode.Cast, bottom);
						Next().Next();
						goto unext;
					}
					Next();
					unary = false;
					goto next;
				case ExCode.Ternary://--------------------------------------------------- ternary ?:
					while (operators.size > bottom)
						PrepareOperator(PopOperator());
					Next().ParseExpression(flags &~Flag.Limited);
					if (Eol)
						NextLine();
					if (Curr != ':')
						throw new ParseError(this, "Expected matching ':' for ternary '?'");
					Next().ParseExpression(flags);
					PrepareOperator(ExCode.Ternary);
					unary = false;
					goto next;
				case ExCode.Var:	//----------------------------------------- variable declaration
					if (Next().Word == null)
						throw new ParseError(this, "Expected variable name");
					if (Word.Length > 127)
						throw new ParseError(this, "Variable name too long");
					Push(ExCode.Identifier, this.Word);
					Next();
					if ((Curr == ':' || ExCode == ExCode.As)
						&& Next().Word == null)
						throw new ParseError(this, "Expected variable type");
					ParseType(flags);
					wasBlock = false;
					if (ExCode == ExCode.Assign)
						wasBlock = Next().ParseExpression(flags);
					else
						Push(ExCode.Void);
					PrepareOperator(ExCode.Var);
					if (wasBlock)
						goto blockend;
					unary = false;
					goto next;
				}
				break;

			case OpKind.Meta:		//######################################################## other
				Debug.Assert(op == ExCode.Unknown);
				switch (Curr)
				{
				case '(':			//------------------------------------------------------------ (
					if (unary || (HasOption(Option.AutocallWhenArgs) && White))
					{
						if (!unary)
							goto autocall;
						Next(true).ParseExpression((flags &~Flag.Limited) | Flag.Hungry);
						if (Curr != ')')
							throw new ParseError(this, "Expected matching ')'");
						Next();
						unary = false;
						goto next;
					}
					if (Next(true).Curr == ')')
					{
						PrepareOperator(ExCode.Call0);
						Next();
						unary = false;
						goto next;
					}

					ParseExpression((flags &~Flag.Limited) | Flag.Hungry);

					if (Curr == ')')
					{
						PrepareOperator(ExCode.Call1);
						Next();
						unary = false;
						goto next;
					}
					if (Curr != ',')
						throw new ParseError(this, "Expected ',' or ')'");
					do
					{
						PushOperator(ExCode.Comma);
						Next(true).ParseExpression((flags &~Flag.Limited) | Flag.Hungry);
					}
					while (Curr == ',');
					if (Eol)
						Next(true);
					if (Curr != ')')
						throw new ParseError(this, "Expected matching ')'");
					Next();
					PrepareOperator(ExCode.CallN);
					unary = false;
					goto next;
				case '[':           //------------------------------------------------------------ [
					if (unary)
					{
						if (!HasOption(Option.ArrayLiteral))
							throw new ParseError(this, "Unexpected '[' - nothing to index");
						Push(ExCode.Void);
						if (Next(true).Curr != ']')
						{
							for (; ; )
							{
								PushOperator(ExCode.Comma);
								ParseExpression((flags &~Flag.Limited) | Flag.Hungry);
								if (Curr == ']')
									break;
								if (Curr != ',')
									throw new ParseError(this, "Expected ',' or ']'");
								Next();
							}
						}
						PrepareOperator(ExCode.Array);
						Next(true);
						unary = false;
						goto next;
					}
					if (Next(true).Curr == ']')
						throw new ParseError(this, "Unexpected ']' - missing index");

					ParseExpression((flags &~Flag.Limited) | Flag.Hungry);

					if (Curr == ']')
					{
						PrepareOperator(ExCode.Index);
						Next();
						unary = false;
						goto next;
					}
					if (Curr != ',')
						throw new ParseError(this, "Expected ',' or ']'");
					do
					{
						PushOperator(ExCode.Comma);
						Next(true).ParseExpression((flags &~Flag.Limited) | Flag.Hungry);
					}
					while (Curr == ',');
					if (Curr != ']')
						throw new ParseError(this, "Expected matching ']'");
					Next();
					PrepareOperator(ExCode.IndexN);
					unary = false;
					goto next;
				default:
					if (Eol && (flags & Flag.Hungry) != 0)
					{
						Next(true);
						goto next;
					}
					goto done;
				}
			//################################################################################# TAIL
			default:
				if (op.Kind() >= OpKind.Statement)
					goto done;
				break;
			}
			throw new ParseError(this, "Unrecognised token: {0} / {1}",
				op, Word ?? Curr.ToString());

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
			while (operators.size > bottom)
				PrepareOperator(PopOperator());
			return false;

		blockend:
			while (operators.size > bottom)
				PrepareOperator(PopOperator());
			return true;

		//################################################################################ auto call
		autocall:
			if (!HasOption(Option.AutocallWhenArgs))
				throw new ParseError(this, "Unexpected literal (autocall is disabled)");
			Debug.Assert(!unary);
			wasBlock = ParseExpression(flags | Flag.Limited);
			if (this.Curr != ',')
			{
				PrepareOperator(ExCode.Call1);
				if (wasBlock)
					goto blockend;
				unary = false;
				goto next;
			}
			do
			{
				PushOperator(ExCode.Comma);
				wasBlock = Next().ParseExpression(flags | Flag.Limited);
			}
			while (this.Curr == ',');
			PrepareOperator(ExCode.CallN);
			if (wasBlock)
				goto blockend;
			unary = false;
			goto next;
		}
	}
}
