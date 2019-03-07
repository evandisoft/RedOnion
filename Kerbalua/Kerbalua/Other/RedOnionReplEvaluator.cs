using System;
using RedOnion.Script;
using UnityEngine;

namespace Kerbalua.Other {
	public class RedOnionReplEvaluator:ReplEvaluator {
		Engine engine;

		public RedOnionReplEvaluator(Engine engine)
		{
			this.engine = engine;
		}

		public override string Evaluate(string source)
		{
			string output = "";
			try {
				engine.Execute(source);
				Value result = engine.Result;
				output = Environment.NewLine;
				output +=result.ToString();
			}
			catch(Exception e) {
				Debug.Log(e);
			}

			return output;
		}
	}
}
