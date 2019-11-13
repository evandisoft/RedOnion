using System;
using UnityEngine;
using Kerbalui.Types;
using Kerbalui.Layout;
using Kerbalui.Controls;

namespace LiveRepl.Parts
{
	/// <summary>
	/// This group holds the FileIO related functionality, including save, load, and also has the "run script" button.
	/// </summary>
	public class EditorStatusGroup:HorizontalSpacer
	{
		public ScriptWindowParts uiparts;

		public EditorStatusGroup(ScriptWindowParts uiparts)
		{
			this.uiparts=uiparts;

			AddMinSized(uiparts.editorChangesIndicator=new EditorChangesIndicator(uiparts));
			AddWeighted(1,uiparts.editorStatusLabel=new EditorStatusLabel());
			//AddWeighted(1, new Filler());
			//AddWeighted(3, uiparts.scriptNameInputArea=new ScriptNameInputArea(uiparts));
		}
	}
}
