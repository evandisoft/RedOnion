using System.Collections.Generic;

namespace LiveRepl.Gui {
    public interface ICompletionSelector:IFocusable {
		void SetContentFromICompletable(ICompletableElement completable);
		int SelectionIndex { get; }
	}
}