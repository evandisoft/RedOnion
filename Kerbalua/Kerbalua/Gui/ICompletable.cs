using System.Collections.Generic;

namespace Kerbalua.Gui {
	/// <summary>
	/// Interface for focusable objects that can complete. Completable objects
	/// can produce a list of possible completions, and can complete the indexth
	/// string on the list. They can also output the PartialCompletion which is
	/// the portion of the text that is to be replaced.
	/// </summary>
    public interface ICompletable:IFocusable {
		void Complete(int index);
		IList<string> GetCompletionContent(out int replaceStart,out int replaceEnd);
		bool ReceivedInput { get; }
	}
}