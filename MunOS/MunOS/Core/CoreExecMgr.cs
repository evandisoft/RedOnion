using System;
using System.Collections.Generic;
using System.Diagnostics;
using MunOS.Core.Executors;

namespace MunOS.Core
{
	public enum ExecStatus
	{
		// represents that the executable is sleeping.
		SLEEPING,
		// Represents that the executable voluntarily yielded
		YIELDED,
		// represents that the executable was interrupted by an auto-yield or timeout type functionality
		// In otherwords, represents that the executable wanted to do more computations this update but was
		// paused prematurely.
		INTERRUPTED,
		// represents that the executable finished execution and can be removed.
		FINISHED,
	}

	public enum ExecPriority
	{
		REALTIME,
		ONESHOT,
		IDLE,
		MAIN
	}

	/// <summary>
	/// Manages execution of IExecutables among 4 priority levels. Lowest level of execution.
	/// </summary>
	public class CoreExecMgr
	{


		static public int MaxIdleSkips = 9;
		static public int MaxOneShotSkips = 1;
		public static readonly double TicksPerMicro=Stopwatch.Frequency/(1000.0 * 1000.0);

		Dictionary<long,ExecInfoEntry> execInfoDictionary = new Dictionary<long, ExecInfoEntry>();
		Dictionary<ExecPriority, PriorityExecutor> priorities = new Dictionary<ExecPriority, PriorityExecutor>();

		static long NextExecID=0;


		internal struct ExecInfoEntry
		{
			public ExecPriority priority;
			public ExecInfo execInfo;

			public ExecInfoEntry(ExecPriority priority, ExecInfo execInfo)
			{
				this.priority=priority;
				this.execInfo=execInfo;
			}
		}

		public bool Contains(long ID)
		{
			return execInfoDictionary.ContainsKey(ID);
		}

		bool needToRemoveTerminated;
		/// <summary>
		/// Kill the process with the specified ID. mark it as terminated, don't hunt it down and remove it from
		/// the queues, just mark it and it will not be executed.
		/// </summary>
		/// <param name="ID">Identifier.</param>
		public void Kill(long ID)
		{
			if (execInfoDictionary.ContainsKey(ID))
			{
				var execInfoEntry=execInfoDictionary[ID];
				var execInfo=execInfoEntry.execInfo;
				execInfo.executable.OnTerminated();
				execInfo.terminated=true; // mark it and it will be removed at the beginning of the update.
				//var priority=execInfoEntry.priority;
				//priorities[priority].Kill(ID);
				Remove(ID);
				needToRemoveTerminated=true;
			}
		}

		internal void Remove(long ID)
		{
			execInfoDictionary.Remove(ID);
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

		void RemoveTerminated()
		{
			foreach (var executor in priorities.Values)
			{
				executor.RemoveTerminated();
			}
		}

		private CoreExecMgr()
		{
			priorities[ExecPriority.REALTIME] = new RealtimeExecutor();
			priorities[ExecPriority.ONESHOT] = new OneShotExecutor();
			priorities[ExecPriority.IDLE] = new IdleExecutor();
			priorities[ExecPriority.MAIN] = new NormalExecutor();
		}

		static public void Initialize()
		{
			instance = new CoreExecMgr();
		}
		static public CoreExecMgr instance;
		/// <summary>
		/// Should be initialized by LiveReplMain prior to anything else being
		/// able to use it. Must be reinitialized on every scene change.
		/// </summary>
		/// <value>The instance.</value>
		static public CoreExecMgr Instance
		{
			get
			{
				if (instance==null)
				{
					throw new Exception(nameof(CoreExecMgr)+" was not initialized!");
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
		public long RegisterExecutable(ExecPriority priority, IExecutable executable)
		{
			var execInfo=new ExecInfo(NextExecID++,executable);
			execInfoDictionary[execInfo.ID]=new ExecInfoEntry(priority,execInfo);
			priorities[priority].waitQueue.Enqueue(execInfo);
			return execInfo.ID;
		}

		Stopwatch stopwatch = new Stopwatch();
		void Execute(long tickLimit)
		{
			stopwatch.Reset();
			stopwatch.Start();

			long realtimeTicks = tickLimit - (long)(tickLimit * RealtimeFractionalLimit);
			priorities[ExecPriority.REALTIME].Execute(realtimeTicks);

			long remainingTicks = tickLimit - stopwatch.ElapsedTicks;
			long oneshotTicks = remainingTicks - (long)(tickLimit * OneShotFractionalLimit);
			// if oneshotTicks is < 0 we still call Execute on it because execution of 
			// oneshots is occasionally forced even when no time is available
			priorities[ExecPriority.ONESHOT].Execute(oneshotTicks);

			remainingTicks = tickLimit - stopwatch.ElapsedTicks;
			long idleTicks = remainingTicks - (long)(tickLimit * IdleFractionalLimit);
			// if idleTicks is < 0 we still call Execute on it because execution of 
			// oneshots is occasionally forced even when no time is available
			priorities[ExecPriority.IDLE].Execute(idleTicks);
			stopwatch.Stop();

			long normalTicks = tickLimit - stopwatch.ElapsedTicks;
			stopwatch.Reset();
			stopwatch.Start();
			priorities[ExecPriority.MAIN].Execute(normalTicks);
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
			if (needToRemoveTerminated)
			{
				RemoveTerminated();
			}
			Execute((long)(UpdateMicros*TicksPerMicro));
		}
	}
}
