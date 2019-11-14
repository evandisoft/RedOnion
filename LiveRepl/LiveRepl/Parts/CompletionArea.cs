using System;
using System.Collections.Generic;
using System.Text;
using Kerbalui.Controls;
using Kerbalui.Decorators;
using Kerbalui.EventHandling;
using Kerbalui.Util;
using LiveRepl.Interfaces;
using UnityEngine;

namespace LiveRepl.Parts
{
	public class CompletionArea: EditingAreaScroller, ICompletionSelector
	{
		IList<string> contentStrings = new List<string>();
		ScriptWindowParts uiparts;
		int partialLength;

		public string ControlName => editingArea.editableText.ControlName;

		public int SelectionIndex { get; private set; } = 0;

		public CompletionArea(ScriptWindowParts uiparts) : base(new EditingArea(new TextArea()))
		{
			this.uiparts=uiparts;

			InitializeKeyBindings();
		}

		protected override void DecoratorUpdate()
		{
			bool lastEventWasMouseDown = Event.current.type == EventType.MouseDown && GUILibUtil.MouseInRect(rect);// ContentRect.Contains(Event.current.mousePosition); //GUILibUtil.MouseInRect(rect);
			string lastControlname = GUI.GetNameOfFocusedControl();

			base.DecoratorUpdate();

			var scrollbarlessrect=new Rect(rect); //new Rect(ContentRect);
			if (VerticalScrollBarPresent)
				scrollbarlessrect.width-=ScrollbarWidth;
			if (HorizontalScrollBarPresent)
				scrollbarlessrect.height-=ScrollbarWidth;
			if (lastEventWasMouseDown && Event.current.type == EventType.Used && GUILibUtil.MouseInRect(scrollbarlessrect)) //ContentRect.Contains(Event.current.mousePosition))
			{
				//Debug.Log("This was clicked");
				SelectionIndex = editingArea.LineNumber-1;
				//Debug.Log("Selection is "+SelectionIndex);
				UpdateCursorPosition();
				uiparts.scriptWindow.completionManager.Complete();
				uiparts.scriptWindow.needsResize=true;
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
			editingArea.keybindings.Add(new EventKey(KeyCode.K, true), () => {
				SelectionIndex = Math.Min(contentStrings.Count-1, SelectionIndex + 1);
				UpdateCursorPosition();
			});
			editingArea.keybindings.Add(new EventKey(KeyCode.L, true), () => {
				SelectionIndex = Math.Max(0, SelectionIndex - 1);
				UpdateCursorPosition();
			});
			editingArea.keybindings.Add(new EventKey(KeyCode.Comma, true), () => {
				SelectionIndex = Math.Min(contentStrings.Count - 1, SelectionIndex + 4);
				UpdateCursorPosition();
			});
			editingArea.keybindings.Add(new EventKey(KeyCode.Period, true), () => {
				SelectionIndex = Math.Max(0, SelectionIndex - 4);
				UpdateCursorPosition();
			});
		}
	}
}
