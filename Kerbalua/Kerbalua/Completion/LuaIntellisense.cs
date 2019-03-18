using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Kerbalua.Utility;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.Interpreter.Interop;
using System.Collections.Generic;

namespace Kerbalua.Completion {
	public static class LuaIntellisense {
		static public IList<string> GetCompletions(
			Table globals,
			string source,int cursorPos,out int replaceStart,out int replaceEnd
			)
		{
			string relevantText = source.Substring(0, cursorPos);
			var processedIncompleteVar = Parse(relevantText);
			var completionObject = new CompletionObject(globals, processedIncompleteVar.Segments);

			completionObject.ProcessCompletion();
			string Partial = completionObject.CurrentPartial;
			replaceStart = cursorPos - Partial.Length;
			replaceEnd = cursorPos;
			return completionObject.GetCurrentCompletions();
		}

		static public ProcessedIncompleteVar Parse(string str)
		{
			ICharStream stream = CharStreams.fromstring(str);
			ITokenSource lexer = new IncompleteLuaLexer(stream);
			ITokenStream tokens = new CommonTokenStream(lexer);

			var parser = new IncompleteLuaParser(tokens) {
				BuildParseTree = true
			};

			IParseTree tree = parser.incompleteChunk();
			
			var lastIncompleteVarExtractor = new LastIncompleteVarExtractor();
			ParseTreeWalker.Default.Walk(lastIncompleteVarExtractor, tree);
			var lastIncompleteVar = lastIncompleteVarExtractor.LastIncompleteVar;

			var processedIncompleteVar = new ProcessedIncompleteVar(lastIncompleteVar);

			return processedIncompleteVar;
		}

		class LastIncompleteVarExtractor : IncompleteLuaBaseListener {
			public IncompleteLuaParser.IncompleteVarContext LastIncompleteVar;

			public override void EnterIncompleteVar([NotNull] IncompleteLuaParser.IncompleteVarContext context)
			{
				LastIncompleteVar = context;
			}
		}
	}
}
