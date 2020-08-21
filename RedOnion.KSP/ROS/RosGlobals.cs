using MunOS;
using RedOnion.KSP.API;
using RedOnion.ROS;
using RedOnion.ROS.Objects;
using RedOnion.ROS.Utilities;
using Smooth.Algebraics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static RedOnion.Debugging.QueueLogger;

namespace RedOnion.KSP.ROS
{
	public class RosGlobals : RedOnion.ROS.Objects.Globals
	{
		public override void Fill()
		{
			if (readOnlyTop > 0)
				return;
			base.Fill();
			System.Add(typeof(PID));

			//TODO: link all non-main to some main to simulate process-like thread group
			System.Add("update", new Event(MunPriority.Realtime));
			System.Add("idle", new Event(MunPriority.Idle));
			System.Add("once", new Event(MunPriority.Callback));
			System.Add("main", new Event(MunPriority.Main));
			System.Lock();
		}

		class Event : ICallable
		{
			public readonly MunPriority priority;
			public Event(MunPriority priority)
				=> this.priority = priority;

			bool ICallable.Call(ref Value result, object self, Arguments args, bool create)
			{
				if (args.Length != 1)
					return false;
				var it = args[0];
				if (!it.IsFunction)
					return false;
				result = new Value(new Subscription(new RosThread(
					MunProcess.Current as RosProcess, priority, it.obj as Function),
					repeating: priority == MunPriority.Realtime || priority == MunPriority.Idle,
					autoRemove: priority != MunPriority.Main));
				return true;
			}
		}

		[Description("The auto-remove subscription object returned by call to `system.update` or `idle` (or `once`).")]
		public class Subscription : IDisposable, IEquatable<Subscription>, IEquatable<Value>
		{
			protected RosThread thread;
			protected bool autoRemove;
			internal Subscription(RosThread thread, bool repeating, bool autoRemove = true)
			{
				RosLogger.DebugLog($"Creating subscription#{thread.ID}, priority: {thread.Priority}, repeating: {repeating}");

				this.thread = thread;
				this.autoRemove = autoRemove;
				thread.IsBackground = true;
				if (repeating)
					thread.ExecNext = thread;
			}

			[Browsable(false)]
			public bool Equals(Subscription it)
				=> ReferenceEquals(this, it);
			[Browsable(false)]
			public bool Equals(Value value)
				=> value.obj is RosThread t && ReferenceEquals(this, t);
			[Browsable(false)]
			public override bool Equals(object o)
				=> o is Value v ? Equals(v) : ReferenceEquals(this, o);
			[Browsable(false)]
			public override int GetHashCode()
				=> thread.GetHashCode();

			[Description("Remove this subscription from its list. (Ignored if already removed.)")]
			public void Remove()
			{
				thread.Terminate(hard: true);
				autoRemove = false;
				GC.SuppressFinalize(this);
			}
			void IDisposable.Dispose()
				=> Remove();
			~Subscription()
			{
				if (autoRemove)
					thread.KillAsync();
			}

			[Description("Replace current function with another function.")]
			public void Replace(Value value)
			{
				if (!value.IsFunction)
					throw new InvalidOperation("Not a function");
				thread.Function = value.obj as Function;
				if (thread.Status.IsFinal())
					thread.Restart();
			}
		}

		class ReflectedGlobals : Reflected
		{
			public ReflectedGlobals() : base(typeof(API.Globals)) { }
			public int Count => prop.Count;
			public ref Prop this[int idx] => ref prop.items[idx];
			public int Find(string name) => sdict.TryGetValue(name, out var idx) ? idx : -1;
			public bool Has(string name) => sdict.ContainsKey(name);
			public IEnumerator<string> GetEnumerator()
			{
				for (int i = 0; i < prop.size; i++)
					yield return prop.items[i].name;
			}
		}
		static readonly ReflectedGlobals reflected = new ReflectedGlobals();
		const int mark = 0x7F000000;

		public override int Find(string name)
		{
			int at = reflected.Find(name);
			if (at >= 0) return at + mark;
			return base.Find(name);
		}
		public override void Get(ref Value self)
		{
			if (self.idx is string name)
			{
				if (reflected.Has(name))
				{
					reflected.Get(ref self);
					return;
				}
			}
			else if (self.idx is ValueBox box)
			{
				if (box.Value.IsStringOrChar)
				{
					name = box.Value.ToStr();
					if (reflected.Has(name))
					{
						reflected.Get(ref self);
						return;
					}
				}
			}
			base.Get(ref self);
		}
		public override void Set(ref Value self, OpCode op, ref Value value)
		{
			if (self.idx is string name)
			{
				if (reflected.Has(name))
				{
					reflected.Set(ref self, op, ref value);
					return;
				}
			}
			else if (self.idx is ValueBox box)
			{
				if (box.Value.IsStringOrChar)
				{
					name = box.Value.ToStr();
					if (reflected.Has(name))
					{
						reflected.Set(ref self, op, ref value);
						return;
					}
				}
			}
			base.Set(ref self, op, ref value);
		}
		public override IEnumerable<string> EnumerateProperties(object self)
		{
			var seen = new HashSet<string>();
			foreach (var member in reflected)
			{
				seen.Add(member);
				yield return member;
			}
			foreach (var name in EnumerateProperties(self, seen))
				yield return name;
		}
	}
}
