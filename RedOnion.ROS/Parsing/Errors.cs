using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.ROS.Parsing
{
	partial class Parser
	{
		protected Parser CheckUnary(bool unary, bool expect)
		{
			if (unary == expect)
				return this;
			if (expect)
				throw new ExpectedUnary(this);
			throw new ExpectedBinary(this);
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
