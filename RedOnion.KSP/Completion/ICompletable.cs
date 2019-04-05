using System;
using System.Collections.Generic;

namespace RedOnion.KSP.Completion
{
	public interface ICompletable
	{
		IList<string> PossibleCompletions { get; }
		bool TryGetCompletable(string completableName,out object completable);
	}
}
