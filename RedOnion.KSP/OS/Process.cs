using RedOnion.ROS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Kerbalua")]

namespace RedOnion.KSP.OS
{
	/// <summary>
	/// Process is one instance of a script with all its functions subscribed to update
	/// (system.update/idle/once in ROS, one process per processor, function/script per core).
	/// </summary>
	public class Process : IDisposable
	{
		/// <summary>
		/// Currently running process.
		/// </summary>
		/// <remarks>
		/// All scripting engines are to update this property at the start of their Execute method.
		/// (And set it to null when finished with current execute/update.)
		/// </remarks>
		public static Process current { get; internal set; }

		/// <summary>
		/// ID of current process (or zero if not set).
		/// </summary>
		public static ulong currentId => current?.id ?? 0;

		/// <summary>
		/// Unique ID of the process.
		/// </summary>
		public ulong id { get; }
		public Process()
		{
			id = ++id_counter;
			Value.DebugLog("Process #{0} created.", id);
		}
		protected static ulong id_counter;

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
		~Process() => Dispose(false);
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
						Value.Log("Exception in process #{0} shutdown: {1}", id, ex.Message);
					}
				}
				Value.DebugLog("Process #{0} terminated.", id);
			}
			else
			{//	this should really never happen (processor calls process.Dispose on reset/shutdown)
				Value.Log("Process #{0} is being collected with active shutdown subscribers!");
			//	we at least try to notify the subscribers for cleanup and rather schedule it on main/ui thread
				UI.Collector.Add(this);
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
			public Process process { get; protected set; }
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
				process = current;
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
