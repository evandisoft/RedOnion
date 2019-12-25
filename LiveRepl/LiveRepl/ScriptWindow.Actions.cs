using System;
using System.Collections.Generic;
using System.IO;
using LiveRepl.Execution;
using MunOS;
using RedOnion.KSP.API;
using UnityEngine;
using static RedOnion.Debugging.QueueLogger;

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

		//public void SetReplEvaluatorByFilename(string filename)
		//{
		//	string extension=Path.GetExtension(filename);

		//	foreach (var replEvaluatorEntry in replEvaluators)
		//	{
		//		if (replEvaluatorEntry.Value.Extension.ToLower()==extension.ToLower())
		//		{
		//			SetCurrentEvaluator(replEvaluatorEntry.Key);
		//		}
		//	}
		//}

		public void SetEngineProcessByFilename(string filename)
		{
			string extension=Path.GetExtension(filename);

			foreach (var engine in engines)
			{
				if (engine.Value.Extension.ToLower()==extension.ToLower())
				{
					SetCurrentEngineProcess(engine.Key);
				}
			}
		}

		//public ReplEvaluator GetReplEvaluatorByFilename(string filename)
		//{
		//	string extension=Path.GetExtension(filename);

		//	foreach (var replEvaluatorEntry in replEvaluators)
		//	{
		//		if (replEvaluatorEntry.Value.Extension.ToLower()==extension.ToLower())
		//		{
		//			return replEvaluatorEntry.Value;
		//		}
		//	}
		//	return null;
		//}

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
			uiparts.editorChangesIndicator.MarkAsUnchanged();
			SetEngineProcessByFilename(uiparts.scriptNameInputArea.Text);
		}

		public void LoadEditorText()
		{
			string text=uiparts.scriptNameInputArea.LoadText();
			uiparts.editor.ModifyAndResetUndo(text);
			uiparts.editorChangesIndicator.MarkAsUnchanged();
			SetEngineProcessByFilename(uiparts.scriptNameInputArea.Text);
		}

		public void ClearRepl()
		{
			CurrentBuffer.Clear();
		}

		public System.Diagnostics.Stopwatch enableClock=new System.Diagnostics.Stopwatch();
		public System.Diagnostics.Stopwatch disableClock=new System.Diagnostics.Stopwatch();
		public void Evaluate(string source, string path, bool withHistory = false)
		{
			CurrentEngine.Evaluate(source, path, withHistory);
		}

		public void ResetEngine()
		{
			CurrentEngine.Reset();
			CurrentEngine.Initialize(MunCore.Default, GetAutorunScripts(CurrentEngine.Extension));
		}

		private void RunAutorunScripts()
		{
			foreach (var engine in engines.Values)
			{
				engine.Initialize(MunCore.Default, GetAutorunScripts(engine.Extension));
			}
		}

		public void Terminate()
		{
			//MunLogger.Log("Scriptwindow terminate start");
			CurrentEngine.Terminate();
			//evaluationList.Clear();
			//foreach (var replEvaluator in replEvaluators.Values)
			//{
			//	replEvaluator.Terminate();
			//}
			CurrentBuffer.AddError("Execution Manually Terminated");
			//MunLogger.Log("Scriptwindow terminate end");
		}

		public void RunEditorScript()
		{
			if (uiparts.editorChangesIndicator.IsChanged)
			{
				SaveEditorText();
			}
			else
			{
				LoadEditorText();
			}
			Evaluate(uiparts.editor.Text, uiparts.scriptNameInputArea.Text);
			CurrentBuffer.AddFileContent(uiparts.scriptNameInputArea.Text);
		}

		public void EvaluateReplText()
		{
			string text=uiparts.replInputArea.Text;
			CurrentBuffer.AddSourceString(text);
			Evaluate(text, null, true);
		}

		public void SubmitReplText()
		{
			EvaluateReplText();
			uiparts.replInputArea.Text = "";

			needsResize=true;
		}




	}
}
