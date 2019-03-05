using System;
using UnityEngine;

namespace Kerbalua.Gui {
	public class EditingArea:ScrollableTextArea {
		int inc = 0;
		const int spacesPerTab = 4;

		public override void Render(Rect rect, GUIStyle style = null)
		{
			if (style == null) {
				style = new GUIStyle(GUI.skin.textArea);
			}

			if (GUI.GetNameOfFocusedControl() == ControlName) {
				int id = GUIUtility.keyboardControl;
				TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), id);
				//Debug.Log(ControlName+","+inc++);

				HandleInput(editor);
				content.text = editor.text;
			}

			GUI.SetNextControlName(ControlName);
			base.Render(rect, style);


		}

		void HandleInput(TextEditor editor)
		{
			Event event1 = Event.current;
			if (event1.type == EventType.KeyDown) {
				switch (event1.keyCode) {
				case KeyCode.Tab:
					//Debug.Log(event1.keyCode);
					if (event1.shift) {
						Unindent(editor);
						//Debug.Log("Unindent");
					} else {
						Indent(editor);
						//Debug.Log("Indent");
					}

					event1.Use();	
					break;
				}
			}
		}

		void Indent(TextEditor editor)
		{
			int startingSpaces = StartingSpaces(editor);
			int charsNeeded = spacesPerTab - startingSpaces % spacesPerTab;
			//Debug.Log("Chars needed " + charsNeeded);

			int prevCursorIndex = editor.cursorIndex+spacesPerTab;
			editor.MoveLineStart();
			for (int i = 0;i < charsNeeded;i++) {
				editor.Insert(' ');
			}
			editor.cursorIndex = prevCursorIndex;
			editor.selectIndex = prevCursorIndex;
		}

		void Unindent(TextEditor editor)
		{
			int startingSpaces = StartingSpaces(editor);

			if (startingSpaces == 0) {
				return;
			}

			int charsToDelete = startingSpaces % spacesPerTab;
			if (charsToDelete == 0) {
				charsToDelete = spacesPerTab;
			}

			//Debug.Log("charsToDelete " + charsToDelete);

			int prevCursorIndex = editor.cursorIndex-spacesPerTab;
			editor.MoveLineStart();
			for (int i = 0;i < charsToDelete;i++) {
				editor.Delete();
			}
			editor.cursorIndex = prevCursorIndex;
			editor.selectIndex = prevCursorIndex;
		}

		int CharsFromLineStart(TextEditor editor)
		{
			int prevCursorIndex = editor.cursorIndex;
			editor.MoveLineStart();
			int startIndex = editor.cursorIndex;

			int chars = prevCursorIndex - startIndex;

			//Debug.Log("chars from line start is " + chars);

			editor.cursorIndex = prevCursorIndex;
			editor.selectIndex = prevCursorIndex;

			return chars;
		}

		int StartingSpaces(TextEditor editor)
		{
			string currentLine=CurrentLine(editor);
			int i = 0;
			foreach(char c in currentLine) {
				if(c==' ') {
					i++;
				}
			}

			//Debug.Log("Starting spaces is " + i);

			return i;
		}

		string CurrentLine(TextEditor editor)
		{
			int prevCursorIndex = editor.cursorIndex;
			editor.MoveLineEnd();
			int endIndex = editor.cursorIndex;
			editor.MoveLineStart();
			int startIndex = editor.cursorIndex;
			string currentLine = editor.text.Substring(startIndex, endIndex - startIndex);

			//Debug.Log("The Line is " + currentLine);

			editor.cursorIndex = prevCursorIndex;
			editor.selectIndex = prevCursorIndex;

			return currentLine;
		}
	}
}
