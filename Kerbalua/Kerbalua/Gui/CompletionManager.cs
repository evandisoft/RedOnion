using System;
using UnityEngine;
using System.Collections.Generic;

namespace Kerbalua.Gui {
	/// <summary>
	/// Manages an interaction between focusable objects that can produce a 
	/// list of possible completions (ICompletable) and an object for displaying 
	/// that completion (ICompletionSelector). Completes whichever ICompletable 
	/// is focused currently, or if the completionSelector is focused, 
	/// completes whichever ICompletable was focused last.
	/// </summary>
	public class CompletionManager {
		Dictionary<string,ICompletable> completableMap=new Dictionary<string, ICompletable>();
		public ICompletionSelector completionSelector;
		public string mostRecentlyFocusedCompletable = "";
		bool focusChanged;// = true;

		public CompletionManager(ICompletionSelector completionSelector)
		{
			this.completionSelector = completionSelector;
		}

		public void AddCompletable(ICompletable source)
		{
			completableMap[source.ControlName] = source;
		}

		int inc = 0;
		public void Update()
		{
			bool newInput=false;
			foreach(var completable in completableMap.Values) {
				newInput |= completable.ReceivedInput;
			}

			string focusedControlName = GUI.GetNameOfFocusedControl();
			if (GUI.GetNameOfFocusedControl() != mostRecentlyFocusedCompletable &&
				completableMap.ContainsKey(focusedControlName)) {
				mostRecentlyFocusedCompletable = focusedControlName;
				focusChanged = true;
			}

			if (focusChanged || newInput) {
				//Debug.Log("GUI/foc: " + newInput + "," + focusChanged + "," + currentlyFocusedControl + "," + inc++);
				focusChanged = false;
				//Debug.Log("Changed");
				ICompletable currentCompletable;
				if (completableMap.TryGetValue(mostRecentlyFocusedCompletable, out currentCompletable)) {
					//Debug.Log("Displaying completions");
					DisplayCurrentCompletions(currentCompletable);
				}
			}

		}

		public void DisplayCurrentCompletions()
		{
			ICompletable currentCompletable;
			if (completableMap.TryGetValue(mostRecentlyFocusedCompletable, out currentCompletable)) {
				//Debug.Log("Displaying completions");
				DisplayCurrentCompletions(currentCompletable);
			}
		}

		void DisplayCurrentCompletions(ICompletable completable)
		{
			completionSelector.SetContentWithStringList(
				completable.GetCompletionContent(),
				completable.PartialCompletion()
				);
		}

		public void Complete()
		{
			ICompletable completable;
			if (completableMap.TryGetValue(mostRecentlyFocusedCompletable,out completable)) {
				completable.GrabFocus();
				completable.Complete(completionSelector.SelectionIndex);
			} 
		}
	}
}
