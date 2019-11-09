using Kerbalui.Gui;
using LiveRepl.UI.Base;
using UnityEngine;

namespace LiveRepl.UI.Parts
{
	/// <summary>
	/// The Group that holds the Repl and related functionality.
	/// </summary>
	public class ReplGroup : Group
	{
		public ContentGroup contentGroup;

		public ReplGroup(ContentGroup contentGroup)
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