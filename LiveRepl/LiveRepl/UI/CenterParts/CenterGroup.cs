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

			ScriptWindow scriptWindow=contentGroup.scriptWindow;
			AddMinSized(new Button("<<", scriptWindow.ToggleEditor));
			AddMinSized(new Button(">>", scriptWindow.ToggleRepl));
			AddMinSized(new Button("Terminate", scriptWindow.Terminate));
			AddMinSized(new Button("Reset Engine", scriptWindow.ScriptDisabledAction(scriptWindow.ResetEngine)));
			AddMinSized(new Button("Show Hotkeys", scriptWindow.ScriptDisabledAction(scriptWindow.PrintKeyBindingsInOutputArea)));
			AddMinSized(scriptEngineSelector=new ScriptEngineSelector(this));
		}
	}
}