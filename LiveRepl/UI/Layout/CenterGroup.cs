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
		public ContentGroup contentGroup;

		public CenterGroup(ContentGroup contentGroup)
		{
			this.contentGroup=contentGroup;
		}

		protected override void SetChildRects()
		{
			//TODO
		}
	}
}