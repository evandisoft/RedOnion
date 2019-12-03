using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MunOS.Core.Executors
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
		public int Count => waitQueue.Count+executeQueue.Count;
		/// <summary>
		/// Holds processes waiting to be executed. Each update the processes 
		/// here that are not sleeping are put into the executeQueue
		/// </summary>
		public Queue<ExecInfo> waitQueue=new Queue<ExecInfo>();
		/// <summary>
		/// Holds processes being executed. Some processes may remain here
		/// after an update, indicating that they didn't voluntarily yield or they
		/// didn't even get a chance to execute at all last update.
		/// </summary>
		public Queue<ExecInfo> executeQueue=new Queue<ExecInfo>();

		Stopwatch stopwatch = new Stopwatch();

		public void Kill(long ID)
		{
			Kill(ID, waitQueue);
			Kill(ID, executeQueue);
		}

		void Kill(long ID, Queue<ExecInfo> execInfos)
		{
			for (int i = execInfos.Count; i > 0; i--)
			{
				var execInfo=execInfos.Dequeue();

				if (execInfo.ID!=ID)
				{
					execInfos.Enqueue(execInfo);
				}
			}
		}

		void AddNonSleepingExecutablesToExecuteQueue()
		{
			for (int i = waitQueue.Count; i > 0; i--)
			{
				var execInfo=waitQueue.Dequeue();

				if (execInfo.executable.IsSleeping)
				{
					waitQueue.Enqueue(execInfo);
				}
				else
				{
					executeQueue.Enqueue(execInfo);
				}
			}
		}

		protected void ExecuteExecutable(ExecInfo execInfo,long ticks)
		{
			try
			{
				long start=stopwatch.ElapsedTicks;
				var status=execInfo.executable.Execute(ticks);
				long end=stopwatch.ElapsedTicks;
				// EvanTODO: Can record stats with end-start

				switch (status)
				{
				case ExecStatus.FINISHED:
					// if finished, don't put the process back on a queue. Remove
					// it from the process dictionary
					ExecutionManager.Instance.Remove(execInfo.ID);
					break;
				case ExecStatus.INTERRUPTED:
					// if it was interrupted, put it at the back of the queue. it may get more time to execute
					// this update if other processes finish or yield early.
					executeQueue.Enqueue(execInfo);
					break;
				case ExecStatus.YIELDED:
					// if it yielded voluntarily, put it back in the waitQueue to wait for the next
					// update
					waitQueue.Enqueue(execInfo);
					break;
				default:
					throw new NotSupportedException("ExecStatus "+status+" not supported.");
				}
			}
			catch (Exception e)
			{
				ExecutionManager.Instance.Remove(execInfo.ID);
				// If there was an exception don't add the process back into the queue.
				// Remove it from the process dictionary
				// If they want to handle exceptions gracefully, they can surround their
				// code in a try-catch.
				execInfo.executable.HandleException(execInfo.name, execInfo.ID, e);
			}
		}

		public virtual void Execute(long tickLimit)
		{
			stopwatch.Reset();
			stopwatch.Start();

			AddNonSleepingExecutablesToExecuteQueue();

			long remainingTicks = tickLimit;
			while (remainingTicks > 0 && executeQueue.Count > 0)
			{
				long perExecuteTickLimit = remainingTicks / executeQueue.Count;

				for (int i = executeQueue.Count; i > 0 && remainingTicks > 0; i--)
				{
					ExecuteExecutable(executeQueue.Dequeue(),remainingTicks);

					remainingTicks = tickLimit - stopwatch.ElapsedTicks;
				}
			}

			stopwatch.Stop();
		}
	}
}
