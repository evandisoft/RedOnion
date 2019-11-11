using Kerbalui.Layout;
using Kerbalui.Types;
using UnityEngine;

namespace LiveRepl.UI.CompletionParts
{
	/// <summary>
	/// The Group that contains the completion area.
	/// </summary>
	public class CompletionGroup : VerticalSpacer
	{
		public ContentGroup contentGroup;

		public CompletionArea completionArea;

		public CompletionGroup(ContentGroup contentGroup)
		{
			this.contentGroup=contentGroup;

			AddWeighted(1, completionArea=new CompletionArea(this));
		}
	}
}