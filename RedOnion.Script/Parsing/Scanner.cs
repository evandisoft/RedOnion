using System;
using System.IO;
using System.Diagnostics;

namespace RedOnion.Script.Parsing
{
	[DebuggerDisplay("{LineNumber}:{At}: {Curr}; {Word}; {Line}")]
	public class Scanner
	{
		public Scanner()
			=> Line = null;
		public Scanner(string source)
			=> Source = source;
		public Scanner(TextReader reader)
			=> Reader = reader;

		private TextReader _reader;
		/// <summary>
		/// Source text reader (if used)
		/// </summary>
		public TextReader Reader
		{
			get => _reader;
			set
			{
				_reader = value;
				_source = null;
				SetLine(null);
				LineNumber = 0;
				CharCounter = 0;
				State = 0;
				NextLine();
			}
		}

		private string _source;
		/// <summary>
		/// Full source string (if available)
		/// </summary>
		public string Source
		{
			get => _source;
			set
			{
				_reader = null;
				_source = value;
				SetLine(null);
				LineNumber = 0;
				CharCounter = 0;
				State = 0;
				NextLine();
			}
		}

		private string _line;
		/// <summary>
		/// Current line
		/// </summary>
		public string Line
		{
			get => _line;
			set
			{
				_reader = null;
				_source = null;
				SetLine(value);
				Next();
			}
		}
		public virtual void SetLine(string value)
		{
			_line = value;
			LineNumber++;
			At = 0;
			End = 0;
			Word = null;
			if (!InString)
				Curr = '\0';
			TabExtra = 0;
			Indent = -1;
		}

		/// <summary>
		/// Start of current token (index of first character)
		/// </summary>
		public int At { get; protected set; }
		/// <summary>
		/// End of current token (index after last character)
		/// </summary>
		public int End { get; protected set; }

		/// <summary>
		/// Current line number
		/// </summary>
		public int LineNumber { get; set; }
		/// <summary>
		/// Character counter (position in full source string)
		/// </summary>
		public int CharCounter { get; set; }
		/// <summary>
		/// Current word or null (if not word)
		/// </summary>
		public string Word { get; protected set; }
		/// <summary>
		/// Current character.
		/// Identifies type of token, which for strings means " or ' after any $@
		/// </summary>
		public char Curr { get; protected set; }
		/// <summary>
		/// Indicates that there was some white-space before current token
		/// </summary>
		public bool White { get; protected set; }
		/// <summary>
		/// Escape character for identifiers
		/// </summary>
		public char EscapeChar { get; set; } = '$';
		/// <summary>
		/// Scanner state
		/// </summary>
		public byte State { get; set; }

		// note: not using enum to be able to add more states in derived classes
		public const byte TEXT = 0;
		public const byte CHAR = 1;
		public const byte STRING = 2;
		public const byte VERBATIM = 3;
		public const byte COMMENT = 4;

		/// <summary>
		/// Normal state (text)
		/// </summary>
		public bool Normal => State == TEXT;
		/// <summary>
		/// Inside string (or character literal)
		/// </summary>
		public bool InString => State >= CHAR && State <= VERBATIM;
		/// <summary>
		/// Inside multi-line comment
		/// </summary>
		public bool Comment => State == COMMENT;
		/// <summary>
		/// End of line indicator (Curr == '\n')
		/// </summary>
		public bool Eol => Curr == '\n';
		/// <summary>
		/// End of file indicator
		/// </summary>
		public bool Eof => Line == null;
		/// <summary>
		/// Next (look-ahead) is end of line
		/// </summary>
		public bool PeekEol => Eof || End >= Line.Length;
		/// <summary>
		/// Next (look-ahead) char ('\n' for end of line)
		/// </summary>
		public char Peek => PeekEol ? '\n' : Line[End];
		/// <summary>
		/// Next (look-ahead) char is white (or end of line/file)
		/// </summary>
		public bool PeekWhite => System.Char.IsWhiteSpace(Peek);
		/// <summary>
		/// Peek characters after (or before) current token
		/// </summary>
		public char PeekAt(int i)
		{
			i += End;
			return Eof ? i == 0 ? '\n' : '\0' :
				i < Line.Length ? i >= 0 ? Line[i] :
				i == -1 ? '\n' : '\0' :
				i == Line.Length ? '\n' : '\0';
		}
		/// <summary>
		/// Get character of current token
		/// </summary>
		public char CharAt(int i)
		{
			i += At;
			return i < End ? Line[i] : '\0';
		}
		/// <summary>
		/// Makes the difference between Column and At
		/// </summary>
		public int TabWidth { get; set; } = 4;
		/// <summary>
		/// The difference between Column and At (extra spaces)
		/// </summary>
		protected int TabExtra { get; set; }
		/// <summary>
		/// Column of current token (uses TabWidth)
		/// </summary>
		public int Column
		{
			get => At + TabExtra;
			protected set => TabExtra = value - At;
		}
		/// <summary>
		/// Indentation of current line (in spaces, tabs converted, -1 if undecided or EOF)
		/// </summary>
		public int Indent { get; protected set; }
		/// <summary>
		/// Indicates that scanner is at first token of the line (after leading whitespace)
		/// </summary>
		public bool First => Indent == Column;

