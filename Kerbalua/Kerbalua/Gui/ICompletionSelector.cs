using System.Collections.Generic;

namespace Kerbalua.Gui {
    public interface ICompletionSelector:IFocusable {
		void SetContentFromICompletable(ICompletableElement completable);
		int SelectionIndex { get; }
	}
}