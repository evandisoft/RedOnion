using System;
using System.Collections.Generic;

namespace RedOnion.KSP.Completion
{
	public interface ICompletable
	{
		IList<string> PossibleCompletions { get; }
		object GetCompletable(string completableName);
	}
}
