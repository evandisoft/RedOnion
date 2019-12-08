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

		public bool IsChanged { get; private set; } = false;

		public void MarkAsChanged()
		{
			Content.text="*";
			IsChanged=true;
			uiparts.scriptWindow.needsResize=true;
		}

		public void MarkAsUnchanged()
		{
			Content.text="";
			IsChanged=false;
			uiparts.scriptWindow.needsResize=true;
		}
	}
}
