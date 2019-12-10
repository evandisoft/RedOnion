using System;
using System.Collections.Generic;
using System.Linq;
using MunOS.Core;
using static MunOS.Debugging.QueueLogger;

namespace MunOS.ProcessLayer
{
	/// <summary>
	/// A <see cref="MunProcess"/> manages a number of <see cref="MunThread"/>s. It has a list of running threads and
	/// receives notice from a <see cref="MunThread"/> any time it completes execution, whereby the <see cref="MunProcess"/>
	/// removes it from the list of running threads, but not necessarily from the total list of threads this process manages.
	/// In the future, this class may implement functionality to have a thread wait on the completion of another thread, or another
	/// process.
	/// </summary>
	public abstract class MunProcess : IDisposable
	{
		static long nextID=1;
		public readonly long ID=nextID++;

		public string Name { get; set; } = "";
		protected MunProcess()
		{
		}

		public readonly ProcessOutputBuffer outputBuffer=new ProcessOutputBuffer();
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

		public long ExecuteThread(ExecPriority priority, MunThread thread)
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
			if (e!=null)
			{
				outputBuffer.AddError(e.Message);
			}

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
			UpdatePhysics();
		}

		public virtual void Update()
		{

		}

		public virtual void Terminate()
		{
			//MunLogger.Log("Before munprocess terminate");

			// each thread will be removed from the list immediatly after
			// being killed.
			var ids=runningThreads.Values.ToList();
			foreach(var id in ids)
			{
				CoreExecMgr.Instance.Kill(id);
			}

			//MunLogger.Log("after munprocess terminate");
		}



		/// <summary>
		/// Process terminated.
		/// Subscribers can use <see cref="ShutdownHook{T}" /> to avoid hard-links.
		/// All subscriptions are removed prior to executing the handlers.
		/// </summary>
		public Action shutdown;

		/// <summary>
		/// Event invoked on every physics update (Unity FixedUpdate).
		/// </summary>
		public event Action physicsUpdate;

		/// <summary>
		/// To be called every physics tick (Unity: FixedUpdate)
		/// after the execution of current script and all async events.
		/// </summary>
		public void UpdatePhysics()
		{
			// other updates (e.g. vector drawing)
			var physics = physicsUpdate;
			if (physics != null)
			{
				foreach (Action handler in physics.GetInvocationList())
				{
					try
					{
						handler();
					}
					catch //(Exception ex)
					{
						physicsUpdate -= handler;
						//TODO: print the exception
					}
				}
			}
		}

		/// <summary>
		/// Terminate this process.
		/// </summary>
		/// <remarks>
		/// All scripting engines should themselves subscribe to <see cref="shutdown"/>
		/// and terminate when process is terminated/disposed.
		/// Should also terminate/dispose the process if the engine is reset.
		/// </remarks>
		public void terminate()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}
		~MunProcess() => Dispose(false);
		void IDisposable.Dispose() => terminate();
		protected virtual void Dispose(bool disposing)
		{
			var shutdown = this.shutdown;
			if (shutdown == null)
				return;
			if (disposing)
			{
				// clearing the list also prevents recursion when processor is itself hooked and calls terminate()
				this.shutdown = null;
				foreach (var fn in shutdown.GetInvocationList())
				{
					try
					{
						fn.DynamicInvoke();
					}
					catch (Exception ex)
					{
						MunLogger.Log($"Exception in process #{ID} shutdown: {ex.Message}");
					}
				}
				MunLogger.Log("Process #{ID} terminated.");
			}
			else
			{//	this should really never happen (processor calls process.Dispose on reset/shutdown)
				MunLogger.Log($"Process #{ID} is being collected with active shutdown subscribers!");
				//	we at least try to notify the subscribers for cleanup and rather schedule it on main/ui thread
				// EvanTodo: commented this out but probably is necessary
				throw new NotImplementedException("Evan commented this out but it needs firda's attention");
				//UI.Collector.Add(this);
			}
		}

		/// <summary>
		/// Used to subscribe to <see cref="shutdown"/> but avoid direct hard-link
		/// so that the target/subscriber can be garbage-collected.
		/// </summary>
		/// <remarks>
		/// The target/subscriber should dispose this object in its own Dispose() method.
		/// </remarks>
		public class ShutdownHook : ShutdownHook<IDisposable>
		{
			public ShutdownHook(IDisposable target) : base(target) { }
		}
		/// <summary>
		/// Used to subscribe to <see cref="shutdown"/> but avoid direct hard-link
		/// so that the target/subscriber can be garbage-collected.
		/// </summary>
		/// <remarks>
		/// The target/subscriber should dispose this object in its own Dispose() method.
		/// </remarks>
		public class ShutdownHook<T> : IDisposable
			where T : class, IDisposable
		{
			protected WeakReference<T> _target;
			public MunProcess process { get; protected set; }
			public T Target
			{
				get
				{
					T target = null;
					return _target?.TryGetTarget(out target) == true ? target : null;
				}
			}

			public ShutdownHook(T target)
			{
				_target = new WeakReference<T>(target);
				process = MunThread.ExecutingThread.parentProcess;
				process.shutdown += Shutdown;
			}
			~ShutdownHook() => Dispose(false);
			public void Dispose()
			{
				GC.SuppressFinalize(this);
				Dispose(true);
			}
			protected virtual void Dispose(bool disposing)
			{
				if (process == null)
					return;
				process.shutdown -= Shutdown;
				process = null;
				_target = null;

			}
			protected virtual void Shutdown()
			{
				Target?.Dispose();
				Dispose();
			}
		}
	}
}
