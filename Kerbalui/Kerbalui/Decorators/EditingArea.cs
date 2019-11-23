using System;
using Kerbalui.Controls.Abstract;
using Kerbalui.EventHandling;
using Kerbalui.Types;
using Kerbalui.Util;
using RedOnion.KSP.Debugging;
using UnityEngine;

namespace Kerbalui.Decorators
{
	/// <summary>
	/// Editing area. Contains an EditableText (currently only either aTextArea or Text Field), 
	/// and handles editing for them.
	/// </summary>
	public class EditingArea : Decorator
	{
		//int inc = 0;
		const int spacesPerTab = 4;
		public KeyBindings keybindings = new KeyBindings();
		public TextEditor backingEditor;
		public int LineNumber { get; private set; } = 1;
		public int ColumnNumber { get; private set; } = 1;

		private int _cursorIndex;
		public int CursorIndex { get => _cursorIndex; set
			{
				if (value!=_cursorIndex)
				{
					QueueLogger.UILogger.Log("For control", editableText.ControlName);
					QueueLogger.UILogger.Log("EditingArea SetCursorIndex","Value was ", value, " _cursorIndex is ", _cursorIndex);
					_cursorIndex=value;
				}
			} }
		public int SelectIndex { get; set; }

		public string Text { get => editableText.content.text; set => editableText.content.text=value; }
		public override Vector2 MinSize => editableText.MinSize;
		public GUIStyle StyleOrDefault => editableText.StyleOrDefault;
		public bool HasFocus() => editableText.HasFocus();
		public void GrabFocus() => editableText.GrabFocus();

		public EditableText editableText;
		public bool EditorAssigned { get => backingEditor!=null; }

		public bool ReceivedInput { get; set; }
		/// <summary>
		/// Setting this to true will not allow any key-down input events
		/// to reach the control's default handling of events.
		/// </summary>
		public bool onlyUseKeyBindings;

		public bool TrySetFont(Font font)
		{
			if (editableText.style!=null && editableText.style.font!=font)
			{
				editableText.style.font=font;
				return true;
			}
			return false;
		}

		public EditingArea(EditableText editableText)
		{
			this.editableText = editableText;
			InitializeDefaultKeyBindings();
		}

		protected override void SetChildRect() => editableText.SetRect(rect);

		protected override void DecoratorUpdate()
		{
			// Initialize editor
			if (backingEditor == null)
			{
				editableText.GrabFocus();
				int id = GUIUtility.keyboardControl;
				backingEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), id);
			}
			
			if (editableText.style == null)
			{
				editableText.style = new GUIStyle(GUI.skin.textArea);
				editableText.style.font = GUILibUtil.GetMonoSpaceFont();
				editableText.style.hover.textColor
					= editableText.style.normal.textColor
					= editableText.style.active.textColor
					= Color.white;
			}

