using System;
using System.Collections.Generic;
using System.Diagnostics;
using MunOS.Executors;
using static RedOnion.Debugging.QueueLogger;

namespace MunOS
{
	//TODO: some multi-core manager that can assign time slices to cores

	/// <summary>
	/// Manages execution of threads among 4 priority levels.
	/// </summary>
	public class MunCore
	{
		#region Static

		/// <summary>
		/// Default execution engine.
		/// </summary>
		public static MunCore Default { get; }
		/// <summary>
		/// Current execution engine (null if not inside any method of this class).
		/// </summary>
		public static MunCore Current { get; protected set; }

		/// <summary>
		/// Clock ticks used to measure time. Returns <see cref="Stopwatch.GetTimestamp()"/>.
		/// Note that it may overflow, therefore should only be used in comparisions like
		/// <code>var start = Ticks; while((Ticks - start) &lt; limit) ...</code>
		/// </summary>
		public static long Ticks => Stopwatch.GetTimestamp();
		/// <summary>
		/// Number of ticks in one second. Returns <see cref="Stopwatch.Frequency"/>.
		/// </summary>
		public static readonly long Frequency = Stopwatch.Frequency;
		/// <summary>
		/// Number of ticks in one microsecond. <see cref="Frequency"/> / 1_000_000.
		/// </summary>
		public static readonly double TicksPerMicro = Frequency / 1_000_000.0;

		/// <summary>
		/// Default time limit for one update (2ms = 2 * Frequency / 1000).
		/// </summary>
		public static long DefaultUpdateTicks = 2 * Frequency / 1000; // 2ms

		/// <summary>
		/// Default maximum number of updates when no one-shot handler was executed (if there was any).
		/// </summary>
		public static int DefaultMaxIdleSkips = 9;
		/// <summary>
		/// Default maximum number of updates when no idle handler was executed (if there was any).
		/// </summary>
		public static int DefaultMaxCallbackSkips = 1;

		/// <summary>
		/// Realtime threads will be executing until this fraction of the time assigned is reached (or all done).
		/// 50% by default.
		/// </summary>
		public static double DefaultRealtimeLimit = 0.5;
		/// <summary>
		/// Callbacks will be executing until this fraction of the time assigned is reached (or all done).
		/// 60% by default (thus 10% is reserved for callbacks even if realtime threads use all their time,
		/// but it is possible that they use even more, so at least one callback gets executed every other update).
		/// </summary>
		public static double DefaultCallbackLimit = 0.6;
		/// <summary>
		/// Idle threads will be executing until this fraction of the time assigned is reached (or all done).
		/// 40% by default (may therefore not be executed on high load, but at least one every 10th update is).
		/// </summary>
		public static double DefaultIdleLimit = 0.4;

		/// <summary>
		/// Default minimal execution time for realtime threads when they get any time.
		/// </summary>
		public static long DefaultMinRealtimeTicks = Frequency / 100_000L; // 10us
		/// <summary>
		/// Default minimal execution time for callbacks when they get any time.
		/// </summary>
		public static long DefaultMinCallbackTicks = Frequency / 100_000L; // 10us
		/// <summary>
		/// Default minimal execution time idle threds when they get any time.
		/// </summary>
		public static long DefaultMinIdleTicks = Frequency / 100_000L; // 10us
		/// <summary>
		/// Default minimal execution time for main threads when they get any time.
		/// </summary>
		public static long DefaultMinMainTicks = Frequency / 50_000L; // 20us

		static MunCore()
		{
			Default = new MunCore();
		}

		#endregion

		#region Instance

		/// <summary>
		/// Event executed on unexpected exceptions.
		/// </summary>
		public event Action<MunEvent> Error;

		protected internal virtual void OnError(MunEvent err)
		{
			MunLogger.DebugLog("MunCore.OnError: " + err.ToString());
			Error?.Invoke(err);
		}

		/// <summary>
		/// Time limit for one update (2ms = 2 * Frequency / 1000).
		/// </summary>
		public long UpdateTicks = DefaultUpdateTicks;

