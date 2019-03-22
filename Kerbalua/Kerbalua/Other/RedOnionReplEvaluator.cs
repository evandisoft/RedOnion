using System;
using System.Collections.Generic;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
using RedOnion.Script.ReflectedObjects;
using UnityEngine;
using Kerbalua.AutoPilot;
using KSP.UI.Screens;
using RedOnion.KSP;
using RedOnion.KSP.Autopilot;

namespace Kerbalua.Other {
	public class RedOnionReplEvaluator:ReplEvaluator {
		ImmediateEngine engine;
		ReplHintsEngine hints;

		public RedOnionReplEvaluator()
		{
			engine = new ImmediateEngine();
			hints = new ReplHintsEngine(engine);
		}

		public RedOnionReplEvaluator(Engine engine)
		{
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
		/// See the abstract version for complete comments.
		/// </summary>
		public override IList<string> GetCompletions(string source, int cursorPos,out int replaceStart,out int replaceEnd)
		{
			try {
				return hints.Complete(source, cursorPos, out replaceStart, out replaceEnd);
			}
			catch(Exception e) {
				Debug.Log(e);
				replaceStart = replaceEnd = cursorPos;
				return new List<string>();
			}
		}


		public override void ResetEngine()
		{
			engine.Reset();
			hints.Reset();
			FlightControl.GetInstance().Shutdown();
		}

		public override void Terminate()
		{
			throw new NotImplementedException();
		}
	}
}
