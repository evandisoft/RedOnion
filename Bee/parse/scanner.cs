using System;
using System.IO;
using System.Diagnostics;

namespace Bee
{
	[DebuggerDisplay("{_lnum}:{_at}: {_curr}; {_word}; {_line}")]
	public class Scanner
	{
		public Scanner(  )
		{
			Line = null;
		}//.ctor
		
		public Scanner( string line )
		{
			this.Line = line;
		}//.ctor
		
		public Scanner( TextReader textReader )
		{
			Reader = textReader;
		}//.ctor
		
		private TextReader _reader;
		/// <summary>
		/// source text reader
		/// </summary>
		public TextReader Reader
		{
			get => _reader;
			set
			{
				_reader = value;
				SetLine( null );
				Lnum = 0;
				State = 0;
				NextLine();
			}
		}//Reader
		
		private string _line;
		/// <summary>
		/// current line
		/// </summary>
		public string Line
		{
			get => _line;
			set
			{
				_reader = null;
				SetLine( value );
				Next();
			}
		}//Line
		
		public Scanner SetLine( string value )
		{
			this.SetLine_( value );
			return this;
		}//SetLine
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void SetLine_( string value )
		{
			_line = value;
			Lnum++;
			At = 0;
			End = 0;
			Word = null;
			if( !Instr )
			{
				Curr = '\0';
			}
			TabExtra = 0;
			Indent = -1;
		}//SetLine_
		
		private int _lnum;
		/// <summary>
		/// current line number
		/// </summary>
		public int Lnum
		{
			get => _lnum;
			set
			{
				_lnum = value;
			}
		}//Lnum
		
		private int _at;
		/// <summary>
		/// start of current token/lexeme (index of first character)
		/// </summary>
		public int At
		{
			get => _at;
			protected set
			{
				_at = value;
			}
		}//At
		
		private int _end;
		/// <summary>
		/// end of current token/lexeme (index after last character)
		/// </summary>
		public int End
		{
			get => _end;
			protected set
			{
				_end = value;
			}
		}//End
		
		private string _word;
		/// <summary>
		/// current word or null (if not word)
		/// </summary>
		public string Word
		{
			get => _word;
			protected set
			{
				_word = value;
			}
		}//Word
		
		private char _curr;
		/// <summary>
		/// current character or '\0' (if word)
		/// @note identifies type of token, which for strings means " or ' after any $@
		/// </summary>
		public char Curr
		{
			get => _curr;
			protected set
			{
				_curr = value;
			}
		}//Curr
		
		private bool _white;
		/// <summary>
		/// indicates that there was some white-space before current token
		/// </summary>
		public bool White
		{
			get => _white;
			protected set
			{
				_white = value;
			}
		}//White
		
		private char _iesc = '$';
		/// <summary>
		/// escape character for identifiers (words - e.g. $ for B#, @ for C#)
		/// </summary>
		public char Iesc
		{
			get => _iesc;
			set
			{
				_iesc = value;
			}
		}//Iesc
		
		private byte _state;
		/// <summary>
		/// scanner state
		/// </summary>
		public byte State
		{
			get => _state;
			set
			{
				_state = value;
			}
		}//State
		
