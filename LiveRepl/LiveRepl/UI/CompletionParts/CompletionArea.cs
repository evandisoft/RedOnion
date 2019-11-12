using System;
using Kerbalui.Controls;
using Kerbalui.Decorators;
using Kerbalui.Util;
using UnityEngine;

namespace LiveRepl.UI.CompletionParts
{
	public class CompletionArea:EditingAreaScroller
	{
		public CompletionGroup completionGroup;

		public int SelectionIndex { get; private set; } = 0;

		public CompletionArea(CompletionGroup completionGroup) : base(new EditingArea(new TextArea()))
		{
			this.completionGroup=completionGroup;
		}

		protected override void DecoratorUpdate()
		{
			bool lastEventWasMouseDown = Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition);
			if (lastEventWasMouseDown)
			{
				editingArea.GrabFocus();
			}

			if (HasFocus()) editingArea.keybindings.ExecuteAndConsumeIfMatched(Event.current);
			base.DecoratorUpdate();

			if (lastEventWasMouseDown && Event.current.type == EventType.Used && rect.Contains(Event.current.mousePosition))
			{
				SelectionIndex = editingArea.LineNumber;
				UpdateCursorPosition();
			}
		}

		void UpdateCursorPosition()
		{
			if (editingArea.backingEditor==null)
			{
				return;
			}

			editingArea.backingEditor.MoveTextStart();
			for (int i = 0; i < SelectionIndex; i++)
			{
				editingArea.backingEditor.MoveDown();
			}

			editingArea.CursorIndex = editingArea.backingEditor.cursorIndex;
			editingArea.SelectIndex = editingArea.backingEditor.selectIndex;
		}
	}
}
