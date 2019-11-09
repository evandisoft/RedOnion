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

		protected override void SetChildRects()
		{
			//TODO
		}
	}
}