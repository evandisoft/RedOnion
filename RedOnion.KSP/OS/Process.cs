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
		/// </remarks>
		public static Process current { get; internal set; }

		/// <summary>
		/// Unique ID of the process.
		/// </summary>
		public uint id { get; }
		public Process() => id = ++id_counter;
		protected static uint id_counter;

		/// <summary>
		/// Process terminated.
		/// Subscribers can use <see cref="ShutdownHook{T}" /> to avoid hard-links.
		/// </summary>
		public Action shutdown;

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
			}
			else
			{//	this should really never happen (processor calls process.Dispose on reset/shutdown)
				Value.Log("Process #{0} is being collected with active shutdown subscribers!");
			//	we at least try to notify the subscribers for cleanup and rather schedule it on main/ui thread
				UI.Collector.Add(this);
			}
		}

		public class ShutdownHook : ShutdownHook<IDisposable>
		{
			public ShutdownHook(IDisposable target) : base(target) { }
		}
		/// <summary>
		/// Used to subscribe to Shutdown but avoid direct hard-link
		/// so that the target/subscriber can be garbage-collected.
		/// </summary>
		/// <remarks>
		/// The target/subscriber should dispose this object in its own Dispose() method.
		/// </remarks>
		/// <typeparam name="T"></typeparam>
		public class ShutdownHook<T> : IDisposable
			where T : class, IDisposable
		{
			protected WeakReference<T> _target;
			public Process process { get; protected set; }

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
				T target = null;
				if (_target?.TryGetTarget(out target) == true)
					target.Dispose();
				Dispose();
			}
		}
	}
}
