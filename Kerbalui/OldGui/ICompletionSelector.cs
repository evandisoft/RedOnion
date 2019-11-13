using System.Collections.Generic;

namespace Kerbalui.Obsolete {
    public interface ICompletionSelector:IFocusable {
		void SetContentFromICompletable(ICompletableElement completable);
		int SelectionIndex { get; }
	}
}