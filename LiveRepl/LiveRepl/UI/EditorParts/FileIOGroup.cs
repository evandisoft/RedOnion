using System;
using UnityEngine;
using Kerbalui.Types;
using Kerbalui.Layout;
using Kerbalui.Controls;

namespace LiveRepl.UI.EditorParts
{
	/// <summary>
	/// This group holds the FileIO related functionality, including save, load, and also has the "run script" button.
	/// </summary>
	public class FileIOGroup:HorizontalSpacer
	{
		public EditorGroup editorGroup;

		public Label changesIndicator=new Label("");

		public TextField inputField;

		public FileIOGroup(EditorGroup editorGroup)
		{
			this.editorGroup=editorGroup;
			//TODO: use script name input field instead of generic textfield
			AddWeighted(3, inputField=new TextField());
			AddMinSized(changesIndicator);
			AddWeighted(1, new Button("Save", () => throw new NotImplementedException("Save Button not Implemented")));
			AddWeighted(1, new Button("Load", () => throw new NotImplementedException("Load Button not Implemented")));
			AddWeighted(1, new Button("Run", () => throw new NotImplementedException("Run Button not Implemented")));
		}
	}
}
