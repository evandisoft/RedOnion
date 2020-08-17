using MunOS.Repl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static RedOnion.Debugging.QueueLogger;

namespace MunOS
{
	public class MunProcess : ICollection<MunThread>, IDisposable
	{
		private static MunProcess _current;
		public static MunProcess Current
		{
			get => _current ?? MunThread.Current?.Process;
			protected internal set => _current = value;
		}
		public static MunID CurrentID => Current?.ID ?? MunID.Zero;

		public MunID ID { get; }
		public string Name { get; set; }
		public MunCore Core { get; }
		/// <summary>
		/// Next process on same core (cyclic).
		/// </summary>
		public MunProcess Next { get; private set; }
		/// <summary>
		/// Previous process on same core (cyclic).
		/// </summary>
		public MunProcess Prev { get; private set; }
		/// <summary>
		/// First thread of this process.
		/// </summary>
		public MunThread First { get; internal set; }

		/// <summary>
		/// Every thread notifies the process when it is done executing
		/// (really done executing - finished and not restarted or terminated).
		/// Calls <see cref="Remove(MunThread)"/> if the thread has <see cref="MunThread.AutoRemove"/> set.
		/// </summary>
		protected internal virtual void OnThreadDone(MunThread thread)
		{
			ThreadDone?.Invoke(thread);
			if (thread.AutoRemove)
				Remove(thread);
		}
		/// <summary>
		/// Event that is called from <see cref="OnThreadDone"/>.
		/// </summary>
		public event Action<MunThread> ThreadDone;

		/// <summary>
		/// Called from <see cref="MunThread.OnError(MunEvent)"/>.
		/// </summary>
		protected internal virtual void OnError(MunEvent err) { }
		/// <summary>
		/// Called from <see cref="OutputBuffer"/> when this property is changed
		/// (so that script engines can redirect their printing to the buffer).
		/// </summary>
		protected virtual void OnSetOutputBuffer(OutputBuffer value, OutputBuffer prev) { }

		private OutputBuffer _outputBuffer;
		/// <summary>
		/// This is used by REPL and may be null (but used if assigned).
		/// </summary>
		public OutputBuffer OutputBuffer
		{
			get => _outputBuffer;
			set
			{
				if (value == _outputBuffer)
					return;
				var prev = _outputBuffer;
				_outputBuffer = value;
				OnSetOutputBuffer(value, prev);
			}
		}
		/// <summary>
		/// Associated script manager (if any - can be null).
		/// </summary>
		public ScriptManager ScriptManager { get; set; }

		/// <summary>
		/// Automatically remove the process from the core when it is terminated (last foreground thread is terminated).
		/// Set to false if you want to reuse the process - but you have to remove it yourself.
		/// </summary>
		public bool AutoRemove { get; set; } = true;
		/// <summary>
		/// Indicates that process was terminated and detached from the core.
		/// </summary>
		public bool IsDisposed { get; private set; }

		public MunProcess(MunCore core, string name = null)
		{
			ID = MunID.GetPositive();
			if (string.IsNullOrWhiteSpace(name))
				name = ID.ToString();
			Name = name;
			if (core == null)
				core = MunCore.Default;
			Core = core;
			core.processes.Add(ID, this);
			var first = core.FirstProcess;
			if (first == null)
			{
				core.FirstProcess = this;
				Next = this;
				Prev = this;
			}
			else
			{
				Next = first;
				Prev = first.Prev;
				Prev.Next = this;
				first.Prev = this;
			}
		}

		protected readonly Dictionary<MunID, MunThread> threads = new Dictionary<MunID, MunThread>();
		public int Count => threads.Count;
		public int ForegroundCount { get; protected internal set; }
		public int BackgroundCount { get; protected internal set; }
		bool ICollection<MunThread>.IsReadOnly => false;

