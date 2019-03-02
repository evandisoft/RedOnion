using System;

namespace Bee
{
	public class ParseError: Exception
	{
		private string _line;
		/// <summary>
		/// line with error
		/// </summary>
		public string Line
		{
			get => _line;
			protected set => _line = value;
		}
		
		private int _lnum;
		/// <summary>
		/// line number of error
		/// </summary>
		public int Lnum
		{
			get => _lnum;
			protected set => _lnum = value;
		}
		
		private int _at;
		/// <summary>
		/// start of erroneous token/span
		/// </summary>
		public int At
		{
			get => _at;
			protected set => _at = value;
		}
		
		private int _end;
		/// <summary>
		/// end of erroneous token/span
		/// </summary>
		public int End
		{
			get => _end;
			protected set => _end = value;
		}
		
		private int _col;
		/// <summary>
		/// column of erroneous token/span
		/// </summary>
		public int Col
		{
			get => _col;
			protected set => _col = value;
		}
		
		private int _endln;
		/// <summary>
		/// line number of end of erroneous token/span
		/// </summary>
		public int Endln
		{
			get => _endln;
			protected set => _endln = value;
		}
		
		private void Init(Scanner scanner)
		{
			Line = scanner.Line;
			Endln = Lnum = scanner.Lnum;
			At = scanner.At;
			End = scanner.End;
			Col = scanner.Col;
		}
		
		public ParseError(Scanner scanner)
		{
			Init(scanner);
		}
		
		public ParseError(Scanner scanner, string message)
			: base(message)
		{
			Init(scanner);
		}
		
		public ParseError(Scanner scanner, string message, params object[] @params)
			: base(System.String.Format(message, @params))
		{
			Init(scanner);
		}
		
		public ParseError(Scanner scanner, Exception inner)
			: base(null, inner)
		{
			Init(scanner);
		}
		
		public ParseError(Scanner scanner, string message, Exception inner)
			: base(message, inner)
		{
			Init(scanner);
		}
		
		public ParseError(Scanner scanner, Exception inner, string message)
			: base(message, inner)
		{
			Init(scanner);
		}
		
		public ParseError(Scanner scanner, Exception inner, string message, params object[] @params)
			: base(System.String.Format(message, @params), inner)
		{
			Init(scanner);
		}
	}
	
	public partial class Parser
	{
		protected Parser CheckUnary(bool unary, bool expect)
		{
			if (unary == expect)
			{
				return this;
			}
			if (expect)
			{
				throw new ExpectedUnary(this);
			}
			throw new ExpectedBinary(this);
		}
		
		public class ExpectedUnary: ParseError
		{
			public ExpectedUnary(Scanner scanner)
				: base(scanner, "Expected argument or unary operator")
			{
			}
		}
		
		public class ExpectedBinary: ParseError
		{
			public ExpectedBinary(Scanner scanner)
				: base(scanner, "Expected binary or postfix operator")
			{
			}
		}
		
		public class BadEscapeSequence: ParseError
		{
			public BadEscapeSequence(Scanner scanner)
				: base(scanner, "Bad escape sequence")
			{
			}
		}
	}
}
