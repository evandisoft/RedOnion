using System;
using System.Collections.Generic;
using System.Diagnostics;
using MunOS.Executors;

namespace MunOS
{
	public class ExecutionManager
	{
		public enum Priority
		{
			REALTIME,
			ONESHOT,
			IDLE,
			MAIN
		}

		static public int MaxIdleSkips = 9;
		static public int MaxOneShotSkips = 1;
		long frequency=Stopwatch.Frequency;
		long nanosecPerTick = (1000L*1000L*1000L) / Stopwatch.Frequency;
		public static readonly double MicrosPerTick = (1000.0*1000.0) / Stopwatch.Frequency;

		Dictionary<Priority, PriorityExecutor> priorities = new Dictionary<Priority, PriorityExecutor>();

		public ExecutionManager()
		{
			priorities[Priority.REALTIME] = new RealtimeExecutor();
			priorities[Priority.ONESHOT] = new OneShotExecutor();
			priorities[Priority.IDLE] = new IdleExecutor();
			priorities[Priority.MAIN] = new NormalExecutor();
		}

		static public ExecutionManager instance;
		static public ExecutionManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ExecutionManager();
				}
				return instance;
			}
		}

		// Only let the exectors run until this percentage of update execution
		// time is remaining. In other words, only run Realtime at most
		// for the first half of execution time. Only run oneshot until 40%
		// remains (at most). Only run idle until 60% remains.
		static public double RealtimeFractionalLimit = 0.5;
		static public double OneShotFractionalLimit = 0.4;
		static public double IdleFractionalLimit = 0.6;
		static public double OneShotForceExecuteTime = 1;
		static public double IdleForceExecuteTime = 1;

		public void Reset()
		{
			processes.Clear();
		}

		public void RegisterExecutable(Priority priority, IExecutable executable)
		{
			priorities[priority].RegisterExecutable(executable);
		}

		Stopwatch stopwatch = new Stopwatch();
		public void Execute(double timeLimitMicros=2000)
		{
			double remainingTime = 0;

			remainingTime = timeLimitMicros;
			var realtimeRuntime = remainingTime - timeLimitMicros * RealtimeFractionalLimit;
			stopwatch.Reset();
			stopwatch.Start();
			priorities[Priority.REALTIME].Execute(realtimeRuntime);
			stopwatch.Stop();

			remainingTime = remainingTime - stopwatch.ElapsedTicks*MicrosPerTick;
			var oneshotRuntime = remainingTime - timeLimitMicros * OneShotFractionalLimit;
			stopwatch.Reset();
			stopwatch.Start();
			priorities[Priority.ONESHOT].Execute(oneshotRuntime);
			stopwatch.Stop();

			remainingTime = remainingTime - stopwatch.ElapsedTicks*MicrosPerTick;
			var idleRuntime = remainingTime - timeLimitMicros * IdleFractionalLimit;
			stopwatch.Reset();
			stopwatch.Start();
			priorities[Priority.IDLE].Execute(idleRuntime);
			stopwatch.Stop();

			remainingTime = remainingTime - stopwatch.ElapsedTicks*MicrosPerTick;
			var normalRuntime = remainingTime;
			stopwatch.Reset();
			stopwatch.Start();
			priorities[Priority.MAIN].Execute(normalRuntime);
			stopwatch.Stop();
		}


		public float UpdateMicros = 2000;
		List<ROProcess> processes = new List<ROProcess>();

		public void RegisterProcess(ROProcess process)
		{
			processes.Add(process);
		}

		public void FixedUpdate()
		{
			Execute(UpdateMicros);
		}
	}
}
