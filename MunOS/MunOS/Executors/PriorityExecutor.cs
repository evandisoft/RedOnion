using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MunOS.Executors
{

	public enum ExecStatus
	{
		// Represents that the executable voluntarily yielded
		YIELDED, 
		// represents that the executable was interrupted by an auto-yield or timeout type functionality
		// In otherwords, represents that the executable wanted to do more computations this update but was
		// paused prematurely.
		INTERRUPTED,
		// represents that the executable finished execution and can be removed.
		FINISHED, 
	}
	/// <summary>
	/// Holds all executables of a given priority and can execute them
	/// with the given time limit.
	/// </summary>
	public abstract class PriorityExecutor
	{
		/// <summary>
		/// Holds processes waiting to be executed. Each update the processes 
		/// here that are not sleeping are put into the executeQueue
		/// </summary>
		public Queue<Process> waitQueue=new Queue<Process>();
		/// <summary>
		/// Holds processes being executed. Some processes may remain here
		/// after an update, indicating that they didn't voluntarily yield.
		/// </summary>
		public Queue<Process> executeQueue=new Queue<Process>();

		Stopwatch stopwatch = new Stopwatch();

		void AddNonSleepingToExecuteQueue()
		{
			for (int i = waitQueue.Count; i > 0; i--)
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

		protected void ExecuteProcess(Process process,long ticks)
		{
			try
			{
				long start=stopwatch.ElapsedTicks;
				var status=process.executable.Execute(ticks);
				long end=stopwatch.ElapsedTicks;
				// EvanTODO: Can record stats with end-start

				switch (status)
				{
				case ExecStatus.FINISHED:
					// if finished, don't put the process back on a queue.
					break;
				case ExecStatus.INTERRUPTED:
					// if it was interrupted, put it at the back of the queue. it may get more time to execute
					// this update if other processes finish or yield early.
					executeQueue.Enqueue(process);
					break;
				case ExecStatus.YIELDED:
					// if it yielded voluntarily, put it back in the waitQueue to wait for the next
					// update
					waitQueue.Enqueue(process);
					break;
				default:
					throw new NotSupportedException("ExecStatus "+status+" not supported.");
				}
			}
			catch (Exception e)
			{
				// If there was an exception don't add the process back into the queue.
				// If they want to handle exceptions gracefully, they can surround their
				// code in a try-catch.
				process.executable.HandleException(process.name, process.ID, e);
			}
		}

		public virtual void Execute(long tickLimit)
		{
			stopwatch.Reset();
			stopwatch.Start();

			AddNonSleepingToExecuteQueue();

			long remainingTicks = tickLimit;
			while (remainingTicks > 0 && executeQueue.Count > 0)
			{
				long perExecuteTickLimit = remainingTicks / executeQueue.Count;

				for (int i = executeQueue.Count; i > 0 && remainingTicks > 0; i--)
				{
					ExecuteProcess(executeQueue.Dequeue(),remainingTicks);

					remainingTicks = tickLimit - stopwatch.ElapsedTicks;
				}
			}

			stopwatch.Stop();
		}
	}
}
