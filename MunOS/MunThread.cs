using System;
using System.Collections.Generic;
using System.Diagnostics;
using MunOS.Executors;
using MunOS.Repl;
using static RedOnion.Debugging.QueueLogger;

namespace MunOS
{
	//TODO: count how many ticks this thread consumed in some defined number of updates
	//..... implement as cyclic buffer and use MunCore.Ticks and MunCore.UpdateCounter,
	//..... so that we can insert zeros for skipped updates. (Maybe add another level - 10x10)

	/// <summary>
	/// Executable code - script, function or native.
	/// </summary>
	public abstract class MunThread
	{
		/// <summary>
		/// Currently executing thread (set by <see cref="MunExecutor"/> when calling <see cref="Execute(long)"/>.
		/// </summary>
		public static MunThread Current { get; protected internal set; }
		/// <summary>
		/// <see cref="MunID"/> of currently executing thread or <see cref="MunID.Zero"/> if none.
		/// </summary>
		public static MunID CurrentID => Current?.ID ?? MunID.Zero;

		/// <summary>
		/// This method is called by the <see cref="Executor"/> and represents the work of the thread.
		/// Sleeping threads should only test the condition and return new state (or MunStatus.Sleeping again).
		/// </summary>
		/// <param name="tickLimit">The number of ticks allocated (to be used with <see cref="MunCore.Ticks"/>).</param>
		/// <returns>New state</returns>
		protected internal abstract MunStatus Execute(long tickLimit);

		/// <summary>
		/// Called when some method (e.g. <see cref="Execute(long)"/>) throws exception.
		/// Should call Process?.OnError(err) if not handled (which the base implementation does).
		/// </summary>
		protected internal virtual void OnError(MunEvent err)
			=> Process?.OnError(err);
		/// <summary>
		/// Called when status is changed to <see cref="MunStatus.Terminating"/>
		/// (by <see cref="Execute(long)"/>, <see cref="Terminate(bool)"/> or <see cref="MunCore.Kill(MunThread, bool)"/>).
		/// Base implementation just changes the state to <see cref="MunStatus.Terminated"/>
		/// to signal there is no cleanup needed.
		/// </summary>
		protected internal virtual void OnTerminating()
			=> Status = MunStatus.Terminated;
		/// <summary>
		/// Called when thread is done executing (<see cref="MunStatus.Finished"/> or <see cref="MunStatus.Terminated"/>)
		/// and is not restarted (<see cref="NextThread"/> != this).
		/// Base implementation calls <see cref="Done"/> and <see cref="MunProcess.OnThreadDone(MunThread)"/>.
		/// </summary>
		protected internal virtual void OnDone()
		{
			Done?.Invoke(this);
			Process?.OnThreadDone(this);
		}

		/// <summary>
		/// Unique identifier of the thread.
		/// </summary>
		public MunID ID { get; internal set; }
		/// <summary>
		/// MunCore it is associated with (assigned during creation, shall never be null, cannot be migrated).
		/// </summary>
		public MunCore Core { get; }
		/// <summary>
		/// Name of the thread (can be null).
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Priority assigned during creation (cannot be changed).
		/// </summary>
		/// <remarks>
		/// We may decide to allow changing this in the future.
		/// </remarks>
		public MunPriority Priority { get; }
		/// <summary>
		/// Execution status of this thread.
		/// </summary>
		public MunStatus Status { get; protected internal set; }
		/// <summary>
		/// Exception which was the reason for hard termination (if that happened).
		/// </summary>
		public Exception Exception { get; protected internal set; }
		/// <summary>
		/// To be executed when thread is finished or terminated.
		/// </summary>
		public event Action<MunThread> Done;

		/// <summary>
		/// Current executor (if any - null if not currently executed).
		/// </summary>
		public MunExecutor Executor { get; internal set; }
		internal MunExecutor.Links _executorLinks;
		internal void RemoveFromExecutor() => _executorLinks.Remove(this);

		/// <summary>
		/// Process this thread belongs to. Can be null.
		/// </summary>
		public MunProcess Process { get; protected internal set; }

		/// <summary>
		/// Execute that thread after this one finishes.
		/// Can point to itself for self-restarting threads,
		/// but other cycles are not allowed with <see cref="AutoRemove"/> = true.
		/// </summary>
		/// <remarks>
		/// Realtime and Idle threads usually point to themselves,
		/// Main can point to another main to create a chain
		/// (the last being the actual script, the previous being init scripts).
		/// </remarks>
		public MunThread NextThread { get; set; }

		bool isBackground;
		/// <summary>
		/// Background threads are used to automatically terminate a process,
		/// when its last foreground thread finishes (or is terminated).
		/// </summary>
		public bool IsBackground
		{
			get => isBackground;
			set
			{
				if (isBackground == value)
					return;
				isBackground = value;
				Process.OnBackgroundChange(this);
			}
		}

		/// <summary>
		/// Automatically remove the thread from the core and process when it is terminated.
		/// Set to false if you want to reuse the thread - but you have to remove it yourself.
		/// </summary>
		public bool AutoRemove { get; set; } = true;

		protected MunThread(MunCore core, MunProcess process, MunPriority priority, string name, bool start = true)
		{
			ID = priority == MunPriority.Callback ? MunID.GetNegative() : MunID.GetPositive();
			Name = name ?? ID.ToString();
			Priority = priority;
			Status = MunStatus.Incomplete;
			Core = core;
			if (process != null)
				process.Add(this);
			if (start)
				Executor = core.Schedule(this);
		}

		public void Terminate(bool hard = false)
		{
			if (!Status.IsFinal())
				Core.Kill(this, hard);
		}
		public void KillAsync()
		{
			if (!Status.IsFinal())
				Core.KillAsync(ID);
		}

		/// <summary>
		/// Called when executor is restarting a thread (which has <see cref="NextThread"/> == this).
		/// Designed to do context-reset or whatever the scripting engine may need to do in such situation.
		/// </summary>
		public virtual void Restart()
		{
			Status = MunStatus.Incomplete;
		}

		public override string ToString()
			=> string.IsNullOrWhiteSpace(Name) ? ID.ToString() : Name;
	}
}
