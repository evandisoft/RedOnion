using Kerbalui.Gui;
using LiveRepl.UI.ElementTypes;
using UnityEngine;

namespace LiveRepl.UI.Layout
{
	/// <summary>
	/// The Center Group between the Editor and Repl.
	/// Will tend to contain functionality for the overall ScriptWindow
	/// </summary>
	public class CenterGroup : Group
	{
		public ScriptWindow scriptWindow;

		public CenterGroup(ScriptWindow scriptWindow)
		{
			this.scriptWindow=scriptWindow;
		}

		protected override void SetChildRects()
		{
			//TODO
		}
	}
}