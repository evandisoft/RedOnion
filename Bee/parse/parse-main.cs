using Bee.Run;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee
{
	public partial class Parser: Lexer
	{
		private Opt _opts;
		public Opt Opts
		{
			get => _opts;
			set => _opts = value;
		}
		
		[Flags]
		public enum Opt
		{
			None = 0,
			/// <summary>
			/// targeting stript engine (not .NET)
			/// </summary>
			Script = 1 << 0,
			/// <summary>
			/// typed language (require static types)
			/// </summary>
			Typed = 1 << 1,
			/// <summary>
			/// untyped language (no static types)
			/// </summary>
			Untyped = 1 << 2,
			/// <summary>
			/// "x .y" => "x(this.y)" if set, "x.y" otherwise
			/// </summary>
			DotThisAfterWhite = 1 << 16,
			/// <summary>
			/// "f[] * [3 + 4]" => "f() * (3 + 4)" if set, error otherwise
			/// </summary>
			BracketsAsParens = 1 << 17,
		}
		
		/// <summary>
		/// @note: "g f [x], y" => "g(f(x), y)"; "g f (x), y" => "g(f(x,y))"
		/// </summary>
		[Flags]
		public enum Flag
		{
			None = 0,
			/// <summary>
			/// expression in limited context (inside if/while/using... condition/declaration)
			/// </summary>
			Limit = 1 << 0,
			/// <summary>
			/// allow empty/no expression/type (used in fullexpr and fulltype)
			/// </summary>
			Noexpr = 1 << 1,
			/// <summary>
			/// do not emit block size (in @block)
			/// </summary>
			Nosize = 1 << 2,
			/// <summary>
			/// after 'do' (used in @statements - terminate on while/until)
			/// </summary>
			Wasdo = 1 << 3,
			/// <summary>
			/// after 'if' (used in @statements - terminate on else)
			/// </summary>
			Wasif = 1 << 4,
			/// <summary>
			/// parse only one class (used in @classes when called from @expression)
			/// </summary>
			Single = 1 << 5,
			/// <summary>
			/// top level of member function (can become property)
			/// </summary>
			Member = 1 << 6,
			/// <summary>
			/// inside `with` statement (.x => {ivalue}.x, not this.x)
			/// </summary>
			With = 1 << 16,
			/// <summary>
			/// in static method/class (.x not allowed, unless inside `with`)
			/// </summary>
			Static = 1 << 17,
		}
		
		/// <summary>
		/// parse compilation unit (full source file / script) from string
		/// </summary>
		public void Unit(string @string)
		{
			Reader = new StringReader(@string);
			Unit();
			Reader = null;
		}
		
		/// <summary>
		/// parse compilation unit (full source file / script) from stream
		/// </summary>
		public void Unit(TextReader textReader, bool dispose)
		{
			if (!dispose)
			{
				Reader = textReader;
				Unit();
			}
			else
			{
				using (Reader = textReader)
				{
					Unit();
				}
			}
			Reader = null;
		}
		
		/// <summary>
		/// parse compilation unit (full source file / script) from stream
		/// </summary>
		public void Unit(Stream stream, bool dispose)
		{
			Unit(new StreamReader(stream), dispose);
		}
		
		/// <summary>
		/// parent indentation (childs must have higher except labels that can have same)
		/// </summary>
		protected int Pindent = -1;
		/// <summary>
		/// parser saved context (for current line only)
		/// </summary>
		public struct SavedAt
		{
			/// <summary>
			/// parser position
			/// </summary>
			public int At;
			/// <summary>
			/// previous indentation (pindent)
			/// </summary>
			public int Pi;
			public SavedAt(Parser parser)
			{
				At = parser.At;
				Pi = parser.Pindent;
			}
		}
		
		/// <summary>
		/// revert parser state
		/// </summary>
		protected Parser Revert(SavedAt savedAt)
		{
			Pindent = savedAt.Pi;
			if (At == savedAt.At)
			{
				return this;
			}
			End = savedAt.At;
			Next();
			return this;
		}
		
		/// <summary>
		/// parse compilation unit (full source file / script) from the reader or line already set on scanner
		/// </summary>
		protected virtual void Unit()
		{
			Pindent = -1;
			Imports(Flag.None);
			do
			{
				while (unchecked((byte)Opcode) == unchecked((byte)Opcode.Space))
				{
					var opword = Word;
					var ns = Next(true).Word;
					if (ns == null)
					{
						throw new ParseError(this, "Expected namespace or type name after '{0}'", opword);
					}
					while (Next().Opcode == Opcode.Dot)
					{
						if (Next(true).Word == null)
						{
							throw new ParseError(this, "Expected namespace or type name after '.'");
						}
						ns += "." + Word;
					}
					Cgen.Ident(Opcode.Space, ns);
				}
			}
			while (Classes(Flag.None));
			Block(Flag.Nosize);
		}
		
		/// <summary>
		/// parse imports (use, using, import)
		/// </summary>
		protected virtual bool Imports(Flag flags)
		{
			if (unchecked((byte)Opcode) != unchecked((byte)Opcode.Import) && Opcode != Opcode.Using)
			{
				return false;
			}
			do
			{
				var opword = Word;
				var ns = Next(true).Word;
				if (ns == null)
				{
					throw new ParseError(this, "Expected namespace or type name after '{0}'", opword);
				}
				string ns2 = null;
				for (;;)
				{
					while (Next().Opcode == Opcode.Dot)
					{
						if (Next(true).Word == null)
						{
							throw new ParseError(this, "Expected namespace or type name after '.'");
						}
						ns += "." + Word;
					}
					Cgen.Ident(Opcode.Import, ns);
					while (Curr == ':')
					{
						if (Next(true).Word == null)
						{
							if (Eol)
							{
								break;
							}
							throw new ParseError(this, "Expected namespace or type name after ':'");
						}
						ns2 = ns;
						ns = (ns + ".") + Word;
						while (Next().Opcode == Opcode.Dot)
						{
							if (Next(true).Word == null)
							{
								throw new ParseError(this, "Expected namespace or type name after '.'");
							}
							ns += "." + Word;
						}
						Cgen.Ident(Opcode.Import, ns);
					}
					if (Curr != ',')
					{
						break;
					}
					do
					{
						Next(true);
					}
					while (Curr == ',');
					if (Word == null)
					{
						if (Eol)
						{
							break;
						}
						throw new ParseError(this, "Expected namespace or type name after ','");
					}
					if (ns2 == null)
					{
						ns = Word;
					}
					else
					{
						ns = (ns2 + ".") + Word;
					}
				}
				if (Curr == ';')
				{
					do
					{
						Next();
					}
					while (Curr == ';');
					if (!Eol)
					{
						continue;
					}
				}
				if (!Eol)
				{
					throw new ParseError(this, "Expected end of line after import(s)");
				}
				NextLine();
			}
			while (unchecked((byte)Opcode) == unchecked((byte)Opcode.Import) || Opcode == Opcode.Using);
			return true;
		}
		
		public new Parser Next()
		{
			this.Next_();
			return this;
		}
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected override void Next_()
		{
			base.Next_();
		}
		
		public new Parser Next(bool line, bool skipEmpty = true)
		{
			this.Next_(line, skipEmpty);
			return this;
		}
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected override void Next_(bool line, bool skipEmpty = true)
		{
			base.Next_(line, skipEmpty);
		}
	}
}
