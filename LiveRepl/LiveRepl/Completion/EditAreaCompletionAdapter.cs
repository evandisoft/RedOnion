using System;
using System.Collections.Generic;
using Kerbalui.Decorators;
using Kerbalui.EditingChanges;
using Kerbalui.Interfaces;
using LiveRepl.Interfaces;
using static RedOnion.Debugging.QueueLogger;

namespace LiveRepl.Completion {
	/// <summary>
	/// Represents an ICompletable using an editingArea and the
	/// scriptWindow. Scriptwindow is used in order to complete based off of its
	/// currentReplEvaluator.
	/// </summary>
	public class CompletionHelper {
		public static EditingState Complete(int index,ScriptWindowParts uiparts, EditingState editingState)
		{
			UILogger.DebugLogArray("In EditAreaCompletionAdapter Complete for index "+index);
			//var completions = uiparts.scriptWindow.currentReplEvaluator.GetCompletions(editingState.text, editingState.cursorIndex,out int replaceStart,out int replaceEnd);
			var completions = uiparts.scriptWindow.CurrentEngine.GetCompletions(editingState.text, editingState.cursorIndex,out int replaceStart,out int replaceEnd);
			UILogger.DebugLogArray("CursorIndex", editingState.cursorIndex,"replaceStart",replaceStart,"replaceEnd",replaceEnd);
			if (completions.Count > index) {
				int partialLength = replaceEnd - replaceStart;
				UILogger.DebugLogArray("PartialLength", partialLength);
				int partialStart = replaceStart;
				UILogger.DebugLogArray("PartialStart", partialStart);
				string textPriorToPartial =  editingState.text.Substring(0, partialStart);
				//UILogger.Log("TextPriorToPartial", textPriorToPartial);
				string completion = completions[index];
				UILogger.DebugLogArray("completion", completion);
				int cursorChange = completion.Length - partialLength;
				UILogger.DebugLogArray("cursorChange", cursorChange);
				int newCursor = replaceEnd+cursorChange; //editingArea.cursorIndex + cursorChange;
				UILogger.DebugLogArray("newCursor", newCursor);
				string textAfterPartial = editingState.text.Substring(partialStart + partialLength);
				string newText=textPriorToPartial + completion + textAfterPartial;
				return new EditingState(newText, newCursor, newCursor);
			}
			return editingState;
		}
	}
}
