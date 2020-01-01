using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Grammar.IncompleteLuaParsing;
using Kerbalua.Completion.CompletionTypes;
using Kerbalua.Parsing;
using MoonSharp.Interpreter;
using UnityEngine;
using static RedOnion.Debugging.QueueLogger;

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

			Complogger.Clear();
			ProcessedIncompleteVar processedIncompleteVar;
			try
			{
				Complogger.DebugLog($"Source: \"{relevantText}\"");
				processedIncompleteVar = Parse(relevantText);
			}
			catch (LuaIntellisenseException e)
			{
				Complogger.DebugLog(e.Message);
				replaceStart = replaceEnd = cursorPos;
				return new List<string>();
			}

			var operations = new CompletionOperations(processedIncompleteVar.Segments);
			Complogger.DebugLog(""+operations);

			CompletionObject completionObject=CompletionObject.GetCompletionObject(globals);

			try
			{
				//Compl.Log("Last Operation:"+operations.LastOperation);
				Complogger.DebugLog("Op is "+operations.Current);
				Complogger.DebugLog("Object is "+completionObject);
				while (!operations.LastOperation)
				{
					if (operations.IsFinished)
					{
						Complogger.DebugLog("Operations should not have finished.");
						throw new LuaIntellisenseException("Operations should not have been finished ");
					}

					if (!completionObject.TryOperation(operations,out completionObject))
					{
						Complogger.DebugLog("Last Operation:"+operations.LastOperation);
						Complogger.DebugLog(""+completionObject);
						Complogger.DebugLog("Operation failed");
						replaceStart = replaceEnd = cursorPos;
						return new List<string>();
					}

					Complogger.DebugLog("Op is "+operations.Current);
					Complogger.DebugLog("Object is "+completionObject);
				}
			}
			catch (LuaIntellisenseException e)
			{
				//Debug.Log(e);
				Complogger.DebugLog(e.Message);
				replaceStart = replaceEnd = cursorPos;
				return new List<string>();
			}

			var lastOp = operations.Current;
			if (lastOp is GetMemberOperation getMemberOperation)
			{
				Complogger.DebugLog("Getting Completions...");
				string lowercasePartial = getMemberOperation.Name.ToLower();
				List<string> completions = new List<string>();
				foreach (var possibleCompletion in completionObject.GetPossibleCompletions())
				{
					//Complogger.Log("trying to add completion",possibleCompletion);
					if (possibleCompletion.ToLower().Contains(lowercasePartial))
					{
						//Complogger.Log("ading completion", possibleCompletion);
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
