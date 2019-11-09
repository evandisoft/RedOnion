using Kerbalui.Gui;
using LiveRepl.UI.Base;
using UnityEngine;

namespace LiveRepl.UI.Parts
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