using System;
using Kerbalui.Controls;
using Kerbalui.Layout;
using Kerbalui.Types;

namespace LiveRepl.Parts
{
	/// <summary>
	/// The Center Group between the Editor and Repl.
	/// Will tend to contain functionality for the overall ScriptWindow
	/// </summary>
	public class CenterGroup : VerticalSpacer
	{
		ScriptWindowParts uiparts;

		public CenterGroup(ScriptWindowParts uiparts)
		{
			this.uiparts=uiparts;

			AddMinSized(new Button("<<", uiparts.scriptWindow.ToggleRepl));
			AddMinSized(new Button(">>", uiparts.scriptWindow.ToggleEditor));
			AddMinSized(new Button("Save", uiparts.scriptWindow.ScriptDisabledAction(uiparts.scriptWindow.SaveEditorText)));
			AddMinSized(new Button("Load", uiparts.scriptWindow.ScriptDisabledAction(uiparts.scriptWindow.LoadEditorText)));
			AddMinSized(new Button("Run", uiparts.scriptWindow.ScriptDisabledAction(uiparts.scriptWindow.RunEditorScript)));
			AddMinSized(new Button("Terminate", uiparts.scriptWindow.Terminate));
			AddMinSized(new Button("Reset Engine", uiparts.scriptWindow.ScriptDisabledAction(uiparts.scriptWindow.ResetEngine)));
			AddMinSized(new Button("Show Hotkeys", uiparts.scriptWindow.ScriptDisabledAction(uiparts.scriptWindow.PrintKeyBindingsInOutputArea)));
			AddMinSized(uiparts.scriptEngineSelector=new ScriptEngineSelector(uiparts));
		}
	}
}