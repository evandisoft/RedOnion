using System;
using UnityEngine;
using System.Collections.Generic;

namespace Kerbalua.Gui {
	public class EditingArea:ScrollableTextArea {
		int inc = 0;
		public int cursorIndex = 0;
		public int selectIndex = 0;
		const int spacesPerTab = 4;
		public KeyBindings KeyBindings = new KeyBindings();
		TextEditor editor;

		public EditingArea()
		{
			InitializeDefaultKeyBindings();
		}

		public override void Render(Rect rect, GUIStyle style = null)
		{
			if (style == null) {
				style = new GUIStyle(GUI.skin.textArea);
				style.font = GUIUtil.GetMonoSpaceFont();
				style.hover.textColor
					=style.normal.textColor
					=style.active.textColor 
					=Color.white;

			}


			if (HasFocus()) {
				int id = GUIUtility.keyboardControl;
				editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), id);
				//Debug.Log(ControlName+","+inc++);
				editor.text = content.text;
				editor.cursorIndex = cursorIndex;
				editor.selectIndex = selectIndex;
				//HandleInput(editor);
				KeyBindings.ExecuteAndConsumeIfMatched(Event.current);
				if(Event.current.type==EventType.KeyDown) 
					GUIUtil.ConsumeMarkedCharEvent(Event.current);

				content.text = editor.text;

				base.Render(rect, style);

