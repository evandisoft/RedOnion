using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace Kerbalua.Gui {
	public class CompletionBox:EditingArea,ICompletionSelector {
		List<string> contentStrings = new List<string>();
		string partialCompletion;

		public int SelectionIndex { get; private set; } = 0;

		public void SetContentWithStringList(List<string> contentStrings,string partialCompletion)
		{
			this.contentStrings = contentStrings;
			StringBuilder sb = new StringBuilder();
			foreach(string str in contentStrings) {
				sb.Append(str);
				sb.Append('\n');
			}
			content.text = sb.ToString();
			SelectionIndex = 0;
			this.partialCompletion = partialCompletion;
			SelectPartialCompletion();
		}

		void SelectPartialCompletion()
		{
			if (editor == null) {
				return;
			}
			int partialCompletionLength = partialCompletion.Length;
			editor.MoveTextStart();
			for(int i = 0;i < SelectionIndex; i++) {
				editor.MoveDown();
			}
			for(int i=0;i < partialCompletionLength;i++) {
				editor.SelectRight();
			}
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
			KeyBindings.Add(new EventKey(KeyCode.K), () => {
				SelectionIndex = Math.Min(contentStrings.Count-1, SelectionIndex + 1);
				SelectPartialCompletion();
			});
			KeyBindings.Add(new EventKey(KeyCode.L), () => {
				SelectionIndex = Math.Max(0, selectIndex - 1);
				SelectPartialCompletion();
			});
		}

		public override void Render(Rect rect, GUIStyle style = null)
		{
			if (style == null) {
				style = new GUIStyle(GUI.skin.textArea);
			}

			if (HasFocus()) KeyBindings.ExecuteAndConsumeIfMatched(Event.current);
			base.Render(rect, style);
		}
	}
}