		public const byte TEXT = 0;
		public const byte CHAR = 1;
		public const byte STRING = 2;
		public const byte VERBATIM = 3;
		public const byte COMMENT = 4;
		/// <summary>
		/// normal state (text)
		/// </summary>
		public bool Normal => State == TEXT;
		/// <summary>
		/// still inside string (or character literal)
		/// </summary>
		public bool Instr => (State >= CHAR) && (State <= VERBATIM);
		/// <summary>
		/// still inside multi-line comment
		/// </summary>
		public bool Comment => State == COMMENT;
		/// <summary>
		/// end of line indicator (curr == '\n')
		/// </summary>
		public bool Eol => Curr == '\n';
		/// <summary>
		/// end of file indicator
		/// </summary>
		public bool Eof => Line == null;
		/// <summary>
		/// next is end of line
		/// </summary>
		public bool PeekEol => Eof || (End >= Line.Length);
		/// <summary>
		/// next (look-ahead) char ('\n' for end of line)
		/// </summary>
		public char Peek => PeekEol ? '\n' : Line[End];
		/// <summary>
		/// next (look-ahead) char is white (or end of line/file)
		/// </summary>
		public bool PeekWhite => System.Char.IsWhiteSpace( Peek );
		/// <summary>
		/// peek characters after (or before) current token
		/// </summary>
		public char PeekAt( int i )
		{
			i += End;
			return Eof ? i == 0 ? '\n' : '\0' : i < Line.Length ? i >= 0 ? Line[i] : i == -1 ? '\n' : '\0' : i == Line.Length ? '\n' : '\0';
		}//PeekAt
		
		/// <summary>
		/// get character of current token
		/// </summary>
		public char CharAt( int i )
		{
			i += At;
			return i < End ? Line[i] : '\0';
		}//CharAt
		
		private int _tabWidth = 4;
		/// <summary>
		/// makes the difference between @col and @at
		/// </summary>
		public int TabWidth
		{
			get => _tabWidth;
			set
			{
				_tabWidth = value;
			}
		}//TabWidth
		
		private int _tabExtra;
		/// <summary>
		/// the difference between @col and @at (extra artifitial spaces)
		/// </summary>
		protected int TabExtra
		{
			get => _tabExtra;
			set
			{
				_tabExtra = value;
			}
		}//TabExtra
		
		/// <summary>
		/// column of current token (uses @tabWidth)
		/// </summary>
		public int Col
		{
			get => At + TabExtra;
			protected set
			{
				TabExtra = value - At;
			}
		}//Col
		
		private int _indent;
		/// <summary>
		/// indentation of current line (in spaces, tabs converted, -1 if undecided or EOF)
		/// </summary>
		public int Indent
		{
			get => _indent;
			protected set
			{
				_indent = value;
			}
		}//Indent
		
		/// <summary>
		/// indicates that scanner is at first token of the line (after leading whitespace)
		/// </summary>
		public bool First => Indent == Col;
		/// <summary>
		/// return true if @c starts a word
		/// (e.g. c == '_' || char.isLetter c)
		/// </summary>
		protected virtual bool Wstart( char c )
		{
			return (c == '_') || System.Char.IsLetter( c );
		}//Wstart
		
		/// <summary>
		/// return true if @c is still in a word
		/// (e.g. c == '_' || char.isLetterOrDigit c)
		/// </summary>
		protected virtual bool Wcont( char c )
		{
			return (c == '_') || System.Char.IsLetterOrDigit( c );
		}//Wcont
		
		/// <summary>
		/// return true if @curr starts a word
		/// (e.g. curr == '_' || char.isLetter curr)
		/// </summary>
		protected bool Wstart(  )
		{
			return Wstart( Curr );
		}//Wstart
		
		/// <summary>
		/// return true if @curr is still in a word
		/// (e.g. curr == '_' || char.isLetterOrDigit curr)
		/// </summary>
		protected bool Wcont(  )
		{
			return Wcont( Curr );
		}//Wcont
		
		/// <summary>
		/// read next line and parse first token
		/// @skipEmpty skip empty lines (whitespace-only)
		/// @remarks Won't call @next() if @skipEmpty = false
		/// </summary>
		public Scanner NextLine( bool skipEmpty = true )
		{
			do
			{
				SetLine( Reader?.ReadLine() );
				if( Eof )
				{
					return this;
				}
				if( !skipEmpty )
				{
					return this;
				}
				Next();
			}
			while( Eol );
			return this;
		}//NextLine
		
