using System;
using System.Collections.Generic;
using MunOS.Core;

namespace MunOS.ProcessLayer
{
	public abstract class EngineProcess:MunProcess
	{
		protected EngineProcess() : base()
		{
		}

		public void Init(IList<string> initSourceFilenames)
		{
			foreach (var sourceFilename in initSourceFilenames)
			{
				string importString=GetImportString(sourceFilename);
				var thread=CreateThread(importString, sourceFilename);
				thread.Name=sourceFilename+" init";
				initQueue.Enqueue(thread);
			}
			ExecuteNextInit();
		}

		protected abstract MunThread CreateThread(string source, string path);

		const int maxHistorySize = 1000;

		public abstract string Extension { get; }

		/// <summary>
		/// Get a string that will run the file as an import. 
		/// This is for autorun. The file is to be ran to populate globals
		/// in the engine. And run functions.
		/// </summary>
		public abstract string GetImportString(string scriptname);

		LinkedListNode<string> currentHistoryItem = null;
		/// <summary>
		/// Creates a new thread to evaluate the source, by calling the abstract method
		/// ProtectedEvaluate that is implemented by the specific Engine Process that extends this class.
		/// </summary>
		/// <param name="source">The source string to be evaluated.</param>
		/// <param name="path">Script path or null for repl.</param>
		/// <param name="withHistory">True if this source should be added to the history.</param>
		public void ExecuteInRepl(ExecPriority priority, string source, string path = null, bool withHistory = false)
		{
			if (withHistory)
			{
				if (!(History.Count > 0 && source == History.First.Value))
				{
					History.AddFirst(source);
				}

				if (History.Count > maxHistorySize)
				{
					History.RemoveLast();
				}

				currentHistoryItem = null;
			}

			Execute(priority, source, path, true);
		}

		public abstract void Execute(ExecPriority priority, string source, string path, bool inRepl=false);

		public string GetCurrentHistoryItem()
		{
			if (currentHistoryItem == null)
			{
				return "";
			}

			return currentHistoryItem.Value;
		}

		public string HistoryUp()
		{
			if (History.Count==0)
			{
				return "";
			}

			if (currentHistoryItem == null)
			{
				currentHistoryItem = History.First;
			}
			else if (currentHistoryItem != History.Last)
			{
				currentHistoryItem = currentHistoryItem.Next;
			}

			return currentHistoryItem.Value;
		}

		public string HistoryDown()
		{
			if (currentHistoryItem == null)
			{
				return "";
			}

			if (currentHistoryItem != History.First)
			{
				currentHistoryItem = currentHistoryItem.Previous;
			}

			return currentHistoryItem.Value;
		}

		protected LinkedList<string> History = new LinkedList<string>();

		/// <summary>
		/// Looking at the source text and an index to the current cursor
		/// position, return a sorted list of strings that are the possible completions
		/// in the current context.
		/// </summary>
		/// <returns>The completion.</returns>
		/// <param name="source">Source.</param>
		/// <param name="cursorPos">Cursor position.</param>
		/// <param name="replaceStart">The start index of the string to be replaced by a completion.</param>
		/// <param name="replaceEnd">The end index of the string to be replaced by a completion</param>
		public abstract IList<string> GetCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd);

		/// <summary>
		/// Gets the displayable completions. These will be the completions that the
		/// user is to see when deciding what option to select. For example
		/// the actual completion might be to turn 'anobject.completion' 
		/// into 'anobject["completion"]'. But the user would see just 'completion'
		/// in the list.
		/// </summary>
		/// <returns>The displayable completions.</returns>
		/// <param name="source">Source.</param>
		/// <param name="cursorPos">Cursor position.</param>
		/// <param name="replaceStart">Replace start.</param>
		/// <param name="replaceEnd">Replace end.</param>
		public abstract IList<string> GetDisplayableCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd);

		/// <summary>
		/// Resets the engine.
		/// </summary>
		public virtual void ResetEngine()
		{
			Terminate();
		}
	}
}
