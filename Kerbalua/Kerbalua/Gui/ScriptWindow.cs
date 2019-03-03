using UnityEngine;
using Kerbalua.Other;
using Kerbalua.Completion;
using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace Kerbalua.Gui {
	public partial class ScriptWindow {
		const int maxOutputBytes = 80000;

		public Editor editor = new Editor();
		public Repl repl = new Repl();
		public CompletionBox completionBox = new CompletionBox();
		public SimpleScript script;
		int windowID = 0;
		Rect replRect;

		public ScriptWindow(SimpleScript script,Rect replRect)
		{
			this.script = script;
			this.replRect = replRect;
			Complete(false);
		}

		public void Render()
		{
			replRect = GUI.Window(windowID, replRect, ReplWindow, "kerbalua REPL");
			editor.Render(new Rect(replRect.x-replRect.width, replRect.y, replRect.width, replRect.height));
			completionBox.Render(new Rect(replRect.x+replRect.width, replRect.y, replRect.width, replRect.height));
		}

		void ReplWindow(int id)
		{
			GUI.DragWindow(new Rect(0, 0, replRect.width, 20));
			repl.Render(replRect);
			HandleInput(replRect);
		}

		void Complete(bool completing)
		{
			AllCompletion.Complete(script.Globals, repl.inputBox.content, completionBox.content, repl.inputBox.cursorPos, completing, out int newCursorPos);
			repl.inputBox.cursorPos = newCursorPos;
		}

		void HandleInput(Rect rect)
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
				if (rect.Contains(new Vector2(event1.mousePosition.x + 100, event1.mousePosition.y + 100))) {
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
