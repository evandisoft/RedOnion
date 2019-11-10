using Kerbalui.Gui;
using LiveRepl.UI.ElementTypes;
using UnityEngine;

namespace LiveRepl.UI.Layout
{
	/// <summary>
	/// The Group that holds the Repl and related functionality.
	/// </summary>
	public class ReplGroup : Group
	{
		public ScriptWindow scriptWindow;

		public ReplGroup(ScriptWindow scriptWindow)
		{
			this.scriptWindow=scriptWindow;
		}

		protected override void SetChildRects()
		{
			//TODO
		}
	}
}