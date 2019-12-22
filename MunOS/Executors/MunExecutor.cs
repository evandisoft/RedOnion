using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MunOS.Executors
{
	/// <summary>
	/// Holds all executables of a given priority and can execute them
	/// with the given time limit.
	/// </summary>
	public abstract class MunExecutor
	{
		/// <summary>
		/// All threads are held in cyclic-doubly-linked list for quick removal.
		/// </summary>
		internal struct Links
		{
			public MunThread next, prev; // cyclic if in the executor, null if not

			public void Remove(MunThread thread)
			{
				var executor = thread.Executor;
				if (executor == null)
					return;
				if (--executor.Count == 0)
				{
					executor.first = null;
					executor.next = null;
				}
				else
				{
					if (executor.first == thread)
						executor.first = next;
					if (executor.next == thread)
						executor.next = next;
					next._executorLinks.prev = prev;
					prev._executorLinks.next = next;
				}
				next = null;
				prev = null;
				thread.Executor = null;
			}
		}

		public MunCore Core { get; }
		public string Name { get; protected set; }
		public int Count { get; protected set; }

		protected MunExecutor(MunCore core, string name)
		{
			Core = core;
			Name = name;
		}

		/// <summary>
		/// Counter of rounds that did not execute any thread.
		/// </summary>
		protected int numSkips;
		/// <summary>
		/// Maximum number of rounds not executing any thread.
		/// </summary>
		protected internal int maxSkips;
		/// <summary>
		/// Fraction of update time we can use.
		/// (Used as: while (MunCore.Ticks - startTicks) &lt; Core.UpdateTicks * limit)
		/// </summary>
		protected internal double limit;
		/// <summary>
		/// Minimal amout of ticks to give a thread for execution.
		/// </summary>
		protected internal long minTicks;

		protected MunThread first, next;
		/// <summary>
		/// The next thread ready for execution is also the first (start of the round).
		/// </summary>
		protected bool AtFirst => next == first;
		/// <summary>
		/// Get one thread for execution and advance the iterator.
		/// </summary>
		protected MunThread GetNext()
		{
			var it = next;
			next = next._executorLinks.next;
			return it;
		}
		protected internal void Add(MunThread thread)
		{
			thread.RemoveFromExecutor();
			Count++;
			thread.Executor = this;
			if (first == null)
			{
				thread._executorLinks.next = thread;
				thread._executorLinks.prev = thread;
				first = thread;
				next = thread;
			}
			else
			{
				thread._executorLinks.next = first;
				thread._executorLinks.prev = first._executorLinks.prev;
				thread._executorLinks.prev._executorLinks.next = thread;
				first._executorLinks.prev = thread;
			}
		}

		public virtual void Execute(long startTicks)
		{
			if (Count == 0)
			{
				numSkips = 0;
				return;
			}
			long tickLimit = (long)(Core.UpdateTicks * limit);
			if (numSkips < maxSkips && (MunCore.Ticks - startTicks) >= tickLimit)
			{
				numSkips++;
				return;
			}
			numSkips = 0;
			// TODO: reconsider this allocation logic - it may look fair,
			// but does not give the threads as much as it could.
			// maybe recalculate the limit for each thread
			// and either shuffle the list or move those that did not use the time or yielded at the beginning.
			// also, the numSkips was designed with only one script in mind,
			// maybe we should finish the round once we start one.
			long execLimit = Math.Max(minTicks,
				(tickLimit - (MunCore.Ticks - startTicks)) / Count);
			do Execute(GetNext(), execLimit);
			while (!AtFirst // not the full round yet
			&& (MunCore.Ticks - startTicks) < tickLimit); // and we have time
		}

		protected virtual void Execute(MunThread thread, long tickLimit)
		{
			Core.Thread = thread;
			try
			{
				var status = thread.Execute(tickLimit);
				thread.Status = status;

				switch (status)
				{
				case MunStatus.Incomplete:
				case MunStatus.Yielded:
				case MunStatus.Terminating:
					// nothing to do, it just stays where it is
					break;
				case MunStatus.Finished:
				{
					var next = thread.NextThread;
					if (next != null)
					{
						if (next == thread)
						{
							// restart the thread
							thread.Status = MunStatus.Incomplete;
							thread.OnRestart();
							break;
						}
						// this is probably init-chain
						thread.NextThread = null; // rather clean this in case Core.Schedule(thread) also implements the logic
						Core.Schedule(next);
					}
					goto default;
				}
				default:
					// hand it over to the core (finished, terminated or sleeping)
					Core.Schedule(thread);
					break;
				}
			}
			catch (Exception ex)
			{
				thread.Exception = ex;
				var err = new MunEvent(Core, thread, ex);
				thread.OnError(err);
				if (!err.Handled)
				{
					Core.OnError(err);
					if (!err.Handled)
						Core.Kill(thread, hard: true);
				}
			}
			finally
			{
				Core.Thread = null;
			}
		}
	}
}
