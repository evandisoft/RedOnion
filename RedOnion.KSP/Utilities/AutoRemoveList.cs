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
	[Description("Designed for events with auto-remove on process shutdown, but can be used with any type of elements.")]
	public class AutoRemoveList<T> : IOperators, IEnumerable<T>
	{
		protected Dictionary<T, Subscription> hooks;
		protected Subscription first;

		[Description("Number of subscription.")]
		public int count => hooks.Count;

		[Description("Subscribe to the list. Similar to `add` but returns auto-remove subscription.")]
		public Subscription subscribe(T value)
		{
			var hook = add(value);
			if (hook != null)
				hook.auto = true;
			return hook;
		}

		[Description("Add new item. Returns subscribtion with auto-remove disabled (or null for duplicit item).")]
		public virtual Subscription add(T value)
		{
			if (first == null)
			{
				var hook = new Subscription(value, this);
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
				var hook = new Subscription(value, this);
				hook.next = first;
				hook.prev = first.prev;
				hook.prev.next = hook;
				first.prev = hook;
				hooks[value] = hook;
				return hook;
			}
		}
		protected static Subscription next(Subscription hook) => hook.next;
		protected static Subscription prev(Subscription hook) => hook.prev;
		protected static Subscription next(Subscription hook, Subscription next) => hook.next = next;
		protected static Subscription prev(Subscription hook, Subscription prev) => hook.prev = prev;

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
			[Description("The action")]
			public T item;
			protected AutoRemoveList<T> owner;
			protected MunProcess process;

			bool _auto;
			public bool auto
			{
				get => _auto;
				set
				{
					if (_auto == value)
						return;
					_auto = value;
					if (value)
						GC.ReRegisterForFinalize(this);
					else GC.SuppressFinalize(this);
				}
			}

			internal Subscription(T action, AutoRemoveList<T> owner)
			{
				this.item = action;
				this.owner = owner;
				if ((process = MunProcess.Current) == null)
					return;
				process.shutdown += remove;
				GC.SuppressFinalize(this);
			}
			~Subscription() => UI.Collector.Add(this);
			void IDisposable.Dispose()
				=> remove();
			public void remove()
			{
				if (next == prev)
				{
					if (next == null)
						return;
					owner.first = null;
				}
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
				if (auto)
					GC.SuppressFinalize(this);
				if (process == null)
					return;
				process.shutdown -= remove;
				process = null;
			}
		}
	}
}
