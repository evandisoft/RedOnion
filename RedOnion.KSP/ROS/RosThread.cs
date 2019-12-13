using MunOS.Core;
using MunOS.ProcessLayer;
using RedOnion.ROS;
using System;
using System.Diagnostics;

namespace RedOnion.KSP.ROS
{
	public class RosThread : EngineThread
	{
		RosProcessor processor;
		public RosThread(string source, string path, RosProcess parentProcess) : base(source, path, parentProcess)
		{
			processor = parentProcess.Processor;
			processor.Execute(source, path, countdown: 0);
		}

		public override bool IsSleeping => false;

		protected override ExecStatus ProtectedExecute(long tickLimit)
		{
			processor.UpdateTimeout = TimeSpan.FromSeconds((double)tickLimit / Stopwatch.Frequency);
			processor.UpdatePhysics();
			switch (processor.Exit)
			{
			case ExitCode.Yield:
				return ExecStatus.YIELDED;
			case ExitCode.Countdown:
				return ExecStatus.INTERRUPTED;
			default:
				parentProcess.outputBuffer.AddReturnValue(processor.Result.ToString());
				return ExecStatus.FINISHED;
			}
		}
	}
}
