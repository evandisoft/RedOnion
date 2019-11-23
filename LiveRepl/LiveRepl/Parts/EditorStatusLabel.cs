using System;
using Kerbalui.Controls;

namespace LiveRepl.Parts
{
	public class EditorStatusLabel : Label
	{
		public EditorStatusLabel()
		{
			UpdateCursorInfo(1,1);
		}

		public void UpdateCursorInfo(int row,int column)
		{
			Content.text=" Line: "+row+", Column: "+column;
		}
	}
}
