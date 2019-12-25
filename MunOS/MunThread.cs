using System;
using System.Collections.Generic;
using System.Diagnostics;
using MunOS.Executors;
using MunOS.Repl;
using static RedOnion.Debugging.QueueLogger;

namespace MunOS
{
	//TODO: count how many ticks this thread consumed in some defined number of updates
	//..... implement as cyclic buffer and keep counter of updates in MunCore,
	//..... so that we can insert zeros for skipped updates. (Maybe add another level - 10x10)

	/// <summary>
	/// Executable code - script, function or native.
	/// </summary>
	public abstract class MunThread
	{
		public static MunThread Current { get; protected internal set; }
		public static MunID CurrentID => Current?.ID ?? MunID.Zero;

		protected internal abstract MunStatus Execute(long tickLimit);

		protected internal virtual void OnError(MunEvent err) { }
		protected internal virtual void OnRestart() { }
		protected internal virtual void OnTerminating()
			=> Status = MunStatus.Terminated;

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
		public MunStatus Status { get; internal set; }
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

		public override string ToString()
			=> string.IsNullOrWhiteSpace(Name) ? ID.ToString() : Name;
	}
}
