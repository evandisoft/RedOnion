using System;
using System.Collections.Generic;

namespace Kerbalui.EditingChanges
{
	public class EditingChangesManager
	{
		public int ChangesLength => changesList.Count;
		public IReadOnlyList<EditingChange> ChangesList => changesList;
		private List<EditingChange> changesList=new List<EditingChange>();
		public int CurrentIndex { get; private set; } = -1;

		public bool Undo(EditingState editorState,
			out EditingState newEditorState)
		{
			if (CurrentIndex==-1)
			{
				newEditorState=editorState;
				return false;
			}
			var change=changesList[CurrentIndex--];
			ApplyUndoChange(change, editorState, out newEditorState);
			return true;
		}

		public bool Redo(string currentText, EditingState editorState,
			out EditingState newEditorState)
		{
			if (CurrentIndex==changesList.Count)
			{
				newEditorState=editorState;
				return false;
			}
			var change=changesList[CurrentIndex++];
			ApplyRedoChange(change, editorState, out newEditorState);
			return true;
		}

		public static void ApplyUndoChange(EditingChange change, EditingState editorState,
			out EditingState newEditorState)
		{
			string firstPart=editorState.text.Substring(0,change.textChange.startIndex);
			string replacePart=change.textChange.originalString;
			string lastPart=editorState.text.Substring(firstPart.Length+change.textChange.replacementString.Length);
			newEditorState.text=firstPart+replacePart+lastPart;
			newEditorState.cursorIndex=change.indexChange.originalCursor;
			newEditorState.selectionIndex=change.indexChange.originalSelection;
		}

		public static void ApplyRedoChange(EditingChange change, EditingState editorState,
			out EditingState newEditorState)
		{
			string firstPart=editorState.text.Substring(0,change.textChange.startIndex);
			string replacePart=change.textChange.replacementString;
			string lastPart=editorState.text.Substring(firstPart.Length+change.textChange.originalString.Length);
			newEditorState.text=firstPart+replacePart+lastPart;
			newEditorState.cursorIndex=change.indexChange.replacementCursor;
			newEditorState.selectionIndex=change.indexChange.replacementSelection;
		}

		public void AddChange(EditingState startState, EditingState endState)
		{
			var editingChange=new EditingChange(startState,endState);
			if (editingChange.Equals(EditingChange.NO_CHANGE))
			{
				return;
			}
			CurrentIndex++;
			if (CurrentIndex<changesList.Count)
			{
				// If we are adding a new change, destroy all redo history ahead of us.
				changesList.RemoveRange(CurrentIndex, changesList.Count-CurrentIndex);
			}

			changesList.Add(editingChange);
		}
	}
}
