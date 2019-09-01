using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace RedOnion.ROS.Parsing
{
	[DebuggerDisplay("{LineNumber}:{At}: {ExCode}; {Curr}; {Word}; {Line}")]
	public class Lexer : Scanner
	{
		public ExCode ExCode { get; protected set; }

		public static ExCode WordCode(string word)
			=> Words.TryGetValue(word, out var code) ? code : ExCode.Identifier;

		protected virtual void SetWordCode()
		{
			var code = WordCode(Word);
			if (code == ExCode.As && Peek == '!')
			{
				code = ExCode.AsCast;
				End++;
				Word += '!';
			}
			ExCode = code;
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
				SetWordCode();
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
					ExCode = ExCode.Number;
					return;
				case '.':
					ExCode = ExCode.Dot;
					return;
				case '?':
					if (Peek == '?')
					{
						End++;
						ExCode = ExCode.NullCol;
						return;
					}
					if (Peek == '.')
					{
						End++;
						ExCode = ExCode.NullDot;
						if (Peek == '(')
						{
							End++;
							ExCode = ExCode.NullCall;
						}
						return;
					}
					ExCode = ExCode.Ternary;
					return;
				case '=':
					if (Peek == '>')
					{
						End++;
						ExCode = ExCode.Lambda;
						return;
					}
					if (Peek == '=')
					{
						End++;
						ExCode = ExCode.Equals;
						return;
					}
					ExCode = ExCode.Assign;
					return;
				case '|':
					if (Peek == '=')
					{
						End++;
						ExCode = ExCode.OrAssign;
						return;
					}
					if (Peek == '|')
					{
						End++;
						ExCode = ExCode.LogicOr;
						return;
					}
					ExCode = ExCode.BitOr;
					return;
				case '^':
					if (Peek == '=')
					{
						End++;
						ExCode = ExCode.XorAssign;
						return;
					}
					ExCode = ExCode.BitXor;
					return;
				case '&':
					if (Peek == '=')
					{
						End++;
						ExCode = ExCode.AndAssign;
						return;
					}
					if (Peek == '&')
					{
						End++;
						ExCode = ExCode.LogicAnd;
						return;
					}
					ExCode = ExCode.BitAnd;
					return;
				case '<':
					if (Peek == '<')
					{
						End++;
						if (Peek == '=')
						{
							End++;
							ExCode = ExCode.LshAssign;
							return;
						}
						ExCode = ExCode.ShiftLeft;
						return;
					}
					if (Peek == '=')
					{
						End++;
						ExCode = ExCode.LessEq;
						return;
					}
					ExCode = ExCode.Less;
					return;
				case '>':
					if (Peek == '>')
					{
						End++;
						if (Peek == '=')
						{
							End++;
							ExCode = ExCode.RshAssign;
							return;
						}
						ExCode = ExCode.ShiftRight;
						return;
					}
					if (Peek == '=')
					{
						End++;
						ExCode = ExCode.MoreEq;
						return;
					}
					ExCode = ExCode.More;
					return;
				case '+':
					if (Peek == '=')
					{
						End++;
						ExCode = ExCode.AddAssign;
						return;
					}
					if (Peek == '+')
					{
						End++;
						ExCode = ExCode.Inc;
						return;
					}
					ExCode = ExCode.Add;
					return;
				case '-':
					if (Peek == '=')
					{
						End++;
						ExCode = ExCode.SubAssign;
						return;
					}
					if (Peek == '-')
					{
						End++;
						ExCode = ExCode.Dec;
						return;
					}
					ExCode = ExCode.Sub;
					return;
				case '*':
					if (Peek == '=')
					{
						End++;
						ExCode = ExCode.MulAssign;
						return;
					}
					if (Peek == '*')
					{
						End++;
						ExCode = ExCode.Power;
						if (Peek == '=')
						{
							End++;
							ExCode = ExCode.PwrAssign;
						}
						return;
					}
					ExCode = ExCode.Mul;
					return;
				case '/':
					if (Peek == '=')
					{
						End++;
						ExCode = ExCode.DivAssign;
						return;
					}
					ExCode = ExCode.Div;
					return;
				case '%':
					if (Peek == '=')
					{
						End++;
						ExCode = ExCode.ModAssign;
						return;
					}
					ExCode = ExCode.Mod;
					return;
				case '!':
					if (Peek == '=')
					{
						End++;
						ExCode = ExCode.Differ;
						return;
					}
					ExCode = ExCode.Not;
					return;
				case '~':
					ExCode = ExCode.Flip;
					return;
			}
			ExCode = ExCode.Unknown;
		}

		internal static Dictionary<string, ExCode> Words = new Dictionary<string, ExCode>()
		{
			{ "void",       ExCode.Void },
			{ "null",       ExCode.Null },
			{ "nullptr",    ExCode.Null },
			{ "false",      ExCode.False },
			{ "true",       ExCode.True },
			{ "this",       ExCode.This },
			{ "self",       ExCode.Self },
			{ "base",       ExCode.Base },
			{ "super",      ExCode.Base },
			{ "exception",  ExCode.Exception },
			{ "string",     ExCode.String },
			{ "char",       ExCode.Char },
			{ "byte",       ExCode.Byte },
			{ "ushort",     ExCode.UShort },
			{ "uint",       ExCode.UInt },
			{ "ulong",      ExCode.ULong },
			{ "sbyte",      ExCode.SByte },
			{ "short",      ExCode.Short },
			{ "int",        ExCode.Int },
			{ "long",       ExCode.Long },
			{ "float",      ExCode.Float },
			{ "double",     ExCode.Double },
			{ "bool",       ExCode.Bool },
			{ "boolean",    ExCode.Bool },
			{ "new",        ExCode.Create },
			{ "delete",     ExCode.Delete },
			{ "ref",        ExCode.Ref },
			{ "out",        ExCode.Out },
			{ "is",         ExCode.Is },
			{ "as",         ExCode.As },
			{ "and",        ExCode.LogicAnd },
			{ "or",         ExCode.LogicOr },
			{ "typeof",     ExCode.TypeOf },
			{ "nameof",     ExCode.NameOf },
			{ "var",        ExCode.Var },
			{ "for",        ExCode.For },
			{ "foreach",    ExCode.ForEach },
			{ "in",         ExCode.In },
			{ "while",      ExCode.While },
			{ "do",         ExCode.Do },
			{ "until",      ExCode.Until },
			{ "if",         ExCode.If },
			{ "unless",     ExCode.Unless },
			{ "then",       ExCode.Then },
			{ "else",       ExCode.Else },
			{ "with",       ExCode.With },
			{ "return",     ExCode.Return },
			{ "throw",      ExCode.Raise },
			{ "raise",      ExCode.Raise },
			{ "break",      ExCode.Break },
			{ "continue",   ExCode.Continue },
			{ "switch",     ExCode.Switch },
			{ "case",       ExCode.Case },
			{ "default",    ExCode.Default },
			{ "goto",       ExCode.Goto },
			{ "try",        ExCode.Try },
			{ "catch",      ExCode.Catch },
			{ "finally",    ExCode.Finally },
			{ "using",      ExCode.Using },
			{ "from",       ExCode.From },
			{ "select",     ExCode.Select },
			{ "orderby",    ExCode.OrderBy },
			{ "yield",      ExCode.Yield },
			{ "wait",       ExCode.Wait },
			{ "public",     ExCode.Public },
			{ "private",    ExCode.Private },
			{ "protected",  ExCode.Protected },
			{ "internal",   ExCode.Internal },
			{ "final",      ExCode.Final },
			{ "sealed",     ExCode.Final },
			{ "virtual",    ExCode.Virtual },
			{ "abstract",   ExCode.Abstract },
			{ "override",   ExCode.Override },
			{ "readonly",   ExCode.ReadOnly },
			{ "const",      ExCode.Const },
			{ "static",     ExCode.Static },
			{ "import",     ExCode.Import },
			{ "include",    ExCode.Include },
			{ "namespace",  ExCode.Namespace },
			{ "package",    ExCode.Package },
			{ "class",      ExCode.Class },
			{ "struct",     ExCode.Struct },
			{ "enum",       ExCode.Enum },
			{ "interface",  ExCode.Interface },
			{ "delegate",   ExCode.Delegate },
			{ "where",      ExCode.Where },
			{ "function",   ExCode.Function },
			{ "def",        ExCode.Def },// like function (from Python/Ruby)
			{ "event",      ExCode.Event },
			{ "get",        ExCode.Get },
			{ "set",        ExCode.Set },
			{ "add",        ExCode.Combine },
			{ "combine",    ExCode.Combine },
			{ "remove",     ExCode.Remove }
		};
	}
}
