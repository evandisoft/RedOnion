using Kerbalui.Gui;
using UnityEngine;

namespace LiveRepl.UI.Panes
{
	/// <summary>
	/// The pane that holds the Repl and related functionality.
	/// </summary>
	public class ReplPane : Pane
	{
		public ContentPane contentPane;

		public ReplPane(ContentPane contentPane)
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