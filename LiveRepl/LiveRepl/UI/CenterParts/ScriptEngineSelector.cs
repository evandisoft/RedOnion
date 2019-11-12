using System;
using Kerbalui.Controls;
using Kerbalui.Layout;

namespace LiveRepl.UI.CenterParts
{
	public class ScriptEngineSelector:VerticalSpacer
	{
		public CenterGroup centerGroup;

		public ScriptEngineLabel scriptEngineLabel;

		public ScriptEngineSelector(CenterGroup centerGroup)
		{
			this.centerGroup=centerGroup;

			AddMinSized(scriptEngineLabel=new ScriptEngineLabel());
		}

		protected override void PostAddElement()
		{
			centerGroup.needsResize=true;
		}
	}
}
