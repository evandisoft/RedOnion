using UnityEngine;
using System.Collections.Generic;
using System;
using Kerbalui.EventHandling;
using Kerbalui.Controls.Abstract;
using Kerbalui.Decorators;
using Kerbalui.Controls;
using Kerbalui.Util;
using RedOnion.KSP.Settings;

namespace LiveRepl.Parts
{
    public class Editor:EditingAreaScroller
    {
		public ScriptWindowParts uiparts;

		/// <summary>
		/// These bindings intentionally shadow the base class bindings.
		/// </summary>
		/// 

		public Editor(ScriptWindowParts uiparts) : base(new UndoRedoEditor(new TextArea()))
		{
			this.uiparts=uiparts;

			uiparts.FontChange+=editingArea.editableText.FontChangeEventHandler;

			HorizontalScrollBarPresent=false;
			VerticalScrollBarPresent=true;
		}

		protected override void DecoratorUpdate()
		{
			if (HasFocus()) 
			{
				keybindings.ExecuteAndConsumeIfMatched(Event.current);
			} 

			//if (editingArea.TrySetFont(uiparts.fontSelector.CurrentFont))
			//{
			//	uiparts.scriptWindow.needsResize=true;
			//}

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
