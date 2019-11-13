using System;
using Kerbalui.Controls;
using Kerbalui.Layout;

namespace LiveRepl.Parts
{
	public class ScriptEngineSelector:VerticalSpacer
	{
		public ScriptWindowParts uiparts;

		public ScriptEngineSelector(ScriptWindowParts uiparts)
		{
			this.uiparts=uiparts;

			AddMinSized(uiparts.scriptEngineLabel=new ScriptEngineLabel());
		}

		protected override void PostAddElement()
		{
			uiparts.scriptWindow.needsResize=true;
		}
	}
}
