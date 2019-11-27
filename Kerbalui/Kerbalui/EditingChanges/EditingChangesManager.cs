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

		public EditingState Undo(EditingState editorState)
		{
			if (CurrentIndex==-1)
			{
				return editorState;
			}
			var change=changesList[CurrentIndex--];
			return ApplyUndoChange(change, editorState);
		}

		public EditingState Redo(EditingState editorState)
		{
			CurrentIndex++;
			if (CurrentIndex==changesList.Count)
			{
				return editorState;
			}
			var change=changesList[CurrentIndex];
			return ApplyRedoChange(change, editorState);
		}

		public static EditingState ApplyUndoChange(EditingChange change, EditingState editorState)
		{
			string firstPart=editorState.text.Substring(0,change.textChange.startIndex);
			string replacePart=change.textChange.originalString;
			string lastPart=editorState.text.Substring(firstPart.Length+change.textChange.replacementString.Length);
			return new EditingState(firstPart+replacePart+lastPart,
				change.indexChange.originalCursor,
				change.indexChange.originalSelection);
		}

		public static EditingState ApplyRedoChange(EditingChange change, EditingState editorState)
		{
			string firstPart=editorState.text.Substring(0,change.textChange.startIndex);
			string replacePart=change.textChange.replacementString;
			string lastPart=editorState.text.Substring(firstPart.Length+change.textChange.originalString.Length);
			return new EditingState(firstPart+replacePart+lastPart,
				change.indexChange.replacementCursor,
				change.indexChange.replacementSelection
				);
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