		/// <summary>
		/// Iterate over all threads. Can remove them while iterating.
		/// </summary>
		public IEnumerator<MunThread> GetEnumerator()
		{
			if (First == null)
				yield break;
			for (var it = First; ;)
			{
				var next = it.Next;
				var last = next == First;
				yield return it;
				if (last) break;
				it = next;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public virtual void Add(MunThread thread)
		{
			if (IsDisposed)
				throw new ObjectDisposedException(string.IsNullOrWhiteSpace(Name)
					? "MunProcess#" + ID : "MunProcess#" + ID + ": " + Name);
			if (thread.Process != null)
				throw new InvalidOperationException(thread.Process == this
					? $"Thread {thread} already is in process {thread.Process}"
					: $"Thread {thread} belongs to process {thread.Process}, cannot add to {this}");
			threads.Add(thread.ID, thread);
			thread.Process = this;
			var first = First;
			if (first == null)
			{
				First = thread;
				thread.Next = thread;
				thread.Prev = thread;
			}
			else
			{
				thread.Next = first;
				thread.Prev = first.Prev;
				first.Prev.Next = thread;
				first.Prev = thread;
			}
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
				throw new InvalidOperationException($"Thread {thread} does not belong to process {this}");
			var hadForeground = ForegroundCount > 0;
			threads.Remove(thread.ID);
			thread.Process = null;
			if (thread.Next == thread)
				First = null;
			else
			{
				if (First == thread)
					First = thread.Next;
				thread.Next.Prev = thread.Prev;
				thread.Prev.Next = thread.Next;
			}
			thread.Next = null;
			thread.Prev = null;
			if (thread.IsBackground)
				--BackgroundCount;
			else --ForegroundCount;
			if (hadForeground)
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
			if (AutoRemove && ForegroundCount == 0)
				Terminate();
		}

		/// <summary>
		/// Event invoked on every physics update (Unity FixedUpdate).
		/// </summary>
		public event Action PhysicsUpdate;

		/// <summary>
		/// To be called on every fixed update.
		/// </summary>
		public virtual void FixedUpdate()
		{
			var physics = PhysicsUpdate;
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
						MunLogger.DebugLog($"Exception in process#{ID} physics update: {ex.Message}");

						var err = new MunEvent(Core, this, ex);
						OnError(err);
						if (!err.Handled)
						{
							Core.OnError(err);
							if (!err.Handled)
								PhysicsUpdate -= handler;
						}

						if (!err.Printed)
							OutputBuffer?.AddError(err.ToString());
					}
				}
			}
		}

		/// <summary>
		/// Event invoked on every graphics update (Unity Update).
		/// </summary>
		public event Action GraphicsUpdate;

