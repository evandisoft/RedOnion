using System;
using System.Collections.Generic;
using System.Diagnostics;
using MunOS.Core;
using MunOS.Core.Executors;

namespace MunOS.ProcessLayer
{
	public abstract class MunThread : IExecutable
	{
		static long nextID=0;

		/// <summary>
		/// This will be used to time Executions.
		/// This should be reset every fixed update.
		/// </summary>
		static Stopwatch stopwatch;
		static MunThread()
		{
			stopwatch=new Stopwatch();
			stopwatch.Start(); 
		}
		/// <summary>
		/// Should be called every update to reset the timer.
		/// </summary>
		public static void ResetStopwatch()
		{
			stopwatch.Reset();
			stopwatch.Start();
		}


		/// <summary>
		/// Before executing, a <see cref="MunThread"/> sets this to itself so that 
		/// calls made in the script can be linked to this thread.
		/// </summary>
		public static MunThread ExecutingThread { get; private set; }

		public readonly MunProcess parentProcess;
		public string Name { get; set; } = "";

		protected MunThread(MunProcess parentProcess)
		{
			this.parentProcess=parentProcess;
		}

		public readonly long ID=nextID++; 

		public abstract bool IsSleeping { get; }
		/// <summary>
		/// These queues are to store performance information of the last 10
		/// updates. Since this might not run every update, we have to 
		/// keep track of which update number corresponds to which tick measure.
		/// UpdateNumbers will be provided by some class that runs every update.
		/// UpdateNumbers corresponding to a tick measure will be stored 
		/// in nUpdatesQueue, while the corresponding tick
		/// measures will be in perfQueue in the same order.
		/// </summary>
		Queue<long> perfQueue=new Queue<long>();
		/// <summary>
		/// To store which update number corresponds to the same position in the 
		/// perfQueue
		/// </summary>
		Queue<long> nUpdatesQueue=new Queue<long>();

		public ExecStatus Execute(long tickLimit)
		{
			ExecStatus status;
			ExecutingThread=this;
			try
			{
				status=ProtectedExecute(tickLimit);
			}
			catch(Exception)
			{
				// On an exception, throw it to CoreExecMgr so that it will call 
				// HandleException. But first set the current MunProcess to null
				// as we are supposed to do.
				throw;
			}
			finally
			{
				ExecutingThread=null;
			}


			if (status==ExecStatus.FINISHED)
			{
				ExecutionComplete.Invoke(this, null);
			}

			return status;
		}

		protected abstract ExecStatus ProtectedExecute(long tickLimit);

		public void HandleException(Exception e)
		{
			ExecutionComplete.Invoke(this,e);
		}
		public void OnTerminated()
		{
			ExecutionComplete.Invoke(this,null);
		}

		public override string ToString()
		{
			if (Name =="")
			{
				return ID.ToString();
			}
			return Name +","+ID;
		}

		/// <summary>
		/// For a normal termination, the exception is null;
		/// </summary>
		public event Action<MunThread,Exception> ExecutionComplete;
	}
}
