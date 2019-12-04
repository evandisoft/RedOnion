using UnityEngine;
using System.Collections.Generic;
using System;
using Kerbalui.EventHandling;
using Kerbalui.Controls.Abstract;
using Kerbalui.Decorators;
using Kerbalui.Controls;
using Kerbalui.Util;
using RedOnion.KSP.Settings;
using LiveRepl.Interfaces;
using Kerbalui.Interfaces;
using Kerbalui.EditingChanges;
using LiveRepl.Completion;

namespace LiveRepl.Parts
{
    public class Editor:EditingAreaScroller,ICompletableElement
    {
		public ScriptWindowParts uiparts;

		/// <summary>
		/// These bindings intentionally shadow the base class bindings.
		/// </summary>
		/// 
		UndoRedoEditor undoRedoEditor;

		public new string Text
		{
			get
			{
				return editingArea.Text;
			}
		}

		public EditingState EditingState
		{
			get
			{
				return new EditingState(editingArea.Text, editingArea.CursorIndex, editingArea.SelectIndex);
			}
		}

		/// <summary>
		/// These are to make sure that you only either modify text as part of the
		/// history, or you change the entire text and reset the history.
		/// </summary>
		/// <param name="text">Text.</param>
		public void ModifyAndResetUndo(string text)
		{
			ResetUndoHistory();
			editingArea.Text=text;
		}
		public void ModifyWithUndoRedo(EditingState editingState)
		{
			undoRedoEditor.changesManager.AddChange(EditingState, editingState);
			editingArea.Text=editingState.text; CursorIndex=editingState.cursorIndex; SelectIndex=editingState.selectionIndex;
		}

		public void ResetUndoHistory()
		{
			undoRedoEditor.changesManager.Clear();
		}

		public Editor(ScriptWindowParts uiparts) : base(new UndoRedoEditor(new TextArea()))
		{

			this.uiparts=uiparts;
			undoRedoEditor=editingArea as UndoRedoEditor;

			uiparts.FontChange+=editingArea.FontChangeEventHandler;

			HorizontalScrollBarPresent=false;
			VerticalScrollBarPresent=true;
		}


		protected override void DecoratorUpdate()
		{
			var keyboardInput=Event.current.type == EventType.KeyDown;

			string prevString=Text;

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

			if (editingArea.ReceivedInput && Text!=prevString)
			{
				uiparts.editorChangesIndicator.Changed();
				uiparts.scriptWindow.needsResize=true;
			}
		}

		public void Complete(int index)
		{
			EditingState newEditingState=CompletionHelper.Complete(index,uiparts,EditingState);
			ModifyWithUndoRedo(newEditingState);
		}

		public IList<string> GetCompletionContent(out int replaceStart, out int replaceEnd)
		{
			return uiparts.scriptWindow.currentReplEvaluator.GetDisplayableCompletions(Text, CursorIndex, out replaceStart, out replaceEnd);
		}
	}
}
