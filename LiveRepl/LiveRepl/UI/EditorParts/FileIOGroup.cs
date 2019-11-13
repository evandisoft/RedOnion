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

			ScriptWindow scriptWindow=editorGroup.contentGroup.scriptWindow;
			AddWeighted(1, new Button("Save", scriptWindow.ScriptDisabledAction(editorGroup.SaveEditorText)));
			AddWeighted(1, new Button("Load", scriptWindow.ScriptDisabledAction(editorGroup.LoadEditorText)));
			AddWeighted(1, new Button("Run", scriptWindow.ScriptDisabledAction(editorGroup.RunEditorScript)));
			AddMinSized(editorChangesIndicator=new EditorChangesIndicator(this));
			AddWeighted(3, scriptNameInputArea=new ScriptNameInputArea(this));
		}
	}
}
