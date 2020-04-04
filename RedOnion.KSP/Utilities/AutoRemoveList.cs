using MoonSharp.Interpreter;
using MunOS;
using RedOnion.Attributes;
using RedOnion.ROS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedOnion.KSP.Utilities
{
	[Callable("subscribe")]
	[Description("Designed for events with auto-remove on process shutdown, but can be used with any type of elements."
		+ " Removing elements during enumeration is allowed (for current element of the enumerator).")]
	public class AutoRemoveList<T> : IOperators, IEnumerable<T>
	{
		Dictionary<T, Subscription> hooks;
		Subscription first;

		[Description("Number of subscription.")]
		public int count => hooks.Count;

		[Description("Subscribe to the list. Similar to `add` but returns auto-remove subscription.")]
		public AutoSubscription subscribe(T value)
		{
			var hook = add(value);
			return hook == null ? null : new AutoSubscription(hook);
		}

		[Description("Add new item. Returns pure subscribtion (or null for duplicit item).")]
		public Subscription add(T value)
		{
			if (first == null)
			{
				var hook = CreateSubscription(value);
				hook.next = hook;
				hook.prev = hook;
				first = hook;
				if (hooks == null)
					hooks = new Dictionary<T, Subscription>();
				hooks[value] = hook;
				return hook;
			}
			else if (hooks.ContainsKey(value))
				return null;
			else
			{
				var hook = CreateSubscription(value);
				hook.next = first;
				hook.prev = first.prev;
				hook.prev.next = hook;
				first.prev = hook;
				hooks[value] = hook;
				return hook;
			}
		}
		protected virtual Subscription CreateSubscription(T value)
			=> new Subscription(value, this);

		[Description("Remove item. Returns the subscription on success, null if not found.")]
		public Subscription remove(T value)
		{
			if (first == null)
				return null;
			if (!hooks.TryGetValue(value, out var hook))
				return null;
			hook.remove();
			return hook;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		[Browsable(false), MoonSharpHidden]
		public IEnumerator<T> GetEnumerator()
		{
			if (first == null)
				yield break;
			for (var it = first; ;)
			{//	subscribers can safely remove themselves when called
				var next = it.next;			// because we prefetch the next item
				var last = next == first;	// and pre-check termination
				yield return it.item;       // so even if this leads to calling remove()
				if (last) break;            // we know if it was last
				it = next;                  // and next item did not change
			}
		}

		// to support += and -= syntax
		bool IOperators.Unary(ref Value self, OpCode op)
			=> false;
		bool IOperators.Binary(ref Value lhs, OpCode op, ref Value rhs)
		{
			if (op != OpCode.AddAssign && op != OpCode.SubAssign)
				return false;
			if (!rhs.desc.Convert(ref rhs, descriptor))
				return false;
			lhs = new Value(add((T)rhs.obj));
			return true;
		}
		static readonly Descriptor descriptor = Descriptor.Of(typeof(T));

		[Description("Subscription to the list.")]
		public class Subscription : IDisposable
		{
			internal Subscription next, prev;
			protected internal T item { get; private set; }
			protected AutoRemoveList<T> owner { get; private set; }
			protected MunProcess process { get; private set; }
			internal WeakReference<AutoSubscription> auto;

			protected internal Subscription(T item, AutoRemoveList<T> owner)
			{
				this.item = item;
				this.owner = owner;
				if ((process = MunProcess.Current) == null)
					return;
				process.shutdown += remove;
			}
			void IDisposable.Dispose()
				=> remove();

			[Description("Remove the item.")]
			public void remove()
			{
				if (next == null)
					return;
				if (next == this)
					owner.first = null;
				else
				{
					if (owner.first == this)
						owner.first = next;
					next.prev = prev;
					prev.next = next;
				}
				next = null;
				prev = null;
				owner.hooks.Remove(item);
				owner = null;
				item = default;
				if (process != null)
				{
					process.shutdown -= remove;
					process = null;
				}
				if (auto != null)
				{
					if (auto.TryGetTarget(out var sub))
					{
						sub.cleanup();
						auto.SetTarget(null);
					}
					auto = null;
				}
			}
		}

		// this level of wrapping is necessary, because subscriptions are in list and dictionary

		[Description("Subscription with auto-remove (when no reference).")]
		public class AutoSubscription : IDisposable
		{
			protected Subscription it { get; private set; }
			internal AutoSubscription(Subscription it)
				=> (this.it = it).auto = new WeakReference<AutoSubscription>(this);
			~AutoSubscription()
			{
				if (it?.next != null)
					UI.Collector.Add(this);
			}
			void IDisposable.Dispose()
				=> remove();
			[Description("Remove the item.")]
			public void remove()
				=> it?.remove();
			internal void cleanup()
			{
				it = null;
				GC.SuppressFinalize(this);
			}
		}
	}
}
