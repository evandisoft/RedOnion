using System;
using System.Collections.Generic;
using Kerbalua.Gui;

namespace Kerbalua.Other {
	/// <summary>
	/// Represents an ICompletable using an editingArea and the
	/// scriptWindow. Scriptwindow is used in order to complete based off of its
	/// currentReplEvaluator.
	/// </summary>
	public class EditingAreaCompletionAdapter:ICompletable {
		public EditingArea editingArea;
		public ScriptWindow scriptWindow;

		public EditingAreaCompletionAdapter(EditingArea editingArea,ScriptWindow scriptWindow)
		{
			this.editingArea = editingArea;
			this.scriptWindow = scriptWindow;
		}

		public string ControlName => editingArea.ControlName;

		public bool ReceivedInput => editingArea.ReceivedInput;

		public bool Changed()
		{
			throw new NotImplementedException();
		}

		public void Complete(int index)
		{
			var completions = GetCompletionContent();
			if (completions.Count > index) {
				string partial = PartialCompletion();
				int partialStart = editingArea.cursorIndex - partial.Length;
				string textPriorToPartial = editingArea.content.text.Substring(0, partialStart);
				string completion = completions[index];
				int cursorChange = completion.Length - partial.Length;
				int newCursor = editingArea.cursorIndex + cursorChange;
				string textAfterPartial = editingArea.content.text.Substring(partialStart + partial.Length);
				editingArea.content.text = textPriorToPartial + completion + textAfterPartial;
				editingArea.selectIndex=editingArea.cursorIndex = newCursor;
			}
		}

		public List<string> GetCompletionContent()
		{
			return scriptWindow.currentReplEvaluator.GetCompletions(
				editingArea.content.text,
				editingArea.cursorIndex
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

		public string PartialCompletion()
		{
			return scriptWindow.currentReplEvaluator.GetPartialCompletion(
				editingArea.content.text,
				editingArea.cursorIndex
				);
		}
	}
}
