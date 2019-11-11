using Kerbalui.Groups;
using Kerbalui.Types;
using UnityEngine;

namespace LiveRepl.UI.ReplParts
{
	/// <summary>
	/// The Group that holds the Repl and related functionality.
	/// </summary>
	public class ReplGroup : VerticalSpacer
	{
		public ContentGroup contentGroup;

		public ReplGroup(ContentGroup contentGroup)
		{
			this.contentGroup=contentGroup;

			AddWeighted(1, new Repl(this));
		}
	}
}