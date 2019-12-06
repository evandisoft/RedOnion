using System;
using System.Collections.Generic;
using MunOS.Core;

namespace MunOS.Processing
{
	/// <summary>
	/// A <see cref="MunProcess"/> manages a number of <see cref="MunThread"/>s. It has a list of running threads and
	/// receives notice from a <see cref="MunThread"/> any time it completes execution, whereby the <see cref="MunProcess"/>
	/// removes it from the list of running threads, but not necessarily from the total list of threads this process manages.
	/// In the future, this class may implement functionality to have a thread wait on the completion of another thread, or another
	/// process.
	/// </summary>
	public abstract class MunProcess
	{
		static long nextID=0;
		public readonly long ID=nextID++;

		public readonly string name;
		protected MunProcess(string name="")
		{
			this.name=name;
		}

		/// <summary>
		/// While executing, a <see cref="MunThread"/> sets this to its parent process so that threads and special resources
		/// it creates (like VecDraw) can be linked up to its parent process.
		/// </summary>
		public static MunProcess current;

		protected ProcessOutputBuffer outputBuffer=new ProcessOutputBuffer();
		/// <summary>
		/// A mapping of running threads to <see cref="ExecInfo"/> ID's. Distinct from a thread ID.
		/// Threads can exist outside of <see cref="CoreExecMgr"/>, but these <see cref="ExecInfo"/> IDs are only valid between
		/// the time an IExecutable was added to <see cref="CoreExecMgr"/> and the time it was removed.
		/// Adding the same IExecutable to <see cref="CoreExecMgr"/> again leads to a different <see cref="ExecInfo"/> ID.
		/// Basically these IDs can be used to kill the corresponding threads execution and should not be used for anything else. 
		/// Threads will be removed from this dictionary by MunProcessThreadExecutionComplete, which will be fired whenever
		/// a MunThread completes execution.
		/// </summary>
		protected Dictionary<MunThread,long> runningThreads=new Dictionary<MunThread,long>();

		public int RunningThreadsCount => runningThreads.Count;

		public long RunThread(CoreExecMgr.Priority priority, MunThread thread)
		{
			long RunningExecutableID=CoreExecMgr.Instance.RegisterExecutable(priority, thread);
			runningThreads[thread]=RunningExecutableID;
			thread.ExecutionComplete+=MunProcessThreadExecutionComplete;
			return RunningExecutableID;
		}

		void MunProcessThreadExecutionComplete(MunThread thread,Exception e)
		{
			// This thread should always be removed from runningThreads, regarldess
			// of what will be done with the thread because it is no longer running
			// in CoreExecMgr
			runningThreads.Remove(thread);
			ThreadExecutionComplete(thread, e);
		}

		/// <summary>
		/// Called when a <see cref="MunThread"/> completes execution. The parameter e is only non-null if
		/// execution of the thread's <see cref="MunThread.Execute"/> function threw an Exception to the <see cref="CoreExecMgr"/>
		/// </summary>
		/// <param name="thread">The thread for which execution completed.</param>
		/// <param name="e">The exception that was thrown. Null if the thread completed normally or was terminated.</param>
		protected virtual void ThreadExecutionComplete(MunThread thread, Exception e)
		{

		}

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
