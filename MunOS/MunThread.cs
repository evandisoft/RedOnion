using System;
using System.Collections.Generic;
using System.Diagnostics;
using MunOS.Executors;
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
		public static MunThread Current => MunCore.Current?.Thread;
		public static MunID CurrentID => Current?.ID ?? MunID.Zero;

		public MunID ID { get; internal set; }
		public string Name { get; set; }
		public MunPriority Priority { get; }
		public MunStatus Status { get; internal set; }
		public Exception Exception { get; protected internal set; }
		public MunCore Core { get; }
		public MunExecutor Executor { get; internal set; }
		internal MunExecutor.Links _executorLinks;
		internal void RemoveFromExecutor()
		{
			_executorLinks.Remove(this);
			OnDone();
		}

		/// <summary>
		/// Process this thread belongs to. Can be null.
		/// </summary>
		public MunProcess Process { get; protected internal set; }

		/// <summary>
		/// Execute that thread after this one finishes.
		/// Can point to itself for self-restarting threads,
		/// but other cycles are not allowed.
		/// </summary>
		/// <remarks>
		/// Realtime and Idle threads usually point to themselves,
		/// Main can point to another main to create a chain
		/// (the last being the actual script, the previous being init scripts).
		/// </remarks>
		public MunThread NextThread { get; protected internal set; }

		bool isBackground;
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

		protected MunThread(MunCore core, MunProcess process, MunPriority priority, string name)
		{
			ID = priority == MunPriority.Callback ? MunID.GetNegative() : MunID.GetPositive();
			Name = name ?? ID.ToString();
			Priority = priority;
			Status = MunStatus.Incomplete;
			Core = core;
			if (process != null)
			{
				Process = process;
				process.Add(this);
			}
			Executor = core.Schedule(this);
		}

		public override string ToString()
			=> string.IsNullOrWhiteSpace(Name) ? ID.ToString() : Name;

		protected internal abstract MunStatus Execute(long tickLimit);

		protected internal virtual void OnError(MunEvent err) { }
		protected internal virtual void OnRestart() { }
		protected internal virtual void OnDone() => Process?.OnThreadDone(this);

		public void Kill(bool hard = false)
		{
			if (!Status.IsFinal())
				Core.Kill(this, hard);
		}
		public void KillAsync()
		{
			if (!Status.IsFinal())
				Core.KillAsync(ID);
		}
	}
}
