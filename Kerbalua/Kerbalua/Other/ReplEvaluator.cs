using System;
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
		/// <returns>The result of evaluating the source string.</returns>
		/// <param name="source">The source string to be evaluated.</param>
		public abstract string Evaluate(string source);
	}
}
