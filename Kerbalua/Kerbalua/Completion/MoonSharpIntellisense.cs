using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Kerbalua.Completion.CompletionTypes;
using Kerbalua.Parsing;
using MoonSharp.Interpreter;
using UnityEngine;
using static RedOnion.KSP.Debugging.QueueLogger;

namespace Kerbalua.Completion
{
	public static class MoonSharpIntellisense
	{
		static public IList<string> GetCompletions(
			Table globals,
			string source, int cursorPos, out int replaceStart, out int replaceEnd
			)
		{
			string relevantText = source.Substring(0, cursorPos);

			Compl.Clear();
			ProcessedIncompleteVar processedIncompleteVar;
			try
			{
				Compl.Log($"Source: \"{relevantText}\"");
				processedIncompleteVar = Parse(relevantText);
			}
			catch (LuaIntellisenseException)
			{
				replaceStart = replaceEnd = cursorPos;
				return new List<string>();
			}

			var operations = new CompletionOperations(processedIncompleteVar.Segments);
			Compl.Log(""+operations);

			CompletionObject completionObject=CompletionObject.GetCompletionObject(globals);

			try
			{
				//Compl.Log("Last Operation:"+operations.LastOperation);
				Compl.Log("Op is "+operations.Current);
				Compl.Log("Object is "+completionObject);
				while (!operations.LastOperation)
				{
					if (operations.IsFinished)
					{
						throw new LuaIntellisenseException("Operations should not have been finished ");
					}

					if (!completionObject.TryOperation(operations,out completionObject))
					{
						Compl.Log("Last Operation:"+operations.LastOperation);
						Compl.Log(""+completionObject);
						Compl.Log("Operation failed");
						replaceStart = replaceEnd = cursorPos;
						return new List<string>();
					}

					Compl.Log("Op is "+operations.Current);
					Compl.Log("Object is "+completionObject);
				}
			}
			catch (LuaIntellisenseException e)
			{
				Debug.Log(e);
				replaceStart = replaceEnd = cursorPos;
				return new List<string>();
			}

			var lastOp = operations.Current;
			if (lastOp is GetMemberOperation getMemberOperation)
			{
				string lowercasePartial = getMemberOperation.Name.ToLower();
				List<string> completions = new List<string>();
				foreach (var possibleCompletion in completionObject.GetPossibleCompletions())
				{
					if (possibleCompletion.ToLower().Contains(lowercasePartial))
					{
						completions.Add(possibleCompletion);
					}
				}
				completions.Sort();
				replaceStart = cursorPos - lowercasePartial.Length;
				replaceEnd = cursorPos;

				return completions;
			}

			replaceStart = replaceEnd = cursorPos;
			return new List<string>();
		}

		static public ProcessedIncompleteVar Parse(string str)
		{
			ICharStream stream = CharStreams.fromstring(str);
			ITokenSource lexer = new IncompleteLuaLexer(stream);
			ITokenStream tokens = new CommonTokenStream(lexer);

			var parser = new IncompleteLuaParser(tokens)
			{
				BuildParseTree = true
			};
			var errorListener = new AnyErrorsListener();

			var incompleteChunk = parser.incompleteChunk();

			//if (incompleteChunk.exception != null)
			if (errorListener.HasError)
			{
				throw new LuaIntellisenseException("Could not parse incompleteChunk");
			}

			var lastIncompleteVarExtractor = new LastIncompleteVarExtractor();
			ParseTreeWalker.Default.Walk(lastIncompleteVarExtractor, incompleteChunk);
			var lastIncompleteVar = lastIncompleteVarExtractor.LastIncompleteVar;

			var processedIncompleteVar = new ProcessedIncompleteVar(lastIncompleteVar);

			return processedIncompleteVar;
		}

		class LastIncompleteVarExtractor : IncompleteLuaBaseListener
		{
			public IncompleteLuaParser.IncompleteVarContext LastIncompleteVar;

			public override void EnterIncompleteVar([NotNull] IncompleteLuaParser.IncompleteVarContext context)
			{
				LastIncompleteVar = context;
			}
		}
	}
}