			if (editableText.HasFocus())
			{
				ReceivedInput = Event.current.type == EventType.KeyDown;
				int id = GUIUtility.keyboardControl;
				backingEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), id);
				backingEditor.text = editableText.content.text;
				backingEditor.cursorIndex = CursorIndex;
				backingEditor.selectIndex = SelectIndex;

				HandleInput();

				editableText.content.text = backingEditor.text;

				// When the event is a KeyDown or MouseDown, at this point, we want the underlying control
				// to be able to modify the cursor position using the key or new mouse click so we take the results
				// of the controls automatic handling of those events and use it to update the CursorIndex and SelectIndex.
				if (Event.current.type == EventType.KeyDown || Event.current.isMouse)
				{
					editableText.Update();

					CursorIndex = backingEditor.cursorIndex;
					SelectIndex = backingEditor.selectIndex;

					LineNumber = CurrentLineNumber() + 1;
					ColumnNumber = CharsFromLineStart() + 1;
				}
				// Otherwise, we don't want the update on the base control to modify the CursorIndex and SelectIndex,
				// as it seems to sometimes cause problems. Specifically it caused a problem when scrolling down
				// the CompletionArea of LiveRepl and then clicking an entry to complete. It failed to update the
				// CursorIndex of the Editor after the completion, leaving it at the beginning of the completion
				// rather than it being at the end. The first editableText.Update() call for the editor that it
				// received after the completion did not use the backingEditor's modified selectIndex but rather
				// ignored it and then set the CursorIndex to the value it had had prior to the completion.
				else
				{
					CursorIndex = backingEditor.cursorIndex;
					SelectIndex = backingEditor.selectIndex;

					LineNumber = CurrentLineNumber() + 1;
					ColumnNumber = CharsFromLineStart() + 1;

					editableText.Update();
				}

				//bool wasmousedown=Event.current.type==EventType.MouseDown;




				//if (wasmousedown)
				//{
				//	Debug.Log("was");
				//	Debug.Log(LineNumber);
				//	Debug.Log(ColumnNumber);
				//}

				
			}
			else
			{
				editableText.Update();
			}
		}

		//void AdjustScrollX()
		//{
		//	//Debug.Log("Adjusting scroll x");
		//	float cursorX = CursorX();
		//	float diff = lastContentVector2.x - lastScrollViewVector2.x;
		//	float contentStartX = scrollPos.x;
		//	float contentEndX = contentStartX + lastScrollViewVector2.x;
		//	if (Math.Max(cursorX - editableText.style.lineHeight, 0) < contentStartX)
		//	{
		//		scrollPos.x = Math.Max(cursorX - editableText.style.lineHeight, 0);
		//	}
		//	else if (cursorX + editableText.style.lineHeight > contentEndX)
		//	{
		//		scrollPos.x = cursorX - lastContentVector2.x + editableText.style.lineHeight;
		//	}
		//}

		//void AdjustScrollY()
		//{
		//	//Debug.Log("Adjusting scroll y");
		//	float cursorY = CursorY();
		//	//Debug.Log("CursorY " + cursorY);
		//	float diff = lastContentVector2.y - lastScrollViewVector2.y;
		//	//Debug.Log("diff " + diff);
		//	float contentStartY = scrollPos.y;
		//	//Debug.Log("contentStartY " + contentStartY);
		//	float contentEndY = contentStartY + lastScrollViewVector2.y;
		//	//Debug.Log("contentEndY " + contentEndY);
		//	if (cursorY - editableText.style.lineHeight < contentStartY)
		//	{
		//		scrollPos.y = cursorY - editableText.style.lineHeight;
		//		//Debug.Log("reducing to " + scrollPos.y);
		//	}
		//	else if (cursorY + editableText.style.lineHeight > contentEndY)
		//	{
		//		scrollPos.y = cursorY - lastContentVector2.y + editableText.style.lineHeight;
		//		//Debug.Log("expanding to " + scrollPos.y);
		//	}
		//}

		public float CursorX()
		{
			int c = CharsFromLineStart();
			string startOfLineToCursor = CurrentLine().Substring(0, c);
			GUIContent tempContent = new GUIContent(startOfLineToCursor);
			return editableText.style.CalcSize(tempContent).x;
		}

		public float CursorY()
		{
			string contentToCursor = editableText.content.text.Substring(0, CursorIndex);
			GUIContent tempContent = new GUIContent(contentToCursor);
			return editableText.style.CalcSize(tempContent).y;
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
				backingEditor.SelectTextStart();
			});
			keybindings.Add(new EventKey(KeyCode.End, true, true), () =>
			{
				backingEditor.SelectTextEnd();
			});
			keybindings.Add(new EventKey(KeyCode.Home, true), () =>
			{
				backingEditor.MoveTextStart();
			});
			keybindings.Add(new EventKey(KeyCode.End, true), () =>
			{
				backingEditor.MoveTextEnd();
			});
			keybindings.Add(new EventKey(KeyCode.Insert, true), () =>
			{
				backingEditor.Copy();
			});
			keybindings.Add(new EventKey(KeyCode.Insert, false, true), () =>
			{
				backingEditor.Paste();
			});
			keybindings.Add(new EventKey(KeyCode.Backspace, false, true), () =>
			{
				int toMove = NextTabLeft(CursorIndex);
				for (int i = 0; i < toMove; i++)
				{
					backingEditor.Backspace();
				}
			});
			keybindings.Add(new EventKey(KeyCode.Backspace, true), () =>
			{
				while (true)
				{
					if (backingEditor.cursorIndex <= 0)
					{
						break;
					}

					if (backingEditor.text[backingEditor.cursorIndex - 1] == '\n')
					{
						break;
					}

					backingEditor.Backspace();

					if (backingEditor.cursorIndex <= 0)
					{
						break;
					}
					if (backingEditor.text[backingEditor.cursorIndex - 1] == '.')
					{
						break;
					}

				}
			});
			keybindings.Add(new EventKey(KeyCode.Tab), () => Indent());
			keybindings.Add(new EventKey(KeyCode.Tab, false, true), () => Unindent());
			keybindings.Add(new EventKey(KeyCode.Tab, true), () => IndentToPreviousLine());
			keybindings.Add(new EventKey(KeyCode.J, true), () => backingEditor.MoveLeft());
			keybindings.Add(new EventKey(KeyCode.J, true, true), () => backingEditor.SelectLeft());
			keybindings.Add(new EventKey(KeyCode.K, true), () => backingEditor.MoveDown());
			keybindings.Add(new EventKey(KeyCode.K, true, true), () => backingEditor.SelectDown());
			keybindings.Add(new EventKey(KeyCode.L, true), () => backingEditor.MoveUp());
			keybindings.Add(new EventKey(KeyCode.L, true, true), () => backingEditor.SelectUp());
			keybindings.Add(new EventKey(KeyCode.Semicolon, true), () => backingEditor.MoveRight());
			keybindings.Add(new EventKey(KeyCode.Semicolon, true, true), () => backingEditor.SelectRight());
			keybindings.Add(new EventKey(KeyCode.M, true), () =>
			{
				int toMove = NextTabLeft(CursorIndex);
				for (int i = 0; i < toMove; i++)
				{
					backingEditor.MoveLeft();
				}
			});
			keybindings.Add(new EventKey(KeyCode.M, true, true), () =>
			{
				int toMove = NextTabLeft(SelectIndex);
				for (int i = 0; i < toMove; i++)
				{
					backingEditor.MoveLeft();
				}
			});
			keybindings.Add(new EventKey(KeyCode.Comma, true), () =>
			{
				for (int i = 0; i < 4; i++)
				{
					backingEditor.MoveDown();
				}
			});
			keybindings.Add(new EventKey(KeyCode.Comma, true, true), () =>
			{
				for (int i = 0; i < 4; i++)
				{
					backingEditor.SelectDown();
				}
			});
			keybindings.Add(new EventKey(KeyCode.Period, true), () =>
			{
				for (int i = 0; i < 4; i++)
				{
					backingEditor.MoveUp();
				}
			});
			keybindings.Add(new EventKey(KeyCode.Period, true, true), () =>
			{
				for (int i = 0; i < 4; i++)
				{
					backingEditor.SelectUp();
				}
			});
			keybindings.Add(new EventKey(KeyCode.Slash, true), () =>
			{
				int toMove = NextTabRight(CursorIndex);
				for (int i = 0; i < toMove; i++)
				{
					backingEditor.MoveRight();
				}
			});
			keybindings.Add(new EventKey(KeyCode.Slash, true, true), () =>
			{
				int toMove = NextTabRight(SelectIndex);
				for (int i = 0; i < toMove; i++)
				{
					backingEditor.MoveRight();
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
				backingEditor.ReplaceSelection("\n");
				IndentToPreviousLine();
			});
			keybindings.Add(new EventKey(KeyCode.Return, false, true), () =>
			{
				backingEditor.ReplaceSelection("\n");
				IndentToPreviousLine();
			});
		}

		protected int NextTabLeft(int fromIndex)
		{
			int prevCursorIndex = backingEditor.cursorIndex;
			int prevSelectIndex = backingEditor.selectIndex;

			backingEditor.cursorIndex = fromIndex;
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

			backingEditor.cursorIndex = prevCursorIndex;
			backingEditor.selectIndex = prevSelectIndex;

			return charsToMove;
		}

		protected int NextTabRight(int fromIndex)
		{
			int prevCursorIndex = backingEditor.cursorIndex;
			int prevSelectIndex = backingEditor.selectIndex;

			backingEditor.cursorIndex = fromIndex;
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

			backingEditor.cursorIndex = prevCursorIndex;
			backingEditor.selectIndex = prevSelectIndex;

			return charsToMove;
		}

		protected void InsertLineBefore()
		{
			backingEditor.MoveLineStart();
			backingEditor.ReplaceSelection("\n");
			backingEditor.MoveLeft();
		}

		protected void InsertLineAfter()
		{
			backingEditor.MoveLineEnd();
			backingEditor.ReplaceSelection("\n");
		}

		protected void IndentToPreviousLine()
		{
			RemoveIndentation();
			int currentIndex = backingEditor.cursorIndex;
			backingEditor.MoveUp();
			int indentation = StartingSpaces();
			backingEditor.MoveDown();
			backingEditor.MoveLineStart();

			for (int i = 0; i < indentation; i++)
			{
				backingEditor.Insert(' ');
			}

			int newIndex = currentIndex + indentation;

			MoveToIndex(newIndex);
		}

		protected void RemoveIndentation()
		{
			int indentation = StartingSpaces();
			int newIndex = Math.Max(backingEditor.cursorIndex - indentation, 0);
			backingEditor.MoveLineStart();
			for (int i = 0; i < indentation; i++)
			{
				backingEditor.Delete();
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

			int prevCursorIndex = backingEditor.cursorIndex + spacesPerTab;
			backingEditor.MoveLineStart();
			for (int i = 0; i < charsNeeded; i++)
			{
				backingEditor.Insert(' ');
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

			int prevCursorIndex = backingEditor.cursorIndex;
			backingEditor.MoveLineStart();
			int lineStartIndex = backingEditor.cursorIndex;
			int newIndex = Math.Max(lineStartIndex, prevCursorIndex - charsToDelete);

			for (int i = 0; i < charsToDelete; i++)
			{
				backingEditor.Delete();
			}
			MoveToIndex(newIndex);
		}

		protected void MoveToIndex(int index)
		{
			backingEditor.cursorIndex = index;
			backingEditor.selectIndex = index;
		}

		/// <summary>
		/// Returns the number of characters from the start
		/// of the line to the cursor
		/// </summary>
		/// <returns>The from line start.</returns>
		protected int CharsFromLineStart()
		{
			int prevCursorIndex = backingEditor.cursorIndex;
			int prevSelectIndex = backingEditor.selectIndex;
			backingEditor.MoveLineStart();
			int startIndex = backingEditor.cursorIndex;

			int chars = prevCursorIndex - startIndex;

			//Debug.Log("chars from line start is " + chars);

			//MoveToIndex(prevCursorIndex);

			backingEditor.cursorIndex = prevCursorIndex;
			backingEditor.selectIndex = prevSelectIndex;

			return chars;
		}

		protected int CharsFromLineEnd()
		{
			int prevCursorIndex = backingEditor.cursorIndex;
			int prevSelectIndex = backingEditor.selectIndex;
			backingEditor.MoveLineEnd();
			int endIndex = backingEditor.cursorIndex;

			int chars = endIndex - prevCursorIndex;

			//Debug.Log("chars from line start is " + chars);
			backingEditor.cursorIndex = prevCursorIndex;
			backingEditor.selectIndex = prevSelectIndex;
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
			int prevCursorIndex = backingEditor.cursorIndex;
			int prevSelectIndex = backingEditor.selectIndex;
			backingEditor.MoveLineEnd();
			int endIndex = backingEditor.cursorIndex;
			backingEditor.MoveLineStart();
			int startIndex = backingEditor.cursorIndex;
			string currentLine = backingEditor.text.Substring(startIndex, endIndex - startIndex);

			//Debug.Log("The Line is " + currentLine);

			//MoveToIndex(prevCursorIndex);
			backingEditor.cursorIndex = prevCursorIndex;
			backingEditor.selectIndex = prevSelectIndex;

			return currentLine;
		}


		protected int CurrentLineNumber()
		{
			int prevCursorIndex = backingEditor.cursorIndex;
			int prevSelectIndex = backingEditor.selectIndex;

			int lineNum = 0;
			backingEditor.MoveLineStart();
			//Debug.Log("index is " + editor.cursorIndex);
			//Debug.Log("lineNum is " + lineNum);
			while (backingEditor.cursorIndex > 0)
			{
				backingEditor.MoveUp();
				lineNum++;
				//Debug.Log("index is " + editor.cursorIndex);
				//Debug.Log("lineNum is " + lineNum);
			}

			backingEditor.cursorIndex = prevCursorIndex;
			backingEditor.selectIndex = prevSelectIndex;

			return lineNum;
		}


	}
}
