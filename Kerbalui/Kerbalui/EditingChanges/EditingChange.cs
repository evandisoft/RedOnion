using System;
namespace Kerbalui.EditingChanges
{
	public struct EditingChange
	{
		public TextChange textChange;
		public IndexChange indexChange;

		public static readonly EditingChange NO_CHANGE;
		public EditingChange(TextChange textChange,IndexChange indexChange)
		{
			this.textChange=textChange;
			this.indexChange=indexChange;
		}

		public EditingChange(EditingState startState, EditingState endState) : this()
		{
			textChange=TextChange.Calculate(startState.text, endState.text);
			indexChange=new IndexChange(startState, endState);
		}
	}
}
