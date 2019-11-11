using System;
using Kerbalui.Controls;
using Kerbalui.Decorators;
using Kerbalui.Util;
using UnityEngine;

namespace LiveRepl.UI.CompletionParts
{
	public class CompletionArea:EditingArea
	{
		public CompletionGroup completionGroup;

		public int SelectionIndex { get; private set; } = 0;

		public CompletionArea(CompletionGroup completionGroup) : base(new TextArea())
		{
			this.completionGroup=completionGroup;
		}

		protected override void DecoratorUpdate()
		{
			bool lastEventWasMouseDown = Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition);
			if (lastEventWasMouseDown)
			{
				editableTextControl.GrabFocus();
			}

			if (editableTextControl.HasFocus()) keybindings.ExecuteAndConsumeIfMatched(Event.current);
			base.DecoratorUpdate();

			if (lastEventWasMouseDown && Event.current.type == EventType.Used && rect.Contains(Event.current.mousePosition))
			{
				SelectionIndex = CurrentLineNumber();
				UpdateCursorPosition();
			}
		}

		void UpdateCursorPosition()
		{
			if (editor == null)
			{
				return;
			}

			editor.MoveTextStart();
			for (int i = 0; i < SelectionIndex; i++)
			{
				editor.MoveDown();
			}

			CursorIndex = editor.cursorIndex;
			SelectIndex = editor.selectIndex;
		}
	}
}
