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

		/// <summary>
		/// The changesList will be resized to a size of HistorySoftLimit
		/// when it reaches HistorySoftLimit*2. 
		/// </summary>
		public int HistorySoftLimit=50;

		public void Clear()
		{
			CurrentIndex=-1;
			changesList.Clear();
		}

		public EditingState Undo(EditingState editingState)
		{
			if (CurrentIndex==-1)
			{
				return editingState;
			}
			var change=changesList[CurrentIndex--];
			return ApplyUndoChange(change, editingState);
		}

		public EditingState Redo(EditingState editingState)
		{
			CurrentIndex++;
			if (CurrentIndex==changesList.Count)
			{
				CurrentIndex--;
				return editingState;
			}
			var change=changesList[CurrentIndex];
			return ApplyRedoChange(change, editingState);
		}

		public static EditingState ApplyUndoChange(EditingChange change, EditingState editingState)
		{
			var newState=editingState;
			if (!change.textChange.Equals(TextChange.NO_CHANGE))
			{
				string firstPart=editingState.text.Substring(0,change.textChange.startIndex);
				string replacePart=change.textChange.originalString;
				string lastPart=editingState.text.Substring(firstPart.Length+change.textChange.replacementString.Length);
				newState.text=firstPart+replacePart+lastPart;
			}
			if (!change.indexChange.Equals(IndexChange.NO_CHANGE))
			{
				newState.cursorIndex=change.indexChange.originalCursor;
				newState.selectionIndex=change.indexChange.originalSelection;
			}
			return newState;
		}

		public static EditingState ApplyRedoChange(EditingChange change, EditingState editingState)
		{
			var newState=editingState;
			if (!change.textChange.Equals(TextChange.NO_CHANGE))
			{
				string firstPart=editingState.text.Substring(0,change.textChange.startIndex);
				string replacePart=change.textChange.replacementString;
				string lastPart=editingState.text.Substring(firstPart.Length+change.textChange.originalString.Length);
				newState.text=firstPart+replacePart+lastPart;
			}
			if (!change.indexChange.Equals(IndexChange.NO_CHANGE))
			{
				newState.cursorIndex=change.indexChange.replacementCursor;
				newState.selectionIndex=change.indexChange.replacementSelection;
			}
			return newState;
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
				// If we are adding a new change but are not at the end of the list, 
				// destroy all redo history ahead of us.
				changesList.RemoveRange(CurrentIndex, changesList.Count-CurrentIndex);
			}

			if (CurrentIndex>HistorySoftLimit*2)
			{
				changesList=changesList.GetRange(0, HistorySoftLimit);
				CurrentIndex=changesList.Count-1;
			}
			changesList.Add(editingChange);
		}
	}
}
