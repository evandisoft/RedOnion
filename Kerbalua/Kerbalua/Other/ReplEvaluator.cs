using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kerbalua.Other {
	/// <summary>
	/// Subclasses of this class implement a function, ProtectedEvaluate, that evaluates
	/// a string (source) and returns a toString of the result of evaluating
	/// source.
	/// </summary>
	public abstract class ReplEvaluator {
		const int maxHistorySize = 1000;
		public Action<string> PrintAction;
		public Action<string> PrintErrorAction;

		LinkedListNode<string> currentHistoryItem=null;
		/// <summary>
		/// Evaluate the source and return the result of that evaluation.
		/// Subclasses override the protected method "ProtectedEvaluate" to provide a per-engine evaluation
		/// implementation that will be called by this function.
		/// </summary>
		/// <returns>True if evaluation has completed. False if evaluation is unfinished.</returns>
		/// <param name="source">The source string to be evaluated.</param>
		/// <param name="output">A ToString of the result of evaluating source.</param>
		/// <param name="withHistory">True if this source should be added to the history.</param>
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

		/// <summary>
		/// Overriden by subclasses for their per-engine evaluation functionality.
		/// </summary>
		/// <returns><c>true</c>, if evaluation was complete, <c>false</c> otherwise.</returns>
		/// <param name="source">String to be evaluated.</param>
		/// <param name="output">ToString of the result of the evaluation.</param>
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
		/// <param name="replaceStart">The start index of the string to be replaced by a completion.</param>
		/// <param name="replaceEnd">The end index of the string to be replaced by a completion</param>
		public abstract IList<string> GetCompletions(string source, int cursorPos,out int replaceStart,out int replaceEnd);

		/// <summary>
		/// Resets the engine.
		/// </summary>
		public abstract void ResetEngine();
	}
}
