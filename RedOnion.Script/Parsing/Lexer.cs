using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace RedOnion.Script.Parsing
{
	partial class Parser
	{
		protected class Lexer : Parsing.Lexer
		{
			protected Parser Parser { get; }
			public Lexer(Parser parser) => Parser = parser;

			protected CompiledCode.SourceLine[] _lines = new CompiledCode.SourceLine[64];
			public CompiledCode.SourceLine[] Lines => _lines;
			public int LinesAt { get; set; }

			public override void SetLine(string value)
			{
				base.SetLine(value);
				if (Line == null)
					return;
				if (LinesAt == _lines.Length)
					Array.Resize(ref _lines, _lines.Length << 1);
				_lines[LinesAt++] = new CompiledCode.SourceLine(CharCounter, Line);
				Parser.RecordLine();
			}

			public CompiledCode.SourceLine[] LinesToArray()
			{
				var arr = new CompiledCode.SourceLine[LinesAt];
				Array.Copy(_lines, 0, arr, 0, arr.Length);
				return arr;
			}
		}
	}

	[DebuggerDisplay("{LineNumber}:{At}: {Code}; {Curr}; {Word}; {Line}")]
	public class Lexer : Scanner
	{
		public OpCode Code { get; protected set; }

		public static OpCode WordCode(string word)
			=> Words.TryGetValue(word, out var code) ? code : OpCode.Identifier;

		protected Lexer WordCode()
		{
			WordOpCode();
			return this;
		}
		protected virtual void WordOpCode()
		{
			var code = WordCode(Word);
			if (code == OpCode.As && Peek == '!')
			{
				code = OpCode.AsCast;
				End++;
				Word += '!';
			}
			Code = code;
		}

		/// <summary>
		/// Parse next word, literal, operator or character on current line
		/// </summary>
		public new Lexer Next()
		{
			NextToken();
			return this;
		}
		/// <summary>
		/// Parse next word, literal, operator or character on current line
		/// </summary>
		protected override void NextToken()
		{
			base.NextToken();
			if (Word != null)
			{
				WordOpCode();
				return;
			}
			switch (Curr)
			{
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			case '"':   // parser will switch that to string/char
			case '\'':  // here 'number' actually means literal
				Code = OpCode.Number;
				return;
			case '.':
				if (Peek == '[')
				{
					End++;
					Code = OpCode.Generic;
					return;
				}
				Code = OpCode.Dot;
				return;
			case '?':
				if (Peek == '?')
				{
					End++;
					Code = OpCode.NullCol;
					return;
				}
				if (Peek == '.')
				{
					End++;
					Code = OpCode.NullDot;
					if (Peek == '(')
					{
						End++;
						Code = OpCode.NullCall;
					}
					return;
				}
				Code = OpCode.Ternary;
				return;
			case '=':
				if (Peek == '>')
				{
					End++;
					Code = OpCode.Lambda;
					return;
				}
				if (Peek == '=')
				{
					End++;
					Code = OpCode.Equals;
					return;
				}
				Code = OpCode.Assign;
				return;
			case '|':
				if (Peek == '=')
				{
					End++;
					Code = OpCode.OrAssign;
					return;
				}
				if (Peek == '|')
				{
					End++;
					Code = OpCode.LogicOr;
					return;
				}
				Code = OpCode.BitOr;
				return;
			case '^':
				if (Peek == '=')
				{
					End++;
					Code = OpCode.XorAssign;
					return;
				}
				Code = OpCode.BitXor;
				return;
			case '&':
				if (Peek == '=')
				{
					End++;
					Code = OpCode.AndAssign;
					return;
				}
				if (Peek == '&')
				{
					End++;
					Code = OpCode.LogicAnd;
					return;
				}
				Code = OpCode.BitAnd;
				return;
			case '<':
				if (Peek == '<')
				{
					End++;
					if (Peek == '=')
					{
						End++;
						Code = OpCode.LshAssign;
						return;
					}
					Code = OpCode.ShiftLeft;
					return;
				}
				if (Peek == '=')
				{
					End++;
					Code = OpCode.LessEq;
					return;
				}
				Code = OpCode.Less;
				return;
			case '>':
				if (Peek == '>')
				{
					End++;
					if (Peek == '=')
					{
						End++;
						Code = OpCode.RshAssign;
						return;
					}
					Code = OpCode.ShiftRight;
					return;
				}
				if (Peek == '=')
				{
					End++;
					Code = OpCode.MoreEq;
					return;
				}
				Code = OpCode.More;
				return;
			case '+':
				if (Peek == '=')
				{
					End++;
					Code = OpCode.AddAssign;
					return;
				}
				if (Peek == '+')
				{
					End++;
					Code = OpCode.Inc;
					return;
				}
				Code = OpCode.Add;
				return;
			case '-':
				if (Peek == '=')
				{
					End++;
					Code = OpCode.SubAssign;
					return;
				}
				if (Peek == '-')
				{
					End++;
					Code = OpCode.Dec;
					return;
				}
				Code = OpCode.Sub;
				return;
			case '*':
				if (Peek == '=')
				{
					End++;
					Code = OpCode.MulAssign;
					return;
				}
				if (Peek == '*')
				{
					End++;
					Code = OpCode.Power;
					return;
				}
				Code = OpCode.Mul;
				return;
			case '/':
				if (Peek == '=')
				{
					End++;
					Code = OpCode.DivAssign;
					return;
				}
				Code = OpCode.Div;
				return;
			case '%':
				if (Peek == '=')
				{
					End++;
					Code = OpCode.ModAssign;
					return;
				}
				Code = OpCode.Mod;
				return;
			case '!':
				if (Peek == '=')
				{
					End++;
					Code = OpCode.Differ;
					return;
				}
				Code = OpCode.Not;
				return;
			case '~':
				Code = OpCode.Flip;
				return;
			}
			Code = OpCode.Unknown;
		}

		internal static Dictionary<string, OpCode> Words = new Dictionary<string, OpCode>()
		{
			{ "undefined",  OpCode.Undefined },
			{ "null",       OpCode.Null },
			{ "nullptr",    OpCode.Null },
			{ "false",      OpCode.False },
			{ "true",       OpCode.True },
			{ "this",       OpCode.This },
			{ "self",       OpCode.Self },
			{ "base",       OpCode.Base },
			{ "super",      OpCode.Base },
			{ "exception",  OpCode.Exception },
			{ "object",     OpCode.Object },
			{ "string",     OpCode.String },
			{ "char",       OpCode.Char },
			{ "byte",       OpCode.Byte },
			{ "ushort",     OpCode.UShort },
			{ "uint",       OpCode.UInt },
			{ "ulong",      OpCode.ULong },
			{ "sbyte",      OpCode.SByte },
			{ "short",      OpCode.Short },
			{ "int",        OpCode.Int },
			{ "long",       OpCode.Long },
			{ "float",      OpCode.Float },
			{ "double",     OpCode.Double },
			//{ "ldouble",	OpCode.LongDouble },
			//{ "decimal",	OpCode.Decimal },
			//{ "quad",		OpCode.Quad },
			//{ "hyper",	OpCode.Hyper },
			{ "bool",       OpCode.Bool },
			{ "boolean",    OpCode.Bool },
			{ "new",        OpCode.Create },
			{ "delete",     OpCode.Delete },
			{ "ref",        OpCode.Ref },
			{ "out",        OpCode.Out },
			{ "is",         OpCode.Is },
			{ "as",         OpCode.As },
			{ "typeof",     OpCode.TypeOf },
			{ "nameof",     OpCode.NameOf },
			//{ "await",	OpCode.Await },// not implemented
			{ "var",        OpCode.Var },
			{ "for",        OpCode.For },
			{ "foreach",    OpCode.ForEach },
			{ "in",         OpCode.In },
			{ "while",      OpCode.While },
			{ "do",         OpCode.Do },
			{ "until",      OpCode.Until },
			{ "if",         OpCode.If },
			{ "then",       OpCode.Then },
			{ "else",       OpCode.Else },
			{ "with",       OpCode.With },
			{ "return",     OpCode.Return },
			{ "throw",      OpCode.Raise },
			{ "raise",      OpCode.Raise },
			{ "break",      OpCode.Break },
			{ "continue",   OpCode.Continue },
			{ "switch",     OpCode.Switch },
			{ "case",       OpCode.Case },
			{ "default",	OpCode.Default },
			{ "goto",		OpCode.Goto },
			{ "try",		OpCode.Try },
			{ "catch",		OpCode.Catch },
			{ "finally",	OpCode.Finally },
			{ "using",		OpCode.Using },
			{ "from",		OpCode.From },
			{ "select",		OpCode.Select },
			{ "orderby",	OpCode.OrderBy },
			{ "public",		OpCode.Public },
			{ "private",	OpCode.Private },
			{ "protected",	OpCode.Protected },
			{ "internal",	OpCode.Internal },
			{ "final",		OpCode.Final },
			{ "sealed",		OpCode.Final },
			{ "virtual",	OpCode.Virtual },
			{ "abstract",	OpCode.Abstract },
			{ "override",	OpCode.Override },
			{ "readonly",	OpCode.ReadOnly },
			{ "const",		OpCode.Const },
			{ "static",		OpCode.Static },
			//{ "partial",	OpCode.Partial },// not implemented
			//{ "unsafe",	OpCode.Unsafe },// not implemented
			//{ "async",	OpCode.Async },// not implemented
			{ "import",		OpCode.Import },
			{ "include",	OpCode.Include },
			//{ "use",		OpCode.Use },// too short
			{ "namespace",	OpCode.Namespace },
			{ "package",	OpCode.Package },
			//{ "pkg",		OpCode.Pkg },// too short
			{ "class",		OpCode.Class },
			{ "struct",		OpCode.Struct },
			{ "enum",		OpCode.Enum },
			{ "interface",	OpCode.Interface },
			{ "delegate",	OpCode.Delegate },
			{ "where",		OpCode.Where },
			{ "function",	OpCode.Function },
			{ "def",		OpCode.Def },// like function (from Python/Ruby)
			{ "event",		OpCode.Event },
			{ "get",		OpCode.Get },
			{ "set",		OpCode.Set },
			{ "add",		OpCode.Combine },
			{ "combine",	OpCode.Combine },
			{ "remove",		OpCode.Remove }
		};
	}
}