		/// <summary>
		/// True if char starts a word
		/// (e.g. c == '_' || char.isLetter(c))
		/// </summary>
		protected virtual bool StartsWord(char c)
			=> c == '_' || char.IsLetter(c);
		/// <summary>
		/// True if char is still in a word
		/// (e.g. c == '_' || char.isLetterOrDigit(c))
		/// </summary>
		protected virtual bool InsideWord(char c)
			=> c == '_' || char.IsLetterOrDigit(c);

		/// <summary>
		/// Read next line and parse first token.
		/// </summary>
		/// <param name="skipEmpty">Skip empty lines (whitespace-only)</param>
		/// <remarks>Won't call Next() if skipEmpty = false</remarks>
		public void NextLine(bool skipEmpty = true)
		{
			do
			{
				if (Source == null)
					SetLine(Reader?.ReadLine());
				else 
				{
					if (Line != null)
					{
						char c = '\0';
						while (CharCounter < Source.Length)
						{
							c = Source[CharCounter++];
							if (c == '\0' || c == '\r' || c == '\n')
								break;
						}
						if (c == '\r' || c == '\n')
						{
							while (CharCounter < Source.Length)
							{
								var c2 = Source[CharCounter];
								if (c2 != '\r' && c2 != '\n')
									break;
								if (c2 == c)
									break;
								CharCounter++;
							}
						}
					}
					if (CharCounter >= Source.Length)
						SetLine(null);
					else
					{
						int pos = CharCounter;
						while (pos < Source.Length)
						{
							var c3 = Source[pos];
							if (c3 == '\r' || c3 == '\n')
								break;
							pos++;
						}
						SetLine(Source.Substring(CharCounter, pos-CharCounter));
					}
				}
				if (Eof)
					return;
				if (!skipEmpty)
					return;
				Next();
			}
			while (Eol);
		}

		/// <summary>
		/// Parse next word, literal or character on this or next (non-empty) line
		/// </summary>
		public Scanner Next(bool line, bool skipEmpty = true)
		{
			NextToken(line, skipEmpty);
			return this;
		}
		/// <summary>
		/// Parse next word, literal or character on this or next (non-empty) line
		/// </summary>
		protected virtual void NextToken(bool line, bool skipEmpty = true)
		{
			if (line && Eol)
				NextLine(skipEmpty);
			else
			{
				NextToken();
				if (line && Eol)
					NextLine(skipEmpty);
			}
		}

		/// <summary>
		/// Parse next word, literal or character on this line
		/// </summary>
		public Scanner Next()
		{
			NextToken();
			return this;
		}
		/// <summary>
		/// Parse next word, literal or character on this line
		/// </summary>
		protected virtual void NextToken()
		{
			if (InString)
			{
				SkipString(State == VERBATIM);
				return;
			}
			Word = null;
			Curr = '\0';
			SkipWhite();
			CharCounter += End-At;
			At = End;
			if (Indent < 0)
				Indent = Column;
			Curr = Peek;
			if (Eol)
				return;
			if (StartsWord(Curr))
			{
				SkipWord();
				Word = Line.Substring(At, End - At);
				return;
			}
			var ahead = PeekAt(1);
			if (Curr == EscapeChar && StartsWord(ahead))
			{
				End++;
				SkipWord();
				Word = Line.Substring(At, End - At);
				return;
			}
			if (char.IsDigit(Curr))
			{
				for (;;)
				{
					if (++End == Line.Length)
						break;
					var prev = Curr;
					Curr = Line[End];
					if ((Curr == '+' || Curr == '-') && (prev == 'e' || prev == 'E'))
						continue;
					if (!InsideWord(Curr) && (Curr != '.' || !char.IsDigit(PeekAt(1))))
						break;
				}
				Curr = Line[At];
				return;
			}
			if (Curr == '"' || Curr == '\'')
			{
				SkipString();
				return;
			}
			if (Curr == '@' || Curr == '$')
			{
				if (ahead == '"' || ahead == '\'')
				{
					End++;
					var verbatim = Curr == '@';
					Curr = ahead;
					SkipString(verbatim);
					return;
				}
				if (Curr == '$' && ahead == '@')
				{
					var ahead2 = PeekAt(2);
					if (ahead2 == '"' || ahead2 == '\'')
					{
						End += 2;
						Curr = ahead2;
						SkipString(true);
						return;
					}
				}
			}
			End++;
		}

