using System;
using System.Diagnostics;
using System.Globalization;

namespace RedOnion.Script.Parsing
{
	[DebuggerDisplay("{lexer.LineNumber}:{lexer.At}: {lexer.Code}; {lexer.Curr}; {lexer.Word}; {lexer.Line}")]
	public partial class Parser
	{
		protected Lexer lexer = new Lexer();

		public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
		public Option Options { get; set; }
			= Option.Script | Option.Autocall | Option.Untyped | Option.Typed;
		public bool HasOption(Option opt)
			=> (Options & opt) != 0;

		public Parser() { }
		public Parser(Option opts) => Options = opts;
		public Parser(Option opton, Option optoff) => Options = (Options | opton) &~ optoff;

		[Flags]
		public enum Option : uint
		{
			None = 0,
			/// <summary>
			/// Targeting stript engine (not .NET)
			/// </summary>
			Script = 1 << 0,
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
			/// "x .y" => "x(this.y)" if set, "x.y" otherwise
			/// </summary>
			/// 
			/// <remarks>
			/// This option was designed for constructors and normal `this` shortcuts like:
			/// 
			///   .ctor name string, value // name:string, value:value or name as string, value as value
			///     .name = name           // this.name = name
			///     .value = value         // this.value = value
			///
			/// Blocks indented one more after expression statement
			/// are converted to 'with' statement regardless of this setting:
			/// <code>
			///   getObject
			///     .doSomething
			/// </code>
			/// becomes:
			/// <code>
			///   with getObject
			///     .doSomething
			/// </code>
			/// equivalent of:
			/// <code>
			///   var it = getObject()
			///   it.doSomething()
			/// </code>
			/// </remarks>
			DotThisAfterWhite = 1 << 16,
		}

		[Flags]
		public enum Flag : uint
		{
			None = 0,
			/// <summary>
			/// Expression in limited context (inside if/while/using... condition/declaration)
			/// </summary>
			LimitedContext = 1 << 0,
			/// <summary>
			/// Allow empty/no expression/type (used in FullExpression and FullType)
			/// </summary>
			NoExpression = 1 << 1,
			/// <summary>
			/// Do not emit block size (in Block)
			/// </summary>
			NoSize = 1 << 2,
			/// <summary>
			/// After 'do' (used in Statements - terminate on while/until)
			/// </summary>
			WasDo = 1 << 3,
			/// <summary>
			/// After 'if' (used in Statements - terminate on else)
			/// </summary>
			WasIf = 1 << 4,
			/// <summary>
			/// Parse only one class (used in Classes when called from Expression)
			/// </summary>
			Single = 1 << 5,
			/// <summary>
			/// Top level of member function (can become property)
			/// </summary>
			Member = 1 << 6,

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
			if (StringsAt > 0)
			{
				Array.Clear(_stringTable, 0, StringsAt);
				StringsAt = 0;
			}
			if (StringValuesAt > 0)
			{
				Array.Clear(StringValues, 0, StringValuesAt);
				StringValuesAt = 0;
			}
			_stringMap.Clear();
			CodeAt = 0;
			OperatorAt = 0;
			ValuesAt = 0;
			LabelTable?.Clear();
			GotoTable?.Clear();
			ParentIndent = -1;
			return this;
		}

		/// <summary>
		/// Compile provided expression to code buffer
		/// </summary>
		public Parser Expression(string value)
		{
			Reset();
			lexer.Source = value;
			var state = StartExpression();
			ParseExpression();
			FinishExpression(state);
			lexer.Source = null;
			return this;
		}

		/// <summary>
		/// Compile full source file / script from string
		/// </summary>
		public Parser Unit(string source)
		{
			Reset();
			lexer.Source = source;
			Unit();
			lexer.Source = null;
			return this;
		}

		/// <summary>
		/// Parse next word, literal, operator or character on current line
		/// </summary>
		protected Parser Next()
		{
			lexer.Next();
			return this;
		}
		/// <summary>
		/// Parse next word, literal or character on this or next (non-empty) line
		/// </summary>
		protected Parser Next(bool line, bool skipEmpty = true)
		{
			lexer.Next(line, skipEmpty);
			return this;
		}

		/// <summary>
		/// Parent indentation (childs must have higher except labels that can have same)
		/// </summary>
		protected int ParentIndent = -1;

		/// <summary>
		/// Parse compilation unit (full source file / script)
		/// from the reader or line already set on lexer
		/// </summary>
		protected virtual void Unit()
		{
			ParentIndent = -1;
			Imports(Flag.None);
			do
			{
				while (lexer.Code.Code() == OpCode.Namespace.Code())
				{
					var opword = lexer.Word;
					var ns = Next(true).lexer.Word;
					if (ns == null)
						throw new ParseError(lexer, "Expected namespace or type name after '{0}'", opword);
					while (Next().lexer.Code == OpCode.Dot)
					{
						if (Next(true).lexer.Word == null)
							throw new ParseError(lexer, "Expected namespace or type name after '.'");
						ns += "." + lexer.Word;
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
			if (lexer.Code != OpCode.Import)
				return false;
			do
			{
				var opword = lexer.Word;
				var ns = Next(true).lexer.Word;
				if (ns == null)
					throw new ParseError(lexer, "Expected namespace or type name after '{0}'", opword);
				string ns2 = null;
				for (;;)
				{
					while (Next().lexer.Code == OpCode.Dot)
					{
						if (Next(true).lexer.Word == null)
							throw new ParseError(lexer, "Expected namespace or type name after '.'");
						ns += "." + lexer.Word;
					}
					Write(OpCode.Import, ns);

					// import system: io, text => import system; import system.io; import system.text
					while (lexer.Curr == ':')
					{
						if (Next(true).lexer.Word == null)
						{
							if (lexer.Eol)
								break;
							throw new ParseError(lexer, "Expected namespace or type name after ':'");
						}
						ns2 = ns;
						ns = ns + "." + lexer.Word;
						while (Next().lexer.Code == OpCode.Dot)
						{
							if (Next(true).lexer.Word == null)
								throw new ParseError(lexer, "Expected namespace or type name after '.'");
							ns += "." + lexer.Word;
						}
						Write(OpCode.Import, ns);
					}
					if (lexer.Curr != ',')
						break;
					do Next(true); while (lexer.Curr == ',');
					if (lexer.Word == null)
					{
						if (lexer.Eol)
							break;
						throw new ParseError(lexer, "Expected namespace or type name after ','");
					}
					ns = ns2 == null ? lexer.Word : ns2 + "." + lexer.Word;
				}
				if (lexer.Curr == ';')
				{
					do Next(); while (lexer.Curr == ';');
					if (!lexer.Eol)
						continue;
				}
				if (!lexer.Eol)
					throw new ParseError(lexer, "Expected end of line after import(s)");
				lexer.NextLine();
			}
			while (lexer.Code == OpCode.Import);
			return true;
		}
	}
}
