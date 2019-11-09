using Kerbalui.Gui;
using UnityEngine;

namespace LiveRepl.UI.Panes
{
	/// <summary>
	/// The Center Pane between the Editor and Repl.
	/// Will tend to contain functionality for the overall ScriptWindow
	/// </summary>
	public class CenterPane : Pane
	{
		public ContentPane contentPane;

		public CenterPane(ContentPane contentPane)
		{
			this.contentPane=contentPane;
		}

		public override void SetRect(Rect rect)
		{
			this.rect=rect;
			//TODO
		}
	}
}