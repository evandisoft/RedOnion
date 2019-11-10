using System.Collections.Generic;

namespace Kerbalui.Gui {
    public interface ICompletionSelector:IFocusable {
		void SetContentFromICompletable(ICompletableElement completable);
		int SelectionIndex { get; }
	}
}