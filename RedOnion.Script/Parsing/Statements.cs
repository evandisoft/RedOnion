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
		/// Test if there is expression at current position
		/// </summary>
		protected virtual bool PeekExpression(Flag flags)
			=> !lexer.Eol
			&& lexer.Curr != ';'
			&& lexer.Curr != ','
			&& lexer.Curr != ':'
			&& (lexer.Code.Kind() <= OpKind.Number
			&& (lexer.Code != OpCode.Undefined || lexer.Word == null)
			|| lexer.Code.Kind() == OpKind.Unary || lexer.Code.Kind() == OpKind.PreOrPost
			|| lexer.Code == OpCode.Add || lexer.Code == OpCode.Sub
			|| lexer.Code == OpCode.Var || lexer.Code == OpCode.Create
			|| lexer.Curr == '(');

		/// <summary>
		/// Full/required expression (e.g. condition of while)
		/// </summary>
		/// <returns>True if there was any expression</returns>
		protected bool FullExpression(Flag flags)
		{
			var state = StartExpression();
			if ((flags & Flag.NoExpression) == 0 || PeekExpression(flags))
				ParseExpression(flags &~Flag.NoExpression);
			return FinishExpression(state);
		}
		/// <summary>
		/// Optional expression (e.g. argument in function declaration)
		/// </summary>
		/// <returns>True if there was any expression</returns>
		protected bool OptionalExpression(Flag flags)
		{
			var state = StartExpression();
			if (lexer.Code == OpCode.Assign)
				Next(true);
			else if (!PeekExpression(flags))
				goto skip;
			ParseExpression(flags &~Flag.NoExpression);
		skip:
			return FinishExpression(state);
		}

		/// <summary>
		/// Full/required type reference (e.g. after new)
		/// </summary>
		/// <returns>True if there was any type</returns>
		protected bool FullType(Flag flags)
		{
			var state = StartExpression();
			ParseType(flags);
			return FinishExpression(state);
		}
		/// <summary>
		/// Optional type reference (e.g. in var)
		/// </summary>
		/// <returns>True if there was any expression</returns>
		protected bool OptionalType(Flag flags)
		{
			if ((Options & (Option.Typed | Option.Untyped)) == Option.Untyped)
				return false;
			var state = StartExpression();
			if (lexer.Curr == ':' || lexer.Code == OpCode.As)
			{
				if (Next(true).lexer.Word == null)
					throw new ParseError(lexer, "Expected type reference");
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
		/// Parse block of statements
		/// </summary>
		/// <returns>Number of statements</returns>
		protected virtual int ParseBlock(Flag flags)
		{
			var ind = lexer.Indent;
			if (ind == 0 && lexer.First && (flags & Flag.LimitedContext) == 0)
				ind = -1;
			var nosize = (flags & Flag.NoSize) != 0;
			var member = (flags & Flag.Member) != 0;
			flags &= ~(Flag.NoSize | Flag.Member);
			var block = 0;
			var count = 0;
			if (!nosize)
			{
				Write(0);
				block = CodeAt;
			}
			while (!lexer.Eof && lexer.Curr != ')' && lexer.Curr != '}' && lexer.Curr != ']')
			{
				while (lexer.Indent >= ind && lexer.Peek == ':' && lexer.Code == OpCode.Identifier)
				{
					Label(flags);
					count++;
				}
				if (lexer.First && lexer.Indent <= ind)
					break;
				if (lexer.Eol)
				{
					lexer.NextLine();
					if (count == 0 && member && (lexer.Code.Code() == OpCode.Property.Code()
						|| lexer.Code.TypeFlags() != 0) && ParseProperty(flags))
						break;
					continue;
				}
				while (lexer.Curr == ';')
					Next();
				if (lexer.Code == OpCode.Catch || lexer.Code == OpCode.Finally
					|| lexer.Code == OpCode.Case || lexer.Code == OpCode.Default)
					break;
				if ((flags & Flag.WasDo) != 0 && (lexer.Code == OpCode.While || lexer.Code == OpCode.Until))
					break;
				if ((flags & Flag.WasIf) != 0 && lexer.Code == OpCode.Else)
					break;
				ParseStatement(flags);
				count++;
			}
			if (!nosize)
				Write(CodeAt - block, block - 4);
			return count;
		}

		protected Dictionary<string, int> LabelTable;
		protected Dictionary<int, string> GotoTable;
		protected virtual void Label(Flag flags)
		{
			if (lexer.Word.Length > 127)
				throw new ParseError(lexer, "Label too long");
			if (LabelTable == null)
				LabelTable = new Dictionary<string, int>();
			if (LabelTable.ContainsKey(lexer.Word))
				throw new ParseError(lexer, "Duplicit label name: " + lexer.Word);
			LabelTable[lexer.Word] = CodeAt;
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
					throw new ParseError(lexer, "No labels but goto");
				foreach(var pair in GotoTable)
				{
					var label = pair.Value;
					if (!LabelTable.TryGetValue(label, out var at))
						throw new ParseError(lexer, "Missing label: " + label);
					Write(at - (pair.Key+4), pair.Key); // relative to after goto
				}
			}
			LabelTable = labels.LabelTable;
			GotoTable = labels.GotoTable;
		}

		protected virtual void ParseStatement(Flag flags)
		{
			int mark;
			switch (lexer.Code)
			{
			default:
				if (lexer.Word != null && lexer.Peek == ':')
				{
					Label(flags);
					return;
				}
				var exprStart = CodeAt;
				FullExpression(flags);
				if (HasOption(Option.AutocallSimple) && CodeAt > exprStart)
				{
					var root = ((OpCode)Code[exprStart]).Extend();
					if (root == OpCode.Dot || root.Kind() <= OpKind.Number)
					{
						Array.Copy(Code, exprStart, Code, exprStart+1, CodeAt-exprStart);
						Code[exprStart] = OpCode.Autocall.Code();
						CodeAt++;
					}
				}
				return;
			case OpCode.Goto:
				Next();
				if (lexer.Code == OpCode.Case)
				{
					Write(OpCode.GotoCase);
					Next().FullExpression(flags | Flag.LimitedContext);
					return;
				}
				if (lexer.Word == null)
					throw new ParseError(lexer, "Expected label after goto (or case)");
				if (lexer.Word.Length > 127)
					throw new ParseError(lexer, "Label too long");
				Write(OpCode.Goto);
				if (GotoTable == null)
					GotoTable = new Dictionary<int, string>();
				GotoTable[CodeAt] = lexer.Word;
				return;
			case OpCode.Return:
				Write(lexer.Code);
				Next().FullExpression(flags | Flag.NoExpression);
				return;
			case OpCode.Raise:
				Write(lexer.Code);
				Next().FullExpression(flags | Flag.NoExpression);
				return;
			case OpCode.Break:
			case OpCode.Continue:
				Write(lexer.Code);
				Next();
				return;
			case OpCode.If:
			case OpCode.Unless:
				Write(lexer.Code);
				Next().FullExpression(flags | Flag.LimitedContext);
				if (lexer.Curr == ';' || lexer.Curr == ':' || lexer.Code == OpCode.Then)
					Next();
				ParseBlock(flags | Flag.WasIf);
				if (lexer.Code == OpCode.Else)
				{
					Write(lexer.Code);
					if (Next().lexer.Curr == ':')
						Next();
					ParseBlock(flags);
				}
				return;
			case OpCode.Else:
				throw new ParseError(lexer, "Unexpected 'else'");

			case OpCode.While:
			case OpCode.Until:
				Write(lexer.Code);
				Next().FullExpression(flags | Flag.LimitedContext);
				if (lexer.Curr == ';' || lexer.Curr == ':' || lexer.Code == OpCode.Do)
					Next();
				ParseBlock(flags);
				return;
			case OpCode.Do:
				var doat = Write(lexer.Code);
				Next();
				ParseBlock(flags | Flag.WasDo);
				if (lexer.Code != OpCode.While)
				{
					if (lexer.Code != OpCode.Until)
						throw new ParseError(lexer, "Expected 'while' or 'until' for 'do'");
					Code[doat] = OpCode.DoUntil.Code();
				}
				Next().FullExpression(flags);
				return;

			case OpCode.For:
				var forat = Write(lexer.Code);
				Next();
				FullExpression(flags | Flag.LimitedContext | Flag.NoExpression);
				if (lexer.Curr == ':' || lexer.Code == OpCode.In)
				{
					Code[forat] = OpCode.ForEach.Code();
					Next();
					goto for_in;
				}
				if (lexer.Curr == ';')
					Next();
				FullExpression(flags | Flag.LimitedContext | Flag.NoExpression);
				if (lexer.Curr == ';')
					Next();
				if (!PeekExpression(flags))
					Write(0); // zero-sized block
				else
				{
					Write(0);
					mark = CodeAt;
					FullExpression(flags | Flag.LimitedContext);
					Write(CodeAt - mark, mark-4);
				}
				if (lexer.Curr == ';')
					Next();
				ParseBlock(flags);
				return;
			case OpCode.ForEach:
				Write(lexer.Code);
				Next();
				FullExpression(flags | Flag.LimitedContext | Flag.NoExpression);
				if (lexer.Curr == ':' || lexer.Code == OpCode.In)
					Next();
			for_in:
				FullExpression(flags | Flag.LimitedContext | Flag.NoExpression);
				if (lexer.Curr == ';')
					Next();
				ParseBlock(flags);
				return;

			case OpCode.Try:
				Write(lexer.Code);
				Next().ParseBlock(flags);
				Write(0);
				mark = CodeAt;
				while (lexer.Code == OpCode.Catch)
				{
					Next();
					Write(-1); // TODO: reserved for variable name
					FullType(flags);
					if (lexer.Curr == ';' || lexer.Curr == ':')
						Next();
					ParseBlock(flags);
				}
				if (lexer.Code == OpCode.Else)
				{
					if (lexer.Curr == ';' || lexer.Curr == ':')
						Next();
					ParseBlock(flags);
				}
				Write(CodeAt - mark, mark-4);
				if (lexer.Code != OpCode.Finally)
					Write(0);
				else
				{
					Next();
					if (lexer.Curr == ';' || lexer.Curr == ':')
						Next();
					ParseBlock(flags);
				}
				return;

			case OpCode.Switch:
				Write(lexer.Code);
				Next().FullExpression(flags | Flag.LimitedContext);
				if (lexer.Curr == ';' || lexer.Curr == ':')
					Next();
				Write(0);
				mark = CodeAt;
				for (;;)
				{
					if (lexer.Code == OpCode.Case)
					{
						Next().FullExpression(flags | Flag.LimitedContext);
						if (lexer.Curr == ';' || lexer.Curr == ':')
							Next();
						ParseBlock(flags);
						continue;
					}
					if (lexer.Code == OpCode.Default)
					{
						Next();
						Write(OpCode.Undefined);
						if (lexer.Curr == ';' || lexer.Curr == ':')
							Next();
						ParseBlock(flags);
						continue;
					}
					break;
				}
				Write(CodeAt - mark, mark-4);
				return;

			//--------------------------------------------------------------------------------------
			case OpCode.Function:
			case OpCode.Def:
				if ((Options & Option.Script) == 0)
					goto default; // TODO: local functions
				if (Next().lexer.Word == null)
					throw new ParseError(lexer, "Expected function name");
				var fname = lexer.Word;
				if (fname.Length > 127)
					throw new ParseError(lexer, "Function name too long");
				Write(OpCode.Function);
				ParseFunction(fname, flags);
				return;
			}
		}

		protected virtual void ParseFunction(string name, Flag flags)
		{
			if (name != null)   // null if parsing lambda / inline function
				Write(name);    // function name (index to string table)
			else flags |= Flag.LimitedContext;
			Write(0);           // header size
			int mark = CodeAt;
			Write((ushort)0);   // type flags
			Write((byte)0);     // number of generic parameters
			Write((byte)0);     // number of arguments

			Write(0);           // return type size
			var typeMark = CodeAt;
			Next().OptionalType(flags);
			Write(CodeAt - typeMark, typeMark-4);

			var argc = 0;
			var paren = lexer.Curr == '(';
			if (paren || lexer.Curr == ',')
				Next(true);
			bool lambda = !paren && lexer.Code == OpCode.Lambda;
			while ((paren || (!lexer.Eol && lexer.Curr != ';')) && !lexer.Eof && !lambda)
			{
				if (lexer.Word == null)
					throw new ParseError(lexer, "Expected argument name");
				if (argc > 127)
					throw new ParseError(lexer, "Too many arguments");

				Write(lexer.Word);  // argument name (index to string table)

				Write(0);           // argument type size
				var argMark = CodeAt;
				Next().OptionalType(flags);
				Write(CodeAt - argMark, argMark-4);

				Write(0);           // argument default value size
				argMark = CodeAt;
				OptionalExpression(flags);
				Write(CodeAt - argMark, argMark-4);
				argc++;

				if (paren && lexer.Curr == ')')
				{
					Next();
					break;
				}
				if (!paren && lexer.Code == OpCode.Lambda)
				{
					lambda = true;
					break;
				}
				if (lexer.Curr == ',')
					Next(true);
			}
			if (lambda)
				Next();

			Write(CodeAt - mark, mark-4);   // header size
			Code[mark + 3] = (byte)argc;    // number of arguments

			var labels = StoreLabels();
			var blockAt = CodeAt;
			var count = ParseBlock(flags);
			if (lambda && count == 1)
			{
				var op = ((OpCode)Code[blockAt+4]).Extend();
				if (op == OpCode.Autocall)
					Code[blockAt+4] = OpCode.Return.Code();
				else if (op.Kind() < OpKind.Statement)
				{
					var sz = BitConverter.ToInt32(Code, blockAt);
					Write(++sz, blockAt);
					Array.Copy(Code, blockAt+4, Code, blockAt+5, CodeAt-blockAt-5);
					Write(OpCode.Return.Code(), blockAt+4);
					CodeAt++;
				}
			}
			RestoreLabels(labels);
		}
	}
}
