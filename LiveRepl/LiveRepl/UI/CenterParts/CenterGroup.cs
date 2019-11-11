using System;
using Kerbalui.Controls;
using Kerbalui.Layout;
using Kerbalui.Types;

namespace LiveRepl.UI.CenterParts
{
	/// <summary>
	/// The Center Group between the Editor and Repl.
	/// Will tend to contain functionality for the overall ScriptWindow
	/// </summary>
	public class CenterGroup : VerticalSpacer
	{
		public ContentGroup contentGroup;

		public CenterGroup(ContentGroup contentGroup)
		{
			this.contentGroup=contentGroup;

			AddMinSized(new Button("<<", contentGroup.scriptWindow.ToggleEditor));
			AddMinSized(new Button(">>", contentGroup.scriptWindow.ToggleRepl));
			AddMinSized(new Button("Terminate", () => throw new NotImplementedException("Centergroup stop button")));
		}
	}
}