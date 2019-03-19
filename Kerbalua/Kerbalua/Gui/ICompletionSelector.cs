using System.Collections.Generic;

namespace Kerbalua.Gui {
    public interface ICompletionSelector:IFocusable {
		void SetContentFromICompletable(ICompletable completable);
		int SelectionIndex { get; }
	}
}