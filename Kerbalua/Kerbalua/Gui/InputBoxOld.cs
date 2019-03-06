using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class InputBoxOld:EditingArea {
		public GUIContent content = new GUIContent("");
		public int cursorPos;

		public void Render(Rect rect)
		{
			Event e = Event.current;
			if (e.type == EventType.Repaint) {
				// I have no idea how to use IMGUI library correctly beyond simple examples. 
				// Docs are hideously insufficient
				// This obscure solution for text selection and cursor control found at:
				// https://answers.unity.com/questions/145698/guistyledrawwithtextselection-how-to-use-.html
				int id = GUIUtility.GetControlID(content, FocusType.Keyboard, rect);
				//GUIUtility.keyboardControl = id; // added
				GUI.skin.textArea.DrawCursor(rect, content, id, cursorPos);

				//inputContent.text=GUI.TextArea(inputRect, inputContent.text);
				GUI.skin.textArea.DrawWithTextSelection(rect, content, id, 0, 0);
			}
		}

		protected override void HandleInput(TextEditor editor)
		{


			base.HandleInput(editor);
		}
	}
}
