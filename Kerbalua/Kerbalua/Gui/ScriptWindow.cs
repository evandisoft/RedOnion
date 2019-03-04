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

		const int windowID = 0;
		const int modalID = 1;
		Rect mainWindowRect;
		bool editorVisible;
		bool replVisible = true;

		Rect buttonBarRect;
		Rect replRect;
		Rect completionBoxRect;
		Rect editorRect;

		Rect SaveLoadRect;

		const float titleHeight = 20;

		bool inputIsLocked;

		public ScriptWindow(SimpleScript script,Rect mainWindowRect)
		{
			this.script = script;
			this.mainWindowRect = mainWindowRect;

			buttonBarRect = new Rect(0, titleHeight, 100, mainWindowRect.height-titleHeight);
			replRect = new Rect(buttonBarRect.width, titleHeight, mainWindowRect.width - buttonBarRect.width, mainWindowRect.height-titleHeight);
			editorRect = new Rect(0, 0, replRect.width, mainWindowRect.height);
			completionBoxRect = new Rect(0, 0, 150, mainWindowRect.height);

			buttonBar.buttons.Add(new Button("<<", () => editorVisible = !editorVisible));
			buttonBar.buttons.Add(new Button(">>", () => replVisible = !replVisible));
			buttonBar.buttons.Add(new Button("Evaluate", () => {
				Evaluate(editor.editingArea.content.text);
			}));
			buttonBar.buttons.Add(new Button("Save", () => {

			}));
			buttonBar.buttons.Add(new Button("Load", () => {

			}));

			Complete(false);
		}


		/// <summary>
		/// This updates Rects for boxes that are not inside the main window 
		/// that I want near the main window, so that when the main window is 
		/// moved these boxes stay in the specified position relative to the 
		/// window.
		/// </summary>
		/// <param name="rect">The Rect to be updated.</param>
		/// <param name="xOffset">The desired xOffset from the main window.</param>
		/// <param name="yOffset">The desired yOffset from the main window.</param>
		public Rect UpdateBoxPositionWithWindow(Rect rect,float xOffset,float yOffset=0)
		{
			rect.x = mainWindowRect.x + xOffset;
			rect.y = mainWindowRect.y + yOffset;
			return rect;
		}

		/// <summary>
		/// Returns the rectangle covering the entire area of the editor/repl/completion
		/// </summary>
		public Rect getTotalArea()
		{
			Rect totalArea = new Rect();
			totalArea.x = mainWindowRect.x;
			totalArea.y = mainWindowRect.y;
			totalArea.height = mainWindowRect.height;
			totalArea.width = mainWindowRect.width;

			if (editorVisible) {
				totalArea.width += editorRect.width;
				totalArea.x -= editorRect.width;
			}
			if (replVisible) {
				totalArea.width += completionBoxRect.width;
			}

			return totalArea;
		}

		public void SetOrReleaseInputLock()
		{
			if (getTotalArea().Contains(Mouse.screenPos)) {
				if (!inputIsLocked) {
					inputIsLocked = true;
					InputLockManager.SetControlLock(ControlTypes.KEYBOARDINPUT, "kerbalua");
				}
			} else {
				if (inputIsLocked) {
					inputIsLocked = false;
					InputLockManager.ClearControlLocks();
				}
			}
		}

		public void Evaluate(string text)
		{
			DynValue result = new DynValue();
			try {
				result = script.DoString(text);
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
		}

		public void Render()
		{
			SetOrReleaseInputLock();

			// Manually handling scrolling because there is a coordinate issue
			// with the default method. The system measures the mouse vertical
			// position from the bottom instead of the top of the screen for scrolling
			// purposes for whatever bizarre reason.
			if (Event.current.isScrollWheel) {
				float delta = 0;
				if(Input.GetAxis("Mouse ScrollWheel")>0) {
					delta = -1;
				} else {
					delta = 1;
				}

				if (mainWindowRect.Contains(Mouse.screenPos)) {
					repl.outputBox.scrollPos.y += 20*delta;
				} else if(completionBoxRect.Contains(Mouse.screenPos)) {
					completionBox.scrollPos.y += 20 * delta;
				}
				Event.current.Use();
			}

			if (replVisible) {
				mainWindowRect.width = buttonBarRect.width + replRect.width;
			} else {
				mainWindowRect.width = buttonBarRect.width;
			}

			mainWindowRect = GUI.Window(windowID, mainWindowRect, MainWindow, "Live Dev");
			if (editorVisible) {
				editorRect=UpdateBoxPositionWithWindow(editorRect, -editorRect.width);
				editor.Render(editorRect);
			}

			if (replVisible) {
				completionBoxRect=UpdateBoxPositionWithWindow(completionBoxRect, mainWindowRect.width);
				completionBox.Render(completionBoxRect);
			}
		}

		/// <summary>
		/// To make input handling simpler, only the repl and button bar are in
		/// the main window. The other boxes are updated to always be beside this window.
		/// 
		/// This design decision may be reviewed in the future with a better understanding
		/// of IMGUI.
		/// </summary>
		/// <param name="id">Identifier.</param>
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

			if (repl.inputBox.content.text.EndsWith(Environment.NewLine + Environment.NewLine)) {
				Evaluate(repl.inputBox.content.text);
				repl.inputBox.content.text = "";
				repl.inputBox.cursorPos = 0;
				completionBox.content.text = "";
			}
		}
	}
}
