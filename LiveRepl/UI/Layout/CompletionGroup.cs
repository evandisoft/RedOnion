using Kerbalui.Gui;
using LiveRepl.UI.ElementTypes;
using UnityEngine;

namespace LiveRepl.UI.Layout
{
	/// <summary>
	/// The Group that contains the completion area.
	/// </summary>
	public class CompletionGroup : Group
	{
		public ScriptWindow scriptWindow;

		public CompletionGroup(ScriptWindow scriptWindow)
		{
			this.scriptWindow=scriptWindow;
		}

		protected override void SetChildRects()
		{
			//TODO
		}
	}
}