		/// <summary>
		/// parse next word, literal or character on this or next (non-empty) line
		/// </summary>
		public Scanner Next( bool line, bool skipEmpty = true )
		{
			this.Next_( line, skipEmpty );
			return this;
		}//Next
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void Next_( bool line, bool skipEmpty = true )
		{
			if( line && Eol )
			{
				NextLine( skipEmpty );
			}
			else
			{
				Next();
				if( line && Eol )
				{
					NextLine( skipEmpty );
				}
			}
		}//Next_
		
		/// <summary>
		/// parse next word, literal or character on this line
		/// </summary>
		public Scanner Next(  )
		{
			this.Next_();
			return this;
		}//Next
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void Next_(  )
		{
			if( Instr )
			{
				SkipString( State == VERBATIM );
				return;
			}
			Word = null;
			Curr = '\0';
			SkipWhite();
			At = End;
			if( Indent < 0 )
			{
				Indent = Col;
			}
			Curr = Peek;
			if( Eol )
			{
				return;
			}
			if( Wstart() )
			{
				SkipWord();
				Word = Line.Substring( At, End - At );
				return;
			}
			var ahead = PeekAt( 1 );
			if( (Curr == Iesc) && Wstart( ahead ) )
			{
				End++;
				SkipWord();
				Word = Line.Substring( At, End - At );
				return;
			}
			if( System.Char.IsDigit( Curr ) )
			{
				for( ; ;  )
				{
					if( (++End) == Line.Length )
					{
						break;
					}
					var prev = Curr;
					Curr = Line[End];
					if( ((Curr == '+') || (Curr == '-')) && ((prev == 'e') || (prev == 'E')) )
					{
						continue;
					}
					if( (!Wcont()) && ((Curr != '.') || (!System.Char.IsDigit( PeekAt( 1 ) ))) )
					{
						break;
					}
				}
				Curr = Line[At];
				return;
			}
			if( (Curr == '"') || (Curr == '\'') )
			{
				SkipString();
				return;
			}
			if( (Curr == '@') || (Curr == '$') )
			{
				if( (ahead == '"') || (ahead == '\'') )
				{
					End++;
					var verbatim = Curr == '@';
					Curr = ahead;
					SkipString( verbatim );
					return;
				}
				if( (Curr == '$') && (ahead == '@') )
				{
					var ahead2 = PeekAt( 2 );
					if( (ahead2 == '"') || (ahead2 == '\'') )
					{
						End += 2;
						Curr = ahead2;
						SkipString( true );
						return;
					}
				}
			}
			End++;
			return;
		}//Next_
		
		/// <summary>
		/// read one character from token
		/// </summary>
		public char Read(  )
		{
			return At == End ? '\0' : Line[At++];
		}//Read
		
		/// <summary>
		/// read rest of the token (or whole token if at start of it) as string
		/// </summary>
		public string Rest(  )
		{
			var s = Line.Substring( At, End - At );
			At = End;
			return s;
		}//Rest
		
		/// <summary>
		/// skip one character (including end of line)
		/// @return the skipped character or '\n' if next line was read
		/// </summary>
		public char Skip(  )
		{
			if( PeekEol )
			{
				NextLine();
				return '\n';
			}
			return Line[End++];
		}//Skip
		
		/// <summary>
		/// skip whole word or numeric literal
		/// </summary>
		public Scanner SkipWord(  )
		{
			if( Eol )
			{
				throw new InvalidOperationException( "No word" );
			}
			do
			{
				if( (++End) == Line.Length )
				{
					break;
				}
				Curr = Line[End];
			}
			while( Wcont() );
			Curr = Line[At];
			return this;
		}//SkipWord
		
		/// <summary>
		/// parse and return next word (throws if not word)
		/// </summary>
		public string NextWord(  )
		{
			return Next().ReadWord();
		}//NextWord
		
		/// <summary>
		/// read last word (throws if not word)
		/// </summary>
		public string ReadWord(  )
		{
			if( Word == null )
			{
				throw new ParseError( this, "Expected word" );
			}
			return Word;
		}//ReadWord
		
