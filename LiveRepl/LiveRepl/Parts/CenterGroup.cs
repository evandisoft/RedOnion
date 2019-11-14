using System;
using Kerbalui.Controls;
using Kerbalui.Layout;
using Kerbalui.Types;
using LiveRepl.Decorators;

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

			var disableableStuff=new ScriptDisabledButtonsGroup();

			AddMinSized(new Button("<<", uiparts.scriptWindow.ToggleRepl));
			AddMinSized(new Button(">>", uiparts.scriptWindow.ToggleEditor));
			AddMinSized(new ScriptDisabledElement(uiparts, 
				new Button("Run", uiparts.scriptWindow.RunEditorScript)));
			AddMinSized(new Button("Terminate", uiparts.scriptWindow.Terminate));
			disableableStuff.AddMinSized(new Button("Reset Engine", uiparts.scriptWindow.ResetEngine));
			disableableStuff.AddMinSized(uiparts.scriptEngineSelector=new ScriptEngineSelector(uiparts));
			AddMinSized(new ScriptDisabledElement(uiparts, disableableStuff));
		}
	}
}