		public virtual void Update()
		{
			var graphics = GraphicsUpdate;
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
						MunLogger.DebugLog($"Exception in process#{ID} graphics update: {ex.Message}");

						var err = new MunEvent(Core, this, ex);
						OnError(err);
						if (!err.Handled)
						{
							Core.OnError(err);
							if (!err.Handled)
								GraphicsUpdate -= handler;
						}

						if (!err.Printed)
							OutputBuffer?.AddError(err.ToString());
					}
				}
			}
		}

		/// <summary>
		/// Process terminated.
		/// Subscribers can use <see cref="ShutdownHook{T}" /> to avoid hard-links.
		/// All subscriptions are removed prior to executing the handlers.
		/// This event was optimized for fast removal.
		/// </summary>
		public event Action Shutdown
		{
			add
			{
				if (shutdownDict == null)
				{
					shutdownList = new LinkedList<Action>();
					shutdownDict = new Dictionary<Action, LinkedListNode<Action>>();
				}
				else if (shutdownDict.ContainsKey(value))
					return;
				shutdownDict[value] = shutdownList.AddLast(value);
			}
			remove
			{
				if (shutdownDict == null)
					return;
				if (!shutdownDict.TryGetValue(value, out var node))
					return;
				shutdownDict.Remove(value);
				shutdownList.Remove(node);
			}
		}
		protected Dictionary<Action, LinkedListNode<Action>> shutdownDict;
		protected LinkedList<Action> shutdownList;

		/// <summary>
		/// Process disposed (terminated and cannot be restarted).
		/// </summary>
		public event Action<MunProcess> Disposed;

		/// <summary>
		/// Terminate and dispose the process (cannot be restarted).
		/// </summary>
		public void Dispose()
		{
			if (IsDisposed)
				return;
			IsDisposed = true;
			Terminate(hard: true);
		}

		public virtual void Terminate(bool hard = false)
		{
			if (Next == null)
				return; // already disposed
			if (hard && AutoRemove)
				IsDisposed = true; // let them know in shutdown

			// note that it can be called recursively, see below

			var shutdown = shutdownList;
			if (shutdown != null || threads.Count > 0)
				MunLogger.Log($@"Process#{ID} terminating. (hard: {hard
					}; shutdown: {shutdownDict?.Count ?? 0
					}; threads: {threads.Count})");

			// first notify all subscribers that this process is shutting down
			if (shutdown != null)
			{
				shutdownList = null;
				shutdownDict = null;
				foreach (var fn in shutdown)
				{
					try
					{
						fn.DynamicInvoke();
					}
					catch (Exception ex)
					{
						MunLogger.DebugLog($"Exception in process#{ID} shutdown: {ex.Message}");

						var err = new MunEvent(Core, this, ex);
						OnError(err);
						if (!err.Handled)
							Core.OnError(err);

						if (!err.Printed)
							OutputBuffer?.AddError(err.ToString());
					}
				}
			}

			if (threads.Count > 0)
			{
				var list = new MunThread[threads.Count];
				threads.Values.CopyTo(list, 0);
				foreach (var thread in list)
					thread.Terminate(hard); // last can call Terminate again!
			}
			if (Next == null)
				return; // disposed by last thread above

			if (IsDisposed || (AutoRemove && threads.Count == 0))
			{
				IsDisposed = true;
				Core.processes.Remove(ID);
				if (Next == this)
					Core.FirstProcess = null;
				else
				{
					if (Core.FirstProcess == this)
						Core.FirstProcess = Next;
					Next.Prev = Prev;
					Prev.Next = Next;
				}
				Next = null;
				Prev = null;
				Disposed?.Invoke(this);
			}
			MunLogger.Log($"Process#{ID} {(IsDisposed ? "disposed" : "terminated")} (hard: {hard}).");
		}

		void ICollection<MunThread>.Clear() => Terminate();
		bool ICollection<MunThread>.Contains(MunThread thread) => thread.Process == this;
		public void CopyTo(MunThread[] array, int arrayIndex) => threads.Values.CopyTo(array, arrayIndex);

		public override string ToString()
			=> string.IsNullOrWhiteSpace(Name) ? ID.ToString() : Name;

		/// <summary>
		/// Used to subscribe to <see cref="Shutdown"/> but avoid direct hard-link
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
		/// Used to subscribe to <see cref="Shutdown"/> but avoid direct hard-link
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
				MunLogger.DebugLog($"Creating ShutdownHook for process#{CurrentID} in thread#{MunThread.CurrentID}.");
				_target = new WeakReference<T>(target);
				process = Current;
				process.Shutdown += Shutdown;
			}
			~ShutdownHook() => Dispose(false);
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			protected virtual void Dispose(bool disposing)
			{
				if (process == null)
					return;
				process.Shutdown -= Shutdown;
				process = null;
				_target = null;

			}
			protected virtual void Shutdown()
			{
				var target = Target;
				MunLogger.DebugLog($@"ShutdownHook invoked for process ID#{process.ID}, {(
					target == null ? "target is null" : "disposing target")}.");
				target?.Dispose();
				Dispose();
			}
		}
	}
}
