using System;
namespace Kerbalui.EditingChanges
{
	public struct IndexChange
	{
		public int originalCursor;
		public int replacementCursor;
		public int originalSelection;
		public int replacementSelection;

		public static readonly IndexChange NO_CHANGE;

		public IndexChange(EditingState startState, EditingState endState) : this()
		{
			if (!startState.Equals(endState))
			{
				originalCursor=startState.cursorIndex;
				originalSelection=startState.selectionIndex;
				replacementCursor=endState.cursorIndex;
				replacementSelection=endState.selectionIndex;
			}
		}

		public IndexChange(int originalCursor, int replacementCursor, int originalSelection, int replacementSelection)
		{
			if (originalCursor!=replacementCursor || originalSelection!=replacementSelection)
			{
				this.originalCursor=originalCursor;
				this.replacementCursor=replacementCursor;
				this.originalSelection=originalSelection;
				this.replacementSelection=replacementSelection;
			}
			else
			{
				this.originalCursor=0;
				this.replacementCursor=0;
				this.originalSelection=0;
				this.replacementSelection=0;
			}
		}
	}
}
