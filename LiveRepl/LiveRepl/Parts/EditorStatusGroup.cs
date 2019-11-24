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

			AddFixed(ScriptWindow.centerGroupWidth, new Button("Save", uiparts.scriptWindow.SaveEditorText));
			AddFixed(ScriptWindow.centerGroupWidth, new Button("Load", uiparts.scriptWindow.LoadEditorText));
			AddMinSized(uiparts.editorChangesIndicator=new EditorChangesIndicator(uiparts));
			AddWeighted(1,uiparts.editorStatusLabel=new EditorStatusLabel());
			//AddMinSized(new Label("Font:"));
			AddWeighted(1, uiparts.fontSelector=new FontSelector(uiparts));
			//AddMinSized(uiparts.editorStatusLabel=new EditorStatusLabel());
			//AddWeighted(1, new Filler());
			//AddWeighted(3, uiparts.scriptNameInputArea=new ScriptNameInputArea(uiparts));
		}
	}
}
