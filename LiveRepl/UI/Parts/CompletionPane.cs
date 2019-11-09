using Kerbalui.Gui;
using LiveRepl.UI.Base;
using UnityEngine;

namespace LiveRepl.UI.Parts
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