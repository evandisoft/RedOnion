using System.Collections.Generic;

namespace LiveRepl.Interfaces {
	/// <summary>
	/// Interface for focusable objects that can complete. Completable objects
	/// can produce a list of possible completions, and can complete the indexth
	/// string on the list. 
	/// </summary>
    public interface ICompletableElement:IFocusable {
		void Complete(int index);
		IList<string> GetCompletionContent(out int replaceStart,out int replaceEnd);
		bool ReceivedInput { get; }
	}
}