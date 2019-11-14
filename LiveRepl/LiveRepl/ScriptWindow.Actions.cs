using System;
using System.IO;
using Kerbalua.Other;

namespace LiveRepl
{
	public partial class ScriptWindow
	{
		/// <summary>
		/// Takes an action, action1, and returns an action that, when executed, will only
		/// execute action1 if ScriptRunning returns true.
		/// </summary>
		/// <returns>The disabled action.</returns>
		/// <param name="action1">The action.</param>
		public Action ScriptDisabledAction(Action action1)
		{
			return () =>
			{
				if (ScriptRunning) return;
				action1();
			};
		}

		public void SetReplEvaluatorByFilename(string filename)
		{
			string extension=Path.GetExtension(filename);
			foreach(var replEvaluatorEntry in replEvaluators)
			{
				if (replEvaluatorEntry.Value.Extension.ToLower()==extension.ToLower())
				{
					SetCurrentEvaluator(replEvaluatorEntry.Key);
				}
			}
		}

		public void ToggleEditor()
		{
			EditorVisible=!editorVisible;
		}

		public void ToggleRepl()
		{
			ReplVisible=!replVisible;
		}

		public void SaveEditorText()
		{
			uiparts.scriptNameInputArea.SaveText(uiparts.editor.editingArea.Text);
			uiparts.editorChangesIndicator.Unchanged();
		}

		public void LoadEditorText()
		{
			string text=uiparts.scriptNameInputArea.LoadText();
			uiparts.editor.editingArea.Text=text;
			uiparts.editorChangesIndicator.Unchanged();
			SetReplEvaluatorByFilename(uiparts.scriptNameInputArea.Text);
		}

		public void Evaluate(string source, string path, bool withHistory = false)
		{
			evaluationList.Add(new Evaluation(source, path, currentReplEvaluator, withHistory));
		}

		public void ResetEngine()
		{
			currentReplEvaluator?.ResetEngine();
		}

		public void Terminate()
		{
			evaluationList.Clear();
			foreach (var replEvaluator in replEvaluators.Values)
			{
				replEvaluator.Terminate();
			}
			uiparts.replOutoutArea.AddError("Execution Manually Terminated");
		}

		public void RunEditorScript()
		{
			SaveEditorText();
			Evaluate(uiparts.editor.editingArea.Text, uiparts.scriptNameInputArea.Text);
			uiparts.replOutoutArea.AddFileContent(uiparts.scriptNameInputArea.Text);
		}

		public void EvaluateReplText()
		{
			string text=uiparts.replInputArea.editingArea.Text;
			uiparts.replOutoutArea.AddSourceString(text);
			uiparts.scriptWindow.Evaluate(text, null, true);
		}

		public void SubmitReplText()
		{
			EvaluateReplText();
			uiparts.replInputArea.editingArea.Text = "";

			needsResize=true;
		}
	}
}