		/// <summary>
		/// Maximum number of updates when no one-shot handler was executed (if there was any).
		/// </summary>
		public int MaxIdleSkips
		{
			get => idle.maxSkips;
			set => idle.maxSkips = value;
		}
		/// <summary>
		/// Maximum number of updates when no idle handler was executed (if there was any).
		/// </summary>
		public int MaxCallbackSkips
		{
			get => callback.maxSkips;
			set => callback.maxSkips = value;
		}

		/// <summary>
		/// Realtime threads will be executing until this fraction of the time assigned is reached (or all done).
		/// 50% by default.
		/// </summary>
		public double RealtimeLimit
		{
			get => realtime.limit;
			set => realtime.limit = value;
		}
		/// <summary>
		/// Callbacks will be executing until this fraction of the time assigned is reached (or all done).
		/// 60% by default (thus 10% is reserved for callbacks even if realtime threads use all their time,
		/// but it is possible that they use even more, so at least one callback gets executed every other update).
		/// </summary>
		public double CallbackLimit
		{
			get => callback.limit;
			set => callback.limit = value;
		}
		/// <summary>
		/// Idle threads will be executing until this fraction of the time assigned is reached (or all done).
		/// 40% by default (may therefore not be executed on high load, but at least one every 10th update is).
		/// </summary>
		public double IdleLimit
		{
			get => idle.limit;
			set => idle.limit = value;
		}

		/// <summary>
		/// Minimal execution time for realtime threads when they get any time.
		/// </summary>
		public long RealtimeMinTicks
		{
			get => realtime.minTicks;
			set => realtime.minTicks = value;
		}
		/// <summary>
		/// Minimal execution time for callbacks when they get any time.
		/// </summary>
		public long CallbackMinTicks
		{
			get => callback.minTicks;
			set => callback.minTicks = value;
		}
		/// <summary>
		/// Minimal execution time idle threds when they get any time.
		/// </summary>
		public long IdleMinTicks
		{
			get => idle.minTicks;
			set => idle.minTicks = value;
		}
		/// <summary>
		/// Minimal execution time for main threads when they get any time.
		/// </summary>
		public long MainMinTicks
		{
			get => main.minTicks;
			set => main.minTicks = value;
		}

		// may add finalizing executor for soft-terminated threads that still need to perform some cleanup (finally-blocks)
		// and maybe also something in between callback and idle (which could in fact be used for finalizers as well).
		protected readonly MunSleepingExecutor sleeping;
		protected readonly MunRealtimeExecutor realtime;
		protected readonly MunCallbackExecutor callback;
		protected readonly MunIdleExecutor idle;
		protected readonly MunMainExecutor main;
		protected readonly Dictionary<MunPriority, MunExecutor> priorities = new Dictionary<MunPriority, MunExecutor>();
		protected MunExecutor[] executors;

		protected internal readonly Dictionary<MunID, MunProcess> processes = new Dictionary<MunID, MunProcess>();
		protected internal readonly Dictionary<MunID, MunThread> threads = new Dictionary<MunID, MunThread>();

		// access only under lock
		protected readonly List<MunID> asyncKillList = new List<MunID>();
		// can be set to true asynchronously (GC finalizers)
		protected bool asyncKillActive;

		public MunCore()
		{
			// note that they get executed in that order
			executors = new MunExecutor[]
			{
				// sleeping threads are not really executed, but get a chance to change their state
				sleeping = new MunSleepingExecutor(this),
				// realtime threads are high priority and can use upto 50% of the time assigned
				priorities[MunPriority.Realtime] = realtime = new MunRealtimeExecutor(this),
				// callbacks are mostly native events like Button.Click, but can be used to execute something later
				priorities[MunPriority.Callback] = callback = new MunCallbackExecutor(this),
				// idle threads are executed on low load and we may need to add one higher priority for some things
				priorities[MunPriority.Idle] = idle = new MunIdleExecutor(this),
				// main threads are executed last and can use all the remaining time (which should be at least 50%)
				priorities[MunPriority.Main] = main = new MunMainExecutor(this)
			};
		}

