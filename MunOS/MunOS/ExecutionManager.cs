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

		Dictionary<long,ProcessEntry> processDictionary = new Dictionary<long, ProcessEntry>();
		Dictionary<Priority, PriorityExecutor> priorities = new Dictionary<Priority, PriorityExecutor>();

		internal struct ProcessEntry
		{
			public Priority priority;
			public Process process;

			public ProcessEntry(Priority priority, Process process)
			{
				this.priority=priority;
				this.process=process;
			}
		}
		/// <summary>
		/// Kill the process with the specified ID. mark it as terminated, don't hunt it down and remove it from
		/// the queues, just mark it and it will not be executed.
		/// </summary>
		/// <param name="ID">Identifier.</param>
		public void Kill(long ID)
		{
			if (processDictionary.ContainsKey(ID))
			{
				var processEntry=processDictionary[ID];
				var process=processEntry.process;
				process.executable.OnTerminated(process.name, process.ID);
				var priority=processEntry.priority;
				priorities[priority].Kill(ID);
			}
		}

		internal void Remove(long ID)
		{
			processDictionary.Remove(ID);
		}

		public int Count
		{
			get
			{
				int count=0;
				foreach (var executor in priorities.Values)
				{
					count+=executor.Count;
				}
				return count;
			}
		}

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
		static public long OneshotForceTicks = 100;
		static public long IdleForceTicks = 100;

		//public void Reset()
		//{
		//	processes.Clear();
		//}

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
			processDictionary[process.ID]=new ProcessEntry(priority,process);
			priorities[priority].waitQueue.Enqueue(process);
			return process.ID;
		}

		Stopwatch stopwatch = new Stopwatch();
		public void Execute(long tickLimit)
		{
			stopwatch.Reset();
			stopwatch.Start();

			long realtimeTicks = tickLimit - (long)(tickLimit * RealtimeFractionalLimit);
			priorities[Priority.REALTIME].Execute(realtimeTicks);

			long remainingTicks = tickLimit - stopwatch.ElapsedTicks;
			long oneshotTicks = remainingTicks - (long)(tickLimit * OneShotFractionalLimit);
			// if oneshotTicks is < 0 we still call Execute on it because execution of 
			// oneshots is occasionally forced even when no time is available
			priorities[Priority.ONESHOT].Execute(oneshotTicks);

			remainingTicks = tickLimit - stopwatch.ElapsedTicks;
			long idleTicks = remainingTicks - (long)(tickLimit * IdleFractionalLimit);
			// if idleTicks is < 0 we still call Execute on it because execution of 
			// oneshots is occasionally forced even when no time is available
			priorities[Priority.IDLE].Execute(idleTicks);
			stopwatch.Stop();

			long normalTicks = tickLimit - stopwatch.ElapsedTicks;
			stopwatch.Reset();
			stopwatch.Start();
			priorities[Priority.MAIN].Execute(normalTicks);
			stopwatch.Stop();
		}


		public double UpdateMicros = 2000;
		////List<ROProcess> processes = new List<ROProcess>();

		//public void RegisterProcess(ROProcess process)
		//{
		//	processes.Add(process);
		//}

		public void FixedUpdate()
		{
			Execute((long)(UpdateMicros*TicksPerMicro));
		}
	}
}
