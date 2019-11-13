using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using LiveRepl.Interfaces;

namespace LiveRepl.Completion
{
	/// <summary>
	/// Manages an interaction between focusable objects that can produce a 
	/// list of possible completions (ICompletable) and an object for displaying 
	/// that completion (ICompletionSelector). Completes whichever ICompletable 
	/// is focused currently, or if the completionSelector is focused, 
	/// completes whichever ICompletable was focused last.
	/// </summary>
	public class CompletionManager 
	{
		Dictionary<string,ICompletableElement> completableMap=new Dictionary<string, ICompletableElement>();
		public ICompletionSelector completionSelector;
		public string mostRecentlyFocusedCompletable = "";
		public Stopwatch stopwatch = new Stopwatch();
		public int CompletionDelay = 0;

		//bool completeOnNextUpdate = false;


		public CompletionManager(ICompletionSelector completionSelector)
		{
			this.completionSelector = completionSelector;
			stopwatch.Start();
		}

		public void AddCompletable(ICompletableElement source)
		{
			completableMap[source.ControlName] = source;
		}

		//int inc = 0;
		bool newInput = false;
		bool focusChanged = false;
		public bool Update(bool hadMouseDownLastUpdate)
		{
			newInput|= hadMouseDownLastUpdate;
			//if (hadMouseDownLastUpdate) {
			//	Debug.Log("mouse down last update");
			//}
			foreach (var completable in completableMap.Values) {
				newInput |= completable.ReceivedInput;
			}

			string focusedControlName = GUI.GetNameOfFocusedControl();
			if (GUI.GetNameOfFocusedControl() != mostRecentlyFocusedCompletable &&
				completableMap.ContainsKey(focusedControlName)) {
				mostRecentlyFocusedCompletable = focusedControlName;
				focusChanged |= true;
			}

			if ((focusChanged || newInput)&& stopwatch.ElapsedMilliseconds>CompletionDelay) {
				//Debug.Log("GUI/foc: " + newInput + "," + focusChanged + "," + mostRecentlyFocusedCompletable + "," + inc++);
				focusChanged = false;
				newInput = false;
				stopwatch.Reset();
				stopwatch.Start();
				//Debug.Log("Changed");
				ICompletableElement currentCompletable;
				if (completableMap.TryGetValue(mostRecentlyFocusedCompletable, out currentCompletable)) {
					//Debug.Log("Displaying completions");
					DisplayCurrentCompletions(currentCompletable);
					return true;
				}
			}
			return false;
		}

		//public void CompleteOnNextUpdate()
		//{
		//	completeOnNextUpdate = true;
		//}

		public void DisplayCurrentCompletions()
		{
			ICompletableElement currentCompletable;
			if (completableMap.TryGetValue(mostRecentlyFocusedCompletable, out currentCompletable)) {
				//Debug.Log("Displaying completions");
				DisplayCurrentCompletions(currentCompletable);
			}
		}

		void DisplayCurrentCompletions(ICompletableElement completable)
		{
			completionSelector.SetContentFromICompletable(completable);
		}

		public void Complete()
		{
			//Debug.Log("completing");
			ICompletableElement completable;
			if (completableMap.TryGetValue(mostRecentlyFocusedCompletable,out completable)) {

				completable.Complete(completionSelector.SelectionIndex);
				completable.GrabFocus();
			} 
		}
	}
}
