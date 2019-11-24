using System;
using System.Collections.Generic;
using System.IO;
using Kerbalui.Controls;
using LiveRepl.Execution;
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

		public bool DisableElements;
		public void FixedUpdate()
		{
			if (evaluationList.Count!=0)
			{
				// make a delay after script starts running before disabling elements
				if (!disableClock.IsRunning) 
				{
					disableClock.Start();
				}
				if (disableClock.ElapsedMilliseconds>50)
				{
					DisableElements=true;
					disableClock.Reset();
				}

				var currentEvaluation = evaluationList[0];
				if (currentEvaluation.Evaluate())
				{
					uiparts.replOutoutArea.AddReturnValue(currentEvaluation.Result);
					evaluationList.RemoveAt(0);
					completionManager.DisplayCurrentCompletions();
				}
			}
			else
			{
				disableClock.Reset();
				// have a delay after script ends before enabling elements
				if (!enableClock.IsRunning) 
				{
					enableClock.Start();
				}
				if (enableClock.ElapsedMilliseconds>50)
				{
					DisableElements=false;
					enableClock.Reset();
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
//#if DEBUG
//			replEvaluators["nLua"] = new KerbnluaReplEvaluator()
//			{
//				PrintAction = uiparts.replOutoutArea.AddOutput,
//				PrintErrorAction = uiparts.replOutoutArea.AddError
//			};
//#endif
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

			RunAutorunScripts();
		}
	}
}
