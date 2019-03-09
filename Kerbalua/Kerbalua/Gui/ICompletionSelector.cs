using System.Collections.Generic;

namespace Kerbalua.Gui {
    public interface ICompletionSelector:IFocusable {
		void SetContentWithStringList(List<string> contentStrings, string partialCompletion);
		int SelectionIndex { get; }
	}
}