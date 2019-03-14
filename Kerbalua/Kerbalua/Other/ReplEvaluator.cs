using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kerbalua.Other {
	/// <summary>
	/// Subclasses of this class implement a function, Evaluate, that evaluates
	/// a string (source) and returns a toString of the result of evaluating
	/// source.
	/// </summary>
	public abstract class ReplEvaluator {
		const int maxHistorySize = 1000;
		public Action<string> PrintAction;

		LinkedListNode<string> currentHistoryItem=null;
		/// <summary>
		/// Evaluate the source and return the result of that evaluation.
		/// </summary>
		/// <returns>A toString of the result of evaluating the source string.</returns>
		/// <param name="source">The source string to be evaluated.</param>
		public bool Evaluate(string source,out string output,bool withHistory=false)
		{
			if (withHistory) {
				if(!(History.Count>0 && source == History.First.Value)) {
					History.AddFirst(source);
				}

				if (History.Count > maxHistorySize) {
					History.RemoveLast();
				}

				currentHistoryItem = null;

				//foreach (var item in History) {
				//	Debug.Log(item);
				//}
			}

			return ProtectedEvaluate(source, out output);
		}

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

		protected abstract bool ProtectedEvaluate(string source,out string output);

		protected LinkedList<string> History = new LinkedList<string>();

		/// <summary>
		/// Looking at the source text and an index to the current cursor
		/// position, return a sorted list of strings that are the possible completions
		/// in the current context.
		/// </summary>
		/// <returns>The completion.</returns>
		/// <param name="source">Source.</param>
		/// <param name="cursorPos">Cursor position.</param>
		public abstract List<string> GetCompletions(string source, int cursorPos);

		/// <summary>
		/// Return the part of the string that will be replaced by a completion.
		/// For example "a.bla" may complete to "a.blah" but the partial would be
		/// "bla"
		/// </summary>
		/// <returns>The partial completion.</returns>
		/// <param name="source">Source.</param>
		/// <param name="cursorPos">Cursor position.</param>
		public abstract string GetPartialCompletion(string source, int cursorPos);

		/// <summary>
		/// Resets the engine.
		/// </summary>
		public abstract void ResetEngine();
	}
}
