using System;
using System.Collections.Generic;
using RedOnion.Script;
using UnityEngine;

namespace Kerbalua.Other {
	public class RedOnionReplEvaluator:ReplEvaluator {
		Engine engine;

		public RedOnionReplEvaluator()
		{
			engine = new Engine();
		}

		public override string Evaluate(string source)
		{
			string output = "";
			try {
				//Debug.Log("Running statement with Execution Countdown at " + engine.ExecutionCountdown);
				engine.ExecutionCountdown = 10000;
				engine.Execute(source);
				Value result = engine.Result;
				output = "\n";
				output +=result.ToString();
			}
			catch(Exception e) {
				Debug.Log(e);
			}

			return output;
		}


		public override List<string> GetCompletions(string source, int cursorPos)
		{
			throw new NotImplementedException();
		}

		public override string GetPartialCompletion(string source, int cursorPos)
		{
			throw new NotImplementedException();
		}

		public override void ResetEngine()
		{
			engine.Reset();
		}
	}
}
