using System;
using System.Collections.Generic;
using Kerbalua.Gui;

namespace Kerbalua.Other {
	/// <summary>
	/// Represents an ICompletable using an editingArea and the
	/// scriptWindow. Scriptwindow is used in order to complete based off of its
	/// currentReplEvaluator.
	/// </summary>
	public class EditingAreaCompletionAdapter:ICompletableElement {
		public EditingArea editingArea;
		public ScriptWindow scriptWindow;

		public EditingAreaCompletionAdapter(EditingArea editingArea,ScriptWindow scriptWindow)
		{
			this.editingArea = editingArea;
			this.scriptWindow = scriptWindow;
		}

		public string ControlName => editingArea.ControlName;

		public bool ReceivedInput => editingArea.ReceivedInput;

		public void Complete(int index)
		{
			var completions = GetCompletionContent(out int replaceStart,out int replaceEnd);
			if (completions.Count > index) {
				int partialLength = replaceEnd - replaceStart;
				int partialStart = replaceStart;
				string textPriorToPartial = editingArea.content.text.Substring(0, partialStart);
				string completion = completions[index];
				int cursorChange = completion.Length - partialLength;
				int newCursor = replaceEnd+cursorChange; //editingArea.cursorIndex + cursorChange;
				string textAfterPartial = editingArea.content.text.Substring(partialStart + partialLength);
				editingArea.content.text = textPriorToPartial + completion + textAfterPartial;
				editingArea.selectIndex=editingArea.cursorIndex = newCursor;
			}
		}

		public IList<string> GetCompletionContent(out int replaceStart,out int replaceEnd)
		{
			return scriptWindow.currentReplEvaluator.GetCompletions(
				editingArea.content.text,
				editingArea.cursorIndex,
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
