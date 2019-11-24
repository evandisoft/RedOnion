using System.Collections.Generic;

namespace LiveRepl.Interfaces
{
    public interface ICompletionSelector:IFocusable 
    {
		void SetContentFromICompletable(ICompletableElement completable);
		int SelectionIndex { get; }
	}
}