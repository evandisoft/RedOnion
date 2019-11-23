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
			Content.text="*";
			uiparts.scriptWindow.needsResize=true;
		}

		public void Unchanged()
		{
			Content.text="";
			uiparts.scriptWindow.needsResize=true;
		}
	}
}
