using System;
using System.Collections.Generic;
using System.IO;
using Kerbalua.Scripting;
using Kerbalui.Controls;
using LiveRepl.Execution;
using MunOS;
using MunOS.Repl;
using RedOnion.KSP.API;
using RedOnion.KSP.ROS;
using RedOnion.KSP.Settings;
using UnityEngine;

namespace LiveRepl
{
	public partial class ScriptWindow
	{
		//public bool ScriptRunning => evaluationList.Count!=0;


		//List<Evaluation> evaluationList = new List<Evaluation>();
		//public void SetCurrentEvaluator(string evaluatorName)
		//{
		//	currentReplEvaluator = replEvaluators[evaluatorName];
		//	uiparts.scriptEngineLabel.SetEngine(evaluatorName);
		//}
		//Dictionary<string,ReplEvaluator> replEvaluators=new Dictionary<string, ReplEvaluator>();
		//public ReplEvaluator currentReplEvaluator;



		// this needs some better logic because there is no way to know if there is some callback
		public bool ScriptRunning => true;// currentEngineProcess.TotalThreadCount > 0;

		public void SetCurrentEngineProcess(string engineName)
		{
			currentEngineProcess = engineProcesses[engineName];
			uiparts.scriptEngineLabel.SetEngine(engineName);
		}

		Dictionary<string, MunProcess> engineProcesses=new Dictionary<string, MunProcess>();
		public MunProcess currentEngineProcess;

		private MunProcess GetEngineProcessByExtension(string extension)
		{
			foreach(var engineProcessEntry in engineProcesses)
			{
				if (engineProcessEntry.Value.ScriptManager.Extension.ToLower()==extension.ToLower())
				{
					return engineProcessEntry.Value;
				}
			}
			return null;
		}

		public bool DisableElements;
		public void FixedUpdate()
		{
			//if (ScriptRunning)
			//{
			//	// make a delay after script starts running before disabling elements
			//	if (!disableClock.IsRunning) 
			//	{
			//		disableClock.Start();
			//	}
			//	if (disableClock.ElapsedMilliseconds>50)
			//	{
			//		DisableElements=true;
			//		disableClock.Reset();
			//	}

			//	//var currentEvaluation = evaluationList[0];
			//	//if (currentEvaluation.Evaluate())
			//	//{
			//	//	uiparts.replOutoutArea.AddReturnValue(currentEvaluation.Result);
			//	//	evaluationList.RemoveAt(0);
			//	//	completionManager.DisplayCurrentCompletions();
			//	//}
			//}
			//else
			//{
			//	disableClock.Reset();
			//	// have a delay after script ends before enabling elements
			//	if (!enableClock.IsRunning) 
			//	{
			//		enableClock.Start();
			//	}
			//	if (enableClock.ElapsedMilliseconds>50)
			//	{
			//		DisableElements=false;
			//		enableClock.Reset();
			//	}
			//}

			//foreach (var engineName in replEvaluators.Keys)
			//{
			//	var replEvaluator = replEvaluators[engineName];
			//	try
			//	{
			//		replEvaluator.FixedUpdate();
			//	}
			//	catch (Exception ex)
			//	{
			//		Debug.Log("Exception in REPL.FixedUpdate: " + ex.Message);
			//		replEvaluator.ResetEngine();
			//		//RunStartupScripts(engineName);
			//	}
			//}
		}


		void InitEvaluation()
		{
			var rosProcess = new RosProcess(MunCore.Default);
			rosProcess.ScriptManager = new RosManager();
			engineProcesses["ROS"] = rosProcess;

			var kerbaluaProcess = new KerbaluaProcess();
			kerbaluaProcess.ScriptManager = new KerbaluaManager();
			engineProcesses["Lua"] = kerbaluaProcess;

			string lastEngineName = SavedSettings.LoadSetting("lastEngine", "Lua");
			if (engineProcesses.ContainsKey(lastEngineName))
			{
				SetCurrentEngineProcess(lastEngineName);
			}
			else
			{
				foreach (var engineName in engineProcesses.Keys)
				{
					SetCurrentEngineProcess(engineName);
					SavedSettings.SaveSetting("lastEngine", engineName);
					break;
				}
			}

			foreach (var engineName in engineProcesses.Keys)
			{
				uiparts.scriptEngineSelector.AddMinSized(new Button(engineName,() =>
				{
					SetCurrentEngineProcess(engineName);
					SavedSettings.SaveSetting("lastEngine", engineName);
				}));
			}
			//replEvaluators["ROS"] = new RedOnionReplEvaluator()
			//{
			//	PrintAction = uiparts.replOutoutArea.AddOutput,
			//	PrintErrorAction = uiparts.replOutoutArea.AddError
			//};
			//replEvaluators["Lua"] = new MoonSharpReplEvaluator()
			//{
			//	PrintAction = uiparts.replOutoutArea.AddOutput,
			//	PrintErrorAction = uiparts.replOutoutArea.AddError
			//};
			//#if DEBUG
			//			replEvaluators["nLua"] = new KerbnluaReplEvaluator()
			//			{
			//				PrintAction = uiparts.replOutoutArea.AddOutput,
			//				PrintErrorAction = uiparts.replOutoutArea.AddError
			//			};
			//#endif
			//var scriptEngineSelector=uiparts.scriptEngineSelector;

			//string lastEngineName = SavedSettings.LoadSetting("lastEngine", "Lua");
			//if (replEvaluators.ContainsKey(lastEngineName))
			//{
			//	SetCurrentEvaluator(lastEngineName);
			//}
			//else
			//{
			//	foreach (var evaluatorName in replEvaluators.Keys)
			//	{
			//		SetCurrentEvaluator(evaluatorName);
			//		SavedSettings.SaveSetting("lastEngine", evaluatorName);
			//		break;
			//	}
			//}

			//foreach (var evaluatorName in replEvaluators.Keys)
			//{
			//	scriptEngineSelector.AddMinSized(new Button(evaluatorName,() =>
			//	{
			//		SetCurrentEvaluator(evaluatorName);
			//		SavedSettings.SaveSetting("lastEngine", evaluatorName);
			//	}));
			//}

			RunAutorunScripts();
		}

		public IList<string> GetAutorunScripts(string extensionToMatch)
		{
			// EvanTodo: Disabling autorun scripts for now
			var allScriptNames = AutoRun.scripts();
			var extensionScriptNames=new List<string>();
			foreach (var scriptname in allScriptNames)
			{
				var extension=Path.GetExtension(scriptname);
				if (extension==extensionToMatch)
				{
					extensionScriptNames.Add(scriptname);
				}
			}
			return extensionScriptNames;
		}
	}
}
