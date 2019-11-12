using System;
using Kerbalui.Controls;

namespace LiveRepl.UI.Controls
{
	public class ScriptDisabledButton:Button
	{
		ScriptWindow scriptWindow;

		public ScriptDisabledButton(ScriptWindow scriptWindow, string text,Action action):base(text,action)
		{
			this.scriptWindow=scriptWindow;

			this.action=ScriptDisabledAction(action);
		}

		Action ScriptDisabledAction(Action theAction)
		{
			return () =>
			{
				if (scriptWindow.ScriptRunning) return;
				theAction();
			};
		}

		public override Action Action { get => base.Action; set => base.Action=ScriptDisabledAction(value); }
	}
}
