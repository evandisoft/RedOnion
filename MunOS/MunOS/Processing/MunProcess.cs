using System;
using System.Collections.Generic;
using MunOS.Core;

namespace MunOS.Processing
{
	public abstract class MunProcess
	{
		static long nextID=0;
		public readonly long ID=nextID++;

		public readonly string name;
		protected MunProcess(string name="")
		{
			this.name=name;
		}

		protected ProcessOutputBuffer outputBuffer=new ProcessOutputBuffer();
		/// <summary>
		/// A mapping of running threads to <see cref="ExecInfo"/> ID's. Distinct from a thread ID.
		/// Threads can exist outside of <see cref="CoreExecMgr"/>, but these IDs are only valid between
		/// the time an IExecutable was added to <see cref="CoreExecMgr"/> and the time it was removed.
		/// Adding the same IExecutable to <see cref="CoreExecMgr"/> again leads to a different ID.
		/// Basically these IDs can be used to kill the corresponding threads execution and should not be used for anything else. 
		/// Threads will be removed from this dictionary by MunProcessThreadExecutionComplete, which will be fired whenever
		/// a MunThread completes execution.
		/// </summary>
		protected Dictionary<MunThread,long> runningThreads=new Dictionary<MunThread,long>();

		public int Count => runningThreads.Count;

		public long RunThread(CoreExecMgr.Priority priority, MunThread thread)
		{
			long RunningExecutableID=CoreExecMgr.Instance.RegisterExecutable(priority, thread);
			runningThreads[thread]=RunningExecutableID;
			thread.ExecutionComplete+=MunProcessThreadExecutionComplete;
			return RunningExecutableID;
		}

		void MunProcessThreadExecutionComplete(MunThread thread,Exception e)
		{
			runningThreads.Remove(thread);
			ThreadExecutionComplete(thread, e);
		}

		/// <summary>
		/// Called when a thread completes execution. The parameter e is only non-null if
		/// execution of the thread's Execute function threw an Exception to the <see cref="CoreExecMgr"/>
		/// </summary>
		/// <param name="thread">The thread for which execution completed.</param>
		/// <param name="e">The exception that was thrown. Null if the thread completed normally or was terminated.</param>
		protected abstract void ThreadExecutionComplete(MunThread thread, Exception e);

		public virtual void FixedUpdate()
		{

		}

		public virtual void Update()
		{

		}

		public virtual void Terminate()
		{

		}
	}
}
