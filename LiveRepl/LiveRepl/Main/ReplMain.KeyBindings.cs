using System;
using Kerbalua.Other;
using Kerbalui.Gui;
using UnityEngine;

namespace LiveRepl.Main
{
	public partial class ReplMain
	{
		public KeyBindings GlobalKeyBindings = new KeyBindings();

		void InitializeKeyBindings()
		{
			//repl.inputBox.KeyBindings.Add(new EventKey(KeyCode.LeftBracket, true), () =>
			//{
			//	//Debug.Log("history up");
			//	repl.inputBox.content.text = currentReplEvaluator.HistoryUp();
			//	repl.inputBox.selectIndex = repl.inputBox.content.text.Length;
			//	repl.inputBox.cursorIndex = repl.inputBox.content.text.Length;
			//});
			//repl.inputBox.KeyBindings.Add(new EventKey(KeyCode.Quote, true), () =>
			//{
			//	//Debug.Log("history down");
			//	repl.inputBox.content.text = currentReplEvaluator.HistoryDown();
			//	repl.inputBox.selectIndex = repl.inputBox.content.text.Length;
			//	repl.inputBox.cursorIndex = repl.inputBox.content.text.Length;
			//});
			//repl.inputBox.KeyBindings.Add(new EventKey(KeyCode.UpArrow, true), () =>
			//{
			//	//Debug.Log("history up");
			//	repl.inputBox.content.text = currentReplEvaluator.HistoryUp();
			//	repl.inputBox.selectIndex = repl.inputBox.content.text.Length;
			//	repl.inputBox.cursorIndex = repl.inputBox.content.text.Length;
			//});
			//repl.inputBox.KeyBindings.Add(new EventKey(KeyCode.DownArrow, true), () =>
			//{
			//	//Debug.Log("history down");
			//	repl.inputBox.content.text = currentReplEvaluator.HistoryDown();
			//	repl.inputBox.selectIndex = repl.inputBox.content.text.Length;
			//	repl.inputBox.cursorIndex = repl.inputBox.content.text.Length;
			//});

			GlobalKeyBindings.Add(new EventKey(KeyCode.U, true), () => editor.GrabFocus());
			GlobalKeyBindings.Add(new EventKey(KeyCode.I, true), () => scriptIOTextArea.GrabFocus());
			GlobalKeyBindings.Add(new EventKey(KeyCode.O, true), () => repl.inputBox.GrabFocus());
			GlobalKeyBindings.Add(new EventKey(KeyCode.P, true), () => completionBox.GrabFocus());
			GlobalKeyBindings.Add(new EventKey(KeyCode.D, true), () =>
			{
				editor.content.text = scriptIOTextArea.Load();
			});
			GlobalKeyBindings.Add(new EventKey(KeyCode.S, true), () =>
			{
				scriptIOTextArea.Save(editor.content.text);
			});
			GlobalKeyBindings.Add(new EventKey(KeyCode.Space, false, true), completionManager.Complete);
			GlobalKeyBindings.Add(new EventKey(KeyCode.Return, true), completionManager.Complete);

			//editor.KeyBindings.Add(new EventKey(KeyCode.E, true), () =>
			//{
			//	scriptIOTextArea.Save(editor.content.text);
			//	repl.outputBox.AddFileContent(scriptIOTextArea.content.text);
			//	evaluationList.Add(new Evaluation(editor.content.text, scriptIOTextArea.content.text, currentReplEvaluator));
			//});
			//repl.inputBox.KeyBindings.Add(new EventKey(KeyCode.E, true), () =>
			//{
			//	repl.outputBox.AddSourceString(repl.inputBox.content.text);
			//	evaluationList.Add(new Evaluation(repl.inputBox.content.text, null, currentReplEvaluator));
			//});
			//repl.inputBox.KeyBindings.Add(new EventKey(KeyCode.Return), () =>
			//{
			//	repl.outputBox.AddSourceString(repl.inputBox.content.text);
			//	// OperatingSystem.
			//	evaluationList.Add(new Evaluation(repl.inputBox.content.text, null, currentReplEvaluator, true));
			//	repl.inputBox.content.text = "";
			//	completionBox.content.text = "";
			//});
		}

		void PrintKeyBindingsInOutputArea()
		{
			string hotkeyText =

@"CURRENT KEYBINDINGS(partially inspired by vim):

Common to Editor and Repl input area:
			shift + space: Intellisense completion. 
ctrl + e: evaluate content. For editor this also saves to the file in the filename input area
tab: indent
shift + tab: unindent
ctrl + tab: make current line's indentation match previous line's indentation
ctrl + j: move cursor left
ctrl + k: move cursor down
ctrl + l: move cursor up
ctrl +;: move cursor right
ctrl + m: move cursor to next tab left
shift + backspace: delete to next tab left
ctrl + backspace: delete to next '.' left
ctrl + comma: move cursor 4 lines down
ctrl + period: move cursor 4 lines up
ctrl +/: move cursor to next tab right
ctrl + Home: move cursor to start
ctrl + End: move cursor to end
Shift plus movement commands selects text.
ctrl + insert: copy selected text
shift + insert: paste selected text
ctrl + u: focus editor
ctrl + i: focus scriptNameTextArea
ctrl + o: focus repl input area
ctrl + p: focus completion/ intellisense box
ctrl + n: insert and move to new line after current line
ctrl + h: insert and move to new line before current line
  When creating a new line, its indentation starts matched with the indentation of the line above it.

Editor only:
ctrl + s: Saves the file in `Scripts` folder within KSP folder based on the name specified in the textarea that is dedicated to this.
ctrl + d: Loads the file from `Scripts` folder within KSP folder based on the name specified in the textarea that is dedicated to this.

REPL input area only:
enter: evaluate content and clear repl input area.
shift + enter: add newline
ctrl +[or ctrl + UpArrow: go to previous history
ctrl + ' or ctrl+DownArrow: go to next history

Completion area only:
ctrl + k: select next line down
ctrl + l: select next line up
ctrl + comma: select line 4 lines down
ctrl + period: select line 4 lines up
ctrl + enter: submit completion

Output Area:
Only allows the following.
ctrl + c: copy selected area.
ctrl + insert: copy selected area.
Any other key gives focus to input box.
";

			repl.outputBox.content.text += "\n" + hotkeyText;
		}
	}
}
