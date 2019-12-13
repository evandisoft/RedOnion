using System;
using Kerbalui.Controls.Abstract;
using Kerbalui.EditingChanges;
using Kerbalui.EventHandling;
using UnityEngine;

using static RedOnion.Debugging.QueueLogger;

namespace Kerbalui.Decorators
{
	public class UndoRedoEditor : EditingArea
	{
		public EditingChangesManager changesManager=new EditingChangesManager();
		public new KeyBindings keybindings=new KeyBindings();

		public UndoRedoEditor(EditableText editableText) : base(editableText)
		{
			keybindings.Add(new EventKey(KeyCode.Z, true), () =>
			{
				UndoLogger.DebugLogArray("Undo pressed");
				var initialState=new EditingState(Text,CursorIndex,SelectIndex);
				var newState=changesManager.Undo(initialState);
				Text=newState.text; CursorIndex=newState.cursorIndex; SelectIndex=newState.selectionIndex;
				UndoLogger.DebugLogArray("CursorIndex", CursorIndex, "SelectIndex", SelectIndex);
			});
			keybindings.Add(new EventKey(KeyCode.Z, true, true), () =>
			{
				UndoLogger.DebugLogArray("Redo pressed");
				var initialState=new EditingState(Text,CursorIndex,SelectIndex);
				var newState=changesManager.Redo(initialState);
				Text=newState.text; CursorIndex=newState.cursorIndex; SelectIndex=newState.selectionIndex;
			});
		}

		protected override void DecoratorUpdate()
		{
			var mouseOrKeyboardInput=Event.current.type == EventType.KeyDown || (Event.current.type == EventType.KeyDown && rect.Contains(Event.current.mousePosition));
			EditingState initialState=new EditingState();

			if (mouseOrKeyboardInput)
			{
				UndoLogger.DebugLogArray("Has input");
				initialState=new EditingState(Text, CursorIndex, SelectIndex);
			}

			bool undoRedoMatched=false;
			if (HasFocus())
			{
				undoRedoMatched=keybindings.ExecuteAndConsumeIfMatched(Event.current);
			}

			base.DecoratorUpdate();

			if (!undoRedoMatched && mouseOrKeyboardInput)
			{
				var endState=new EditingState(Text,CursorIndex,SelectIndex);
				changesManager.AddChange(initialState, endState);
				UndoLogger.DebugLogArray("Recording change", 
					"\nchanges length:", changesManager.ChangesLength,
					"\nchanges index:", changesManager.CurrentIndex);
			}
		}
	}
}
