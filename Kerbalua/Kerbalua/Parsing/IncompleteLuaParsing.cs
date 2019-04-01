using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Kerbalua.Parsing
{
	public class IncompleteLuaParsing
	{
		public IncompleteLuaParsing()
		{
		}

		static public bool IsImplicitReturn(string source)
		{
			var parser = GetParser(source);
			return parser.implicitReturn() != null;
		}

		static public IncompleteLuaParser GetParser(string source)
		{
			ICharStream stream = CharStreams.fromstring(source);
			ITokenSource lexer = new IncompleteLuaLexer(stream);
			ITokenStream tokens = new CommonTokenStream(lexer);

			var parser = new IncompleteLuaParser(tokens)
			{
				BuildParseTree = true
			};

			return parser;
		}
	}
}
