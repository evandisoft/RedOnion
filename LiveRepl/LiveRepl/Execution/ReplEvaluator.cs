using RedOnion.KSP.API;
using RedOnion.KSP.Autopilot;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiveRepl.Evaluation {
	/// <summary>
	/// Subclasses of this class implement a function, ProtectedEvaluate, that evaluates
	/// a string (source) and returns a toString of the result of evaluating
	/// source.
	/// </summary>
	public abstract class ReplEvaluator : IDisposable
	{
		const int maxHistorySize = 1000;

		~ReplEvaluator() => Dispose(false);
		public void Dispose() => Dispose(true);
		protected virtual void Dispose(bool disposing) { }

		public Action<string> PrintAction;
		public Action<string> PrintErrorAction;

		public abstract string Extension { get; }

		/// <summary>
		/// Get a string that will run the file as an import. 
		/// This is for autorun. The file is to be ran to populate globals
		/// in the engine.
		/// </summary>
		public abstract string GetImportString(string scriptname);

		LinkedListNode<string> currentHistoryItem = null;
		/// <summary>
		/// Sets the source and return the result of that evaluation.
		/// Subclasses override the protected method "ProtectedSetSource" to provide a per-engine SetSource
		/// implementation that will be called by this function.
		/// </summary>
		/// <returns>True if evaluation has completed. False if evaluation is unfinished.</returns>
		/// <param name="source">The source string to be evaluated.</param>
		/// <param name="path">Script path or null for repl.</param>
		/// <param name="withHistory">True if this source should be added to the history.</param
		public void SetSource(string source, string path = null, bool withHistory = false)
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

				//foreach (var item in History) {
				//	Debug.Log(item);
				//}

			}

			ProtectedSetSource(source, path);
		}

		protected abstract void ProtectedSetSource(string source, string path);
		public abstract bool Evaluate(out string result);
		public abstract void FixedUpdate();
		public virtual void Update() { }

		/// <summary>
		/// Tell the engine to end an incomplete evaluation.
		/// </summary>
		public abstract void Terminate();

		public string GetCurrentHistoryItem()
		{
			if (currentHistoryItem == null) {
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

			if (currentHistoryItem == null) {
				currentHistoryItem = History.First;
			} else if(currentHistoryItem != History.Last) {
				currentHistoryItem = currentHistoryItem.Next;
			}

			return currentHistoryItem.Value;
		}

		public string HistoryDown()
		{
			if (currentHistoryItem == null) {
				return "";
			} 

			if (currentHistoryItem != History.First) {
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
		public abstract IList<string> GetCompletions(string source, int cursorPos,out int replaceStart,out int replaceEnd);

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
			FlightControl.Instance.Shutdown();
			Ship.DisableAutopilot();
		}
	}
}
