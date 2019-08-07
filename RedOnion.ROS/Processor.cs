using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using RedOnion.ROS.Objects;
using RedOnion.ROS.Parsing;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS
{
	public class Processor : Core, IProcessor
	{
		public Processor() : base(null) => ctx = new Context();
		protected override void SetGlobals(Globals value)
		{
			if ((globals = value) == null)
				return;
			value.Processor = this;
			value.Fill();
		}

		/// <summary>
		/// Total time-limit for all handlers (one of each type can still be executed).
		/// Main loop always gets at least StepCountdown (100 by default) instructions executed.
		/// </summary>
		public TimeSpan UpdateTimeout { get; set; } = TimeSpan.FromMilliseconds(10.0);
		/// <summary>
		/// Limit of instructions per fixed update
		/// (not a hard max-limit, can be breached by some small multiple of StepCountdown).
		/// </summary>
		public int UpdateCountdown { get; set; } = 1000;
		/// <summary>
		/// Minimum number of instructions executed by sub-cycle
		/// (limits are checked after reaching this number of instructions, or yield/wait instruction).
		/// </summary>
		public int StepCountdown { get; set; } = 100;
		/// <summary>
		/// The percentage of the limit (going down from 100% to zero)
		/// when to stop executing update handlers.
		/// At least one handler is always executed (with at least StepCountdown instructions).
		/// 50% by default (500 instructions total with default StepCountdown).
		/// </summary>
		public int UpdatePercent { get; set; } = 50;
		/// <summary>
		/// The percentage of the limit (going down from 100% to zero)
		/// when to stop executing one-shot handlers.
		/// 40% by default (at least 100 instructions total with default StepCountdown,
		/// upto 600 if no update handlers).
		/// At least one handler is always executed every (MaxOneShotSkips+1) updates
		/// (every other fixed update by default).
		/// </summary>
		public int OneShotPercent { get; set; } = 40;
		/// <summary>
		/// The percentage of the limit (going down from 100% to zero)
		/// when to stop executing idle handlers.
		/// 60% by default (skipped if update + one-shot already used 400 instructions,
		/// upto 400 if update + one-shot used none).
		/// At least one handler is always executed every (MaxIdleSkips+1) updates
		/// (every 10th update by default).
		/// </summary>
		public int IdlePercent { get; set; } = 60;
		/// <summary>
		/// Maximum number of updates when no one-shot handler was executed (if there was any).
		/// </summary>
		public int MaxOneShotSkips { get; set; } = 1;
		/// <summary>
		/// Maximum number of updates when no idle handler was executed (if there was any).
		/// </summary>
		public int MaxIdleSkips { get; set; } = 9;

		public event Func<IProcessor, bool> Shutdown;
		public Action<string> Print;
		void IProcessor.Print(string msg) => Print?.Invoke(msg);
		event Action IProcessor.Update
		{
			add => updateList.Add(value);
			remove => updateList.Remove(value);
		}

		//TODO: cache scripts - both compiled and the source (watch file modification time)
		protected Parser Parser { get; } = new Parser();
		public virtual CompiledCode Compile(string source, string path = null)
			=> Parser.Compile(source, path);
		public virtual string ReadScript(string path)
		{
			if (!Path.IsPathRooted(path))
				path = Path.Combine(Path.GetDirectoryName(typeof(Processor).Assembly.Location), path);
			if (File.Exists(path))
				return File.ReadAllText(path, Encoding.UTF8);
			return null;
		}

		/// <summary>
		/// Reset the engine - clear all event lists
		/// and reset/recreate globals.
		/// </summary>
		public virtual void Reset()
		{
			var shutdown = Shutdown;
			if (shutdown != null)
			{
				foreach (Func<IProcessor, bool> handler in shutdown.GetInvocationList())
				{
					try
					{
						if (!handler(this))
							Shutdown -= handler;
					}
					catch (Exception ex)
					{
						Log("Exception in Shutdown: " + ex.Message);
						Log(ex);
						Shutdown -= handler;
					}
				}
			}
			ClearEvents();
			if (globals != null)
			{
				globals.Reset();
				globals.Fill();
			}
			stack.Clear();
			ctx = new Context();
			Exit = ExitCode.None;
		}

		protected EventList updateList = new EventList();
		protected EventList oneShotList = new EventList();
		protected EventList idleList = new EventList();

		public Event Update
		{
			get => new Event(updateList);
			set { }
		}
		public Event OneShot
		{
			get => new Event(oneShotList);
			set { }
		}
		public Event Idle
		{
			get => new Event(idleList);
			set { }
		}

		public bool HasEvents
			=> updateList.Count > 0
			|| oneShotList.Count > 0
			|| idleList.Count > 0;
		public void ClearEvents()
		{
			updateList.Clear();
			oneShotList.Clear();
			idleList.Clear();
			oneShotSkipped = 0;
			idleSkipped = 0;
			PeakMillis = AverageMillis;
		}

		public int TotalCountdown { get; protected set; }
		public int CountdownPercent => 100 * TotalCountdown / UpdateCountdown;
		public int TimeoutPercent => (int)(100 * (1 - watch.Elapsed.TotalMilliseconds / UpdateTimeout.TotalMilliseconds));
		public double AverageMillis { get; set; }
		public double PeakMillis { get; set; }
		Stopwatch watch = new Stopwatch();
		int oneShotSkipped, idleSkipped;

		public void FixedUpdate()
		{
			TotalCountdown = UpdateCountdown;
			watch.Reset();
			watch.Start();

			// update
			if (updateList.Count > 0)
			{
				do
				{
					ref var call = ref updateList.GetNext();
					if (!Call(ref call, "Update", UpdatePercent))
						updateList.Remove(call.Value);
				} while (!updateList.AtEnd
				&& CountdownPercent > UpdatePercent
				&& TimeoutPercent > UpdatePercent);
			}

			// one shot
			if (oneShotList.IsEmpty)
				oneShotSkipped = 0;
			else if (CountdownPercent <= OneShotPercent && oneShotSkipped < MaxOneShotSkips)
				oneShotSkipped++;
			else
			{
				oneShotSkipped = 0;
				do
				{
					ref var call = ref oneShotList.GetNext();
					Call(ref call, "OneShot", OneShotPercent);
					oneShotList.Remove(call.Value);
				} while (!oneShotList.AtEnd
				&& CountdownPercent > OneShotPercent
				&& TimeoutPercent > OneShotPercent);
			}

			// idle
			if (idleList.IsEmpty)
				idleSkipped = 0;
			else if (CountdownPercent <= IdlePercent && idleSkipped < MaxIdleSkips)
				idleSkipped++;
			else
			{
				idleSkipped = 0;
				do
				{
					ref var call = ref idleList.GetNext();
					if (!Call(ref call, "Idle", IdlePercent))
						idleList.Remove(call.Value);
				} while (!idleList.AtEnd
				&& CountdownPercent > IdlePercent
				&& TimeoutPercent > IdlePercent);
			}

			// main
			do
			{
				if (!Paused)
					break;
				var countdown = Math.Min(StepCountdown, TotalCountdown);
				Execute(countdown);
				TotalCountdown -= (countdown - Countdown);
			} while (Exit == ExitCode.Countdown
			&& TotalCountdown > 0 && watch.Elapsed < UpdateTimeout);

			// watch
			watch.Stop();

			var milli = watch.Elapsed.TotalMilliseconds;
			if (AverageMillis <= 0)
				AverageMillis = milli;
			else AverageMillis = (AverageMillis * 9 + milli) * 0.1;
			if (milli > PeakMillis)
				PeakMillis = milli;
		}
		private bool Call(ref EventList.Element e, string name, int downtoPercent)
		{
			try
			{
				if (!e.Value.IsFunction)
				{
					var result = e.Value;
					e.Value.desc.Call(ref result, null, new Arguments(Arguments, 0));
				}
				else
				{
					do
					{
						var countdown = Math.Min(StepCountdown, TotalCountdown);
						if (e.Core == null)
						{
							e.Core = new Core(this);
							e.Core.Globals = Globals;
							var fn = e.Value.obj as Function;
							e.Core.Execute(fn, countdown);
						}
						else e.Core.Execute(countdown);
						TotalCountdown -= (countdown - Countdown);
					} while (e.Core.Exit == ExitCode.Countdown
					&& CountdownPercent > downtoPercent && UpdatePercent > downtoPercent);
					if (!e.Core.Paused)
					{
						// TODO: pool
						e.Core = null;
					}
				}
			}
			catch (Exception ex)
			{
				Log("Exception in FixedUpdate.{0}: {1}", name, ex.Message);
				Log(ex);
				return false;
			}
			return true;
		}

		/// <summary>
		/// List of callable objects that tracks removals during enumeration
		/// </summary>
		public class EventList : IList<Value>
		{
			//TODO: doubly-linked list (for fast remove) with free-list (pool to save GC)
			public struct Element : IEquatable<Element>
			{
				/// <summary>
				/// Function or Action
				/// </summary>
				public readonly Value Value;
				/// <summary>
				/// Core it is currently running on (if yielding)
				/// </summary>
				public Core Core;

				public Element(Value value)
				{
					Value = value;
					Core = null;
				}

				public bool Equals(Element e)
					=> Value.Equals(e.Value);
				public override bool Equals(Object o)
					=> o is Element e && Equals(e);
				public override int GetHashCode()
					=> Value.GetHashCode();
			}
			protected ListCore<Element> list;
			protected internal int index;

			public int Count => list.size;
			public bool IsEmpty => list.size == 0;
			public bool AtEnd => index >= list.size;

			public ref Element GetNext()
			{
				if (index >= list.size)
					index = 0;
				return ref list.items[index++];
			}

			public void Add(Value value)
				=> list.Add(new Element(value));
			public void Add(Action action)
				=> list.Add(new Element(new Value(action)));
			public void Insert(int index, Value value)
			{
				if (index <= this.index && index >= 0)
					this.index++;
				list.Insert(index, new Element(value));
			}
			public void RemoveAt(int index)
			{
				if (index < this.index && index >= 0)
					this.index--;
				list.RemoveAt(index);
			}
			public bool Remove(Value value)
			{
				int index = IndexOf(value);
				if (index < 0)
					return false;
				RemoveAt(index);
				return true;
			}
			public bool Remove(Action action)
				=> Remove(new Value(action));

			public Value this[int index]
			{
				get => list[index].Value;
				set => list[index] = new Element(value);
			}
			public void Clear()
			{
				list.Clear();
				index = 0;
			}

			public bool Contains(Value value)
			{
				foreach (var e in list)
					if (e.Value.Equals(value))
						return true;
				return false;
			}
			public void CopyTo(Value[] array, int index)
			{
				foreach (var e in list)
					array[index++] = e.Value;
			}
			public int IndexOf(Value value)
			{
				for (int i = 0; i < list.size; i++)
					if (list.items[i].Value.Equals(value))
						return i;
				return -1;
			}
			public IEnumerator<Value> GetEnumerator()
			{
				foreach (var e in list)
					yield return e.Value;
			}
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			bool ICollection<Value>.IsReadOnly => false;
		}
	}
}
