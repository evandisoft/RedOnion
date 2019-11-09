using LiveRepl.UI.Elements;
using UnityEngine;

namespace LiveRepl.UI.Panes
{
	/// <summary>
	/// The pane that holds the Editor and related functionality.
	/// </summary>
	public class EditorPane : Pane
	{
		public ContentPane contentPane;

		public FileIOPane fileIOPane;
		public EditingArea editingArea;

		public EditorPane(ContentPane contentPane)
		{
			this.contentPane=contentPane;

			RegisterForUpdate(fileIOPane=new FileIOPane(this));
			RegisterForUpdate(editingArea=new EditingArea(new TextArea()));
		}

		public override void SetRect(Rect rect)
		{
			this.rect=rect;

			fileIOPane.SetRect(new Rect(0, 0, rect.width, 20));
			//TODO: TabsPane
			editingArea.rect=new Rect(0, fileIOPane.rect.height, rect.width, rect.height-fileIOPane.rect.height);
			//TODO: EditorStatusPane
		}
	}
}