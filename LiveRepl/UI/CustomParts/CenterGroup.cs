using Kerbalui.Gui;
using LiveRepl.UI.Base;
using UnityEngine;

namespace LiveRepl.UI.CustomParts
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

		public override void SetRect(Rect rect)
		{
			this.rect=rect;
			//TODO
		}
	}
}