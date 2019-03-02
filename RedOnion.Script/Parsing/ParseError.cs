using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script.Parsing
{
	public class ParseError : Exception
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

		private void Init(Scanner scanner)
		{
			Line = scanner.Line;
			EndLine = LineNumber = scanner.LineNumber;
			At = scanner.At;
			End = scanner.End;
			Column = scanner.Column;
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
}
