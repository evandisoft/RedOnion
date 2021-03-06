using MunOS;
using RedOnion.KSP.API;
using RedOnion.ROS;
using RedOnion.ROS.Objects;
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
					throw InvalidOperation("Not a function");
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
			public IEnumerator<string> GetEnumerator()
			{
				for (int i = 0; i < prop.size; i++)
					yield return prop.items[i].name;
			}
		}
		const int mark = 0x7F000000;
		static ReflectedGlobals reflected = new ReflectedGlobals();

		public override int Find(string name)
		{
			int at = reflected.Find(null, name, false);
			if (at >= 0) return at + mark;
			return base.Find(name);
		}
		public override bool Get(ref Value self, int at)
		{
			if (at < mark)
				return base.Get(ref self, at);
			if ((at -= mark) >= reflected.Count)
				return false;
			ref var member = ref reflected[at];
			if (member.read == null)
				return false;
			self = member.read(self.obj);
			return true;
		}
		public override bool Set(ref Value self, int at, OpCode op, ref Value value)
		{
			if (at < mark)
				return base.Set(ref self, at, op, ref value);
			if ((at -= mark) >= reflected.Count)
				return false;
			ref var member = ref reflected[at];
			if (member.write == null)
				return false;
			if (op != OpCode.Assign)
				return false;
			member.write(self.obj, value);
			return true;
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
