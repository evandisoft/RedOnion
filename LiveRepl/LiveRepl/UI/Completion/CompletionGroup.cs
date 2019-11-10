using Kerbalui.Types;
using UnityEngine;

namespace LiveRepl.UI.Completion
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