		/// <summary>
		/// skip/read white space (but not comments) on this line
		/// </summary>
		public Scanner SkipSimpleWhite(  )
		{
			White = End == 0;
			for( ; ;  )
			{
				if( PeekEol )
				{
					Curr = '\n';
					return this;
				}
				var c = Peek;
				if( System.Char.IsWhiteSpace( c ) )
				{
					White = true;
					do
					{
						if( c == '\t' )
						{
							TabExtra += (TabWidth - 1) - (Col % TabWidth);
						}
					}
					while( ((++End) < Line.Length) && System.Char.IsWhiteSpace( c = Line[End] ) );
					continue;
				}
				return this;
			}
			return this;
		}//SkipSimpleWhite
		
		/// <summary>
		/// skip/read white space (including comments) on this line
		/// </summary>
		public Scanner SkipWhite(  )
		{
			this.SkipWhite_();
			return this;
		}//SkipWhite
		
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
		protected virtual void SkipWhite_(  )
		{
			White = End == 0;
			if( Comment )
			{
				MultiLineComment();
				if( Comment )
				{
					return;
				}
			}
			for( ; ;  )
			{
				if( PeekEol )
				{
					Curr = '\n';
					return;
				}
				var c = Peek;
				if( System.Char.IsWhiteSpace( c ) )
				{
					White = true;
					do
					{
						if( c == '\t' )
						{
							TabExtra += (TabWidth - 1) - (Col % TabWidth);
						}
					}
					while( ((++End) < Line.Length) && System.Char.IsWhiteSpace( c = Line[End] ) );
					continue;
				}
				if( (c == '#') && PreprocOrComment() )
				{
					White = true;
					break;
				}
				if( (c == '/') && ((End + 1) < Line.Length) )
				{
					c = Line[End + 1];
					if( (c == '/') && SingleLineComment() )
					{
						White = true;
						break;
					}
					if( (c == '*') && MultiLineComment() )
					{
						White = true;
						if( Comment )
						{
							return;
						}
						continue;
					}
				}
				return;
			}
			if( Eol )
			{
				Curr = '\n';
			}
		}//SkipWhite_
		
		/// <summary>
		/// parse preprocessor line or comment: #...
		/// @return true if supported
		/// </summary>
		protected virtual bool PreprocOrComment(  )
		{
			End = Line.Length;
			return true;
		}//PreprocOrComment
		
		/// <summary>
		/// parse single line comment: //...
		/// @return true if supported
		/// </summary>
		protected virtual bool SingleLineComment(  )
		{
			End = Line.Length;
			return true;
		}//SingleLineComment
		
		/// <summary>
		/// parse multi line comment: /*...*/
		/// @return true if supported
		/// </summary>
		protected virtual bool MultiLineComment(  )
		{
			if( State != COMMENT )
			{
				End += 2;
			}
			State = COMMENT;
			for( ; ;  )
			{
				if( PeekEol )
				{
					if( NextLine( false ).Eof )
					{
						return true;
					}
					continue;
				}
				if( ((Line[End++] == '*') && (End < Line.Length)) && (Line[End] == '/') )
				{
					End++;
					State = TEXT;
					return true;
				}
			}
		}//MultiLineComment
		
		/// <summary>
		/// skip/read string or char literal
		/// @verbatim don't interpret control characters
		/// @remarks any character (except '\' if not verbatim) can be used as delimiter
		/// which means that it can be used to read e.g. /regex/
		/// </summary>
		public Scanner SkipString( bool verbatim = false )
		{
			var kind = Curr;
			if( !Instr )
			{
				State = verbatim ? VERBATIM : kind == '\'' ? CHAR : STRING;
				if( !PeekEol )
				{
					End++;
				}
			}
			for( ; ;  )
			{
				if( PeekEol )
				{
					return this;
				}
				var c = Line[End++];
				if( c == kind )
				{
					break;
				}
				if( ((!verbatim) && (c == '\\')) && (End < Line.Length) )
				{
					End++;
				}
			}
			State = TEXT;
			return this;
		}//SkipString
	}//Scanner
}//Bee
