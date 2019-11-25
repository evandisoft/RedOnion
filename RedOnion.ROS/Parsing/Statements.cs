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
		/// Test if there is expression at current position
		/// </summary>
		protected virtual bool PeekExpression(Flag flags)
			=> !Eol
			&& Curr != ';'
			&& Curr != ','
			&& Curr != ':'
			&&(ExCode.Kind() <= OpKind.Number
			|| ExCode.Kind() == OpKind.Unary || ExCode.Kind() == OpKind.PreOrPost
			|| ExCode == ExCode.Add || ExCode == ExCode.Sub
			|| ExCode == ExCode.Var || ExCode == ExCode.Create
			|| ExCode == ExCode.Def || ExCode == ExCode.Function
			|| Curr == '(' || Curr == '[' || Curr == '{');

		/// <summary>
		/// Full/required expression (e.g. condition of while).
		/// Returns root opcode (OpCode.Void if there was not expression)
		/// </summary>
		protected OpCode FullExpression(Flag flags)
		{
			var state = StartExpression();
			if ((flags & Flag.NoExpression) == 0 || PeekExpression(flags))
				ParseExpression(flags &~Flag.NoExpression);
			return FinishExpression(state);
		}
		/// <summary>
		/// Optional expression (e.g. argument in function declaration)
		/// Returns root opcode (OpCode.Void if there was not expression)
		/// </summary>
		protected OpCode OptionalExpression(Flag flags)
		{
			var state = StartExpression();
			if (ExCode == ExCode.Assign)
				Next(true);
			else if (!PeekExpression(flags))
				goto skip;
			ParseExpression(flags &~Flag.NoExpression);
		skip:
			return FinishExpression(state);
		}

		/// <summary>
		/// Full/required type reference (e.g. after new)
		/// Returns root opcode (OpCode.Void if there was not expression)
		/// </summary>
		protected OpCode FullType(Flag flags)
		{
			var state = StartExpression();
			ParseType(flags);
			return FinishExpression(state, type: true);
		}
		/// <summary>
		/// Optional type reference (e.g. in var)
		/// Returns root opcode (OpCode.Void if there was not expression)
		/// </summary>
		protected OpCode OptionalType(Flag flags)
		{
			if ((Options & (Option.Typed | Option.Untyped)) == Option.Untyped)
				return OpCode.Void;
			var state = StartExpression();
			if (Curr == ':' || ExCode == ExCode.As)
			{
				if (Next(true).Word == null)
					throw new ParseError(this, "Expected type reference");
			}
			else if ((Options & (Option.Typed | Option.Untyped)) == (Option.Typed | Option.Untyped))
			{
				//NOTE: combining both typed and untyped options means
				//..... that you have to use ':' or 'as' to specify type
				goto skip;
			}
			ParseType(flags);
		skip:
			return FinishExpression(state);
		}

		/// <summary>
		/// Parse block of statements.
		/// Returns number of statements parsed
		/// </summary>
		protected virtual int ParseBlock(Flag flags, int ind /* = -1 */)
		{
			/*if (ind < 0)
				ind = Indent;*/
			if (ind == 0 && First && (flags & Flag.Limited) == 0)
				ind = -1;
			var nosize = (flags & Flag.NoSize) != 0;
			var member = (flags & Flag.Member) != 0;
			flags &= ~(Flag.NoSize | Flag.Member);
			var block = 0;
			var count = 0;
			if (!nosize)
			{
				Write(0);
				block = code.size;
			}
			while (!Eof && Curr != ')' && Curr != '}' && Curr != ']')
			{
				while (Indent >= ind && Peek == ':' && ExCode == ExCode.Identifier)
				{
					Label(flags);
					count++;
				}
				if (First && Indent <= ind)
					break;
				if (Eol)
				{
					NextLine();
					if (count == 0 && member && (ExCode.Code() == ExCode.Property.Code()
						|| ExCode.TypeFlags() != 0) && ParseProperty(flags))
						break;
					continue;
				}
				while (Curr == ';')
					Next();
				if (ExCode == ExCode.Catch || ExCode == ExCode.Finally
					|| ExCode == ExCode.Case || ExCode == ExCode.Default)
					break;
				if ((flags & Flag.WasDo) != 0 && (ExCode == ExCode.While || ExCode == ExCode.Until))
					break;
				if ((flags & Flag.WasIf) != 0 && ExCode == ExCode.Else)
					break;
				ParseStatement(flags);
				count++;
			}
			if (!nosize)
				Write(code.size - block, block - 4);
			return count;
		}

		protected virtual void ParseStatement(Flag flags)
		{
			int mark;
			var ind = Indent;
			var op = ExCode;
			switch (op)
			{
			default:
				if (Word != null && Peek == ':')
				{
					Label(flags);
					return;
				}
				var exprStart = code.size;
				var root = FullExpression(flags);
				if (HasOption(Option.AutocallSimple) && code.size > exprStart)
				{
					if (HasOption(Option.Prefix))
					{
						root = ((OpCode)code.items[exprStart]);
						if (root == OpCode.Dot || root.Kind() <= OpKind.Number)
						{
							code.EnsureCapacity(code.size+1);
							Array.Copy(code.items, exprStart, code.items, exprStart+1, code.size-exprStart);
							code.items[exprStart] = OpCode.Autocall.Code();
							code.size++;
						}
					}
					else
					{
						if (root == OpCode.Dot || root.Kind() <= OpKind.Number)
							Write(OpCode.Autocall);
						Write(OpCode.Pop);
					}
				}
				return;
			case ExCode.Goto:
				Next();
				if (ExCode == ExCode.Case)
				{
					Write(OpCode.GotoCase);
					Next().FullExpression(flags | Flag.Limited);
					return;
				}
				if (Word == null)
					throw new ParseError(this, "Expected label after goto (or case)");
				if (Word.Length > 127)
					throw new ParseError(this, "Label too long");
				Write(OpCode.Goto);
				if (gotoTable == null)
					gotoTable = new Dictionary<int, string>();
				gotoTable[code.size] = Word;
				return;
			case ExCode.Return:
			case ExCode.Raise:
				if (HasOption(Option.Prefix)) Write(op);
				Next().FullExpression(flags | Flag.NoExpression);
				if (!HasOption(Option.Prefix)) Write(op);
				return;
			case ExCode.Break:
			case ExCode.Continue:
				Write(ExCode);
				Next();
				return;
			case ExCode.If:
			case ExCode.Unless:
			{
				if (HasOption(Option.Prefix)) Write(op);
				Next().FullExpression(flags | Flag.Limited);
				if (!HasOption(Option.Prefix)) Write(op);
				if (Curr == ';' || Curr == ':' || ExCode == ExCode.Then)
					Next();
				ParseBlock(flags | Flag.WasIf, ind);
				if (ExCode == ExCode.Else && Indent == ind)
				{
					Write(ExCode);
					if (Next().Curr == ':')
						Next();
					ParseBlock(flags, ind);
				}
				return;
			}
			case ExCode.Else:
				throw new ParseError(this, "Unexpected 'else'");

			// prefix:  while/until; cond size; cond; block size; block
			// postfix: while/until; cond size; cond + marker; block size; block
			case ExCode.While:
			case ExCode.Until:
			{
				Write(op);
				Write(0);
				var condAt = code.size;
				Next().FullExpression(flags | Flag.Limited);
				if (!HasOption(Option.Prefix))
					Write(OpCode.Cond);
				Write(code.size-condAt, condAt-4);
				if (ExCode == ExCode.Do)
					Next();
				if (Curr == ';' || Curr == ':')
					Next();
				ParseBlock(flags, ind);
				return;
			}
			// do; cond size; block size; block; cond
			case ExCode.Do:
			{
				var doAt = Write(op);
				Write(0);
				Next().ParseBlock(flags | Flag.WasDo, ind);
				if (ExCode != ExCode.While || Indent != ind)
				{
					if (ExCode != ExCode.Until || Indent != ind)
						throw new ParseError(this, "Expected 'while' or 'until' for 'do'");
					code.items[doAt] = ExCode.DoUntil.Code();
				}
				var condAt = code.size;
				Next().FullExpression(flags);
				Write(code.size-condAt, doAt+1);
				return;
			}
			// for; init size; test size; init; test; last size; last; block size; block
			case ExCode.For:
			{
				mark = Write(ExCode);

				Write(0); // size of init expression
				Write(0); // size of test expression

				// init expression
				var iniAt = code.size;
				Next().FullExpression(flags | Flag.Limited | Flag.NoExpression | Flag.WasFor);
				if (Curr == ':' || ExCode == ExCode.In)
				{
					Write(code.size - iniAt, mark + 1);
					code.items[mark] = ExCode.ForEach.Code();
					Next();
					goto for_in;
				}
				if (!HasOption(Option.Prefix))
					Write(OpCode.Pop);
				Write(code.size - iniAt, mark + 1);
				if (Curr == ';')
					Next();

				// test expression
				var testAt = code.size;
				var test = FullExpression(flags | Flag.Limited | Flag.NoExpression);
				if (!HasOption(Option.Prefix))
				{
					if (test == OpCode.Void && code.size == testAt + 1)
						Write(OpCode.True, testAt);
					Write(OpCode.Cond);
				}
				Write(code.size-testAt, mark+5);
				if (Curr == ';')
					Next();

				// last expression
				Write(0);
				var lastAt = code.size;
				FullExpression(flags | Flag.Limited | Flag.NoExpression);
				if (!HasOption(Option.Prefix))
					Write(OpCode.Pop);
				Write(code.size-lastAt, lastAt-4);
				if (ExCode == ExCode.Do)
					Next();
				if (Curr == ';' || Curr == ':')
					Next();

				// statements
				ParseBlock(flags, ind);
				return;
			}
			case ExCode.ForEach:
			{
				mark = Write(ExCode);

				Write(0); // size of var expression
				Write(0); // size of list expression

				var varAt = code.size;
				Next().FullExpression(flags | Flag.Limited | Flag.NoExpression | Flag.WasFor);
				if (Curr == ':' || ExCode == ExCode.In)
					Next();
				Write(code.size - varAt, mark + 1);
			}
		for_in:
			{
				var listAt = code.size;
				FullExpression(flags | Flag.Limited | Flag.NoExpression);
				if (!HasOption(Option.Prefix))
					Write(OpCode.Cond);
				Write(code.size - listAt, mark + 5);
				if (ExCode == ExCode.Do)
					Next();
				if (Curr == ';' || Curr == ':')
					Next();
				ParseBlock(flags, ind);
				return;
			}

			case ExCode.Try:
			{
				mark = Write(ExCode);

				Write(0); // size of try block
				Write(0); // size of catch sequence
				Write(0); // size of finally sequence

				var tryAt = code.size;
				Next().ParseBlock(flags | Flag.NoSize, ind);
				Write(code.size - tryAt, mark+1);

				var catchAt = code.size;
				while (ExCode == ExCode.Catch && ind == Indent)
				{
					Next();
					Write(-1); // TODO: reserved for variable name
					FullType(flags);
					if (Curr == ';' || Curr == ':')
						Next();
					ParseBlock(flags, ind);
				}
				if (ExCode == ExCode.Else && ind == Indent)
				{
					if (Curr == ';' || Curr == ':')
						Next();
					Write(-1);
					Write(0);
					ParseBlock(flags, ind);
				}
				Write(code.size - catchAt, mark+5);

				var finAt = code.size;
				if (ExCode == ExCode.Finally && ind == Indent)
				{
					Next();
					if (Curr == ';' || Curr == ':')
						Next();
					ParseBlock(flags | Flag.NoSize, ind);
				}
				Write(code.size - finAt, mark+9);
				return;
			}

			case ExCode.Switch:
				Write(ExCode);
				Next().FullExpression(flags | Flag.Limited);
				if (Curr == ';' || Curr == ':')
					Next();
				Write(0);
				mark = code.size;
				for (; ; )
				{
					if (ExCode == ExCode.Case)
					{
						Next().FullExpression(flags | Flag.Limited);
						if (Curr == ';' || Curr == ':')
							Next();
						ParseBlock(flags, ind);
						continue;
					}
					if (ExCode == ExCode.Default)
					{
						Next();
						Write(OpCode.Void);
						if (Curr == ';' || Curr == ':')
							Next();
						ParseBlock(flags, ind);
						continue;
					}
					break;
				}
				Write(code.size - mark, mark-4);
				return;

			case ExCode.Yield:
			case ExCode.Wait:
				Write(op);
				Next();
				return;

			//--------------------------------------------------------------------------------------
			case ExCode.Function:
			case ExCode.Def:
				if (Next().Word == null)
					throw new ParseError(this, "Expected function name");
				var fname = Word;
				if (fname.Length > 127)
					throw new ParseError(this, "Function name too long");
				Write(OpCode.Function);
				ParseFunction(fname, flags);
				return;
			}
		}
	}
}
