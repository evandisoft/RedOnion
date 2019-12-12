using MunOS.Core;
using MunOS.ProcessLayer;
using RedOnion.ROS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedOnion.KSP.ROS
{
	public class RosProcess : EngineProcess
	{
		public override string Extension => ".ros";
		public override string GetImportString(string scriptname) => "run.library \"" + scriptname + "\"";

		public RosProcessor Processor { get; private set; }
		public RosProcess()
		{
			Processor = new RosProcessor();
			Processor.Print += outputBuffer.AddOutput;
			Processor.PrintError += outputBuffer.AddError;
		}

		protected override void ExecuteSourceInReplThread(ExecPriority priority, string source, string path)
		{
			try
			{
				EnqueueThread(priority, new RosThread(source, path, this));
			}
			catch (Exception e)
			{
				outputBuffer.AddError(e.Message);
			}
		}

		public override void ExecuteSourceInThread(ExecPriority priority, string source, string path)
		{
			try
			{
				EnqueueThread(priority, new RosThread(source, path, this));
			}
			catch (Exception e)
			{
				outputBuffer.AddError(e.Message);
			}
		}

		public override void ResetEngine()
		{
			base.ResetEngine();
			Processor.Reset();
			suggest.Reset();
		}
		public override void Terminate()
		{
			Value.DebugLog($"ROS Process ID#{ID} terminated. (shutdown: {shutdown?.GetInvocationList().Length ?? 0})");
			Processor.Terminate();
			base.Terminate();
		}

		RosSuggest suggest;
		public override IList<string> GetDisplayableCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd)
			=> GetCompletions(source, cursorPos, out replaceStart, out replaceEnd);
		public override IList<string> GetCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd)
		{
			if (suggest == null)
				suggest = new RosSuggest(Processor);
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


	}
}
