using System;
using Kerbalui.EventHandling;
using UnityEngine;

namespace LiveRepl
{
	public partial class ScriptWindow
	{
		public KeyBindings GlobalKeyBindings = new KeyBindings();

		void InitializeGlobalKeyBindings()
		{
			GlobalKeyBindings.Add(new EventKey(KeyCode.U, true), uiparts.replInputArea.GrabFocus);
			GlobalKeyBindings.Add(new EventKey(KeyCode.I, true), uiparts.editor.GrabFocus);
			GlobalKeyBindings.Add(new EventKey(KeyCode.O, true), uiparts.scriptNameInputArea.GrabFocus);
			GlobalKeyBindings.Add(new EventKey(KeyCode.P, true), uiparts.completionArea.GrabFocus);
			GlobalKeyBindings.Add(new EventKey(KeyCode.S, true), SaveEditorText);
			GlobalKeyBindings.Add(new EventKey(KeyCode.D, true), LoadEditorText);
			GlobalKeyBindings.Add(new EventKey(KeyCode.Space, false, true), completionManager.Complete);
			GlobalKeyBindings.Add(new EventKey(KeyCode.Return, true), completionManager.Complete);
			GlobalKeyBindings.Add(new EventKey(KeyCode.C, true,true), Terminate);
		}

		public void PrintKeyBindingsInOutputArea()
		{
			string hotkeyText =

@"CURRENT KEYBINDINGS(partially inspired by vim):

Common to Editor and Repl input area:
ctrl + e: evaluate content. For editor this also saves to the file in the filename input area
tab: indent
shift + tab: unindent
ctrl + tab: make current line's indentation match previous line's indentation
ctrl + j: move cursor left
ctrl + k: move cursor down
ctrl + l: move cursor up
ctrl + ;: move cursor right
ctrl + m: move cursor to next tab left
shift + backspace: delete to next tab left
ctrl + backspace: delete to next '.' left
ctrl + comma: move cursor 4 lines down
ctrl + period: move cursor 4 lines up
ctrl + /: move cursor to next tab right
ctrl + Home: move cursor to start
ctrl + End: move cursor to end
Shift plus movement commands selects text.
ctrl + insert: copy selected text
shift + insert: paste selected text
ctrl + n: insert and move to new line after current line
ctrl + h: insert and move to new line before current line
  When creating a new line, its indentation starts matched with the indentation of the line above it.

Editor only:
ctrl + s: Saves the file in `Scripts` folder within KSP folder based on the name specified in the textarea that is dedicated to this.
ctrl + d: Loads the file from `Scripts` folder within KSP folder based on the name specified in the textarea that is dedicated to this.
ctrl + z: Undo
ctrl + shift + z: Redo

REPL input area only:
enter: evaluate content and clear repl input area.
shift + enter: add newline
ctrl + [ or ctrl + UpArrow: go to previous history
ctrl + ' or ctrl + DownArrow: go to next history

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

Global Keys:
ctrl + u: focus repl input area
ctrl + i: focus editor
ctrl + o: focus scriptNameTextArea
ctrl + p: focus completion/ intellisense box
shift + space: Intellisense completion.
ctrl + enter: Intellisense completion.
control + shift + c: Terminate current process' threads
";

			CurrentBuffer.AddText(hotkeyText);
		}
	}
}
