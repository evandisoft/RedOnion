using System;
using System.Collections.Generic;
using Kerbalui.Decorators;
using Kerbalui.Interfaces;
using LiveRepl.Interfaces;
using static RedOnion.KSP.Debugging.QueueLogger;

namespace LiveRepl.Completion {
	/// <summary>
	/// Represents an ICompletable using an editingArea and the
	/// scriptWindow. Scriptwindow is used in order to complete based off of its
	/// currentReplEvaluator.
	/// </summary>
	public class EditingAreaCompletionAdapter:ICompletableElement {
		public IEditingArea editingArea;
		public ScriptWindow scriptWindow;

		public EditingAreaCompletionAdapter(IEditingArea editingArea, ScriptWindow scriptWindow)
		{
			this.editingArea = editingArea;
			this.scriptWindow = scriptWindow;
		}

		public string ControlName => editingArea.ControlName;

		public bool ReceivedInput => editingArea.ReceivedInput;

		public void Complete(int index)
		{
			UILogger.Log("In EditAreaCompletionAdapter Complete for index "+index);
			var completions = scriptWindow.currentReplEvaluator.GetCompletions(editingArea.Text, editingArea.CursorIndex,out int replaceStart,out int replaceEnd);
			UILogger.Log("CursorIndex",editingArea.CursorIndex,"replaceStart",replaceStart,"replaceEnd",replaceEnd);
			if (completions.Count > index) {
				int partialLength = replaceEnd - replaceStart;
				UILogger.Log("PartialLength", partialLength);
				int partialStart = replaceStart;
				UILogger.Log("PartialStart", partialStart);
				string textPriorToPartial = editingArea.Text.Substring(0, partialStart);
				//UILogger.Log("TextPriorToPartial", textPriorToPartial);
				string completion = completions[index];
				UILogger.Log("completion", completion);
				int cursorChange = completion.Length - partialLength;
				UILogger.Log("cursorChange", cursorChange);
				int newCursor = replaceEnd+cursorChange; //editingArea.cursorIndex + cursorChange;
				UILogger.Log("newCursor", newCursor);
				string textAfterPartial = editingArea.Text.Substring(partialStart + partialLength);
				editingArea.Text = textPriorToPartial + completion + textAfterPartial;
				editingArea.SelectIndex=editingArea.CursorIndex = newCursor;
			}
		}

		public IList<string> GetCompletionContent(out int replaceStart,out int replaceEnd)
		{
			return scriptWindow.currentReplEvaluator.GetDisplayableCompletions(
				editingArea.Text,
				editingArea.CursorIndex,
				out replaceStart,
				out replaceEnd
				);
		}

		public void GrabFocus()
		{
			editingArea.GrabFocus();
		}

		public bool HasFocus()
		{
			return editingArea.HasFocus();
		}

	}
}
