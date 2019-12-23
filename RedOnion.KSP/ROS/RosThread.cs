using MunOS;
using MunOS.Repl;
using RedOnion.ROS;
using RedOnion.ROS.Objects;
using System;
using System.Diagnostics;

namespace RedOnion.KSP.ROS
{
	//TODO: off-load parsing to another thread (and yield until done)

	public class RosThread : MunThread
	{
		public static int StepCountdown = 100;
		public static int MaxExecLoops = 10;

		RosProcess process;
		RosProcessor processor;
		internal Core core;
		string source, path;
		string[] includes;
		Function fn;

		public RosThread(RosProcess process, MunPriority priority, string source, string path, string[] includes)
			: base(process.Core, process, priority, path)
		{
			this.process = process;
			processor = process.Processor;
			this.source = source;
			this.path = path;
			this.includes = includes;
		}
		public RosThread(RosProcess process, MunPriority priority, Function fn)
			: base(process.Core, process, priority, fn.Name)
		{
			this.process = process;
			processor = process.Processor;
			this.fn = fn;
		}

		bool resultReported = false;
		protected override MunStatus Execute(long tickLimit)
		{
			if (core == null)
			{
				if (path == null && fn == null)
				{// REPL
					core = processor;
				}
				else
				{
					core = new Core(processor);
					core.Globals = processor.Globals;
				}
				if (fn != null) core.Execute(fn, countdown: 0);
				else core.Execute(source, path, countdown: 0);
			}
			else
			{
				var start = MunCore.Ticks;
				for (int i = 0; core.Paused && i < MaxExecLoops; i++)
				{
					core.Execute(StepCountdown);
					if (core.Exit != ExitCode.Countdown)
						break;
					if ((MunCore.Ticks - start) >= tickLimit)
						break;
				}
			}
			switch (core.Exit)
			{
			case ExitCode.Yield:
				return MunStatus.Yielded;
			case ExitCode.Countdown:
				return MunStatus.Incomplete;
			default:
				if (path == null && fn == null)
				{
					if (!resultReported)
					{
						Process?.OutputBuffer?.AddReturnValue(processor.Result.ToString());
						resultReported = true;
					}
					return MunStatus.Finished;
				}
				if (Priority == MunPriority.Callback || core.Exit == ExitCode.Exception)
					return MunStatus.Finished;
				if (fn != null)
					core.Execute(fn, countdown: 0);
				return MunStatus.Yielded;
			}
		}
	}
}
