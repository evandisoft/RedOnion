using System;
using System.Collections.Generic;
using Kerbalua.Other;
using Kerbalui.Controls;
using Kerbalui.Util;
using RedOnion.KSP.Settings;
using UnityEngine;

namespace LiveRepl
{
	public partial class ScriptWindow
	{
		public bool ScriptRunning => evaluationList.Count!=0;

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

		public void Terminate()
		{
			evaluationList.Clear();
			foreach (var replEvaluator in replEvaluators.Values)
			{
				replEvaluator.Terminate();
			}
			repl.replOutoutArea.AddError("Execution Manually Terminated");
		}

		public void ResetEngine()
		{
			currentReplEvaluator?.ResetEngine();
		}

		public void Evaluate(string source, string path, bool withHistory = false)
		{
			evaluationList.Add(new Evaluation(source, path, currentReplEvaluator, withHistory));
		}

		List<Evaluation> evaluationList = new List<Evaluation>();

		public void SetCurrentEvaluator(string evaluatorName)
		{
			currentReplEvaluator = replEvaluators[evaluatorName];
			contentGroup.centerGroup.scriptEngineSelector.scriptEngineLabel.SetEngine(evaluatorName);
		}

		Dictionary<string,ReplEvaluator> replEvaluators=new Dictionary<string, ReplEvaluator>();
		public ReplEvaluator currentReplEvaluator;

		public void FixedUpdate()
		{
			if (evaluationList.Count!=0)
			{
				var currentEvaluation = evaluationList[0];
				if (currentEvaluation.Evaluate())
				{
					repl.replOutoutArea.AddReturnValue(currentEvaluation.Result);
					evaluationList.RemoveAt(0);
					completionManager.DisplayCurrentCompletions();
				}
			}
			foreach (var engineName in replEvaluators.Keys)
			{
				var replEvaluator = replEvaluators[engineName];
				try
				{
					replEvaluator.FixedUpdate();
				}
				catch (Exception ex)
				{
					Debug.Log("Exception in REPL.FixedUpdate: " + ex.Message);
					replEvaluator.ResetEngine();
					//RunStartupScripts(engineName);
				}
			}
		}

		void InitEvaluation()
		{
			replEvaluators["ROS"] = new RedOnionReplEvaluator()
			{
				PrintAction = repl.replOutoutArea.AddOutput,
				PrintErrorAction = repl.replOutoutArea.AddError
			};
			replEvaluators["Lua"] = new MoonSharpReplEvaluator()
			{
				PrintAction = repl.replOutoutArea.AddOutput,
				PrintErrorAction = repl.replOutoutArea.AddError
			};
			var scriptEngineSelector=contentGroup.centerGroup.scriptEngineSelector;

			string lastEngineName = SavedSettings.LoadSetting("lastEngine", "Lua");
			if (replEvaluators.ContainsKey(lastEngineName))
			{
				SetCurrentEvaluator(lastEngineName);
			}
			else
			{
				foreach (var evaluatorName in replEvaluators.Keys)
				{
					SetCurrentEvaluator(evaluatorName);
					SavedSettings.SaveSetting("lastEngine", evaluatorName);
					break;
				}
			}

			foreach (var evaluatorName in replEvaluators.Keys)
			{
				scriptEngineSelector.AddMinSized(new Button(evaluatorName, ScriptDisabledAction(() =>
				{
					SetCurrentEvaluator(evaluatorName);
					SavedSettings.SaveSetting("lastEngine", evaluatorName);
				})));
			}
		}
	}
}
