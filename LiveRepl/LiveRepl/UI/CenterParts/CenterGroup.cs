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

		public ScriptEngineSelector scriptEngineSelector;

		public CenterGroup(ContentGroup contentGroup)
		{
			this.contentGroup=contentGroup;

			AddMinSized(new Button("<<", contentGroup.scriptWindow.ToggleEditor));
			AddMinSized(new Button(">>", contentGroup.scriptWindow.ToggleRepl));
			AddMinSized(new Button("Terminate", () => throw new NotImplementedException("Centergroup stop button")));
			AddMinSized(scriptEngineSelector=new ScriptEngineSelector(this));
		}
	}
}