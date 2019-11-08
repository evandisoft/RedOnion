using System;
namespace Kerbalua.Other {
	public class Evaluation {
		public string Source { get; private set; }
		public string Path { get; private set; }
		public string Result { get; private set; }
		public bool WithHistory { get; private set; }
		public ReplEvaluator ReplEvaluator { get; private set; }

		bool sourceSet = false;

		public Evaluation(string source, string path,
			ReplEvaluator replEvaluator, bool withHistory=false)
		{
			Source = source;
			Path = path;
			ReplEvaluator = replEvaluator;
			WithHistory = withHistory;
		}

		public bool Evaluate()
		{
			if (!sourceSet)
			{
				ReplEvaluator.SetSource(Source, Path, WithHistory);
				sourceSet = true;
			}

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
