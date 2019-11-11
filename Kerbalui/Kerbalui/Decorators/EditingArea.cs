﻿using System;
using UnityEngine;
using Kerbalui.Types;
using Kerbalui.Controls.Abstract;
using Kerbalui.EventHandling;
using Kerbalui.Util;

namespace Kerbalui.Decorators
{
	/// <summary>
	/// Contains an EditableText Control which it manages as an editing area.
	/// </summary>
	public class EditingArea : ContentScroller
	{
		//int inc = 0;
		const int spacesPerTab = 4;
		public KeyBindings keybindings = new KeyBindings();
		protected TextEditor editor;
		public int LineNumber { get; private set; } = 1;
		public int ColumnNumber { get; private set; } = 1;
		public int CursorIndex { get; private set; }
		public int SelectIndex { get; private set; }

		public EditableText editableTextControl;

		public bool hadKeyDownThisUpdate=false;

		/// <summary>
		/// Setting this to true will not allow any key-down input events
		/// to reach the control's default handling of events.
		/// </summary>
		protected bool onlyUseKeyBindings;

		public EditingArea(EditableText editableTextControl) : base(editableTextControl)
		{
			this.editableTextControl=editableTextControl;
			InitializeDefaultKeyBindings();
		}

		protected override void DecoratorUpdate()
		{
			// Initialize editor
			if (editor == null)
			{
				editableTextControl.GrabFocus();
				int id = GUIUtility.keyboardControl;
				editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), id);
			}

			if (editableTextControl.style == null)
			{
				editableTextControl.style = new GUIStyle(GUI.skin.textArea);
				editableTextControl.style.font = GUILibUtil.GetMonoSpaceFont();
				editableTextControl.style.hover.textColor
					= editableTextControl.style.normal.textColor
					= editableTextControl.style.active.textColor
					= Color.white;
			}

