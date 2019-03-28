using System;
using System.Collections.Generic;
using RedOnion.Script;
using UnityEngine;
using RedOnion.KSP;
using RedOnion.KSP.Autopilot;
using RedOnion.Script.Parsing;

namespace Kerbalua.Other
{
	public class RedOnionReplEvaluator : ReplEvaluator
	{
		ImmediateEngine engine;
		ReplHintsEngine hints;

		public RedOnionReplEvaluator()
		{
			engine = new ImmediateEngine();
			hints = new ReplHintsEngine(engine);

			engine.Printing += msg => PrintAction?.Invoke(msg);
		}

		//string mainResult;
		bool isComplete = true;
		//protected override bool ProtectedEvaluate(string source, out string output)
		//{
		//	output = "";
		//	try
		//	{
		//		if (isComplete)
		//		{
		//			engine.ExecutionCountdown = 10000;
		//			engine.Execute(source);
		//			mainResult = engine.Result.ToString();
		//			if (engine.HasEvents)
		//				return isComplete = false;
		//			output = mainResult;
		//			return isComplete = true;
		//		}
		//		engine.FixedUpdate();
		//		isComplete = !engine.HasEvents;
		//		if (isComplete)
		//			output = mainResult;
		//		return isComplete;
		//	}
		//	catch (Exception e)
		//	{
		//		PrintErrorAction?.Invoke(e.Message);

		//		string FormatLine(int lineNumber, string line)
		//			=> string.Format(Value.Culture,
		//			line == null ? "At line {0}." : "At line {0}: {1}",
		//			lineNumber+1, line);

		//		if (e is RuntimeError runError)
		//			PrintErrorAction?.Invoke(FormatLine(runError.LineNumber, runError.Line));
		//		else if (e is ParseError parseError)
		//			PrintErrorAction?.Invoke(FormatLine(parseError.LineNumber, parseError.Line));

		//		Debug.Log(e);
		//	}
		//	Terminate();
		//	output = "";
		//	return isComplete = true;
		//}

		/// <summary>
		/// See the abstract version for complete comments.
		/// </summary>
		public override IList<string> GetCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd)
		{
			try
			{
				return hints.Complete(source, cursorPos, out replaceStart, out replaceEnd);
			}
			catch (Exception e)
			{
				Debug.Log(e);
				replaceStart = replaceEnd = cursorPos;
				return new List<string>();
			}
		}


		public override void ResetEngine()
		{
			isComplete = true;
			engine.Reset();
			hints.Reset();
			FlightControl.GetInstance().Shutdown();
		}

		public override void Terminate()
		{
			isComplete = true;
			engine.Update.Clear();
			engine.Idle.Clear();
		}

		public override bool Evaluate(out string result)
		{
			throw new NotImplementedException();
		}

		public override void ProtectedSetSource(string source)
		{
			throw new NotImplementedException();
		}
	}
}
