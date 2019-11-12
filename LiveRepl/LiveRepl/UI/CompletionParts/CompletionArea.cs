using System;
using System.Collections.Generic;
using System.Text;
using Kerbalui.Controls;
using Kerbalui.Decorators;
using Kerbalui.EventHandling;
using Kerbalui.Util;
using LiveRepl.Interfaces;
using UnityEngine;

namespace LiveRepl.UI.CompletionParts
{
	public class CompletionArea: EditingAreaScroller, ICompletionSelector
	{
		IList<string> contentStrings = new List<string>();
		public CompletionGroup completionGroup;
		int partialLength;

		public string ControlName => editingArea.editableText.ControlName;

		public int SelectionIndex { get; private set; } = 0;

		public CompletionArea(CompletionGroup completionGroup) : base(new EditingArea(new TextArea()))
		{
			this.completionGroup=completionGroup;

			InitializeKeyBindings();
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

		public void SetContentFromICompletable(ICompletableElement completable)
		{
			contentStrings = completable.GetCompletionContent(out int replaceStart, out int replaceEnd);
			//Debug.Log("Getting Completions");
			StringBuilder sb = new StringBuilder();
			//Debug.Log("partial is " + partialCompletion);
			foreach (string str in contentStrings)
			{
				sb.Append(str);
				sb.Append('\n');
				//Debug.Log("Adding string " + str);
			}
			// Remove last newline
			if (sb.Length > 0)
			{
				sb.Remove(sb.Length - 1, 1);
			}
			editingArea.Text = sb.ToString();
			SelectionIndex = 0;
			partialLength = replaceEnd - replaceStart;
			UpdateCursorPosition();
		}

		void InitializeKeyBindings()
		{
			// Clear default bindings
			editingArea.keybindings.Clear();
			// Prevent underlying control from processing any keydown events.
			editingArea.onlyUseKeyBindings = true;
			keybindings.Add(new EventKey(KeyCode.K, true), () => {
				SelectionIndex = Math.Min(contentStrings.Count-1, SelectionIndex + 1);
				UpdateCursorPosition();
			});
			keybindings.Add(new EventKey(KeyCode.L, true), () => {
				SelectionIndex = Math.Max(0, SelectionIndex - 1);
				UpdateCursorPosition();
			});
			keybindings.Add(new EventKey(KeyCode.Comma, true), () => {
				SelectionIndex = Math.Min(contentStrings.Count - 1, SelectionIndex + 4);
				UpdateCursorPosition();
			});
			keybindings.Add(new EventKey(KeyCode.Period, true), () => {
				SelectionIndex = Math.Max(0, SelectionIndex - 4);
				UpdateCursorPosition();
			});
		}
	}
}