using MunOS.Core;
using MunOS.ProcessLayer;
using RedOnion.ROS;
using RedOnion.ROS.Objects;
using System;
using System.Diagnostics;

namespace RedOnion.KSP.ROS
{
	//TODO: off-load parsing to another thread (and yield until done)

	public class RosThread : EngineThread
	{
		public static int StepCountdown = 100;
		public static int MaxExecLoops = 10;

		ExecPriority priority;
		RosProcess process;
		RosProcessor processor;
		internal Core core;
		Function fn;

		public RosThread(ExecPriority priority, string source, string path, RosProcess parentProcess)
			: base(source, path, parentProcess)
		{
			this.priority = priority;
			this.process = parentProcess;
			processor = parentProcess.Processor;
		}
		public RosThread(ExecPriority priority, Function fn, RosProcess parentProcess)
			: base(fn.Code.Source, fn.Code.Path, parentProcess)
		{
			this.priority = priority;
			this.process = parentProcess;
			processor = parentProcess.Processor;
			this.fn = fn;
		}

		public override bool IsSleeping => false;

		bool resultReported = false;
		protected override ExecStatus ProtectedExecute(long tickLimit)
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
				var start = Stopwatch.GetTimestamp();
				for (int i = 0; core.Paused && i < MaxExecLoops; i++)
				{
					core.Execute(StepCountdown);
					if (core.Exit != ExitCode.Countdown)
						break;
					if ((Stopwatch.GetTimestamp() - start) >= tickLimit)
						break;
				}
			}
			switch (core.Exit)
			{
			case ExitCode.Yield:
				return ExecStatus.YIELDED;
			case ExitCode.Countdown:
				return ExecStatus.INTERRUPTED;
			default:
				if (path == null && fn == null)
				{
					if (!resultReported)
					{
						parentProcess.outputBuffer.AddReturnValue(processor.Result.ToString());
						resultReported = true;
					}
					return ExecStatus.FINISHED;
				}
				if (priority == ExecPriority.ONESHOT || core.Exit == ExitCode.Exception)
					return ExecStatus.FINISHED;
				if (fn != null)
					core.Execute(fn, countdown: 0);
				return ExecStatus.YIELDED;
			}
		}
	}
}
