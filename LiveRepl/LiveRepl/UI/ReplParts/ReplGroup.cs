using Kerbalui.EventHandling;
using Kerbalui.Layout;
using Kerbalui.Types;
using UnityEngine;

namespace LiveRepl.UI.ReplParts
{
	/// <summary>
	/// The Group that holds the Repl and related functionality.
	/// </summary>
	public class ReplGroup : VerticalSpacer
	{
		public ContentGroup contentGroup;

		public Repl repl;

		public ReplGroup(ContentGroup contentGroup)
		{
			this.contentGroup=contentGroup;

			AddWeighted(1, repl=new Repl(this));

			repl.replInputArea.keybindings.Add(new EventKey(KeyCode.E, true), EvaluateReplText);
			repl.replInputArea.keybindings.Add(new EventKey(KeyCode.Return), SubmitReplText);

			ScriptWindow scriptWindow=contentGroup.scriptWindow;
			repl.replInputArea.keybindings.Add(new EventKey(KeyCode.LeftBracket, true), () =>
			{
				//Debug.Log("history up");
				repl.replInputArea.editingArea.Text = scriptWindow.currentReplEvaluator.HistoryUp();
				repl.replInputArea.editingArea.SelectIndex = repl.replInputArea.editingArea.Text.Length;
				repl.replInputArea.editingArea.CursorIndex = repl.replInputArea.editingArea.Text.Length;
			});
			repl.replInputArea.keybindings.Add(new EventKey(KeyCode.Quote, true), () =>
			{
				//Debug.Log("history down");
				repl.replInputArea.editingArea.Text = scriptWindow.currentReplEvaluator.HistoryDown();
				repl.replInputArea.editingArea.SelectIndex = repl.replInputArea.editingArea.Text.Length;
				repl.replInputArea.editingArea.CursorIndex = repl.replInputArea.editingArea.Text.Length;
			});
			repl.replInputArea.keybindings.Add(new EventKey(KeyCode.UpArrow, true), () =>
			{
				//Debug.Log("history up");
				repl.replInputArea.editingArea.Text = scriptWindow.currentReplEvaluator.HistoryUp();
				repl.replInputArea.editingArea.SelectIndex = repl.replInputArea.editingArea.Text.Length;
				repl.replInputArea.editingArea.CursorIndex = repl.replInputArea.editingArea.Text.Length;
			});
			repl.replInputArea.keybindings.Add(new EventKey(KeyCode.DownArrow, true), () =>
			{
				//Debug.Log("history down");
				repl.replInputArea.editingArea.Text = scriptWindow.currentReplEvaluator.HistoryDown();
				repl.replInputArea.editingArea.SelectIndex = repl.replInputArea.editingArea.Text.Length;
				repl.replInputArea.editingArea.CursorIndex = repl.replInputArea.editingArea.Text.Length;
			});
		}

		public void EvaluateReplText()
		{
			string text=repl.replInputArea.editingArea.Text;
			repl.replOutoutArea.AddSourceString(text);
			contentGroup.scriptWindow.Evaluate(text, null, true);
		}

		public void SubmitReplText()
		{
			EvaluateReplText();
			repl.replInputArea.editingArea.Text = "";
			//TODO: deal with completion afterwords.
			needsResize=true;
		}
	}
}