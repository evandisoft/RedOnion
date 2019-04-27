using System;
using System.Diagnostics;
using System.Globalization;

namespace RedOnion.ROS.Parsing
{
	[DebuggerDisplay("{LineNumber}:{At}: {ExCode}; {Curr}; {Word}; {Line}")]
	public partial class Parser : Lexer
	{
		public static readonly Option DefaultOptions
			= Option.Autocall
			| Option.Untyped | Option.Typed	// both typed and untyped variables allowed
			| Option.ArrayLiteral;

		public CultureInfo Culture { get; set; } = Value.Culture;
		public Option Options { get; set; } = DefaultOptions;
		public bool HasOption(Option opt)
			=> (Options & opt) != 0;

		public Parser() { }
		public Parser(Option opts) => Options = opts;

		[Flags]
		public enum Option : uint
		{
			None = 0,
			/// <summary>
			/// Prefix notation of the code is good for analyzers,
			/// code generators and recursive execution.
			/// Postfix notation is good for execution with pause-continue.
			/// </summary>
			Prefix = 1 << 0,
			/// <summary>
			/// Strongly typed language (require static types, unless Untyped is also set)
			/// </summary>
			Typed = 1 << 1,
			/// <summary>
			/// Untyped language (no static types, unless Typed is also set)
			/// </summary>
			Untyped = 1 << 2,
			/// <summary>
			/// Translate e.g. `abs -x` into `abs(-x)`.
			/// </summary>
			AutocallWhenArgs = 1 << 3,
			/// <summary>
			/// Convert simple statements (identifier or root is dot)
			/// into function call (e.g. `stage` becomes `stage()`).
			/// </summary>
			AutocallSimple = 1 << 4,
			/// <summary>
			/// Both versions of function call without parentheses
			/// </summary>
			Autocall = AutocallWhenArgs | AutocallSimple,
			/// <summary>
			/// Array literal like in JavaScript: var a = [1, 2, 3]
			/// </summary>
			ArrayLiteral = 1 << 5,
		}

		[Flags]
		public enum Flag : uint
		{
			None = 0,
			/// <summary>
			/// Expression in limited context (inside if/while/using... condition/declaration).
			/// Disable multiline object initializers and labmdas (until parenthesis or bracket)
			/// </summary>
			Limited = 1 << 0,
			/// <summary>
			/// Expression in hungry context which is inside parenthesis or brackets.
			/// Parse lines until comman or ending parenthesis/bracket is encountered
			/// </summary>
			Hungry = 1 << 1,
			/// <summary>
			/// Allow empty/no expression/type (used in FullExpression and FullType)
			/// </summary>
			NoExpression = 1 << 2,
			/// <summary>
			/// Do not emit block size (in Block)
			/// </summary>
			NoSize = 1 << 3,
			/// <summary>
			/// After 'do' (used in Statements - terminate on while/until)
			/// </summary>
			WasDo = 1 << 4,
			/// <summary>
			/// After 'if' (used in Statements - terminate on else)
			/// </summary>
			WasIf = 1 << 5,
			/// <summary>
			/// Parse only one class (used in Classes when called from Expression)
			/// </summary>
			Single = 1 << 6,
			/// <summary>
			/// Top level of member function (can become property)
			/// </summary>
			Member = 1 << 7,

			/// <summary>
			/// Inside `with` statement (.x => {ivalue}.x, not this.x)
			/// </summary>
			With = 1 << 16,
			/// <summary>
			/// In static method/class (.x not allowed, unless inside `with`)
			/// </summary>
			Static = 1 << 17,
		}

		/// <summary>
		/// Reset the state of parser (empty all stacks and buffers)
		/// </summary>
		public Parser Reset()
		{
			strings.Clear();
			stringValues.Clear();
			stringMap.Clear();
			code.Clear();
			operators.Clear();
			values.Clear();
			lineMap.Clear();
			lines.Clear();
			ctx?.Clear();
			stack.Clear();
			labelTable?.Clear();
			gotoTable?.Clear();
			ParentIndent = -1;
			return this;
		}

