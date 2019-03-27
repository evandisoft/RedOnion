using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script.Parsing
{
	public class ParseError : Exception, IErrorWithLine
	{
		/// <summary>
		/// Line with error
		/// </summary>
		public string Line { get; protected set; }
		/// <summary>
		/// Line number of error
		/// </summary>
		public int LineNumber { get; protected set; }
		/// <summary>
		/// Start of erroneous token/span
		/// </summary>
		public int At { get; protected set; }
		/// <summary>
		/// End of erroneous token/span
		/// </summary>
		public int End { get; protected set; }
		/// <summary>
		/// Column of erroneous token/span
		/// </summary>
		public int Column { get; protected set; }
		/// <summary>
		/// Line number of end of erroneous token/span
		/// </summary>
		public int EndLine { get; protected set; }
		/// <summary>
		/// Character offset from the start of the source
		/// </summary>
		public int CharCounter { get; protected set; }

		private void Init(Scanner scanner)
		{
			Line = scanner.Line;
			EndLine = LineNumber = scanner.LineNumber;
			At = scanner.At;
			End = scanner.End;
			Column = scanner.Column;
			CharCounter = scanner.CharCounter;
		}

		public ParseError(Scanner scanner)
			=> Init(scanner);
		public ParseError(Scanner scanner, string message)
			: base(message)
			=> Init(scanner);
		public ParseError(Scanner scanner, string message, params object[] args)
			: base(System.String.Format(message, args))
			=> Init(scanner);
		public ParseError(Scanner scanner, Exception inner)
			: base(null, inner)
			=> Init(scanner);
		public ParseError(Scanner scanner, string message, Exception inner)
			: base(message, inner)
			=> Init(scanner);
		public ParseError(Scanner scanner, Exception inner, string message)
			: base(message, inner)
			=> Init(scanner);
		public ParseError(Scanner scanner, Exception inner, string message, params object[] args)
			: base(System.String.Format(message, args), inner)
			=> Init(scanner);
	}

	public partial class Parser
	{
		protected Parser CheckUnary(bool unary, bool expect)
		{
			if (unary == expect)
				return this;
			if (expect)
				throw new ExpectedUnary(lexer);
			throw new ExpectedBinary(lexer);
		}

		public class ExpectedUnary : ParseError
		{
			public ExpectedUnary(Scanner scanner)
				: base(scanner, "Expected argument or unary operator")
			{
			}
		}

		public class ExpectedBinary : ParseError
		{
			public ExpectedBinary(Scanner scanner)
				: base(scanner, "Expected binary or postfix operator")
			{
			}
		}

		public class BadEscapeSequence : ParseError
		{
			public BadEscapeSequence(Scanner scanner)
				: base(scanner, "Bad escape sequence")
			{
			}
		}
		public class BadNibbleCharacter : ParseError
		{
			public BadNibbleCharacter(Scanner scanner)
				: base(scanner, "Bad nibble character")
			{
			}
		}
	}
}
