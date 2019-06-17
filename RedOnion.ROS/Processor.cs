using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RedOnion.ROS.Objects;
using RedOnion.ROS.Utilities;

namespace RedOnion.ROS
{
	public class Processor : Core, IProcessor
	{
		public Processor() : base(null) => ctx = new Context();
		public Processor(UserObject globals) : base(null, globals) => ctx = new Context();

		public int UpdateCountdown { get; set; } = 1000;
		public int StepCountdown { get; set; } = 100;
		public int UpdatePercent { get; set; } = 50;
		public int OneShotPercent { get; set; } = 75;
		public int IdlePercent { get; set; } = 80;
		public int MaxOneShotSkips { get; set; } = 1;
		public int MaxIdleSkips { get; set; } = 10;
		public TimeSpan UpdateTimeout { get; set; } = TimeSpan.FromMilliseconds(10.0);

		/// <summary>
		/// Event invoked on engine shutdown or reset.
		/// Return false to auto-remove after invoke.
		/// </summary>
		public event Func<IProcessor, bool> Shutdown;
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
						if (ex is Error err)
						{
							var line = err.Line;
							Log(line == null ? "Error at line {0}."
								: "Error at line {0}: {1}", err.LineNumber, line);
						}
						Shutdown -= handler;
					}
				}
			}
			ClearEvents();
			ctx = new Context();
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
					if (!Call(ref call))
						updateList.Remove(call.Value);
				} while (!updateList.AtEnd
				&& CountdownPercent > UpdatePercent
				&& watch.Elapsed < UpdateTimeout);
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
					Call(ref call);
					oneShotList.Remove(call.Value);
				} while (!oneShotList.AtEnd
				&& CountdownPercent > OneShotPercent
				&& watch.Elapsed < UpdateTimeout);
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
					if (!Call(ref call))
						idleList.Remove(call.Value);
				} while (!idleList.AtEnd
				&& CountdownPercent > IdlePercent
				&& watch.Elapsed < UpdateTimeout);
			}

			// main
			do
			{
				if (Exit != ExitCode.Countdown && Exit != ExitCode.Yield)
					break;
				var countdown = Math.Min(StepCountdown, TotalCountdown);
				Execute(countdown);
				TotalCountdown -= (countdown - Countdown);
			} while (TotalCountdown > 0 && watch.Elapsed < UpdateTimeout);

			// watch
			watch.Stop();

			var milli = watch.Elapsed.TotalMilliseconds;
			if (AverageMillis <= 0)
				AverageMillis = milli;
			else AverageMillis = (AverageMillis * 9 + milli) * 0.1;
			if (milli > PeakMillis)
				PeakMillis = milli;
		}
		private bool Call(ref EventList.Element e)
		{
			try
			{
				if (!e.Value.IsFunction)
				{
					var result = Value.Void;
					e.Value.desc.Call(ref result, null, new Arguments(Arguments, 0));
				}
				else if (e.Core != null)
				{
					do
					{
						if (Exit != ExitCode.Countdown && Exit != ExitCode.Yield)
							break;
						var countdown = Math.Min(StepCountdown, TotalCountdown);
						e.Core.Execute(countdown);
						TotalCountdown -= (countdown - Countdown);
					} while (TotalCountdown > 0 && watch.Elapsed < UpdateTimeout);
					if (Exit != ExitCode.Countdown && Exit != ExitCode.Yield)
					{
						// TODO: pool
						e.Core = null;
					}
				}
				else
				{
					e.Core = new Core(this, Globals);
					var countdown = Math.Min(StepCountdown, TotalCountdown);
					var fn = e.Value.obj as RedOnion.ROS.Objects.Function;
					e.Core.Execute(fn.Code, countdown);
				}
			}
			catch (Exception ex)
			{
				Log("Exception in FixedUpdate: " + ex.Message);
				if (ex is Error err)
				{
					var line = err.Line;
					Log(line == null ? "Error at line {0}."
						: "Error at line {0}: {1}", err.LineNumber, line);
				}
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

			public void Add(Value item)
				=> list.Add(new Element(item));
			public void Insert(int index, Value item)
			{
				if (index <= this.index && index >= 0)
					this.index++;
				list.Insert(index, new Element(item));
			}
			public void RemoveAt(int index)
			{
				if (index < this.index && index >= 0)
					this.index--;
				list.RemoveAt(index);
			}
			public bool Remove(Value item)
			{
				int index = IndexOf(item);
				if (index < 0)
					return false;
				RemoveAt(index);
				return true;
			}

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

			public bool Contains(Value item)
			{
				foreach (var e in list)
					if (e.Value.Equals(item))
						return true;
				return false;
			}
			public void CopyTo(Value[] array, int index)
			{
				foreach (var e in list)
					array[index++] = e.Value;
			}
			public int IndexOf(Value item)
			{
				for (int i = 0; i < list.size; i++)
					if (list.items[i].Value.Equals(item))
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
