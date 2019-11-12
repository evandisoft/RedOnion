using System;
using Kerbalui.Controls;

namespace LiveRepl.UI.EditorParts
{
	public class EditorChangesIndicator:Label
	{
		FileIOGroup fileIOGroup;

		public EditorChangesIndicator(FileIOGroup fileIOGroup)
		{
			this.fileIOGroup=fileIOGroup;
		}

		public void Changed()
		{
			content.text="*";
			fileIOGroup.needsResize=true;
		}

		public void Unchanged()
		{
			content.text="";
			fileIOGroup.needsResize=true;
		}
	}
}
