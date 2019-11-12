using System;
using System.Collections.Generic;
using Kerbalua.Other;
using Kerbalui.Controls;
using Kerbalui.Types;
using LiveRepl.Misc;
using LiveRepl.UI;
using LiveRepl.UI.ReplParts;
using RedOnion.KSP.Settings;
using UnityEngine;

namespace LiveRepl
{
	public partial class ScriptWindow
	{
		public bool ScriptRunning { get; set; }

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

		public void SetCurrentEvaluator(string evaluatorName)
		{
			currentReplEvaluator = replEvaluators[evaluatorName];
			contentGroup.centerGroup.scriptEngineSelector.scriptEngineLabel.SetEngine(evaluatorName);
		}

		Dictionary<string,ReplEvaluator> replEvaluators=new Dictionary<string, ReplEvaluator>();
		CompletionManager completionManager;
		public ReplEvaluator currentReplEvaluator;

		public ScriptWindow(string title) : base(title)
		{
			InitLayout();

			var repl=contentGroup.replGroup.repl;
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
				scriptEngineSelector.AddMinSized(new Button(evaluatorName, () =>
				{
					SetCurrentEvaluator(evaluatorName);
					SavedSettings.SaveSetting("lastEngine", evaluatorName);
				}));
			}

			//completionManager=new CompletionManager(contentGroup.completionGroup.completionArea);
			//completionManager.AddCompletable(new EditingAreaCompletionAdapter(contentGroup.editorGroup.editor));
		}
	}
}