		/// <summary>
		/// Compile provided expression to code buffer
		/// </summary>
		public Parser Expression(string value)
		{
			Reset();
			Source = value;
			var state = StartExpression();
			ParseExpression();
			FinishExpression(state);
			Source = null;
			return this;
		}

		/// <summary>
		/// Compile full source file / script from string
		/// </summary>
		public Parser Unit(string source)
		{
			Reset();
			Source = source;
			Unit();
			Source = null;
			return this;
		}

		public CompiledCode Compile(string source, string path = null)
		{
			Unit(source);
			return new CompiledCode(
				StringsToArray(),
				CodeToArray(),
				LineMapToArray(),
				HasOption(Option.Prefix))
			{
				Path = path,
				Source = source,
				Lines = LinesToArray()
			};
		}

		/// <summary>
		/// Parse next word, literal, operator or character on current line
		/// </summary>
		public new Parser Next()
		{
			NextToken();
			return this;
		}
		/// <summary>
		/// Parse next word, literal or character on this or next (non-empty) line
		/// </summary>
		public new Parser Next(bool line, bool skipEmpty = true)
		{
			NextToken(line, skipEmpty);
			return this;
		}

		/// <summary>
		/// Parent indentation (childs must have higher except labels that can have same)
		/// </summary>
		public int ParentIndent = -1;

		/// <summary>
		/// Parse compilation unit (full source file / script)
		/// from the reader or line already set on lexer
		/// </summary>
		public virtual void Unit()
		{
			ParentIndent = -1;
			Imports(Flag.None);
			do
			{
				while (ExCode.Code() == ExCode.Namespace.Code())
				{
					var opword = Word;
					var ns = Next(true).Word;
					if (ns == null)
						throw new ParseError(this, "Expected namespace or type name after '{0}'", opword);
					while (Next().ExCode == ExCode.Dot)
					{
						if (Next(true).Word == null)
							throw new ParseError(this, "Expected namespace or type name after '.'");
						ns += "." + Word;
					}
					Write(OpCode.Namespace, ns);
				}
			}
			while (ParseClasses(Flag.None));

			ParseBlock(Flag.NoSize);
		}

		/// <summary>
		/// Parse imports
		/// </summary>
		protected virtual bool Imports(Flag flags)
		{
			// note: only 'import' now supported ('use' is too short and 'using' is statement)
			// warn: make sure the condition at the end is the same!
			if (ExCode != ExCode.Import)
				return false;
			do
			{
				var opword = Word;
				var ns = Next(true).Word;
				if (ns == null)
					throw new ParseError(this, "Expected namespace or type name after '{0}'", opword);
				string ns2 = null;
				for (;;)
				{
					while (Next().ExCode == ExCode.Dot)
					{
						if (Next(true).Word == null)
							throw new ParseError(this, "Expected namespace or type name after '.'");
						ns += "." + Word;
					}
					Write(OpCode.Import, ns);

					// import system: io, text => import system; import system.io; import system.text
					while (Curr == ':')
					{
						if (Next(true).Word == null)
						{
							if (Eol)
								break;
							throw new ParseError(this, "Expected namespace or type name after ':'");
						}
						ns2 = ns;
						ns = ns + "." + Word;
						while (Next().ExCode == ExCode.Dot)
						{
							if (Next(true).Word == null)
								throw new ParseError(this, "Expected namespace or type name after '.'");
							ns += "." + Word;
						}
						Write(OpCode.Import, ns);
					}
					if (Curr != ',')
						break;
					do Next(true); while (Curr == ',');
					if (Word == null)
					{
						if (Eol)
							break;
						throw new ParseError(this, "Expected namespace or type name after ','");
					}
					ns = ns2 == null ? Word : ns2 + "." + Word;
				}
				if (Curr == ';')
				{
					do Next(); while (Curr == ';');
					if (!Eol)
						continue;
				}
				if (!Eol)
					throw new ParseError(this, "Expected end of line after import(s)");
				NextLine();
			}
			while (ExCode == ExCode.Import);
			return true;
		}
	}
}