		protected internal virtual MunExecutor Schedule(MunThread thread)
		{
			if (thread.Status.IsFinal())
			{
				Kill(thread, hard: true);
				return null;
			}
			threads[thread.ID] = thread;
			var executor = thread.Status == MunStatus.Sleeping ?
				sleeping : priorities[thread.Priority];
			executor.Add(thread);
			return executor;
		}

		/// <summary>
		/// Number of updates (incremented at the end of <see cref="FixedUpdate"/>).
		/// </summary>
		public ulong UpdateCounter { get; protected set; }

		/// <summary>
		/// To be called every physics update (Unity: FixedUpdate).
		/// </summary>
		public void FixedUpdate()
		{
			try
			{
				Current = this;
				Execute();
			}
			catch (Exception ex)
			{
				var err = new MunEvent(this, ex);
				OnError(err);
				if (!err.Handled)
					throw;
			}
			finally
			{
				Current = null;
				UpdateCounter++;
			}
		}

		/// <summary>
		/// Kill thread asynchronously. To be used from finalizers (or any other thread).
		/// </summary>
		/// <param name="ID"></param>
		public void KillAsync(MunID id)
		{
			lock (asyncKillList)
				asyncKillList.Add(id);
			asyncKillActive = true;
		}

		protected virtual void Execute()
		{
			var startTicks = Ticks;

			if (asyncKillActive)
			{
				// keep that order, because `asyncKillActive` can be set from different thread (GC - finalizers)
				asyncKillActive = false;
				MunID[] asyncKill = null;
				lock (asyncKillList)
				{
					if (asyncKillList.Count > 0)
					{
						asyncKill = asyncKillList.ToArray();
						asyncKillList.Clear();
					}
				}
				if (asyncKill != null)
				{
					foreach (var id in asyncKill)
						Kill(id);
				}
			}

			// first execute the scripts
			foreach (var executor in executors)
				executor.Execute(startTicks);

			// do the updates last (e.g. vector.draw - uses the state created by the scripts)
			foreach (var process in processes.Values)
				process.FixedUpdate();
		}

		public void Kill(MunThread thread, bool hard = false)
		{
			try
			{
				if (!thread.Status.IsFinal())
				{
					thread.Status = hard ? MunStatus.Terminated : MunStatus.Terminating;
					if (!hard)
						thread.OnTerminating();
				}
				if (thread.Status.IsFinal() && thread.Executor != null)
				{
					thread.RemoveFromExecutor();
					thread.OnDone();
				}
				// else ... maybe move to some cleanup executor
			}
			catch (Exception ex)
			{
				var err = new MunEvent(this, ex);
				thread.OnError(err);
				if (!err.Handled)
				{
					OnError(err);
					if (!err.Handled)
						throw;
				}
			}
		}
		public void Kill(MunProcess process, bool hard = false)
		{
			process.Terminate(hard);
		}

		public bool Kill(MunID id, bool hard = false)
		{
			var process = GetProcess(id);
			if (process != null)
			{
				Kill(process, hard);
				return true;
			}
			var thread = GetThread(id);
			if (thread != null)
			{
				Kill(thread, hard);
				return true;
			}
			return false;
		}

		public MunProcess GetProcess(MunID id)
			=> processes.TryGetValue(id, out var process) ? process : null;
		public MunThread GetThread(MunID id)
			=> threads.TryGetValue(id, out var thread) ? thread : null;

		public bool Contains(MunID id)
			=> processes.ContainsKey(id) || threads.ContainsKey(id);
		public bool ContainsProcess(MunID id)
			=> processes.ContainsKey(id);
		public bool ContainsThread(MunID id)
			=> threads.ContainsKey(id);

		public int Count
		{
			get
			{
				int count = 0;
				foreach (var executor in executors)
					count += executor.Count;
				return count;
			}
		}

		#endregion
	}
}
