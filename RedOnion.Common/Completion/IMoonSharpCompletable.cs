using System;
using System.Collections.Generic;

namespace RedOnion.Common.Completion
{
	/// <summary>
	/// This is to deal with special cases, like NamespaceInstance, where
	/// completion returns a type, but we really want to treat it as static.
	/// </summary>
	public interface IMoonSharpCompletable
	{
		IList<string> PossibleCompletions { get; }
		bool TryGetCompletion(string completionName, out object completion);
	}
}
