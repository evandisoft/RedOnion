using System;
using System.IO;
using LiveRepl.Execution;
using RedOnion.KSP.API;
using UnityEngine;

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

			foreach (var replEvaluatorEntry in replEvaluators)
			{
				if (replEvaluatorEntry.Value.Extension.ToLower()==extension.ToLower())
				{
					SetCurrentEvaluator(replEvaluatorEntry.Key);
				}
			}
		}

		public ReplEvaluator GetReplEvaluatorByFilename(string filename)
		{
			string extension=Path.GetExtension(filename);

			foreach (var replEvaluatorEntry in replEvaluators)
			{
				if (replEvaluatorEntry.Value.Extension.ToLower()==extension.ToLower())
				{
					return replEvaluatorEntry.Value;
				}
			}
			return null;
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
			uiparts.scriptNameInputArea.SaveText(uiparts.editor.Text);
			uiparts.editorChangesIndicator.Unchanged();
			SetReplEvaluatorByFilename(uiparts.scriptNameInputArea.Text);
		}

		public void LoadEditorText()
		{
			string text=uiparts.scriptNameInputArea.LoadText();
			uiparts.editor.Text=text;
			uiparts.editorChangesIndicator.Unchanged();
			SetReplEvaluatorByFilename(uiparts.scriptNameInputArea.Text);
		}

		public void ClearRepl()
		{
			uiparts.replOutoutArea.Clear();
		}

		public System.Diagnostics.Stopwatch enableClock=new System.Diagnostics.Stopwatch();
		public System.Diagnostics.Stopwatch disableClock=new System.Diagnostics.Stopwatch();
		public void Evaluate(string source, string path, bool withHistory = false)
		{
			evaluationList.Add(new Evaluation(source, path, currentReplEvaluator, withHistory));
		}

		public void ResetEngine()
		{
			currentReplEvaluator?.ResetEngine();
			RunAutorunScripts();
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
			Evaluate(uiparts.editor.Text, uiparts.scriptNameInputArea.Text);
			uiparts.replOutoutArea.AddFileContent(uiparts.scriptNameInputArea.Text);
		}

		public void EvaluateReplText()
		{
			string text=uiparts.replInputArea.Text;
			uiparts.replOutoutArea.AddSourceString(text);
			uiparts.scriptWindow.Evaluate(text, null, true);
		}

		public void SubmitReplText()
		{
			EvaluateReplText();
			uiparts.replInputArea.Text = "";

			needsResize=true;
		}

		public void RunAutorunScripts()
		{
			var scriptnames = AutoRun.scripts();
			foreach (var scriptname in scriptnames)
			{
				ReplEvaluator replEvaluator=GetReplEvaluatorByFilename(scriptname);
				if (replEvaluator==null) continue;
				uiparts.replOutoutArea.AddFileContent("loading "+scriptname+"...");
				Evaluation newEvaluation=new Evaluation(replEvaluator.GetImportString(scriptname), scriptname, replEvaluator);
				evaluationList.Add(newEvaluation);
			}
		}
	}
}
