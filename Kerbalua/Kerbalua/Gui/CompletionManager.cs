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
		public string lastFocusedControl = "";
		public string currentlyFocusedControl = "";
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

			if (GUI.GetNameOfFocusedControl() != currentlyFocusedControl) {
				lastFocusedControl = currentlyFocusedControl;
				currentlyFocusedControl = GUI.GetNameOfFocusedControl();
				focusChanged = true;
			}

			if (focusChanged || newInput) {
				//Debug.Log("GUI/foc: " + newInput + "," + focusChanged + "," + currentlyFocusedControl + "," + inc++);
				focusChanged = false;
				//Debug.Log("Changed");
				ICompletable currentCompletable;
				if (completableMap.TryGetValue(currentlyFocusedControl, out currentCompletable)) {
					//Debug.Log("Displaying completions");
					DisplayCurrentCompletions(currentCompletable);
				}
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
			if (completableMap.TryGetValue(currentlyFocusedControl,out completable)) {
				completable.Complete(completionSelector.SelectionIndex);
			} else if (completionSelector.HasFocus() && completableMap.TryGetValue(lastFocusedControl, out completable)) {
				completable.Complete(completionSelector.SelectionIndex);
			}
		}
	}
}
