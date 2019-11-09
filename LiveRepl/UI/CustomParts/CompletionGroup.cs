using Kerbalui.Gui;
using LiveRepl.UI.Base;
using UnityEngine;

namespace LiveRepl.UI.CustomParts
{
	/// <summary>
	/// The Group that contains the completion area.
	/// </summary>
	public class CompletionGroup : Group
	{
		public ContentGroup contentGroup;

		public CompletionGroup(ContentGroup contentGroup)
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