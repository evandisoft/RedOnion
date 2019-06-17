using System;
using System.Collections.Generic;
using UnityEngine;
using RedOnion.ROS;
using RedOnion.KSP;
using RedOnion.KSP.Autopilot;
using RedOnion.KSP.ROS;
using RedOnion.ROS.Objects;

namespace Kerbalua.Other
{
	public class RedOnionReplEvaluator : ReplEvaluator
	{
		RosProcessor processor;
		RosSuggest suggest;
		string source, path;
		bool skipUpdate;

		public RedOnionReplEvaluator()
		{
			suggest = new RosSuggest(processor = new RosProcessor());
			processor.Print += PrintRedirect;
		}
		protected override void Dispose(bool disposing)
		{
			processor.Print -= PrintRedirect;
			if (disposing)
				processor.Dispose();
			base.Dispose(disposing);
		}
		protected void PrintRedirect(string msg)
			=> PrintAction?.Invoke(msg);

		public override void FixedUpdate()
		{
			if (skipUpdate)
			{
				skipUpdate = false;
				return;
			}
			processor.FixedUpdate();
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
				processor.Execute(source, path, 10000);
				result = processor.Result.ToString();
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
				return suggest.GetCompletions(source, cursorPos, out replaceStart, out replaceEnd);
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
			processor.Reset();
			suggest.Reset();
			base.ResetEngine();
		}

		public override void Terminate()
		{
			source = null;
			path = null;
			processor.ClearEvents();
		}
	}
}
