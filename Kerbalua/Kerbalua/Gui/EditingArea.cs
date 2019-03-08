using System;
using UnityEngine;
using System.Collections.Generic;

namespace Kerbalua.Gui {
	public class EditingArea:ScrollableTextArea {
		int inc = 0;
		public int cursorIndex = 0;
		public int selectIndex = 0;
		const int spacesPerTab = 4;

		public override void Render(Rect rect, GUIStyle style = null)
		{
			if (style == null) {
				style = new GUIStyle(GUI.skin.textArea);
				style.font = GUIUtil.GetMonoSpaceFont();
			}

			TextEditor editor;
			if (HasFocus()) {
				int id = GUIUtility.keyboardControl;
				editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), id);
				//Debug.Log(ControlName+","+inc++);
				editor.text = content.text;
				editor.cursorIndex = cursorIndex;
				editor.selectIndex = selectIndex;
				HandleInput(editor);
				
				content.text = editor.text;

				base.Render(rect, style);

				cursorIndex = editor.cursorIndex;
				selectIndex = editor.selectIndex;
			} else {
				base.Render(rect, style);
			}
		}

		bool EventWillModifyEditor(Event event1)
		{
			return !(
				event1.keyCode == KeyCode.LeftShift ||
				event1.keyCode == KeyCode.LeftControl ||
				event1.keyCode == KeyCode.LeftAlt
				);
		}

		/// <summary>
		/// Preprocesses input prior to allowing input events to get to the
		/// GUI.TextArea control. Intercepts and consumes input events that
		/// need to be handled differently from the default (like 'tab')
		/// 
		/// Can be overridden by subclasses to process/intercept input and handle some things
		/// a different way than is handled here.
		/// </summary>
		/// <param name="editor">Editor.</param>
		protected virtual void HandleInput(TextEditor editor)
		{
			Event event1 = Event.current;
			// Doesn't seem to work
			//if (EventWillModifyEditor(event1)) {
			//	editor.SaveBackup();
			//}

			if (event1.type == EventType.KeyDown) {
				GUIUtil.ConsumeMarkedCharEvent(event1);
				switch (event1.keyCode) {
				case KeyCode.Tab:
					//Debug.Log(event1.keyCode);
					if (event1.control) {
						IndentToPreviousLine(editor);
					} else if (event1.shift) {
						Unindent(editor);
						//Debug.Log("Unindent");
					} else {
						Indent(editor);
						//Debug.Log("Indent");
					}

					GUIUtil.ConsumeAndMarkNextCharEvent(event1);
					break;
				case KeyCode.J:
					if (event1.control) {
						if (event1.shift) {
							editor.SelectLeft();
						} else {
							editor.MoveLeft();
						}
						GUIUtil.ConsumeAndMarkNextCharEvent(event1);
					}
					break;
				case KeyCode.K:
					if (event1.control) {
						if (event1.shift) {
							editor.SelectDown();
						} else {
							editor.MoveDown();
						}
						GUIUtil.ConsumeAndMarkNextCharEvent(event1);
					}
					break;
				case KeyCode.L:
					if (event1.control) {
						if (event1.shift) {
							editor.SelectUp();
						} else {
							editor.MoveUp();
						}
						GUIUtil.ConsumeAndMarkNextCharEvent(event1);
					}
					break;
				case KeyCode.Semicolon:
					if (event1.control) {
						if (event1.shift) {
							editor.SelectRight();
						} else {
							editor.MoveRight();
						}

						GUIUtil.ConsumeAndMarkNextCharEvent(event1);
					}
					break;
				case KeyCode.M:
					if (event1.control) {
						int toMove;
						if (event1.shift) {
							toMove = NextTabLeft(editor, selectIndex);
							for (int i = 0;i < toMove;i++) {
								editor.SelectLeft();
							}
						} else {
							toMove = NextTabLeft(editor, cursorIndex);
							for (int i = 0;i < toMove;i++) {
								editor.MoveLeft();
							}
						}
						GUIUtil.ConsumeAndMarkNextCharEvent(event1);
					}
					break;
				case KeyCode.Comma:
					if (event1.control) {
						if (event1.shift) {
							for (int i = 0;i < 4;i++) {
								editor.SelectDown();
							}
						} else {
							for (int i = 0;i < 4;i++) {
								editor.MoveDown();
							}
						}
						GUIUtil.ConsumeAndMarkNextCharEvent(event1);
					}
					break;
				case KeyCode.Period:
					if (event1.control) {
						if (event1.shift) {
							for (int i = 0;i < 4;i++) {
								editor.SelectUp();
							}
						} else {
							for (int i = 0;i < 4;i++) {
								editor.MoveUp();
							}
						}
						GUIUtil.ConsumeAndMarkNextCharEvent(event1);
					}
					break;
				case KeyCode.Slash:
					if (event1.control) {
						int toMove;
						if (event1.shift) {
							toMove = NextTabRight(editor, editor.selectIndex);
							for (int i = 0;i < toMove;i++) {
								editor.SelectRight();
							}
						} else {
							toMove = NextTabRight(editor, editor.cursorIndex);
							for (int i = 0;i < toMove;i++) {
								editor.MoveRight();
							}
						}
						GUIUtil.ConsumeAndMarkNextCharEvent(event1);
					}
					break;
				case KeyCode.O:
					if (event1.control) {
						if (event1.shift) {
							InsertLineBefore(editor);
						} else {
							InsertLineAfter(editor);
						}
						IndentToPreviousLine(editor);
						GUIUtil.ConsumeAndMarkNextCharEvent(event1);
					}

					break;
				//case KeyCode.U:
				//if (event1.control) {
				// Doesn't seem to work
				//	editor.Undo();
				//	GUIUtil.ConsumeAndMarkNextCharEvent(event1);
				//}
				//break;
				case KeyCode.Return:
					editor.ReplaceSelection("\n");
					IndentToPreviousLine(editor);
					GUIUtil.ConsumeAndMarkNextCharEvent(event1);
					break;
				}
				//if (event1.type == EventType.Used)
				//{
				//	Debug.Log(event1 + " was used");
				//}
				//else
				//{
				//	Debug.Log(event1 + " was not used");
				//}


			}
		}


		int NextTabLeft(TextEditor editor,int fromIndex)
		{
			int prevCursorIndex = editor.cursorIndex;
			int prevSelectIndex = editor.selectIndex;

			editor.cursorIndex = fromIndex;
			int charsFromStart = CharsFromLineStart(editor);

			// Don't spill over to next line
			if (charsFromStart == 0) {
				return 0;
			}

			int charsToMove = charsFromStart % spacesPerTab;
			if (charsToMove == 0) {
				charsToMove = spacesPerTab;
			}
			//Debug.Log("Chars to move is " + charsToMove);

			editor.cursorIndex = prevCursorIndex;
			editor.selectIndex = prevSelectIndex;

			return charsToMove;
		}

		int NextTabRight(TextEditor editor, int fromIndex)
		{
			int prevCursorIndex = editor.cursorIndex;
			int prevSelectIndex = editor.selectIndex;

			editor.cursorIndex = fromIndex;
			int charsFromStart = CharsFromLineStart(editor);


			// Don't run over to next line
			int charsFromEnd = CharsFromLineEnd(editor);
			if (charsFromEnd < spacesPerTab) {
				return charsFromEnd;
			}

			int charsToMove = spacesPerTab - charsFromStart % spacesPerTab;
			if (charsToMove == 0) {
				charsToMove = spacesPerTab;
			}
			//Debug.Log("Chars to move is " + charsToMove);

			editor.cursorIndex = prevCursorIndex;
			editor.selectIndex = prevSelectIndex;

			return charsToMove;
		}

		void InsertLineBefore(TextEditor editor)
		{
			editor.MoveLineStart();
			editor.ReplaceSelection("\n");
			editor.MoveLeft();
		}

		void InsertLineAfter(TextEditor editor)
		{
			editor.MoveLineEnd();
			editor.ReplaceSelection("\n");
		}

		void IndentToPreviousLine(TextEditor editor)
		{
			RemoveIndentation(editor);
			int currentIndex = editor.cursorIndex;
			editor.MoveUp();
			int indentation = StartingSpaces(editor);
			editor.MoveDown();
			editor.MoveLineStart();

			for(int i=0; i<indentation; i++) {
				editor.Insert(' ');
			}

			int newIndex = currentIndex + indentation;

			MoveToIndex(editor, newIndex);
		}

		void RemoveIndentation(TextEditor editor)
		{
			int indentation = StartingSpaces(editor);
			int newIndex = Math.Max(editor.cursorIndex-indentation,0);
			editor.MoveLineStart();
			for(int i = 0;i < indentation;i++) {
				editor.Delete();
			}

			MoveToIndex(editor, newIndex);
		}

		/// <summary>
		/// Adds one level of indentation
		/// </summary>
		/// <param name="editor">Editor.</param>
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

			MoveToIndex(editor, prevCursorIndex);
		}

		/// <summary>
		/// Removes one level of indentation
		/// </summary>
		/// <param name="editor">Editor.</param>
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

			int prevCursorIndex = editor.cursorIndex;
			editor.MoveLineStart();
			int lineStartIndex = editor.cursorIndex;
			int newIndex = Math.Max(lineStartIndex, prevCursorIndex - charsToDelete);

			for (int i = 0;i < charsToDelete;i++) {
				editor.Delete();
			}
			MoveToIndex(editor, newIndex);
		}

		void MoveToIndex(TextEditor editor,int index)
		{
			editor.cursorIndex = index;
			editor.selectIndex = index;
		}

		/// <summary>
		/// Returns the number of characters from the start
		/// of the line to the cursor
		/// </summary>
		/// <returns>The from line start.</returns>
		/// <param name="editor">Editor.</param>
		int CharsFromLineStart(TextEditor editor)
		{
			int prevCursorIndex = editor.cursorIndex;
			editor.MoveLineStart();
			int startIndex = editor.cursorIndex;

			int chars = prevCursorIndex - startIndex;

			//Debug.Log("chars from line start is " + chars);

			MoveToIndex(editor, prevCursorIndex);
			
			return chars;
		}

		int CharsFromLineEnd(TextEditor editor)
		{
			int prevCursorIndex = editor.cursorIndex;
			editor.MoveLineEnd();
			int endIndex = editor.cursorIndex;

			int chars = endIndex-prevCursorIndex;

			//Debug.Log("chars from line start is " + chars);

			MoveToIndex(editor, prevCursorIndex);

			return chars;
		}

		/// <summary>
		/// Gets number of starting spaces on the current line
		/// </summary>
		/// <returns>The spaces.</returns>
		/// <param name="editor">Editor.</param>
		int StartingSpaces(TextEditor editor)
		{
			string currentLine=CurrentLine(editor);
			int i = 0;
			foreach(char c in currentLine) {
				if(c==' ') {
					i++;
				} else {
					break;
				}
			}

			//Debug.Log("Starting spaces is " + i);

			return i;
		}

		/// <summary>
		/// Returns the text on the current line
		/// </summary>
		/// <returns>The line.</returns>
		/// <param name="editor">Editor.</param>
		string CurrentLine(TextEditor editor)
		{
			int prevCursorIndex = editor.cursorIndex;
			editor.MoveLineEnd();
			int endIndex = editor.cursorIndex;
			editor.MoveLineStart();
			int startIndex = editor.cursorIndex;
			string currentLine = editor.text.Substring(startIndex, endIndex - startIndex);

			//Debug.Log("The Line is " + currentLine);

			MoveToIndex(editor, prevCursorIndex);

			return currentLine;
		}
	}
}
