using System;
using System.IO;
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
			var errorListener = new AnyErrorsListener();
			parser.AddErrorListener(errorListener);

			var implicitReturn = parser.implicitReturn();
			return !errorListener.HasError;
			//return implicitReturn.exception==null;
		}

		/// <summary>
		/// This just listens for any errors.
		/// Perhaps there is a simpler way to be strict.
		/// But I found this method quickly
		/// </summary>
		class AnyErrorsListener : IAntlrErrorListener<IToken>
		{
			public bool HasError = false;

			public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
			{
				//Console.WriteLine("Found Errors");
				HasError = true;
			}
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
