using System;
using System.Collections.Generic;

namespace Kerbalua.Other {
	/// <summary>
	/// Subclasses of this class implement a function, Evaluate, that evaluates
	/// a string (source) and returns a toString of the result of evaluating
	/// source.
	/// </summary>
	public abstract class ReplEvaluator {
		/// <summary>
		/// Evaluate the source and return the result of that evaluation.
		/// </summary>
		/// <returns>A toString of the result of evaluating the source string.</returns>
		/// <param name="source">The source string to be evaluated.</param>
		public abstract string Evaluate(string source);

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
	}
}
