using UnityEngine;
using Kerbalui.Types;
using Kerbalui.Decorators;
using Kerbalui.Controls;
using Kerbalui.Layout;

namespace LiveRepl.UI.EditorParts
{
	/// <summary>
	/// The Group that holds the Editor and related functionality.
	/// </summary>
	public class EditorGroup : VerticalSpacer
	{
		public ContentGroup contentGroup;

		public FileIOGroup fileIOGroup;
		public Editor editor;
		public EditorStatusLabel editorStatusLabel;

		public EditorGroup(ContentGroup contentGroup)
		{
			this.contentGroup=contentGroup;

			AddMinSized(fileIOGroup=new FileIOGroup(this));
			AddWeighted(1,editor=new Editor(this));
			AddMinSized(editorStatusLabel=new EditorStatusLabel());
		}

		protected override void GroupUpdate()
		{
			if (Event.current.type==EventType.KeyDown)
			{
				fileIOGroup.needsResize=true;
			}
		}

		public void SaveEditorText()
		{
			fileIOGroup.scriptNameInputArea.SaveText(editor.editingArea.Text);
			fileIOGroup.editorChangesIndicator.Unchanged();
		}

		public void LoadEditorText()
		{
			string text=fileIOGroup.scriptNameInputArea.LoadText();
			editor.editingArea.Text=text;
			fileIOGroup.editorChangesIndicator.Unchanged();
		}

		public void RunEditorScript()
		{
			SaveEditorText();
			contentGroup.scriptWindow.Evaluate(editor.editingArea.Text, fileIOGroup.scriptNameInputArea.Text);
			contentGroup.replGroup.repl.replOutoutArea.AddFileContent(fileIOGroup.scriptNameInputArea.Text);
		}
	}
}