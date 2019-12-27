using MunOS;
using RedOnion.ROS;
using RedOnion.ROS.Objects;
using static RedOnion.Debugging.QueueLogger;

namespace RedOnion.KSP.ROS
{
	public class RosThread : MunThread
	{
		public static int StepCountdown = 100;
		public static int MaxExecLoops = 10;

		RosProcess process;
		RosProcessor processor;
		internal Core core;
		string source, path;

		Function fn;
		bool restart = true;
		public Function Function
		{
			get => fn;
			set
			{
				fn = value;
				restart = true;
				if (value != null)
					Name = fn.Name;
			}
		}

		public RosThread(RosProcess process, MunPriority priority, string source, string path, bool start = true)
			: base(process.Core, process, priority, path, start)
		{
			this.process = process;
			processor = process.Processor;
			this.source = source;
			this.path = path;
			RosLogger.Log($"Thread#{ID} created with {(path != null ? "path: " + path : source != null ? "source" : "nothing")}");
		}
		public RosThread(RosProcess process, MunPriority priority, Function fn, bool start = true)
			: base(process.Core, process, priority, fn.Name, start)
		{
			this.process = process;
			processor = process.Processor;
			this.fn = fn;
			if (ID > 0) RosLogger.Log($"Thread#{ID} created with function {Name}");
		}

		bool resultReported = false;
		protected override MunStatus Execute(long tickLimit)
		{
			var start = MunCore.Ticks;
			if (restart)
			{
				if (path == null && fn == null)
				{// REPL
					core = processor;
				}
				else if (core == null)
				{
					core = new Core(processor);
					core.Globals = processor.Globals;
				}
				if (fn != null) core.Execute(fn, countdown: 0);
				else core.Execute(source, path, countdown: 0); //TODO: off-load parsing to another thread (and yield until done)
				restart = false;
			}
			for (int i = 0; core.Paused && i < MaxExecLoops; i++)
			{
				core.Execute(StepCountdown);
				if (core.Exit != ExitCode.Countdown)
					break;
				if ((MunCore.Ticks - start) >= tickLimit)
					break;
			}
			switch (core.Exit)
			{
			case ExitCode.Yield:
				return MunStatus.Yielded;
			case ExitCode.Countdown:
				return MunStatus.Incomplete;
			default:
				if (path == null && Function == null && !resultReported)
				{
					Process?.OutputBuffer?.AddReturnValue(processor.Result.ToString());
					resultReported = true;
				}
				return MunStatus.Finished;
			}
		}

		public override void Restart()
		{
			//RosLogger.DebugLog($"Restarting thread#{ID}: {Name}");
			restart = true;
			base.Restart();
		}

#if DEBUG
		protected override void OnDone()
		{
			if (ID > 0) RosLogger.Log($"Thread#{ID}/{Name} done");
			base.OnDone();
		}
#endif
	}
}
