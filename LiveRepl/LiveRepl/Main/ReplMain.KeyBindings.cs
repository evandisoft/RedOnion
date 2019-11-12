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


	}
}
