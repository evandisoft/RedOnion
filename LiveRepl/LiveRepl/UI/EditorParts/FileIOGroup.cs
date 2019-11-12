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

		public EditorChangesIndicator editorChangesIndicator;

		public ScriptNameInputArea scriptNameInputArea;

		public FileIOGroup(EditorGroup editorGroup)
		{
			this.editorGroup=editorGroup;

			AddWeighted(3, scriptNameInputArea=new ScriptNameInputArea(this));
			AddMinSized(editorChangesIndicator=new EditorChangesIndicator(this));

			ScriptWindow scriptWindow=editorGroup.contentGroup.scriptWindow;
			AddWeighted(1, new Button("Save", scriptWindow.ScriptDisabledAction(scriptNameInputArea.LoadEditorText)));
			AddWeighted(1, new Button("Load", scriptWindow.ScriptDisabledAction(scriptNameInputArea.LoadEditorText)));
			AddWeighted(1, new Button("Run", () => throw new NotImplementedException("Run Button not Implemented")));
		}
	}
}
