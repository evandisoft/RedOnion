using Kerbalui.Controls;
using Kerbalui.EventHandling;
using Kerbalui.Layout;
using Kerbalui.Types;
using UnityEngine;

namespace LiveRepl.Parts
{
	/// <summary>
	/// The Group that holds the Repl and related functionality.
	/// </summary>
	public class ReplGroup : VerticalSpacer
	{
		public ScriptWindowParts uiparts;

		public ReplGroup(ScriptWindowParts uiparts)
		{
			this.uiparts=uiparts;

			var replButtons=new HorizontalSpacer();

			replButtons.AddWeighted(1, new Filler());
			replButtons.AddFixed(ScriptWindow.centerGroupWidth, new Button("Clear Repl", uiparts.scriptWindow.ClearRepl));
			replButtons.AddFixed(ScriptWindow.centerGroupWidth, new Button("Show Hotkeys", uiparts.scriptWindow.PrintKeyBindingsInOutputArea));

			AddMinSized(replButtons);
			AddWeighted(1, uiparts.repl=new Repl(uiparts));

			uiparts.replInputArea.keybindings.Add(new EventKey(KeyCode.E, true), uiparts.scriptWindow.EvaluateReplText);
			uiparts.replInputArea.keybindings.Add(new EventKey(KeyCode.Return), uiparts.scriptWindow.SubmitReplText);

			uiparts.replInputArea.keybindings.Add(new EventKey(KeyCode.LeftBracket, true), () =>
			{
				//Debug.Log("history up");
				uiparts.replInputArea.editingArea.Text = uiparts.scriptWindow.currentReplEvaluator.HistoryUp();
				uiparts.replInputArea.editingArea.SelectIndex = uiparts.replInputArea.editingArea.Text.Length;
				uiparts.replInputArea.editingArea.CursorIndex = uiparts.replInputArea.editingArea.Text.Length;
			});
			uiparts.replInputArea.keybindings.Add(new EventKey(KeyCode.Quote, true), () =>
			{
				//Debug.Log("history down");
				uiparts.replInputArea.editingArea.Text = uiparts.scriptWindow.currentReplEvaluator.HistoryDown();
				uiparts.replInputArea.editingArea.SelectIndex = uiparts.replInputArea.editingArea.Text.Length;
				uiparts.replInputArea.editingArea.CursorIndex = uiparts.replInputArea.editingArea.Text.Length;
			});
			uiparts.replInputArea.keybindings.Add(new EventKey(KeyCode.UpArrow, true), () =>
			{
				//Debug.Log("history up");
				uiparts.replInputArea.editingArea.Text = uiparts.scriptWindow.currentReplEvaluator.HistoryUp();
				uiparts.replInputArea.editingArea.SelectIndex = uiparts.replInputArea.editingArea.Text.Length;
				uiparts.replInputArea.editingArea.CursorIndex = uiparts.replInputArea.editingArea.Text.Length;
			});
			uiparts.replInputArea.keybindings.Add(new EventKey(KeyCode.DownArrow, true), () =>
			{
				//Debug.Log("history down");
				uiparts.replInputArea.editingArea.Text = uiparts.scriptWindow.currentReplEvaluator.HistoryDown();
				uiparts.replInputArea.editingArea.SelectIndex = uiparts.replInputArea.editingArea.Text.Length;
				uiparts.replInputArea.editingArea.CursorIndex = uiparts.replInputArea.editingArea.Text.Length;
			});
		}
	}
}