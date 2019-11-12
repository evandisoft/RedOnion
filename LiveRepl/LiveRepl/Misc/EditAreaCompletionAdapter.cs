using System;
using System.Collections.Generic;
using Kerbalui.Decorators;
using LiveRepl.Interfaces;
using LiveRepl.Main;
using LiveRepl.UI;

namespace LiveRepl.Misc {
	/// <summary>
	/// Represents an ICompletable using an editingArea and the
	/// scriptWindow. Scriptwindow is used in order to complete based off of its
	/// currentReplEvaluator.
	/// </summary>
	public class EditingAreaCompletionAdapter:ICompletableElement {
		public EditingArea editingArea;
		public ReplMain replMain;

		public EditingAreaCompletionAdapter(EditingArea editingArea, ReplMain replMain)
		{
			this.editingArea = editingArea;
			this.replMain = replMain;
		}

		public string ControlName => editingArea.editableText.ControlName;

		public bool ReceivedInput => editingArea.ReceivedInput;

		public void Complete(int index)
		{
			var completions = replMain.currentReplEvaluator.GetCompletions(editingArea.Text, editingArea.CursorIndex,out int replaceStart,out int replaceEnd);
			if (completions.Count > index) {
				int partialLength = replaceEnd - replaceStart;
				int partialStart = replaceStart;
				string textPriorToPartial = editingArea.Text.Substring(0, partialStart);
				string completion = completions[index];
				int cursorChange = completion.Length - partialLength;
				int newCursor = replaceEnd+cursorChange; //editingArea.cursorIndex + cursorChange;
				string textAfterPartial = editingArea.Text.Substring(partialStart + partialLength);
				editingArea.Text = textPriorToPartial + completion + textAfterPartial;
				editingArea.SelectIndex=editingArea.CursorIndex = newCursor;
			}
		}

		public IList<string> GetCompletionContent(out int replaceStart,out int replaceEnd)
		{
			return replMain.currentReplEvaluator.GetDisplayableCompletions(
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
