using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
		public Processor() : base(null)
		{
			ctx = new Context();
			eventsToRemove = new List<Event.Subscription>();
			Update = new Event("Update", eventsToRemove);
			Once = new Event("Once", eventsToRemove);
			Idle = new Event("Idle", eventsToRemove);
		}
		protected override void SetGlobals(Globals value)
		{
			if ((globals = value) == null)
				return;
			value.Processor = this;
			value.Fill();
		}

		public event Action Shutdown;
		public event Action PhysicsUpdate;
		public event Action GraphicUpdate;
		public Action<string> Print;
		public Action<string> PrintError;
		void IProcessor.Print(string msg) => Print?.Invoke(msg);
		public void PrintException(string where, Exception ex, bool logOnly = false)
		{
			var print = logOnly ? null : PrintError;
			var hdr = Value.Format("Exception in {0}: {1}", where, ex.Message);
			Log(hdr);
			print?.Invoke(hdr);
			var err = ex as Error;
			var re = err as RuntimeError;
			if (err != null)
			{
				var path = re?.Code?.Path;
				if (path != null)
				{
					var pathstr = "Script path: " + path;
					Log(pathstr);
					print?.Invoke(pathstr);
				}
				var line = err.Line;
				var str = Value.Format(line == null ? "Error at line {0}."
					: "Error at line {0}: {1}", err.LineNumber + 1, line);
				Log(str);
				print?.Invoke(str);
				ex = ex.InnerException;
			}
			var trace = ex?.StackTrace;
			if (trace != null && trace.Length > 0)
			{
				Log(trace);
				print?.Invoke(trace);

				while ((ex = ex.InnerException) != null)
				{
					Log("Inner: " + ex.Message);
					Log(ex.StackTrace);
				}
			}
			if (re != null)
			{
				var code = re.Code?.Code;
				if (code == null)
					return;
				var sb = new StringBuilder();
				var at = re.CodeAt;
				if (at > 0 && at <= code.Length)
				{
					for (int i = Math.Max(0, at - 64); i < at; i++)
						sb.AppendFormat("{0:X2}", code[i]);
					Log(sb.ToString());
				}
				if (at < code.Length)
				{
					sb.Length = 0;
					for (int i = at, n = Math.Min(at + 64, code.Length); i < n; i++)
						sb.AppendFormat("{0:X2}", code[i]);
					Log(sb.ToString());
				}
			}
		}

		/// <summary>
		/// Total time-limit for all handlers (one of each type can still be executed).
		/// Main loop always gets at least StepCountdown (100 by default) instructions executed.
		/// </summary>
		public TimeSpan UpdateTimeout { get; set; } = TimeSpan.FromMilliseconds(1.0);
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
		public int OncePercent { get; set; } = 40;
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

		//TODO: cache scripts - both compiled and the source (watch file modification time)
		protected Parser Parser { get; } = new Parser();
		public virtual CompiledCode Compile(string source, string path = null)
			=> Parser.Compile(source, path);
		public virtual string ReadScript(string path)
		{
			if (!Path.IsPathRooted(path) && !File.Exists(path))
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
			Terminate();
			if (globals != null)
			{
				globals.Reset();
				globals.Fill();
			}
			ctx = new Context();
		}
		/// <summary>
		/// Terminate running script and clear all evets
		/// (but preserve globals and top context).
		/// </summary>
		public virtual void Terminate()
		{
			var shutdown = Shutdown;
			Shutdown = null;
			if (shutdown != null)
			{
				foreach (Action handler in shutdown.GetInvocationList())
				{
					try
					{
						handler();
					}
					catch (Exception ex)
					{
						PrintException("Shutdown", ex);
					}
				}
			}
			ClearEvents();
			stack.Clear();
			ctx.PopAll();
			ctx.CatchBlocks = 0;
			Exit = ExitCode.None;
			result = Value.Void;
			error = Value.Void;
		}

		protected readonly List<Event.Subscription> eventsToRemove;
		public Event Update { get; }
		public Event Once { get; }
		public Event Idle { get; }

		public bool HasEvents
			=> Update.Count > 0
			|| Once.Count > 0
			|| Idle.Count > 0;
		public void ClearEvents()
		{
			Update.Clear();
			Once.Clear();
			Idle.Clear();
			onceSkipped = 0;
			idleSkipped = 0;
		}

		public int TotalCountdown { get; protected set; }
		public int CountdownPercent => 100 * TotalCountdown / UpdateCountdown;
		public int TimeoutPercent => (int)(100 * (1 - watch.Elapsed.TotalMilliseconds / UpdateTimeout.TotalMilliseconds));
		public double AverageMillis { get; set; }
		public double AverageCount { get; set; }
		public double PeakMillis { get; set; }
		public double PeakCount { get; set; }
		double _peakMillis, _peakCount;

		readonly Stopwatch watch = new Stopwatch();
		readonly Stopwatch secWatch = new Stopwatch();
		int onceSkipped, idleSkipped;

		public virtual void UpdateGraphic()
		{
			var graphic = GraphicUpdate;
			if (graphic != null)
			{
				foreach (Action handler in graphic.GetInvocationList())
				{
					try
					{
						handler();
					}
					catch (Exception ex)
					{
						PhysicsUpdate -= handler;
						PrintException("GraphicUpdate", ex);
					}
				}
			}
		}
		public virtual void UpdatePhysics()
		{
			TotalCountdown = UpdateCountdown;
			watch.Reset();
			watch.Start();

			// see Event.AutoRemove (the list is filled from destructors)
			lock (eventsToRemove)
			{
				foreach (var e in eventsToRemove)
					e.Remove();
				eventsToRemove.Clear();
			}

			// update (periodic, every physics/fixed update if possible)
			if (!Update.IsEmpty)
			{
				do
				{
					var call = Update.GetNext();
					if (!Call(call, Update, UpdatePercent))
						call.Remove();
				} while (!Update.AtFirst //TODO: reconsider this condition
				&& CountdownPercent > UpdatePercent
				&& TimeoutPercent > UpdatePercent);
			}

			// one shot (usually from UI like button click)
			if (Once.IsEmpty)
				onceSkipped = 0;
			else if (CountdownPercent <= OncePercent && onceSkipped < MaxOneShotSkips)
				onceSkipped++;
			else
			{
				onceSkipped = 0;
				do
				{
					var call = Once.GetNext();
					if (!Call(call, Once, OncePercent) || call.Core == null)
						call.Remove();
				} while (!Once.IsEmpty
				&& CountdownPercent > OncePercent
				&& TimeoutPercent > OncePercent);
			}

			// idle (less important like staging logic)
			if (Idle.IsEmpty)
				idleSkipped = 0;
			else if (CountdownPercent <= IdlePercent && idleSkipped < MaxIdleSkips)
				idleSkipped++;
			else
			{
				idleSkipped = 0;
				do
				{
					var call = Idle.GetNext();
					if (!Call(call, Idle, IdlePercent))
						call.Remove();
				} while (!Idle.AtFirst //TODO: reconsider this condition
				&& CountdownPercent > IdlePercent
				&& TimeoutPercent > IdlePercent);
			}

			// main (note: exceptions are propagated, terminating execution)
			do
			{
				if (!Paused)
					break;
				var countdown = Math.Min(StepCountdown, TotalCountdown);
				Execute(countdown);
				TotalCountdown -= (countdown - Countdown);
			} while (Exit == ExitCode.Countdown
			&& TotalCountdown > 0 && watch.Elapsed < UpdateTimeout);

			// other updates (e.g. vector drawing)
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
						PhysicsUpdate -= handler;
						PrintException("PhysicsUpdate", ex);
					}
				}
			}

			// watch
			watch.Stop();

			var milli = watch.Elapsed.TotalMilliseconds;
			if (AverageMillis <= 0)
				AverageMillis = milli;
			else AverageMillis = (AverageMillis * 9 + milli) * 0.1;
			if (milli > _peakMillis)
				_peakMillis = milli;
			var count = UpdateCountdown - TotalCountdown;
			if (AverageCount <= 0)
				AverageCount = count;
			else AverageCount = (AverageCount * 9 + count) * 0.1;
			if (count > _peakCount)
				_peakCount = count;

			if (!secWatch.IsRunning)
			{
				secWatch.Start();
				_peakMillis = 0;
				_peakCount = 0;
			}
			if (secWatch.ElapsedMilliseconds >= 3333)
			{
				secWatch.Restart();
				Value.DebugLog("AVG: {0,5:F2}ms {1,4:F2}i PEAK: {2,5:F2}ms {3,4:F2}i", AverageMillis, AverageCount, PeakMillis, PeakCount);
				PeakMillis = 0.5 * (PeakMillis + _peakMillis);
				PeakCount = 0.5 * (PeakCount + _peakCount);
				_peakMillis = 0;
				_peakCount = 0;
			}
		}
		private bool Call(Event.Subscription e, Event loop, int downtoPercent)
		{
			try
			{
				if (!e.Action.IsFunction)
				{
					var result = e.Action;
					e.Action.desc.Call(ref result, null, new Arguments(Arguments, 0));
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
							var fn = e.Action.obj as Function;
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
				PrintException(Value.Format("FixedUpdate.{0}[{1}]", loop.name, loop.Count), ex);
				return false;
			}
			return true;
		}

		[Description(
@"List of subscriptions - actions to be called.
(Periodically for `update` and `idle`, once for `once`.).
Either use `add/remove` pair, or call the list and store the auto-remove subscription object.
The subscription will be automatically removed if there is no reference to the subscription object.

Example:
```
def action
  print ""executed""
system.update.add action
wait // will get executed once
system.update.remove action
wait // will not get executed
var sub = system.update action
wait // will get executed once
sub.remove
wait // will not get executed
sub = system.update action
wait // will get executed once
sub = null
wait // may still get executed, but will get removed eventually
```

Future plan:
```
using system.update action
  wait // will get executed once
wait // will not get executed

def test
  using system.update action
  wait
test // will get executed once
wait // will not get executed
```
")]
		public sealed class Event : ICallable
		{
			internal Event(string name, List<Subscription> eventsToRemove)
			{
				this.name = name;
				this.eventsToRemove = eventsToRemove;
			}
			internal readonly string name;
			internal readonly List<Subscription> eventsToRemove;
			internal Subscription first, next;
			internal bool AtFirst => next == first;
			internal bool IsEmpty => first == null;
			internal Subscription GetNext()
			{
				var it = next;
				next = next.next;
				return it;
			}

			[Description("Number of subscribers")]
			public int Count { get; private set; }

			bool ICallable.Call(ref Value result, object self, Arguments args, bool create)
			{
				if (args.Length != 1)
					return false;
				var it = args[0];
				if (!it.IsFunction)
					return false;
				result = new Value(new AutoRemove(Add(it)));
				return true;
			}

			[Description("The auto-remove subscription object returned by call to `system.update` or `idle` (or `oneShot`).")]
			public sealed class AutoRemove : IDisposable, IEquatable<AutoRemove>, IEquatable<Subscription>, IEquatable<Value>
			{
				internal AutoRemove(Subscription subscription)
					=> Subscription = subscription;
				[Description("The subscription.")]
				public Subscription Subscription { get; }

				[Description("The subscribed action (ROS function or .NET action delegate).")]
				public Value Action => Subscription.Action;
				[Description("The event the action is subscribed to (null when removed).")]
				public Event Event => Subscription.Event;
				[Browsable(false), Description("Core it is currently running on (if yielding).")]
				public Core Core => Subscription.Core;

				[Browsable(false)]
				public bool Equals(AutoRemove it)
					=> ReferenceEquals(this, it);
				[Browsable(false)]
				public bool Equals(Subscription subscription)
					=> ReferenceEquals(Subscription, subscription);
				[Browsable(false)]
				public bool Equals(Value value)
					=> value.obj is Subscription s ? ReferenceEquals(this, s) : Action.Equals(value);
				[Browsable(false)]
				public override bool Equals(object o)
					=> o is Value v ? Equals(v) : ReferenceEquals(this, o);
				[Browsable(false)]
				public override int GetHashCode()
					=> Subscription.GetHashCode();

				[Description("Remove this subscription from its list. (Ignored if already removed.)")]
				public void Remove() => Subscription.Remove();

				void IDisposable.Dispose() => Subscription.Remove();

				// destructors could be called from different thread
				// this is a way to delegate the removal to main thread
				~AutoRemove()
				{
					var e = Subscription.Event;
					if (e != null)
					{
						lock (e.eventsToRemove)
							e.eventsToRemove.Add(Subscription);
					}
				}
			}

			[Description("The subscription to `system.update` or `idle` (or `oneShot`).")]
			public sealed class Subscription : IDisposable, IEquatable<Subscription>, IEquatable<Value>
			{
				internal Subscription(Value action) => Action = action;
				internal Subscription next, prev; // cyclic (when subscribed, null if removed)

				[Description("The subscribed action (ROS function or .NET action delegate).")]
				public Value Action { get; }
				[Description("The event the action is subscribed to (null when removed).")]
				public Event Event { get; internal set; }
				[Browsable(false), Description("Core it is currently running on (if yielding).")]
				public Core Core { get; internal set; }

				[Browsable(false)]
				public bool Equals(Subscription subscription)
					=> ReferenceEquals(this, subscription);
				[Browsable(false)]
				public bool Equals(Value value)
					=> value.obj is Subscription s ? ReferenceEquals(this, s) : Action.Equals(value);
				[Browsable(false)]
				public override bool Equals(object o)
					=> o is Subscription s ? Equals(s) : o is Value v && Equals(v);
				[Browsable(false)]
				public override int GetHashCode()
					=> base.GetHashCode();

				[Description("Remove this subscription from its list. (Ignored if already removed.)")]
				public void Remove()
				{
					if (Event == null)
						return;
					if (--Event.Count == 0)
					{
						Event.first = null;
						Event.next = null;
					}
					else
					{
						if (Event.first == this)
							Event.first = next;
						if (Event.next == this)
							Event.next = next;
					}
					next.prev = prev;
					prev.next = next;
					next = null;
					prev = null;
					Event = null;
					Core = null;
				}
				void IDisposable.Dispose() => Remove();
			}

			public Subscription Add(Value value)
			{
				if (value.obj is Subscription s)
					s.Remove();
				else s = new Subscription(value);
				Count++;
				s.Event = this;
				if (first == null)
				{
					s.next = s;
					s.prev = s;
					first = s;
					next = s;
				}
				else
				{
					s.next = first;
					s.prev = first.prev;
					s.prev.next = s;
					first.prev = s;
				}
				return s;
			}
			public bool Remove(Value value)
			{
				if (first == null)
					return false;
				if (value.obj is Subscription s)
				{
					if (s.Event != this)
						return false;
					s.Remove();
					return true;
				}
				bool found = false;
				var next = first;
				do
				{
					var it = next;
					next = next.next;
					if (it.Action.Equals(value))
					{
						found = true;
						it.Remove();
					}
				}
				while (next != first);
				return found;
			}
			public void Clear()
			{
				var next = first;
				if (next == null)
					return;
				do
				{
					var it = next;
					next = next.next;
					it.Remove();
				}
				while (next != first);
			}
		}
	}
}
