using System;
namespace Kerbalua.Other {
	public class Evaluation {
		public string Source { get; private set; }
		public string Result { get; private set; }
		public bool WithHistory { get; private set; }
		public ReplEvaluator ReplEvaluator { get; private set; }

		public Evaluation(string source,ReplEvaluator replEvaluator,bool withHistory=false)
		{
			Source = source;
			ReplEvaluator = replEvaluator;
			replEvaluator.SetSource(source,withHistory);
			WithHistory = withHistory;
		}

		public bool Evaluate()
		{
			string result;
			if(ReplEvaluator.Evaluate(out result)) {
				Result = result;
				return true;
			}
			return false;
		}

		public void Terminate()
		{
			ReplEvaluator.Terminate();
		}
	}
}
