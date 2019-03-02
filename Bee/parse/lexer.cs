using Bee.Run;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee
{
	[DebuggerDisplay("{_lnum}:{_at}: {_opcode}; {_curr}; {_word}; {_line}")]
	public class Lexer: Scanner
	{
		private Opcode _opcode;
		public Opcode Opcode
		{
			get => _opcode;
			protected set => _opcode = value;
		}
		
		public static Opcode Wordcode( string word )
		{
			Opcode code;
			if (!Words.TryGetValue( word, out code ))
			{
				code = Opcode.Ident;
			}
			return code;
		}
		
		protected Lexer Wordcode(  )
		{
			this.Wordcode_();
			return this;
		}
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void Wordcode_(  )
		{
			var code = Wordcode( Word );
			if ((code == Opcode.As) && (Peek == '!'))
			{
				code = Opcode.Ascast;
				End++;
				Word += '!';
			}
			Opcode = code;
		}
		
		/// <summary>
		/// parse next word, literal, operator or character on this line
		/// </summary>
		public new Lexer Next(  )
		{
			this.Next_();
			return this;
		}
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected override void Next_(  )
		{
			base.Next_();
			if (Word != null)
			{
				Wordcode();
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
			case '"':
			case '\'':
				Opcode = Opcode.Number;
				return;
			case '.':
				if (Peek == '[')
				{
					End++;
					Opcode = Opcode.Generic;
					return;
				}
				Opcode = Opcode.Dot;
				return;
			case '?':
				if (Peek == '?')
				{
					End++;
					Opcode = Opcode.Nullcol;
					return;
				}
				if (Peek == '.')
				{
					End++;
					Opcode = Opcode.Nulldot;
					if (Peek == '(')
					{
						End++;
						Opcode = Opcode.Nullcall;
					}
					return;
				}
				Opcode = Opcode.Ternary;
				return;
			case '=':
				if (Peek == '>')
				{
					End++;
					Opcode = Opcode.Lambda;
					return;
				}
				if (Peek == '=')
				{
					End++;
					Opcode = Opcode.Equals;
					return;
				}
				Opcode = Opcode.Assign;
				return;
			case '|':
				if (Peek == '=')
				{
					End++;
					Opcode = Opcode.OrAssign;
					return;
				}
				if (Peek == '|')
				{
					End++;
					Opcode = Opcode.LogicOr;
					return;
				}
				Opcode = Opcode.BitOr;
				return;
			case '^':
				if (Peek == '=')
				{
					End++;
					Opcode = Opcode.XorAssign;
					return;
				}
				Opcode = Opcode.BitXor;
				return;
			case '&':
				if (Peek == '=')
				{
					End++;
					Opcode = Opcode.AndAssign;
					return;
				}
				if (Peek == '&')
				{
					End++;
					Opcode = Opcode.LogicAnd;
					return;
				}
				Opcode = Opcode.BitAnd;
				return;
			case '<':
				if (Peek == '<')
				{
					End++;
					if (Peek == '=')
					{
						End++;
						Opcode = Opcode.LshAssign;
						return;
					}
					Opcode = Opcode.ShiftLeft;
					return;
				}
				if (Peek == '=')
				{
					End++;
					Opcode = Opcode.Lesseq;
					return;
				}
				Opcode = Opcode.Less;
				return;
			case '>':
				if (Peek == '>')
				{
					End++;
					if (Peek == '=')
					{
						End++;
						Opcode = Opcode.RshAssign;
						return;
					}
					Opcode = Opcode.ShiftRight;
					return;
				}
				if (Peek == '=')
				{
					End++;
					Opcode = Opcode.Moreeq;
					return;
				}
				Opcode = Opcode.More;
				return;
			case '+':
				if (Peek == '=')
				{
					End++;
					Opcode = Opcode.AddAssign;
					return;
				}
				if (Peek == '+')
				{
					End++;
					Opcode = Opcode.Inc;
					return;
				}
				Opcode = Opcode.Add;
				return;
			case '-':
				if (Peek == '=')
				{
					End++;
					Opcode = Opcode.SubAssign;
					return;
				}
				if (Peek == '-')
				{
					End++;
					Opcode = Opcode.Dec;
					return;
				}
				Opcode = Opcode.Sub;
				return;
			case '*':
				if (Peek == '=')
				{
					End++;
					Opcode = Opcode.MulAssign;
					return;
				}
				if (Peek == '*')
				{
					End++;
					Opcode = Opcode.Power;
					return;
				}
				Opcode = Opcode.Mul;
				return;
			case '/':
				if (Peek == '=')
				{
					End++;
					Opcode = Opcode.DivAssign;
					return;
				}
				Opcode = Opcode.Div;
				return;
			case '%':
				if (Peek == '=')
				{
					End++;
					Opcode = Opcode.ModAssign;
					return;
				}
				Opcode = Opcode.Mod;
				return;
			case '!':
				if (Peek == '=')
				{
					End++;
					Opcode = Opcode.Differ;
					return;
				}
				Opcode = Opcode.Not;
				return;
			case '~':
				Opcode = Opcode.Flip;
				return;
			}
			Opcode = Opcode.Unknown;
		}
		
		internal static Dictionary<string, Opcode> Words = new Dictionary<string, Opcode>();
		static Lexer(  )
		{
			Words.Add( "undefined", Opcode.Undef );
			Words.Add( "null", Opcode.Null );
			Words.Add( "nullptr", Opcode.Null );
			Words.Add( "false", Opcode.False );
			Words.Add( "true", Opcode.True );
			Words.Add( "this", Opcode.This );
			Words.Add( "self", Opcode.Self );
			Words.Add( "base", Opcode.Base );
			Words.Add( "super", Opcode.Base );
			Words.Add( "exception", Opcode.Exception );
			Words.Add( "object", Opcode.Object );
			Words.Add( "string", Opcode.String );
			Words.Add( "char", Opcode.Char );
			Words.Add( "byte", Opcode.Byte );
			Words.Add( "ushort", Opcode.Ushort );
			Words.Add( "uint", Opcode.Uint );
			Words.Add( "ulong", Opcode.Ulong );
			Words.Add( "sbyte", Opcode.Sbyte );
			Words.Add( "short", Opcode.Short );
			Words.Add( "int", Opcode.Int );
			Words.Add( "long", Opcode.Long );
			Words.Add( "float", Opcode.Float );
			Words.Add( "double", Opcode.Double );
			Words.Add( "ldouble", Opcode.Ldouble );
			Words.Add( "decimal", Opcode.Decimal );
			Words.Add( "quad", Opcode.Quad );
			Words.Add( "hyper", Opcode.Hyper );
			Words.Add( "bool", Opcode.Bool );
			Words.Add( "boolean", Opcode.Bool );
			Words.Add( "new", Opcode.Create );
			Words.Add( "delete", Opcode.Delete );
			Words.Add( "ref", Opcode.Ref );
			Words.Add( "out", Opcode.Out );
			Words.Add( "is", Opcode.Is );
			Words.Add( "as", Opcode.As );
			Words.Add( "typeof", Opcode.Typeof );
			Words.Add( "nameof", Opcode.Nameof );
			Words.Add( "await", Opcode.Await );
			Words.Add( "var", Opcode.Var );
			Words.Add( "for", Opcode.For );
			Words.Add( "foreach", Opcode.Foreach );
			Words.Add( "in", Opcode.In );
			Words.Add( "while", Opcode.While );
			Words.Add( "do", Opcode.Do );
			Words.Add( "until", Opcode.Until );
			Words.Add( "if", Opcode.If );
			Words.Add( "then", Opcode.Then );
			Words.Add( "else", Opcode.Else );
			Words.Add( "with", Opcode.With );
			Words.Add( "return", Opcode.Return );
			Words.Add( "throw", Opcode.Raise );
			Words.Add( "raise", Opcode.Raise );
			Words.Add( "break", Opcode.Break );
			Words.Add( "continue", Opcode.Continue );
			Words.Add( "switch", Opcode.Switch );
			Words.Add( "case", Opcode.Case );
			Words.Add( "goto", Opcode.Goto );
			Words.Add( "try", Opcode.Try );
			Words.Add( "catch", Opcode.Catch );
			Words.Add( "finally", Opcode.Finally );
			Words.Add( "using", Opcode.Using );
			Words.Add( "from", Opcode.From );
			Words.Add( "select", Opcode.Select );
			Words.Add( "orderby", Opcode.Orderby );
			Words.Add( "public", Opcode.Public );
			Words.Add( "private", Opcode.Private );
			Words.Add( "protected", Opcode.Protected );
			Words.Add( "internal", Opcode.Internal );
			Words.Add( "final", Opcode.Final );
			Words.Add( "sealed", Opcode.Final );
			Words.Add( "virtual", Opcode.Virtual );
			Words.Add( "abstract", Opcode.Abstract );
			Words.Add( "override", Opcode.Override );
			Words.Add( "readonly", Opcode.Readonly );
			Words.Add( "const", Opcode.Const );
			Words.Add( "static", Opcode.Static );
			Words.Add( "partial", Opcode.Partial );
			Words.Add( "unsafe", Opcode.Unsafe );
			Words.Add( "async", Opcode.Async );
			Words.Add( "import", Opcode.Import );
			Words.Add( "include", Opcode.Include );
			Words.Add( "use", Opcode.Use );
			Words.Add( "namespace", Opcode.Space );
			Words.Add( "package", Opcode.Package );
			Words.Add( "pkg", Opcode.Pkg );
			Words.Add( "class", Opcode.Class );
			Words.Add( "def", Opcode.Def );
			Words.Add( "define", Opcode.Define );
			Words.Add( "struct", Opcode.Struct );
			Words.Add( "enum", Opcode.Enum );
			Words.Add( "interface", Opcode.Face );
			Words.Add( "delegate", Opcode.Delegate );
			Words.Add( "where", Opcode.Where );
			Words.Add( "function", Opcode.Func );
			Words.Add( "event", Opcode.Event );
			Words.Add( "get", Opcode.Get );
			Words.Add( "set", Opcode.Set );
			Words.Add( "add", Opcode.Combine );
			Words.Add( "combine", Opcode.Combine );
			Words.Add( "remove", Opcode.Remove );
		}
	}
}
