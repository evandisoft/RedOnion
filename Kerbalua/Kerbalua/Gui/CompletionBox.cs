using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace Kerbalua.Gui {
	public class CompletionBox:EditingArea,ICompletionSelector {
		IList<string> contentStrings = new List<string>();
		int partialLength;

		public int SelectionIndex { get; private set; } = 0;

		public void SetContentFromICompletable(ICompletable completable)
		{
			contentStrings = completable.GetCompletionContent(out int replaceStart, out int replaceEnd);
			//Debug.Log("Getting Completions");
			StringBuilder sb = new StringBuilder();
			//Debug.Log("partial is " + partialCompletion);
			foreach(string str in contentStrings) {
				sb.Append(str);
				sb.Append('\n');
				//Debug.Log("Adding string " + str);
			}
			// Remove last newline
			if (sb.Length > 0) {
				sb.Remove(sb.Length - 1, 1);
			}
			content.text = sb.ToString();
			SelectionIndex = 0;
			partialLength = replaceEnd - replaceStart;
			UpdateCursorPosition();
		}

		void UpdateCursorPosition()
		{
			if (editor == null) {
				return;
			}
			//Debug.Log("Updating Cursor Position");

			//int partialCompletionLength = partialLength.Length;
			editor.MoveTextStart();
			for(int i = 0;i < SelectionIndex; i++) {
				editor.MoveDown();
			}

			//for(int i=0;i < partialCompletionLength;i++) {
			//	editor.SelectRight();
			//}
			cursorIndex = editor.cursorIndex;
			selectIndex = editor.selectIndex;
		}

		public CompletionBox()
		{
			InitializeKeyBindings();
		}

		void InitializeKeyBindings()
		{
			// Clear default bindings
			KeyBindings.Clear();
			// Prevent underlying control from processing any keydown events.
			onlyUseKeyBindings = true;
			KeyBindings.Add(new EventKey(KeyCode.K,true), () => {
				SelectionIndex = Math.Min(contentStrings.Count-1, SelectionIndex + 1);
				UpdateCursorPosition();
			});
			KeyBindings.Add(new EventKey(KeyCode.L, true), () => {
				SelectionIndex = Math.Max(0, SelectionIndex - 1);
				UpdateCursorPosition();
			});
			KeyBindings.Add(new EventKey(KeyCode.Comma, true), () => {
				SelectionIndex = Math.Min(contentStrings.Count - 1, SelectionIndex + 4);
				UpdateCursorPosition();
			});
			KeyBindings.Add(new EventKey(KeyCode.Period, true), () => {
				SelectionIndex = Math.Max(0, SelectionIndex - 4);
				UpdateCursorPosition();
			});
		}

		protected override void ProtectedUpdate(Rect rect)
		{
			if (style == null) {
				style = new GUIStyle(GUI.skin.textArea);
			}
			style.richText = true;

			//if (HasFocus()) {
			//	int id = GUIUtility.keyboardControl;
			//	Debug.Log("Id for completionBox is " + id);
			//}

			bool lastEventWasMouseDown = Event.current.type == EventType.MouseDown && GUIUtil.MouseInRect(rect);
			if (lastEventWasMouseDown) {
				GrabFocus();
			}
			if (HasFocus()) KeyBindings.ExecuteAndConsumeIfMatched(Event.current);
			base.ProtectedUpdate(rect);
			if (lastEventWasMouseDown && Event.current.type == EventType.Used && GUIUtil.MouseInRect(rect)) {
				//Vector2 relativeMouse = GUIUtil.MouseRelativeToRect(rect);
				//Debug.Log(relativeMouse);
				//Debug.Log(style.lineHeight);
				//Debug.Log(relativeMouse.y / style.lineHeight);
				//double lineNum = Math.Floor(relativeMouse.y / style.lineHeight);
				//float contentHeight = style.CalcSize(content).y;
				//float fractionalScreenDown=scrollPos/rect.height
				//Debug.Log("Line num is "+ lineNum);
				//SelectionIndex = (int)lineNum;
				//Debug.Log("SelectionIndex was " + SelectionIndex);
				//UpdateCursorPosition();
				//Debug.Log(editor.cursorIndex);
				//Rect rectMinusScrollBar = rect;
				//rectMinusScrollBar.width = rect.width - 20;
				//rectMinusScrollBar.height = rect.height - 20;
				//if (GUIUtil.MouseInRect(rectMinusScrollBar)) {
				SelectionIndex = CurrentLineNumber();
				UpdateCursorPosition();

			}
			//Debug.Log(editor.cursorIndex);
		}
	}
}
