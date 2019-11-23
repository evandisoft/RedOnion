using UnityEngine;
using System.Collections.Generic;
using System;
using Kerbalui.EventHandling;
using Kerbalui.Controls.Abstract;
using Kerbalui.Decorators;
using Kerbalui.Controls;

namespace LiveRepl.Parts
{
    public class Editor:EditingAreaScroller
    {
		public ScriptWindowParts uiparts;

		/// <summary>
		/// These bindings intentionally shadow the base class bindings.
		/// </summary>

		public Editor(ScriptWindowParts uiparts) : base(new EditingArea(new TextArea()))
		{
			this.uiparts=uiparts;
			HorizontalScrollBarPresent=true;
			VerticalScrollBarPresent=true;
		}

		protected override void DecoratorUpdate()
		{
			if (HasFocus()) 
			{
				keybindings.ExecuteAndConsumeIfMatched(Event.current);
			} 

			base.DecoratorUpdate();

			uiparts.editorStatusLabel.UpdateCursorInfo(editingArea.LineNumber, editingArea.ColumnNumber);

			if (editingArea.ReceivedInput)
			{
				uiparts.editorChangesIndicator.Changed();
				uiparts.scriptWindow.needsResize=true;
			}
		}
	}
}
