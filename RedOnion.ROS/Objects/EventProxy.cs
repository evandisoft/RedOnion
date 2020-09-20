using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RedOnion.ROS.Objects
{
	public interface IEventProxy
	{
		Descriptor DelegateDescriptor { get; }
		bool Add(ref Value value);
		bool Remove(ref Value value);
	}

	/// <summary>
	/// Event proxy object for instance events.
	/// </summary>
	/// <typeparam name="Obj">Type of the object the event is declared on.</typeparam>
	/// <typeparam name="Fn">Type of the event (delegate).</typeparam>
	public class EventProxy<Obj,Fn> :
		Descriptor, ISelfDescribing, IEventProxy
		where Fn : Delegate
	{
		Descriptor ISelfDescribing.Descriptor => this;
		public static Descriptor DelegateDescriptor { get; set; } = Descriptor.Of(typeof(Fn));

		public Obj Owner { get; }
		protected readonly Action<Obj, Fn> _add, _remove;
		// maybe add invoke/raise for field-events

		public EventProxy(Obj owner, Action<Obj, Fn> add, Action<Obj, Fn> remove)
		{
			Owner = owner;
			_add = add;
			_remove = remove;
		}
		public EventProxy(Obj owner, EventInfo e)
		{
			Owner = owner;
			_add = (Action<Obj, Fn>)Delegate.CreateDelegate(typeof(Action<Obj, Fn>), e.GetAddMethod());
			_remove = (Action<Obj, Fn>)Delegate.CreateDelegate(typeof(Action<Obj, Fn>), e.GetRemoveMethod());
		}

		public void Add(Fn value) => _add(Owner, value);
		public void Remove(Fn value) => _remove(Owner, value);

		Descriptor IEventProxy.DelegateDescriptor => DelegateDescriptor;
		bool IEventProxy.Add(ref Value value)
		{
			if (!value.desc.Convert(ref value, DelegateDescriptor))
				return false;
			Add((Fn)value.obj);
			return true;
		}
		bool IEventProxy.Remove(ref Value value)
		{
			if (!value.desc.Convert(ref value, DelegateDescriptor))
				return false;
			Remove((Fn)value.obj);
			return true;
		}

		public override bool Call(ref Value result, object self, in Arguments args)
		{
			if (args.Length != 1)
				return false;
			var it = args[0];
			if (!it.desc.Convert(ref it, DelegateDescriptor))
				return false;
			result = Value.Void;
			Add((Fn)it.obj);
			return true;
		}

		public override void Get(Core core, ref Value self)
		{
			if (self.idx is string name)
			{
				if (name.Equals("add", StringComparison.OrdinalIgnoreCase))
				{
					self = new Value(AddFunc.Instance);
					return;
				}
				if (name.Equals("remove", StringComparison.OrdinalIgnoreCase))
				{
					self = new Value(RemoveFunc.Instance);
					return;
				}
			}
			GetError(ref self);
		}
		public override IEnumerable<string> EnumerateProperties(object self)
		{
			yield return "add";
			yield return "remove";
		}

		public class AddFunc : Descriptor, ISelfDescribing
		{
			Descriptor ISelfDescribing.Descriptor => this;
			public static AddFunc Instance { get; } = new AddFunc();
			public override bool Call(ref Value result, object self, in Arguments args)
			{
				if (args.Length == 1 && self is EventProxy<Obj,Fn> it)
				{
					var fn = args[0];
					if (!fn.desc.Convert(ref fn, DelegateDescriptor))
						return false;
					it.Add(fn.obj as Fn);
					return true;
				}
				return false;
			}
		}
		public class RemoveFunc : Descriptor, ISelfDescribing
		{
			Descriptor ISelfDescribing.Descriptor => this;
			public static RemoveFunc Instance { get; } = new RemoveFunc();
			public override bool Call(ref Value result, object self, in Arguments args)
			{
				if (args.Length == 1 && self is EventProxy<Obj, Fn> it)
				{
					var fn = args[0];
					if (!fn.desc.Convert(ref fn, DelegateDescriptor))
						return false;
					it.Remove(fn.obj as Fn);
					return true;
				}
				return false;
			}
		}
	}

	/// <summary>
	/// Event proxy object for static events.
	/// </summary>
	/// <typeparam name="Fn">Type of the event (delegate).</typeparam>
	public class EventProxy<Fn> :
		Descriptor, ISelfDescribing, IEventProxy
		where Fn : Delegate
	{
		Descriptor ISelfDescribing.Descriptor => this;
		public static Descriptor DelegateDescriptor { get; set; } = Descriptor.Of(typeof(Fn));

		protected readonly Action<Fn> _add, _remove;
		// maybe add invoke/raise for field-events

		public EventProxy(Action<Fn> add, Action<Fn> remove)
		{
			_add = add;
			_remove = remove;
		}
		public EventProxy(EventInfo e)
		{
			_add = (Action<Fn>)Delegate.CreateDelegate(typeof(Action<Fn>), e.GetAddMethod());
			_remove = (Action<Fn>)Delegate.CreateDelegate(typeof(Action<Fn>), e.GetRemoveMethod());
		}

		public void Add(Fn value) => _add(value);
		public void Remove(Fn value) => _remove(value);

		Descriptor IEventProxy.DelegateDescriptor => DelegateDescriptor;
		bool IEventProxy.Add(ref Value value)
		{
			if (!value.desc.Convert(ref value, DelegateDescriptor))
				return false;
			Add((Fn)value.obj);
			return true;
		}
		bool IEventProxy.Remove(ref Value value)
		{
			if (!value.desc.Convert(ref value, DelegateDescriptor))
				return false;
			Remove((Fn)value.obj);
			return true;
		}

		public override bool Call(ref Value result, object self, in Arguments args)
		{
			if (args.Length != 1)
				return false;
			var it = args[0];
			if (!it.desc.Convert(ref it, DelegateDescriptor))
				return false;
			result = Value.Void;
			Add((Fn)it.obj);
			return true;
		}

		public override void Get(Core core, ref Value self)
		{
			if (self.idx is string name)
			{
				if (name.Equals("add", StringComparison.OrdinalIgnoreCase))
				{
					self = new Value(AddFunc.Instance);
					return;
				}
				if (name.Equals("remove", StringComparison.OrdinalIgnoreCase))
				{
					self = new Value(RemoveFunc.Instance);
					return;
				}
			}
			GetError(ref self);
		}
		public override IEnumerable<string> EnumerateProperties(object self)
		{
			yield return "add";
			yield return "remove";
		}

		public class AddFunc : Descriptor, ISelfDescribing
		{
			Descriptor ISelfDescribing.Descriptor => this;
			public static AddFunc Instance { get; } = new AddFunc();
			public override bool Call(ref Value result, object self, in Arguments args)
			{
				if (args.Length == 1 && self is EventProxy<Fn> it)
				{
					var fn = args[0];
					if (!fn.desc.Convert(ref fn, DelegateDescriptor))
						return false;
					it.Add(fn.obj as Fn);
					return true;
				}
				return false;
			}
		}
		public class RemoveFunc : Descriptor, ISelfDescribing
		{
			Descriptor ISelfDescribing.Descriptor => this;
			public static RemoveFunc Instance { get; } = new RemoveFunc();
			public override bool Call(ref Value result, object self, in Arguments args)
			{
				if (args.Length == 1 && self is EventProxy<Fn> it)
				{
					var fn = args[0];
					if (!fn.desc.Convert(ref fn, DelegateDescriptor))
						return false;
					it.Remove(fn.obj as Fn);
					return true;
				}
				return false;
			}
		}
	}
}
