using UnityEngine;
using Kerbalua.Other;
using Kerbalua.Completion;
using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace Kerbalua.Gui {
	public class ScriptWindow {
		const int maxOutputBytes = 80000;

		public Editor editor = new Editor();
		public Repl repl = new Repl();
		public CompletionBox completionBox = new CompletionBox();
		public ButtonBar buttonBar = new ButtonBar();

		public SimpleScript script;
		int windowID = 0;
		Rect mainWindowRect;
		bool editorVisible = true;
		bool replVisible = true;

		Rect buttonBarRect;
		Rect replRect;
		Rect completionBoxRect;
		Rect editorRect;

		const float titleHeight = 20;

		public ScriptWindow(SimpleScript script,Rect mainWindowRect)
		{
			this.script = script;
			this.mainWindowRect = mainWindowRect;

			buttonBarRect = new Rect(0, titleHeight, 100, mainWindowRect.height-titleHeight);
			replRect = new Rect(buttonBarRect.width, titleHeight, mainWindowRect.width - buttonBarRect.width, mainWindowRect.height-titleHeight);
			editorRect = new Rect(0, 0, replRect.width, mainWindowRect.height);
			completionBoxRect = new Rect(0, 0, replRect.width, mainWindowRect.height);

			buttonBar.buttons.Add(new Button("<<", () => editorVisible = !editorVisible));
			buttonBar.buttons.Add(new Button(">>", () => replVisible = !replVisible));

			Complete(false);
		}

		/// <summary>
		/// This updates rects for boxes that are not inside the main window that I want near the main window, so that when the
		/// main window is moved these boxes stay in the specified position relative to the window.
		/// </summary>
		/// <param name="rect">Rect.</param>
		/// <param name="xOffset">X offset.</param>
		public Rect UpdateBoxPositionWithWindow(Rect rect,float xOffset,float yOffset=0)
		{
			rect.x = mainWindowRect.x + xOffset;
			rect.y = mainWindowRect.y + yOffset;
			return rect;
		}

		public void Render()
		{
			if (replVisible) {
				mainWindowRect.width = buttonBarRect.width + replRect.width;
			} else {
				mainWindowRect.width = buttonBarRect.width;
			}

			mainWindowRect = GUI.Window(windowID, mainWindowRect, MainWindow, "Lua Dev");
			if (editorVisible) {
				editorRect=UpdateBoxPositionWithWindow(editorRect, -editorRect.width);
				editor.Render(editorRect);
			}

			if (replVisible) {
				completionBoxRect=UpdateBoxPositionWithWindow(completionBoxRect, mainWindowRect.width);
				completionBox.Render(completionBoxRect);
			}
		}

		void MainWindow(int id)
		{
			GUI.DragWindow(new Rect(0, 0, mainWindowRect.width, titleHeight));

			if (replVisible) {
				repl.Render(replRect);
			}

			buttonBar.Render(buttonBarRect);
			HandleInput();
		}

		void Complete(bool completing)
		{
			AllCompletion.Complete(script.Globals, repl.inputBox.content, completionBox.content, repl.inputBox.cursorPos, completing, out int newCursorPos);
			repl.inputBox.cursorPos = newCursorPos;
		}

		void HandleInput()
		{
			Event event1 = Event.current;
			if (event1.type == EventType.KeyDown) {
				switch (event1.keyCode) {
				case KeyCode.Backspace:
					int curlen = repl.inputBox.content.text.Length;
					if (curlen > 0) {
						repl.inputBox.content.text = repl.inputBox.content.text.Substring(0, curlen - 1);
						repl.inputBox.cursorPos -= 1;
					}
					break;
				case KeyCode.Return:
					repl.inputBox.content.text += event1.character;
					repl.inputBox.cursorPos += 1;
					break;
				case KeyCode.Tab:
					Complete(true);
					repl.outputBox.ResetScroll();
					break;
				default:
					char ch = event1.character;
					if (!char.IsControl(ch)) {
						repl.inputBox.content.text += event1.character;
						repl.inputBox.cursorPos += 1;
					}
					break;
				}
				int diff = repl.outputBox.content.text.Length - maxOutputBytes;
				if (diff > 0) {
					repl.outputBox.content.text = repl.outputBox.content.text.Substring(diff);
				}
				Complete(false);
				repl.outputBox.ResetScroll();
				event1.Use();
			}

			if (Mouse.Left.GetClick() || Mouse.Right.GetClick()) {
				if (new Rect(0,0,mainWindowRect.width,mainWindowRect.height).Contains(new Vector2(event1.mousePosition.x, event1.mousePosition.y))) {
					InputLockManager.SetControlLock(ControlTypes.KEYBOARDINPUT, "kerbalua");
				} else {
					InputLockManager.ClearControlLocks();
				}
			}

			if (repl.inputBox.content.text.EndsWith(Environment.NewLine + Environment.NewLine)) {
				DynValue result = new DynValue();
				try {
					result = script.DoString(repl.inputBox.content.text);
					repl.outputBox.content.text += Environment.NewLine;
					if (result.UserData == null) {
						repl.outputBox.content.text += result;
					} else {
						repl.outputBox.content.text += result.UserData.Object;
						if (result.UserData.Object == null) {
							repl.outputBox.content.text += " (" + result.UserData.Object.GetType() + ")";
						}
					}
				} catch (Exception exception) {
					Debug.Log(exception);
				}
				repl.inputBox.content.text = "";
				repl.inputBox.cursorPos = 0;
				completionBox.content.text = "";

			}
		}
	}
}
