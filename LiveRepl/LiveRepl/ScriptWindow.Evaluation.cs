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



		List<Evaluation> evaluationList = new List<Evaluation>();

		public void SetCurrentEvaluator(string evaluatorName)
		{
			currentReplEvaluator = replEvaluators[evaluatorName];
			uiparts.scriptEngineLabel.SetEngine(evaluatorName);
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
					uiparts.replOutoutArea.AddReturnValue(currentEvaluation.Result);
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
				PrintAction = uiparts.replOutoutArea.AddOutput,
				PrintErrorAction = uiparts.replOutoutArea.AddError
			};
			replEvaluators["Lua"] = new MoonSharpReplEvaluator()
			{
				PrintAction = uiparts.replOutoutArea.AddOutput,
				PrintErrorAction = uiparts.replOutoutArea.AddError
			};
			var scriptEngineSelector=uiparts.scriptEngineSelector;

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
				scriptEngineSelector.AddMinSized(new Button(evaluatorName,() =>
				{
					SetCurrentEvaluator(evaluatorName);
					SavedSettings.SaveSetting("lastEngine", evaluatorName);
				}));
			}
		}
	}
}