			if (editableTextControl.HasFocus())
			{
				int id = GUIUtility.keyboardControl;
				editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), id);
				editor.text = editableTextControl.content.text;
				editor.cursorIndex = CursorIndex;
				editor.selectIndex = SelectIndex;

				hadKeyDownThisUpdate=Event.current.type==EventType.KeyDown;
				HandleInput();

				LineNumber = CurrentLineNumber()+1;
				ColumnNumber = CharsFromLineStart()+1;

				editableTextControl.content.text = editor.text;

				base.DecoratorUpdate();

				CursorIndex = editor.cursorIndex;
				SelectIndex = editor.selectIndex;

				if (hadKeyDownThisUpdate && Event.current.type == EventType.Used)
				{
					AdjustScrollX();
					AdjustScrollY();
				}
			}
			else
			{
				base.DecoratorUpdate();
			}
		}

		void AdjustScrollX()
		{
			//Debug.Log("Adjusting scroll x");
			float cursorX = CursorX();
			float diff = lastContentVector2.x - lastScrollViewVector2.x;
			float contentStartX = scrollPos.x;
			float contentEndX = contentStartX + lastScrollViewVector2.x;
			if (Math.Max(cursorX - editableTextControl.style.lineHeight, 0) < contentStartX)
			{
				scrollPos.x = Math.Max(cursorX - editableTextControl.style.lineHeight, 0);
			}
			else if (cursorX + editableTextControl.style.lineHeight > contentEndX)
			{
				scrollPos.x = cursorX - lastContentVector2.x + editableTextControl.style.lineHeight;
			}
		}

		void AdjustScrollY()
		{
			//Debug.Log("Adjusting scroll y");
			float cursorY = CursorY();
			//Debug.Log("CursorY " + cursorY);
			float diff = lastContentVector2.y - lastScrollViewVector2.y;
			//Debug.Log("diff " + diff);
			float contentStartY = scrollPos.y;
			//Debug.Log("contentStartY " + contentStartY);
			float contentEndY = contentStartY + lastScrollViewVector2.y;
			//Debug.Log("contentEndY " + contentEndY);
			if (cursorY - editableTextControl.style.lineHeight < contentStartY)
			{
				scrollPos.y = cursorY - editableTextControl.style.lineHeight;
				//Debug.Log("reducing to " + scrollPos.y);
			}
			else if (cursorY + editableTextControl.style.lineHeight > contentEndY)
			{
				scrollPos.y = cursorY - lastContentVector2.y + editableTextControl.style.lineHeight;
				//Debug.Log("expanding to " + scrollPos.y);
			}
		}

		float CursorX()
		{
			int c = CharsFromLineStart();
			string startOfLineToCursor = CurrentLine().Substring(0, c);
			GUIContent tempContent = new GUIContent(startOfLineToCursor);
			return editableTextControl.style.CalcSize(tempContent).x;
		}

		float CursorY()
		{
			string contentToCursor = editableTextControl.content.text.Substring(0, CursorIndex);
			GUIContent tempContent = new GUIContent(contentToCursor);
			return editableTextControl.style.CalcSize(tempContent).y;
		}

		void HandleInput()
		{
			keybindings.ExecuteAndConsumeIfMatched(Event.current);

			// Intercept all keydown events that are about to be processed by the
			// control itself if onlyUseKeyBindings is set to true.
			if (onlyUseKeyBindings && Event.current.type == EventType.KeyDown)
			{
				Event.current.Use();
			}
		}

		void InitializeDefaultKeyBindings()
		{
			keybindings.Add(new EventKey(KeyCode.Home, true, true), () =>
			{
				editor.SelectTextStart();
			});
			keybindings.Add(new EventKey(KeyCode.End, true, true), () =>
			{
				editor.SelectTextEnd();
			});
			keybindings.Add(new EventKey(KeyCode.Home, true), () =>
			{
				editor.MoveTextStart();
			});
			keybindings.Add(new EventKey(KeyCode.End, true), () =>
			{
				editor.MoveTextEnd();
			});
			keybindings.Add(new EventKey(KeyCode.Insert, true), () =>
			{
				editor.Copy();
			});
			keybindings.Add(new EventKey(KeyCode.Insert, false, true), () =>
			{
				editor.Paste();
			});
			keybindings.Add(new EventKey(KeyCode.Backspace, false, true), () =>
			 {
				 int toMove = NextTabLeft(CursorIndex);
				 for (int i = 0; i < toMove; i++)
				 {
					 editor.Backspace();
				 }
			 });
			keybindings.Add(new EventKey(KeyCode.Backspace, true), () =>
			{
				while (true)
				{
					if (editor.cursorIndex <= 0)
					{
						break;
					}

					if (editor.text[editor.cursorIndex - 1] == '\n')
					{
						break;
					}

					editor.Backspace();

					if (editor.cursorIndex <= 0)
					{
						break;
					}
					if (editor.text[editor.cursorIndex - 1] == '.')
					{
						break;
					}

				}
			});
			keybindings.Add(new EventKey(KeyCode.Tab), () => Indent());
			keybindings.Add(new EventKey(KeyCode.Tab, false, true), () => Unindent());
			keybindings.Add(new EventKey(KeyCode.Tab, true), () => IndentToPreviousLine());
			keybindings.Add(new EventKey(KeyCode.J, true), () => editor.MoveLeft());
			keybindings.Add(new EventKey(KeyCode.J, true, true), () => editor.SelectLeft());
			keybindings.Add(new EventKey(KeyCode.K, true), () => editor.MoveDown());
			keybindings.Add(new EventKey(KeyCode.K, true, true), () => editor.SelectDown());
			keybindings.Add(new EventKey(KeyCode.L, true), () => editor.MoveUp());
			keybindings.Add(new EventKey(KeyCode.L, true, true), () => editor.SelectUp());
			keybindings.Add(new EventKey(KeyCode.Semicolon, true), () => editor.MoveRight());
			keybindings.Add(new EventKey(KeyCode.Semicolon, true, true), () => editor.SelectRight());
			keybindings.Add(new EventKey(KeyCode.M, true), () =>
			{
				int toMove = NextTabLeft(CursorIndex);
				for (int i = 0; i < toMove; i++)
				{
					editor.MoveLeft();
				}
			});
			keybindings.Add(new EventKey(KeyCode.M, true, true), () =>
			{
				int toMove = NextTabLeft(SelectIndex);
				for (int i = 0; i < toMove; i++)
				{
					editor.MoveLeft();
				}
			});
			keybindings.Add(new EventKey(KeyCode.Comma, true), () =>
			{
				for (int i = 0; i < 4; i++)
				{
					editor.MoveDown();
				}
			});
			keybindings.Add(new EventKey(KeyCode.Comma, true, true), () =>
			{
				for (int i = 0; i < 4; i++)
				{
					editor.SelectDown();
				}
			});
			keybindings.Add(new EventKey(KeyCode.Period, true), () =>
			{
				for (int i = 0; i < 4; i++)
				{
					editor.MoveUp();
				}
			});
			keybindings.Add(new EventKey(KeyCode.Period, true, true), () =>
			{
				for (int i = 0; i < 4; i++)
				{
					editor.SelectUp();
				}
			});
			keybindings.Add(new EventKey(KeyCode.Slash, true), () =>
			{
				int toMove = NextTabRight(CursorIndex);
				for (int i = 0; i < toMove; i++)
				{
					editor.MoveRight();
				}
			});
			keybindings.Add(new EventKey(KeyCode.Slash, true, true), () =>
			{
				int toMove = NextTabRight(SelectIndex);
				for (int i = 0; i < toMove; i++)
				{
					editor.MoveRight();
				}
			});
			keybindings.Add(new EventKey(KeyCode.H, true), () =>
			{
				InsertLineBefore();
				IndentToPreviousLine();
			});
			keybindings.Add(new EventKey(KeyCode.N, true), () =>
			{
				InsertLineAfter();
				IndentToPreviousLine();
			});
			keybindings.Add(new EventKey(KeyCode.Return), () =>
			{
				editor.ReplaceSelection("\n");
				IndentToPreviousLine();
			});
			keybindings.Add(new EventKey(KeyCode.Return,false,true), () =>
			{
				editor.ReplaceSelection("\n");
				IndentToPreviousLine();
			});
		}

		protected int NextTabLeft(int fromIndex)
		{
			int prevCursorIndex = editor.cursorIndex;
			int prevSelectIndex = editor.selectIndex;

			editor.cursorIndex = fromIndex;
			int charsFromStart = CharsFromLineStart();

			// Don't spill over to next line
			if (charsFromStart == 0)
			{
				return 0;
			}

			int charsToMove = charsFromStart % spacesPerTab;
			if (charsToMove == 0)
			{
				charsToMove = spacesPerTab;
			}
			//Debug.Log("Chars to move is " + charsToMove);

			editor.cursorIndex = prevCursorIndex;
			editor.selectIndex = prevSelectIndex;

			return charsToMove;
		}

		protected int NextTabRight(int fromIndex)
		{
			int prevCursorIndex = editor.cursorIndex;
			int prevSelectIndex = editor.selectIndex;

			editor.cursorIndex = fromIndex;
			int charsFromStart = CharsFromLineStart();


			// Don't run over to next line
			int charsFromEnd = CharsFromLineEnd();
			if (charsFromEnd < spacesPerTab)
			{
				return charsFromEnd;
			}

			int charsToMove = spacesPerTab - charsFromStart % spacesPerTab;
			if (charsToMove == 0)
			{
				charsToMove = spacesPerTab;
			}
			//Debug.Log("Chars to move is " + charsToMove);

			editor.cursorIndex = prevCursorIndex;
			editor.selectIndex = prevSelectIndex;

			return charsToMove;
		}

		protected void InsertLineBefore()
		{
			editor.MoveLineStart();
			editor.ReplaceSelection("\n");
			editor.MoveLeft();
		}

		protected void InsertLineAfter()
		{
			editor.MoveLineEnd();
			editor.ReplaceSelection("\n");
		}

		protected void IndentToPreviousLine()
		{
			RemoveIndentation();
			int currentIndex = editor.cursorIndex;
			editor.MoveUp();
			int indentation = StartingSpaces();
			editor.MoveDown();
			editor.MoveLineStart();

			for (int i = 0; i < indentation; i++)
			{
				editor.Insert(' ');
			}

			int newIndex = currentIndex + indentation;

			MoveToIndex(newIndex);
		}

		protected void RemoveIndentation()
		{
			int indentation = StartingSpaces();
			int newIndex = Math.Max(editor.cursorIndex - indentation, 0);
			editor.MoveLineStart();
			for (int i = 0; i < indentation; i++)
			{
				editor.Delete();
			}

			MoveToIndex(newIndex);
		}

		/// <summary>
		/// Adds one level of indentation
		/// </summary>
		protected void Indent()
		{
			int startingSpaces = StartingSpaces();
			int charsNeeded = spacesPerTab - startingSpaces % spacesPerTab;
			//Debug.Log("Chars needed " + charsNeeded);

			int prevCursorIndex = editor.cursorIndex + spacesPerTab;
			editor.MoveLineStart();
			for (int i = 0; i < charsNeeded; i++)
			{
				editor.Insert(' ');
			}

			MoveToIndex(prevCursorIndex);
		}

		/// <summary>
		/// Removes one level of indentation
		/// </summary>
		protected void Unindent()
		{
			int startingSpaces = StartingSpaces();

			if (startingSpaces == 0)
			{
				return;
			}

			int charsToDelete = startingSpaces % spacesPerTab;
			if (charsToDelete == 0)
			{
				charsToDelete = spacesPerTab;
			}

			//Debug.Log("charsToDelete " + charsToDelete);

			int prevCursorIndex = editor.cursorIndex;
			editor.MoveLineStart();
			int lineStartIndex = editor.cursorIndex;
			int newIndex = Math.Max(lineStartIndex, prevCursorIndex - charsToDelete);

			for (int i = 0; i < charsToDelete; i++)
			{
				editor.Delete();
			}
			MoveToIndex(newIndex);
		}

		protected void MoveToIndex(int index)
		{
			editor.cursorIndex = index;
			editor.selectIndex = index;
		}

		/// <summary>
		/// Returns the number of characters from the start
		/// of the line to the cursor
		/// </summary>
		/// <returns>The from line start.</returns>
		protected int CharsFromLineStart()
		{
			int prevCursorIndex = editor.cursorIndex;
			int prevSelectIndex = editor.selectIndex;
			editor.MoveLineStart();
			int startIndex = editor.cursorIndex;

			int chars = prevCursorIndex - startIndex;

			//Debug.Log("chars from line start is " + chars);

			//MoveToIndex(prevCursorIndex);

			editor.cursorIndex = prevCursorIndex;
			editor.selectIndex = prevSelectIndex;

			return chars;
		}

		protected int CharsFromLineEnd()
		{
			int prevCursorIndex = editor.cursorIndex;
			int prevSelectIndex = editor.selectIndex;
			editor.MoveLineEnd();
			int endIndex = editor.cursorIndex;

			int chars = endIndex - prevCursorIndex;

			//Debug.Log("chars from line start is " + chars);
			editor.cursorIndex = prevCursorIndex;
			editor.selectIndex = prevSelectIndex;
			//MoveToIndex(prevCursorIndex);

			return chars;
		}

		/// <summary>
		/// Gets number of starting spaces on the current line
		/// </summary>
		/// <returns>The spaces.</returns>
		protected int StartingSpaces()
		{
			string currentLine = CurrentLine();
			int i = 0;
			foreach (char c in currentLine)
			{
				if (c == ' ')
				{
					i++;
				}
				else
				{
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
		protected string CurrentLine()
		{
			int prevCursorIndex = editor.cursorIndex;
			int prevSelectIndex = editor.selectIndex;
			editor.MoveLineEnd();
			int endIndex = editor.cursorIndex;
			editor.MoveLineStart();
			int startIndex = editor.cursorIndex;
			string currentLine = editor.text.Substring(startIndex, endIndex - startIndex);

			//Debug.Log("The Line is " + currentLine);

			//MoveToIndex(prevCursorIndex);
			editor.cursorIndex = prevCursorIndex;
			editor.selectIndex = prevSelectIndex;

			return currentLine;
		}


		protected int CurrentLineNumber()
		{
			int prevCursorIndex = editor.cursorIndex;
			int prevSelectIndex = editor.selectIndex;

			int lineNum = 0;
			editor.MoveLineStart();
			//Debug.Log("index is " + editor.cursorIndex);
			//Debug.Log("lineNum is " + lineNum);
			while (editor.cursorIndex > 0)
			{
				editor.MoveUp();
				lineNum++;
				//Debug.Log("index is " + editor.cursorIndex);
				//Debug.Log("lineNum is " + lineNum);
			}

			editor.cursorIndex = prevCursorIndex;
			editor.selectIndex = prevSelectIndex;

			return lineNum;
		}
	}
}