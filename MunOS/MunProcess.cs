using MunOS.Repl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static RedOnion.Debugging.QueueLogger;

namespace MunOS
{
	public abstract class MunProcess : ICollection<MunThread>
	{
		public static MunProcess Current => MunCore.Current?.Process;
		public static MunID CurrentID => MunCore.Current?.Process?.ID ?? MunID.Zero;

		public MunID ID { get; internal set; }
		public string Name { get; set; }
		public MunCore Core { get; }

		/// <summary>
		/// Automatically remove the process from the core when it is terminated.
		/// Set to false if you want to reuse the process - but you have to remove it yourself.
		/// Note that adding any background thread when there is no foreground thread
		/// will immediately terminate the process and disconnect it from the core if AutoRemove is true.
		/// </summary>
		public bool AutoRemove { get; set; } = true;

		/// <summary>
		/// This is used by REPL and may be null (but used if assigned).
		/// </summary>
		public OutputBuffer OutputBuffer { get; set; }

		protected MunProcess(MunCore core, string name)
		{
			ID = MunID.GetPositive();
			Name = name ?? ID.ToString();
			Core = core;
		}

		protected readonly Dictionary<MunID, MunThread> threads = new Dictionary<MunID, MunThread>();
		public int Count => threads.Count;
		public int ForegroundCount { get; protected internal set; }
		public int BackgroundCount { get; protected internal set; }
		bool ICollection<MunThread>.IsReadOnly => false;

		public IEnumerator<MunThread> GetEnumerator() => threads.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => threads.Values.GetEnumerator();

		// TODO: this is currently only called from MunThread constructor,
		// but since it is public, consider other usage
		public virtual void Add(MunThread thread)
		{
			if (thread.Process != null)
				throw new InvalidOperationException(thread.Process == this
					? $"Thread {thread} already is in process {thread.Process}"
					: $"Thread {thread} belongs to process {thread.Process}, cannot add to {this}");
			threads.Add(thread.ID, thread);
			if (thread.IsBackground)
				++BackgroundCount;
			else ++ForegroundCount;
			Debug.Assert(Count == ForegroundCount + BackgroundCount);
		}
		// TODO: this is currently only called from OnThreadDone,
		// but since it is public, consider other usage
		public virtual void Remove(MunThread thread)
		{
			if (thread.Process != this)
				throw new InvalidOperationException($"Thread {thread} already is in process {thread.Process}");
			threads.Remove(thread.ID);
			if (thread.IsBackground)
				--BackgroundCount;
			else --ForegroundCount;
			CheckForegroundCount();
		}
		bool ICollection<MunThread>.Remove(MunThread item)
		{
			if (item.Process != this)
				return false;
			Remove(item);
			return true;
		}
		protected internal virtual void OnBackgroundChange(MunThread thread)
		{
			if (thread.IsBackground)
			{
				--ForegroundCount;
				++BackgroundCount;
				CheckForegroundCount();
			}
			else
			{
				++ForegroundCount;
				--BackgroundCount;
			}
		}
		/// <summary>
		/// Terminate all background threads if there is no foreground left.
		/// </summary>
		protected internal virtual void CheckForegroundCount()
		{
			Debug.Assert(Count == ForegroundCount + BackgroundCount);
			if (!AutoRemove)
				return;
			if (BackgroundCount > 0)
				return;
			if (ForegroundCount == 0)
				return;
			Terminate();
		}

		/// <summary>
		/// Every thread notifies the process when it is done executing
		/// (really done executing - finished and not restarted or terminated).
		/// </summary>
		protected internal virtual void OnThreadDone(MunThread thread)
			=> Remove(thread);

		/// <summary>
		/// Event invoked on every physics update (Unity FixedUpdate).
		/// </summary>
		public event Action physicsUpdate;

		/// <summary>
		/// To be called on every fixed update.
		/// </summary>
		public virtual void FixedUpdate()
		{
			var physics = physicsUpdate;
			if (physics != null)
			{
				foreach (Action handler in physics.GetInvocationList())
				{
					try
					{
						handler();
					}
					catch (Exception ex)
					{
						// TODO: hook to Core.OnError and similar
						physicsUpdate -= handler;
						MunLogger.DebugLogArray($"Exception in process #{ID} physics update: {ex.Message}");
					}
				}
			}
		}

		/// <summary>
		/// Event invoked on every graphics update (Unity Update).
		/// </summary>
		public event Action graphicsUpdate;

		public virtual void Update()
		{
			var graphics = graphicsUpdate;
			if (graphics != null)
			{
				foreach (Action handler in graphics.GetInvocationList())
				{
					try
					{
						handler();
					}
					catch (Exception ex)
					{
						// TODO: hook to Core.OnError and similar
						physicsUpdate -= handler;
						MunLogger.DebugLogArray($"Exception in process #{ID} graphics update: {ex.Message}");
					}
				}
			}
		}

		/// <summary>
		/// Process terminated.
		/// Subscribers can use <see cref="ShutdownHook{T}" /> to avoid hard-links.
		/// All subscriptions are removed prior to executing the handlers.
		/// </summary>
		public Action shutdown;

		public virtual void Terminate(bool hard = false)
		{
			// first notify all subscribers that this process is shutting down
			var shutdown = this.shutdown;
			MunLogger.DebugLogArray($"Process ID#{ID} terminating. (shutdown: {shutdown?.GetInvocationList().Length ?? 0})");
			if (shutdown != null)
			{
				this.shutdown = null;
				foreach (var fn in shutdown.GetInvocationList())
				{
					try
					{
						fn.DynamicInvoke();
					}
					catch (Exception ex)
					{
						// TODO: hook to Core.OnError and similar
						MunLogger.DebugLogArray($"Exception in process #{ID} shutdown: {ex.Message}");
					}
				}
			}

			foreach (var thread in threads.Values)
				thread.Kill(hard);

			MunLogger.DebugLogArray($"Process ID#{ID} terminated.");
		}

		void ICollection<MunThread>.Clear() => Terminate();
		bool ICollection<MunThread>.Contains(MunThread thread) => thread.Process == this;
		public void CopyTo(MunThread[] array, int arrayIndex) => threads.Values.CopyTo(array, arrayIndex);

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
				process = Current;
				process.shutdown += Shutdown;
				MunLogger.DebugLogArray($"ShutdownHook for process ID#{process.ID} created.");
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
				var target = Target;
				var tgtstr = target == null ? "target is null" : "disposing target";
				MunLogger.DebugLogArray($"ShutdownHook invoked for process ID#{process.ID}, {tgtstr}.");
				target?.Dispose();
				Dispose();
			}
		}
	}
}
