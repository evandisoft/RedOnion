using System;
using System.Collections.Generic;

namespace RedOnion.KSP.Completion
{
	public interface ICompletable
	{
		IList<string> PossibleCompletions { get; }
		/// <summary>
		/// Outputs the specified completion or returns false.
		/// </summary>
		/// <returns><c>true</c>, if a completion was outputted, <c>false</c> otherwise.</returns>
		/// <param name="completionName">Completable name.</param>
		/// <param name="completion">Completable.</param>
		bool TryGetCompletion(string completionName,out object completion);
	}
}
