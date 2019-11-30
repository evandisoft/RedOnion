using System;
using System.Diagnostics;

namespace MunOS
{
	public class ExecutionManager
	{
		public ExecutionManager()
		{

		}

		// Only let the exectors run until this percentage of update execution
		// time is remaining. In other words, only run Realtime at most
		// for the first half of execution time. Only run oneshot until 40%
		// remains (at most). Only run idle until 60% remains.
		static public double RealtimeFractionalLimit = 0.5;
		static public double OneShotFractionalLimit = 0.6;
		static public double IdleFractionalLimit = 0.4;
		static public double OneShotForceExecuteTime = 1;
		static public double IdleForceExecuteTime = 1;

		double nsPerTick=(1000L*1000L*1000L/Stopwatch.Frequency);
		long frequency=Stopwatch.Frequency;

		Stopwatch stopwatch=new Stopwatch();
		public void Execute(double timeLimitMicros = 1000)
		{
			double remainingTime = 0;

			remainingTime = timeLimitMicros;
			var realtimeRuntime = remainingTime - timeLimitMicros * RealtimeFractionalLimit;
			stopwatch.Reset();
			stopwatch.Start();
			priorities[Priority.REALTIME].Execute(realtimeRuntime);
			stopwatch.Stop();

			remainingTime = remainingTime - stopwatch.Elapsed.TotalMilliseconds;
			var oneshotRuntime = remainingTime - timeLimitMicros * OneShotFractionalLimit;
			stopwatch.Reset();
			stopwatch.Start();
			priorities[Priority.ONESHOT].Execute(oneshotRuntime);
			stopwatch.Stop();

			remainingTime = remainingTime - stopwatch.Elapsed.TotalMilliseconds;
			var idleRuntime = remainingTime - timeLimitMicros * IdleFractionalLimit;
			stopwatch.Reset();
			stopwatch.Start();
			priorities[Priority.IDLE].Execute(idleRuntime);
			stopwatch.Stop();

			remainingTime = remainingTime - stopwatch.Elapsed.TotalMilliseconds;
			var normalRuntime = remainingTime;
			stopwatch.Reset();
			stopwatch.Start();
			priorities[Priority.MAIN].Execute(normalRuntime);
			stopwatch.Stop();
		}
	}
}
