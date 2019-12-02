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
		public static readonly double TicksPerMicro=Stopwatch.Frequency/(1000.0 * 1000.0);

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
		static public long OneshotForceTicks = 1;
		static public long IdleForceTicks = 1;

		public void Reset()
		{
			processes.Clear();
		}

		/// <summary>
		/// Registers the executable.
		/// </summary>
		/// <returns>The ID of the process.</returns>
		/// <param name="priority">The priority you want the execution to run on.</param>
		/// <param name="executable">The object that we will call Execute on.</param>
		/// <param name="name">The optional name of this process, to appear in process managers.</param>
		public long RegisterExecutable(Priority priority, IExecutable executable, string name="")
		{
			var process=new Process(name, executable);
			priorities[priority].waitQueue.Enqueue(process);
			return process.ID;
		}

		Stopwatch stopwatch = new Stopwatch();
		public void Execute(long tickLimit)
		{
			long remainingTicks = 0;

			remainingTicks = tickLimit;
			var realtimeTicks = remainingTicks - (long)(tickLimit * RealtimeFractionalLimit);
			stopwatch.Reset();
			stopwatch.Start();
			priorities[Priority.REALTIME].Execute(realtimeTicks);
			stopwatch.Stop();

			remainingTicks = remainingTicks - stopwatch.ElapsedTicks;
			var oneshotTicks = remainingTicks - (long)(tickLimit * OneShotFractionalLimit);
			stopwatch.Reset();
			stopwatch.Start();
			// if oneshotTicks is < 0 we still call Execute on it because execution of 
			// oneshots is occasionally forced even when no time is available
			priorities[Priority.ONESHOT].Execute(oneshotTicks);
			stopwatch.Stop();

			remainingTicks = remainingTicks - stopwatch.ElapsedTicks;
			var idleTicks = remainingTicks - (long)(tickLimit * IdleFractionalLimit);
			stopwatch.Reset();
			stopwatch.Start();
			// if idleTicks is < 0 we still call Execute on it because execution of 
			// oneshots is occasionally forced even when no time is available
			priorities[Priority.IDLE].Execute(idleTicks);
			stopwatch.Stop();

			remainingTicks = remainingTicks - stopwatch.ElapsedTicks;
			var normalTicks = remainingTicks;
			stopwatch.Reset();
			stopwatch.Start();
			priorities[Priority.MAIN].Execute(normalTicks);
			stopwatch.Stop();
		}


		public double UpdateMicros = 2000;
		List<ROProcess> processes = new List<ROProcess>();

		public void RegisterProcess(ROProcess process)
		{
			processes.Add(process);
		}

		public void FixedUpdate()
		{
			Execute((long)(UpdateMicros*TicksPerMicro));
		}
	}
}
