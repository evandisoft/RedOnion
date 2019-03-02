using System;

namespace RedOnion.Script.Parsing
{
	public partial class Parser
	{
		protected Lexer lexer = new Lexer();

		public Option Options { get; set; } = Option.Script | Option.DotThisAfterWhite;

		[Flags]
		public enum Option : uint
		{
			None = 0,
			/// <summary>
			/// Targeting stript engine (not .NET)
			/// </summary>
			Script = 1 << 0,
			/// <summary>
			/// Strongly typed language (require static types)
			/// </summary>
			Typed = 1 << 1,
			/// <summary>
			/// Untyped language (no static types)
			/// </summary>
			Untyped = 1 << 2,

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
			/// Allow empty/no expression/type (used in fullexpr and fulltype)
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
			CodeAt = 0;
			StringsAt = 0;
			Array.Clear(_stringTable, 0, _stringTable.Length);
			_stringMap.Clear();
			OperatorAt = 0;
			ValuesAt = 0;
			StringValuesAt = 0;
			Array.Clear(StringValues, 0, StringValues.Length);
			return this;
		}

		/// <summary>
		/// Compile expression at parser position to code buffer
		/// </summary>
		public Parser Expression()
		{
			var at = ValuesAt;
			ParseExpression();
			Rewrite(ValuesAt);
			ValuesAt = at;
			return this;
		}
	}
}
