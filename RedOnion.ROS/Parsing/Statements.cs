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
			return FinishExpression(state);
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
		protected virtual int ParseBlock(Flag flags)
		{
			var ind = Indent;
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

		protected Dictionary<string, int> LabelTable;
		protected Dictionary<int, string> GotoTable;
		protected virtual void Label(Flag flags)
		{
			if (Word.Length > 127)
				throw new ParseError(this, "Label too long");
			if (LabelTable == null)
				LabelTable = new Dictionary<string, int>();
			if (LabelTable.ContainsKey(Word))
				throw new ParseError(this, "Duplicit label name: " + Word);
			LabelTable[Word] = code.size;
			Next().Next();
		}
		protected struct StoredLabels
		{
			public Dictionary<string, int> LabelTable { get; }
			public Dictionary<int, string> GotoTable { get; }
			public StoredLabels(Dictionary<string, int> labelTable, Dictionary<int, string> gotoTable)
			{
				LabelTable = labelTable;
				GotoTable = gotoTable;
			}
		}
		protected StoredLabels StoreLabels()
		{
			var labelTable = LabelTable;
			var gotoTable = GotoTable;
			LabelTable = null;
			GotoTable = null;
			return new StoredLabels(labelTable, gotoTable);
		}
		protected void RestoreLabels(StoredLabels labels)
		{
			//TODO: save line, column and character index for each goto
			//..... for better exception reporting
			if (GotoTable != null)
			{
				if (LabelTable == null)
					throw new ParseError(this, "No labels but goto");
				foreach(var pair in GotoTable)
				{
					var label = pair.Value;
					if (!LabelTable.TryGetValue(label, out var at))
						throw new ParseError(this, "Missing label: " + label);
					Write(at - (pair.Key+4), pair.Key); // relative to after goto
				}
			}
			LabelTable = labels.LabelTable;
			GotoTable = labels.GotoTable;
		}

		protected virtual void ParseStatement(Flag flags)
		{
			int mark;
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
				if (GotoTable == null)
					GotoTable = new Dictionary<int, string>();
				GotoTable[code.size] = Word;
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
				if (HasOption(Option.Prefix)) Write(op);
				Next().FullExpression(flags | Flag.Limited);
				if (!HasOption(Option.Prefix)) Write(op);
				if (Curr == ';' || Curr == ':' || ExCode == ExCode.Then)
					Next();
				ParseBlock(flags | Flag.WasIf);
				if (ExCode == ExCode.Else)
				{
					Write(ExCode);
					if (Next().Curr == ':')
						Next();
					ParseBlock(flags);
				}
				return;
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
				if (Curr == ';' || Curr == ':' || ExCode == ExCode.Do)
					Next();
				ParseBlock(flags);
				return;
			}
			// do; cond size; block size; block; cond
			case ExCode.Do:
			{
				var doAt = Write(op);
				Write(0);
				Next().ParseBlock(flags | Flag.WasDo);
				if (ExCode != ExCode.While)
				{
					if (ExCode != ExCode.Until)
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
				Next().FullExpression(flags | Flag.Limited | Flag.NoExpression);
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
				FullExpression(flags | Flag.Limited | Flag.NoExpression);
				if (!HasOption(Option.Prefix))
					Write(OpCode.Cond);
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
				if (Curr == ';')
					Next();

				// statements
				ParseBlock(flags);
				return;
			}
			case ExCode.ForEach:
			{
				mark = Write(ExCode);

				Write(0); // size of var expression
				Write(0); // size of list expression

				var varAt = code.size;
				Next().FullExpression(flags | Flag.Limited | Flag.NoExpression);
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
				if (Curr == ';')
					Next();
				ParseBlock(flags);
				return;
			}

			case ExCode.Try:
				Write(ExCode);
				Next().ParseBlock(flags);
				Write(0);
				mark = code.size;
				while (ExCode == ExCode.Catch)
				{
					Next();
					Write(-1); // TODO: reserved for variable name
					FullType(flags);
					if (Curr == ';' || Curr == ':')
						Next();
					ParseBlock(flags);
				}
				if (ExCode == ExCode.Else)
				{
					if (Curr == ';' || Curr == ':')
						Next();
					ParseBlock(flags);
				}
				Write(code.size - mark, mark-4);
				if (ExCode != ExCode.Finally)
					Write(0);
				else
				{
					Next();
					if (Curr == ';' || Curr == ':')
						Next();
					ParseBlock(flags);
				}
				return;

			case ExCode.Switch:
				Write(ExCode);
				Next().FullExpression(flags | Flag.Limited);
				if (Curr == ';' || Curr == ':')
					Next();
				Write(0);
				mark = code.size;
				for (;;)
				{
					if (ExCode == ExCode.Case)
					{
						Next().FullExpression(flags | Flag.Limited);
						if (Curr == ';' || Curr == ':')
							Next();
						ParseBlock(flags);
						continue;
					}
					if (ExCode == ExCode.Default)
					{
						Next();
						Write(OpCode.Void);
						if (Curr == ';' || Curr == ':')
							Next();
						ParseBlock(flags);
						continue;
					}
					break;
				}
				Write(code.size - mark, mark-4);
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

		protected virtual void ParseFunction(string name, Flag flags)
		{
			if (name != null)   // null if parsing lambda / inline function
				Write(name);    // function name (index to string table)
			else flags |= Flag.Limited;
			Write(0);           // header size
			int mark = code.size;
			Write((ushort)0);   // type flags
			Write((byte)0);     // number of generic parameters
			Write((byte)0);     // number of arguments

			Write(0);           // return type size
			var typeMark = code.size;
			Next().OptionalType(flags);
			Write(code.size - typeMark, typeMark-4);

			var argc = 0;
			var paren = Curr == '(';
			if (paren || Curr == ',')
				Next(true);
			bool lambda = !paren && ExCode == ExCode.Lambda;
			while ((paren || (!Eol && Curr != ';')) && !Eof && !lambda)
			{
				if (Word == null)
					throw new ParseError(this, "Expected argument name");
				if (argc > 127)
					throw new ParseError(this, "Too many arguments");

				Write(Word);  // argument name (index to string table)

				Write(0);           // argument type size
				var argMark = code.size;
				Next().OptionalType(flags);
				Write(code.size - argMark, argMark-4);

				Write(0);           // argument default value size
				argMark = code.size;
				OptionalExpression(flags);
				Write(code.size - argMark, argMark-4);
				argc++;

				if (paren && Curr == ')')
				{
					Next();
					break;
				}
				if (!paren && ExCode == ExCode.Lambda)
				{
					lambda = true;
					break;
				}
				if (Curr == ',')
					Next(true);
			}
			if (lambda)
			{
				Next();
				flags |= Flag.Limited;
			}

			Write(code.size - mark, mark-4);   // header size
			code.items[mark + 3] = (byte)argc;    // number of arguments

			var labels = StoreLabels();
			var blockAt = code.size;
			var count = ParseBlock(flags);
			if (lambda && count == 1)
			{
				var op = ((OpCode)code.items[blockAt+4]).Extend();
				if (op == ExCode.Autocall)
					code.items[blockAt+4] = OpCode.Return.Code();
				else if (op.Kind() < OpKind.Statement)
				{
					code.EnsureCapacity(code.size+1);
					var sz = BitConverter.ToInt32(code.items, blockAt);
					Write(++sz, blockAt);
					Array.Copy(code.items, blockAt+4, code.items, blockAt+5, code.size-blockAt-5);
					Write(OpCode.Return.Code(), blockAt+4);
					code.size++;
				}
			}
			RestoreLabels(labels);
		}
	}
}
