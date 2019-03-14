using System;
using System.Collections.Generic;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
using RedOnion.Script.ReflectedObjects;
using UnityEngine;
using Kerbalua.AutoPilot;
using KSP.UI.Screens;
using RedOnion.KSP;

namespace Kerbalua.Other {
	public class RedOnionReplEvaluator:ReplEvaluator {
		ImmediateEngine engine;

		public RedOnionReplEvaluator()
		{
			//temporarily commenting this out
			//engine = new Engine(engine => new EngineRoot(engine));
			engine = new ImmediateEngine();
		}


		protected override bool ProtectedEvaluate(string source,out string output)
		{
			output = "";
			try {
				//Debug.Log("Running statement with Execution Countdown at " + engine.ExecutionCountdown);
				engine.ExecutionCountdown = 10000;
				engine.Execute(source);
				Value result = engine.Result;
				output +=result.ToString();
			}
			catch(Exception e) {
				Debug.Log(e);
			}

			// TODO: This needs to be replaced when engine can fail to complete in one update
			bool isComplete = true;

			return isComplete;
		}

		/// <summary>
		/// TODO: NOT CURRENTLY IMPLEMENTED
		/// See the abstract version for complete comments.
		/// </summary>
		public override IList<string> GetCompletions(string source, int cursorPos,out int replaceStart,out int replaceEnd)
		{
			List<string> NOT_IMPLEMENTED_COMPLETIONS = new List<string>();
			NOT_IMPLEMENTED_COMPLETIONS.Add("RedOnion");
			NOT_IMPLEMENTED_COMPLETIONS.Add("intellisense");
			NOT_IMPLEMENTED_COMPLETIONS.Add("is");
			NOT_IMPLEMENTED_COMPLETIONS.Add("not");
			NOT_IMPLEMENTED_COMPLETIONS.Add("currently");
			NOT_IMPLEMENTED_COMPLETIONS.Add("implemented");
			for (int i = 0;i < 100;i++) {
				NOT_IMPLEMENTED_COMPLETIONS.Add("test-string #" + i);
			}
			replaceStart = replaceEnd = cursorPos;
			return NOT_IMPLEMENTED_COMPLETIONS;
		}


		public override void ResetEngine()
		{
			engine.Reset();
		}

		public override void Terminate()
		{
			throw new NotImplementedException();
		}
	}
}
