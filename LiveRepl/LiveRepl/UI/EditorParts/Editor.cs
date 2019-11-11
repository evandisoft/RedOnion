using UnityEngine;
using System.Collections.Generic;
using System;
using Kerbalui.EventHandling;
using Kerbalui.Controls.Abstract;
using Kerbalui.Decorators;
using Kerbalui.Controls;

namespace LiveRepl.UI.EditorParts 
{
    public class Editor:EditingArea
    {
		public EditorGroup editorGroup;

		/// <summary>
		/// These bindings intentionally shadow the base class bindings.
		/// </summary>
		public new KeyBindings keybindings = new KeyBindings();

		public Editor(EditorGroup editorGroup) : base(new TextArea())
		{
			this.editorGroup=editorGroup;
			//TODO Define keybindings here.
		}

		protected override void DecoratorUpdate()
		{
			if (editableTextControl.HasFocus()) 
			{

				keybindings.ExecuteAndConsumeIfMatched(Event.current);
			} 
			//if (HasFocus()) {
			//	int id = GUIUtility.keyboardControl;
			//	Debug.Log("Id for editor is " + id);
			//}
			base.DecoratorUpdate();

			editorGroup.editorStatusLabel.UpdateCursorInfo(LineNumber, ColumnNumber);
		}
	}
}