		/// <summary>
		/// Read one character from token
		/// </summary>
		public char Read()
			=> At == End ? '\0' : Line[At++];
		/// <summary>
		/// Read rest of the token (or whole token if at start of it) as string
		/// </summary>
		public string Rest()
		{
			var s = Line.Substring(At, End - At);
			CharCounter += End-At;
			At = End;
			return s;
		}

		/// <summary>
		/// Skip one character (including end of line)
		/// </summary>
		/// <returns>The skipped character or '\n' if next line was read</returns>
		public char Skip()
		{
			if (PeekEol)
			{
				NextLine();
				return '\n';
			}
			return Line[End++];
		}

		/// <summary>
		/// Skip whole word or numeric literal
		/// </summary>
		public Scanner SkipWord()
		{
			if (Eol)
				throw new InvalidOperationException("No word");
			do
			{
				if (++End == Line.Length)
					break;
				Curr = Line[End];
			}
			while (InsideWord(Curr));
			Curr = Line[At];
			return this;
		}

		/// <summary>
		/// Parse and return next word (throws if not word)
		/// </summary>
		public string NextWord()
			=> Next().ReadWord();
		/// <summary>
		/// Read last word (throws if not word)
		/// </summary>
		public string ReadWord()
		{
			if (Word == null)
				throw new ParseError(this, "Expected word");
			return Word;
		}

		/// <summary>
		/// Skip/read white space (but not comments) on this line
		/// </summary>
		public Scanner SkipSimpleWhite()
		{
			White = End == 0;
			for (;;)
			{
				if (PeekEol)
				{
					Curr = '\n';
					return this;
				}
				var c = Peek;
				if (char.IsWhiteSpace(c))
				{
					White = true;
					do
					{
						if (c == '\t')
							TabExtra += (TabWidth - 1) - (Column % TabWidth);
					}
					while (++End < Line.Length && char.IsWhiteSpace(c = Line[End]));
					continue;
				}
				return this;
			}
		}

		/// <summary>
		/// Skip/read white space (including comments) on this line
		/// </summary>
		public Scanner SkipWhite()
		{
			SkipWhitespace();
			return this;
		}
		/// <summary>
		/// Skip/read white space (including comments) on this line
		/// </summary>
		protected virtual void SkipWhitespace()
		{
			White = End == 0;
			if (Comment)
			{
				MultiLineComment();
				if (Comment)
					return;
			}
			for (;;)
			{
				if (PeekEol)
				{
					Curr = '\n';
					return;
				}
				var c = Peek;
				if (char.IsWhiteSpace(c))
				{
					White = true;
					do
					{
						if (c == '\t')
							TabExtra += (TabWidth - 1) - (Column % TabWidth);
					}
					while (++End < Line.Length && char.IsWhiteSpace(c = Line[End]));
					continue;
				}
				if (c == '#' && PreprocOrComment())
				{
					White = true;
					break;
				}
				if (c == '/' && (End + 1 < Line.Length))
				{
					c = Line[End + 1];
					if (c == '/' && SingleLineComment())
					{
						White = true;
						break;
					}
					if (c == '*' && MultiLineComment())
					{
						White = true;
						if (Comment)
							return;
						continue;
					}
				}
				return;
			}
			if (Eol)
				Curr = '\n';
		}

		/// <summary>
		/// Parse preprocessor line or comment: #...
		/// </summary>
		/// <returns>True if supported</returns>
		protected virtual bool PreprocOrComment()
		{
			End = Line.Length;
			return true;
		}

		/// <summary>
		/// Parse single line comment: //...
		/// </summary>
		/// <returns>True if supported</returns>
		protected virtual bool SingleLineComment()
		{
			End = Line.Length;
			return true;
		}

		/// <summary>
		/// Parse multi line comment: /*...*/
		/// </summary>
		/// <returns>True if supported</returns>
		protected virtual bool MultiLineComment()
		{
			if (State != COMMENT)
				End += 2;
			State = COMMENT;
			for (;;)
			{
				if (PeekEol)
				{
					NextLine(false);
					if (Eof)
						return true;
					continue;
				}
				if (Line[End++] == '*' && End < Line.Length && Line[End] == '/')
				{
					End++;
					State = TEXT;
					return true;
				}
			}
		}

		/// <summary>
		/// Skip/read string or char literal
		/// </summary>
		/// <param name="verbatim">Don't interpret control characters</param>
		/// <remarks>Any character (except '\' if not verbatim) can be used as delimiter
		/// which means that it can be used to read e.g. /regex/</remarks>
		public Scanner SkipString(bool verbatim = false)
		{
			var kind = Curr;
			if (!InString)
			{
				State = verbatim ? VERBATIM : kind == '\'' ? CHAR : STRING;
				if (!PeekEol)
					End++;
			}
			for (;;)
			{
				if (PeekEol)
					return this;
				var c = Line[End++];
				if (c == kind)
					break;
				if (!verbatim && c == '\\' && End < Line.Length)
					End++;
			}
			State = TEXT;
			return this;
		}
	}
}
