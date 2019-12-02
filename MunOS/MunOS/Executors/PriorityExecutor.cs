using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MunOS.Executors
{

	public enum ExecStatus
	{
		YIELDED,
		INTERRUPTED,
		TERMINATED,
	}
	/// <summary>
	/// Holds all executables of a given priority and can execute them
	/// with the given time limit.
	/// </summary>
	public abstract class PriorityExecutor
	{

		/// <summary>
		/// Holds processes waiting to be executed.
		/// </summary>
		public Queue<Process> waitQueue=new Queue<Process>();
		/// <summary>
		/// Holds processes being executed. Some processes may remain here
		/// after an update, indicating that they did voluntarily yield.
		/// </summary>
		public Queue<Process> executeQueue=new Queue<Process>();

		Stopwatch stopwatch = new Stopwatch();

		void AddNonSleepingToExecuteQueue()
		{
			for (int i = waitQueue.Count; i-- > 0;)
			{
				var process=waitQueue.Dequeue();
				if (process.executable.IsSleeping)
				{
					waitQueue.Enqueue(process);
				}
				else
				{
					executeQueue.Enqueue(process);
				}
			}
		}

		ExecStatus TryExecute(Process process,long ticks)
		{
			try
			{
				long start=stopwatch.ElapsedTicks;
				var status=process.executable.Execute(ticks);
				long end=stopwatch.ElapsedTicks;
				return status;
			}
			catch (Exception e)
			{
				process.executable.HandleException(e);
				// if there was an exception, don't add the process back into the queue.
				// If they want to gracefully recover from an error they must catch any errors
				// in their Execute function.
				return ExecStatus.TERMINATED;
			}
		}

		public virtual void Execute(long overallTickLimit)
		{
			stopwatch.Reset();
			stopwatch.Start();

			AddNonSleepingToExecuteQueue();

			long remainingTicks = overallTickLimit;
			while (remainingTicks > 0 && executeQueue.Count > 0)
			{
				long perExecuteTickLimit = remainingTicks / executeQueue.Count;

				for (int i = executeQueue.Count; i-- > 0 && remainingTicks > 0;)
				{
					var process = executeQueue.Dequeue();

					var status = TryExecute(process,perExecuteTickLimit);

					if (status == ExecStatus.YIELDED)
					{
						waitQueue.Enqueue(process);
					}
					else if (status == ExecStatus.INTERRUPTED)
					{
						executeQueue.Enqueue(process);
					}

					remainingTicks = overallTickLimit - stopwatch.ElapsedTicks;
				}
			}

			stopwatch.Stop();
		}
	}
}
