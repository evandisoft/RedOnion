using System;
namespace Kerbalui.EditingChanges
{
	public struct EditingState
	{
		public string text;
		public int cursorIndex;
		public int selectionIndex;

		public EditingState(string text,int cursorIndex, int selectionIndex)
		{
			this.selectionIndex = selectionIndex;
			this.cursorIndex = cursorIndex;
			this.text = text;
		}
	}
}
