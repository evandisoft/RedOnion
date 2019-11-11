using System;
using Kerbalui.Controls;

namespace LiveRepl.UI.EditorParts
{
	public class EditorStatusLabel : Label
	{
		public EditorStatusLabel()
		{
			UpdateCursorInfo(1,1);
		}

		public void UpdateCursorInfo(int row,int column)
		{
			content.text=" Line: "+row+", Column: "+column;
		}
	}
}
