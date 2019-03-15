using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Kerbalua.Utility;

namespace Kerbalua.Completion {
	public class LuaIntellisenseParser {
		public LuaIntellisenseParser()
		{
		}

		static public LuaIntellisenseListener Parse(string str)
		{
			ICharStream stream = CharStreams.fromstring(Misc.ReverseString(str));
			ITokenSource lexer = new BackwardsLuaLexer(stream);
			ITokenStream tokens = new CommonTokenStream(lexer);
		    BackwardsLuaParser parser = new BackwardsLuaParser(tokens);
			parser.BuildParseTree = true;

			IParseTree tree = parser.backwardsCompletionExpr();
			var intellisenseListener = new LuaIntellisenseListener();
			ParseTreeWalker.Default.Walk(intellisenseListener, tree);

			return intellisenseListener;
		}
	}
}
