using System;
using RedOnion.Script;
using UnityEngine;

namespace Kerbalua.Other {
	public class RedOnionReplEvaluator:ReplEvaluator {
		Engine engine;

		public RedOnionReplEvaluator(Engine engine)
		{
			this.engine = engine;
			engine.ExecutionCountdown = 10000;
		}

		public override string Evaluate(string source)
		{
			string output = "";
			try {
				//Debug.Log("Running statement with Execution Countdown at " + engine.ExecutionCountdown);
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
	}
}
