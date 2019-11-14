using UnityEngine;
using Kerbalui.Types;
using Kerbalui.Decorators;
using Kerbalui.Controls;
using Kerbalui.Layout;
using Kerbalui.EventHandling;

namespace LiveRepl.Parts
{
	/// <summary>
	/// The Group that holds the Editor and related functionality.
	/// </summary>
	public class EditorGroup : VerticalSpacer
	{
		public ScriptWindowParts uiparts;

		public EditorGroup(ScriptWindowParts uiparts)
		{
			this.uiparts=uiparts;

			AddMinSized(uiparts.editorStatusGroup=new EditorStatusGroup(uiparts));

			AddWeighted(1,uiparts.editor=new Editor(uiparts));

			uiparts.editor.keybindings.Add(new EventKey(KeyCode.E, true), uiparts.scriptWindow.RunEditorScript);
		}

		protected override void GroupUpdate()
		{
			if (Event.current.type==EventType.KeyDown)
			{
				uiparts.editorStatusGroup.needsResize=true;
			}
		}
	}
}