using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using UnityEngine;

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
			var implicitReturn = parser.implicitReturn();

			return implicitReturn.exception==null;
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
