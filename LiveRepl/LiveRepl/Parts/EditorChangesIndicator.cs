using System;
using Kerbalui.Controls;

namespace LiveRepl.Parts
{
	public class EditorChangesIndicator:Label
	{
		ScriptWindowParts uiparts;

		public EditorChangesIndicator(ScriptWindowParts uiparts)
		{
			this.uiparts=uiparts;
		}

		public void Changed()
		{
			content.text="*";
			uiparts.scriptWindow.needsResize=true;
		}

		public void Unchanged()
		{
			content.text="";
			uiparts.scriptWindow.needsResize=true;
		}
	}
}