				cursorIndex = editor.cursorIndex;
				selectIndex = editor.selectIndex;
			} else {
				base.Render(rect, style);
			}
		}

		void InitializeDefaultKeyBindings()
		{
			KeyBindings.Add(new EventKey(KeyCode.Tab), () => Indent());
			KeyBindings.Add(new EventKey(KeyCode.Tab, false, true), () => Unindent());
			KeyBindings.Add(new EventKey(KeyCode.Tab, true), () => IndentToPreviousLine());
			KeyBindings.Add(new EventKey(KeyCode.J, true), () => editor.MoveLeft());
			KeyBindings.Add(new EventKey(KeyCode.J, true, true), () => editor.SelectLeft());
			KeyBindings.Add(new EventKey(KeyCode.K, true), () => editor.MoveDown());
			KeyBindings.Add(new EventKey(KeyCode.K, true, true), () => editor.SelectDown());
			KeyBindings.Add(new EventKey(KeyCode.L, true), () => editor.MoveUp());
			KeyBindings.Add(new EventKey(KeyCode.L, true, true), () => editor.SelectUp());
			KeyBindings.Add(new EventKey(KeyCode.Semicolon, true), () => editor.MoveRight());
			KeyBindings.Add(new EventKey(KeyCode.Semicolon, true, true), () => editor.SelectRight());
			KeyBindings.Add(new EventKey(KeyCode.M, true), () => {
				int toMove = NextTabLeft(cursorIndex);
				for (int i = 0;i < toMove;i++) {
					editor.MoveLeft();
				}
			});
			KeyBindings.Add(new EventKey(KeyCode.M, true, true), () => {
				int toMove = NextTabLeft(selectIndex);
				for (int i = 0;i < toMove;i++) {
					editor.MoveLeft();
				}
			});
			KeyBindings.Add(new EventKey(KeyCode.Comma, true), () => {
				for (int i = 0;i < 4;i++) {
					editor.MoveDown();
				}
			});
			KeyBindings.Add(new EventKey(KeyCode.Comma, true, true), () => {
				for (int i = 0;i < 4;i++) {
					editor.SelectDown();
				}
			});
			KeyBindings.Add(new EventKey(KeyCode.Period, true), () => {
				for (int i = 0;i < 4;i++) {
					editor.MoveUp();
				}
			});
			KeyBindings.Add(new EventKey(KeyCode.Period, true, true), () => {
				for (int i = 0;i < 4;i++) {
					editor.SelectUp();
				}
			});
			KeyBindings.Add(new EventKey(KeyCode.Slash, true), () => {
				int toMove = NextTabRight(cursorIndex);
				for (int i = 0;i < toMove;i++) {
					editor.MoveRight();
				}
			});
			KeyBindings.Add(new EventKey(KeyCode.Slash, true, true), () => {
				int toMove = NextTabRight(selectIndex);
				for (int i = 0;i < toMove;i++) {
					editor.MoveRight();
				}
			});
			KeyBindings.Add(new EventKey(KeyCode.O, true), () => {
				InsertLineAfter();
				IndentToPreviousLine();
			});
			KeyBindings.Add(new EventKey(KeyCode.O, true, true), () => {
				InsertLineBefore();
				IndentToPreviousLine();
			});
			KeyBindings.Add(new EventKey(KeyCode.Return), () => {
				editor.ReplaceSelection("\n");
				IndentToPreviousLine();
			});
		}

		//bool EventWillModifyEditor(Event event1)
		//{
		//	return !(
		//		event1.keyCode == KeyCode.LeftShift ||
		//		event1.keyCode == KeyCode.LeftControl ||
		//		event1.keyCode == KeyCode.LeftAlt
		//		);
		//}

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
				switch (event1.keyCode) {
				//case KeyCode.Tab:
					////Debug.Log(event1.keyCode);
					//if (event1.control) {
					//	IndentToPreviousLine();
					//} else if (event1.shift) {
					//	Unindent();
					//	//Debug.Log("Unindent");
					//} else {
					//	Indent();
					//	//Debug.Log("Indent");
					//}

					//GUIUtil.ConsumeAndMarkNextCharEvent(event1);
					//break;
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
							toMove = NextTabLeft(selectIndex);
							for (int i = 0;i < toMove;i++) {
								editor.SelectLeft();
							}
						} else {
							toMove = NextTabLeft(cursorIndex);
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
							toMove = NextTabRight(editor.selectIndex);
							for (int i = 0;i < toMove;i++) {
								editor.SelectRight();
							}
						} else {
							toMove = NextTabRight(editor.cursorIndex);
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
							InsertLineBefore();
						} else {
							InsertLineAfter();
						}
						IndentToPreviousLine();
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
					IndentToPreviousLine();
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

				GUIUtil.ConsumeMarkedCharEvent(event1);
			}
		}


		int NextTabLeft(int fromIndex)
		{
			int prevCursorIndex = editor.cursorIndex;
			int prevSelectIndex = editor.selectIndex;

			editor.cursorIndex = fromIndex;
			int charsFromStart = CharsFromLineStart();

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

		int NextTabRight(int fromIndex)
		{
			int prevCursorIndex = editor.cursorIndex;
			int prevSelectIndex = editor.selectIndex;

			editor.cursorIndex = fromIndex;
			int charsFromStart = CharsFromLineStart();


			// Don't run over to next line
			int charsFromEnd = CharsFromLineEnd();
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

		void InsertLineBefore()
		{
			editor.MoveLineStart();
			editor.ReplaceSelection("\n");
			editor.MoveLeft();
		}

		void InsertLineAfter()
		{
			editor.MoveLineEnd();
			editor.ReplaceSelection("\n");
		}

		void IndentToPreviousLine()
		{
			RemoveIndentation();
			int currentIndex = editor.cursorIndex;
			editor.MoveUp();
			int indentation = StartingSpaces();
			editor.MoveDown();
			editor.MoveLineStart();

			for(int i=0; i<indentation; i++) {
				editor.Insert(' ');
			}

			int newIndex = currentIndex + indentation;

			MoveToIndex(newIndex);
		}

		void RemoveIndentation()
		{
			int indentation = StartingSpaces();
			int newIndex = Math.Max(editor.cursorIndex-indentation,0);
			editor.MoveLineStart();
			for(int i = 0;i < indentation;i++) {
				editor.Delete();
			}

			MoveToIndex(newIndex);
		}

		/// <summary>
		/// Adds one level of indentation
		/// </summary>
		void Indent()
		{
			int startingSpaces = StartingSpaces();
			int charsNeeded = spacesPerTab - startingSpaces % spacesPerTab;
			//Debug.Log("Chars needed " + charsNeeded);

			int prevCursorIndex = editor.cursorIndex+spacesPerTab;
			editor.MoveLineStart();
			for (int i = 0;i < charsNeeded;i++) {
				editor.Insert(' ');
			}

			MoveToIndex(prevCursorIndex);
		}

		/// <summary>
		/// Removes one level of indentation
		/// </summary>
		void Unindent()
		{
			int startingSpaces = StartingSpaces();

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
			MoveToIndex(newIndex);
		}

		void MoveToIndex(int index)
		{
			editor.cursorIndex = index;
			editor.selectIndex = index;
		}

		/// <summary>
		/// Returns the number of characters from the start
		/// of the line to the cursor
		/// </summary>
		/// <returns>The from line start.</returns>
		int CharsFromLineStart()
		{
			int prevCursorIndex = editor.cursorIndex;
			editor.MoveLineStart();
			int startIndex = editor.cursorIndex;

			int chars = prevCursorIndex - startIndex;

			//Debug.Log("chars from line start is " + chars);

			MoveToIndex(prevCursorIndex);
			
			return chars;
		}

		int CharsFromLineEnd()
		{
			int prevCursorIndex = editor.cursorIndex;
			editor.MoveLineEnd();
			int endIndex = editor.cursorIndex;

			int chars = endIndex-prevCursorIndex;

			//Debug.Log("chars from line start is " + chars);

			MoveToIndex(prevCursorIndex);

			return chars;
		}

		/// <summary>
		/// Gets number of starting spaces on the current line
		/// </summary>
		/// <returns>The spaces.</returns>
		int StartingSpaces()
		{
			string currentLine=CurrentLine();
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
		string CurrentLine()
		{
			int prevCursorIndex = editor.cursorIndex;
			editor.MoveLineEnd();
			int endIndex = editor.cursorIndex;
			editor.MoveLineStart();
			int startIndex = editor.cursorIndex;
			string currentLine = editor.text.Substring(startIndex, endIndex - startIndex);

			//Debug.Log("The Line is " + currentLine);

			MoveToIndex(prevCursorIndex);

			return currentLine;
		}
	}
}
