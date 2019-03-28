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
		string source, path;
		bool skipUpdate;

		public RedOnionReplEvaluator()
		{
			engine = new ImmediateEngine();
			hints = new ReplHintsEngine(engine);

			engine.Printing += msg => PrintAction?.Invoke(msg);
		}

		public override void FixedUpdate()
		{
			if (skipUpdate)
			{
				skipUpdate = false;
				return;
			}
			engine.FixedUpdate();
		}

		protected override void ProtectedSetSource(string source, string path)
		{
			this.source = source;
			this.path = path;
			//TODO: unsubscribe events from last execution if path is the same
		}
		public override bool Evaluate(out string result)
		{
			try
			{
				skipUpdate = true;
				engine.ExecutionCountdown = 10000;
				engine.Execute(source);
				result = engine.Result.ToString();
				return true; // for now we always complete immediately
			}
			catch (Exception e)
			{
				PrintErrorAction?.Invoke(e.Message);

				string FormatLine(int lineNumber, string line)
					=> string.Format(Value.Culture,
					line == null ? "At line {0}." : "At line {0}: {1}",
					lineNumber+1, line);

				if (e is RuntimeError runError)
					PrintErrorAction?.Invoke(FormatLine(runError.LineNumber, runError.Line));
				else if (e is ParseError parseError)
					PrintErrorAction?.Invoke(FormatLine(parseError.LineNumber, parseError.Line));

				Debug.Log(e);
			}
			Terminate();
			result = "";
			return true;
		}

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
			source = null;
			path = null;
			engine.Reset();
			hints.Reset();
			FlightControl.GetInstance().Shutdown();
		}

		public override void Terminate()
		{
			source = null;
			path = null;
			engine.ClearEvents();
		}
	}
}
