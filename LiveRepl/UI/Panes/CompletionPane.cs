using Kerbalui.Gui;
using UnityEngine;

namespace LiveRepl.UI.Panes
{
	/// <summary>
	/// The pane that contains the completion area.
	/// </summary>
	public class CompletionPane : Pane
	{
		public ContentPane contentPane;

		public CompletionPane(ContentPane contentPane)
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