using System;
using System.Collections.Generic;

namespace RedOnion.KSP.Completion
{
	public interface ICompletable
	{
		IList<string> PossibleCompletions { get; }
		/// <summary>
		/// Outputs the specified completion or returns false. Even if the
		/// completion is listed in PossibleCompletions, an attempt to output
		/// the specified completion may be considered too expensive or as having
		/// unwanted side effects. In these cases, or simply if the given completion
		/// is not available, TryGetCompletion should return false.
		/// 
		/// Whether the outputted completion can/should return completions
		/// in turn will be decided by the completion engine.
		/// 
		/// <paramref name="completion"/> may not be the actual object that is to
		/// be returned but an ICompletable that merely represents the object for
		/// completion purposes.
		/// </summary>
		/// <returns><c>true</c>, if a completion was outputted, <c>false</c> otherwise.</returns>
		/// <param name="completionName">Completable name.</param>
		/// <param name="completion">Completable.</param>
		bool TryGetCompletion(string completionName,out object completion);
	}
}
