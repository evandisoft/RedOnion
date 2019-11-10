using System;
using System.Collections.Generic;
using System.IO;
using Kerbalua.Other;
using Kerbalui.Gui;
using RedOnion.KSP.API;
using UnityEngine;

namespace LiveRepl.Main
{
	public partial class ReplMain
	{
		List<Evaluation> evaluationList = new List<Evaluation>();
		public ReplEvaluator currentReplEvaluator;
		public Dictionary<string, ReplEvaluator> replEvaluators = new Dictionary<string, ReplEvaluator>();

		private void HandleInputWhenExecuting()
		{
			if (Event.current.type == EventType.KeyDown
								&& Event.current.keyCode == KeyCode.C
								&& Event.current.control)
			{
				GUILibUtil.ConsumeAndMarkNextCharEvent(Event.current);
				evaluationList.Clear();
				foreach (var replEvaluator in replEvaluators.Values)
				{
					replEvaluator.Terminate();
				}
				repl.outputBox.AddError("Execution Manually Terminated");
			}

			EventType t = Event.current.type;
			if (t == EventType.KeyDown)
			{
				Event.current.Use();
			}
			else if (t == EventType.MouseDown)
			{
				Event.current.Use();
			}
		}

		public void SetCurrentEvaluator(string evaluatorName)
		{
			currentReplEvaluator = replEvaluators[evaluatorName];
			replEvaluatorLabel.content.text = evaluatorName;
		}

		void RunLuaStartupScripts()
		{
			var scriptnames = AutoRun.Instance.scripts();
			foreach (var scriptname in scriptnames)
			{
				string extension = Path.GetExtension(scriptname).ToLower();
				string basename = Path.GetFileNameWithoutExtension(scriptname);
				if (extension==".lua")
				{
					repl.outputBox.AddFileContent("loading "+scriptname+"...");
					var newEvaluation = new Evaluation("require(\"" + basename + "\")",scriptname,replEvaluators["Lua Engine"]);
					evaluationList.Add(newEvaluation);
				}
			}
		}

		void RunRosStartupScripts()
		{
			var scriptnames = AutoRun.Instance.scripts();
			foreach (var scriptname in scriptnames)
			{
				string extension = Path.GetExtension(scriptname).ToLower();
				string basename = Path.GetFileNameWithoutExtension(scriptname);
				if (extension == ".ros")
				{
					repl.outputBox.AddFileContent("loading " + scriptname + "...");
					var newEvaluation = new Evaluation("run \"" + scriptname+"\"", scriptname, replEvaluators["ROS Engine"]);
					evaluationList.Add(newEvaluation);
				}
			}
		}

		void RunStartupScripts(string engineName)
		{
			if (engineName=="Lua Engine")
			{
				RunLuaStartupScripts();
			}
			if (engineName=="ROS Engine")
			{
				RunRosStartupScripts();
			}
		}

		public void FixedUpdate()
		{
			if (evaluationList.Count!=0)
			{
				var currentEvaluation = evaluationList[0];
				if (currentEvaluation.Evaluate())
				{
					repl.outputBox.AddReturnValue(currentEvaluation.Result);
					evaluationList.RemoveAt(0);
				}
			}
			foreach (var engineName in replEvaluators.Keys)
			{
				var repl = replEvaluators[engineName];
				try
				{
					repl.FixedUpdate();
				}
				catch (Exception ex)
				{
					Debug.Log("Exception in REPL.FixedUpdate: " + ex.Message);
					repl.ResetEngine();
					RunStartupScripts(engineName);
				}
			}
		}
	}
}
