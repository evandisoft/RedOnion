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

		public RosProcessor Processor { get; private set; }
		protected internal RosThread ReplThread { get; set; }

		public RosProcess() : this(false) { }
		public RosProcess(bool lateBind)
		{
			if (!lateBind)
				SetProcessor(new RosProcessor(this));
		}
		public void SetProcessor(RosProcessor processor)
		{
			if (Processor != null)
				throw new InvalidOperationException("ROS Processor already set");
			Processor = processor;
			Processor.Print += outputBuffer.AddOutput;
			Processor.PrintError += outputBuffer.AddError;
		}

		// TODO: redesign Init
		public override string GetImportString(string scriptname)
			=> "run \"" + scriptname + "\"";
		protected override MunThread CreateThread(string source, string path)
			=> new RosThread(ExecPriority.MAIN, source, path, this);

		// note that `inRepl` is true for full scripts as well, so we check path==null
		public override void Execute(ExecPriority priority, string source, string path, bool inRepl)
		{
			if (path == null)
			{
				if (ReplThread == null)
				{
					EnqueueThread(priority, ReplThread = new RosThread(priority, source, path, this));
					return;
				}
				ReplThread.core = null; // this forces source replacement
				ReplThread.source = source;
				return;
			}
			EnqueueThread(priority, new RosThread(priority, source, path, this));
		}

		protected override void ThreadExecutionComplete(MunThread thread, Exception e)
		{
			if (ReplThread == thread)
				ReplThread = null;
			if (e != null)
				Processor.PrintException(null, e);
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
