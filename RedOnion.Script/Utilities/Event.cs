using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script.Utilities
{
	public struct Event
	{
		readonly IList<ICallable> list;
		public Event(IList<ICallable> list) => this.list = list;
		public void Add(ICallable call) => list.Add(call);
		public void Add(Action call) => list.Add(new ActionWrapper(call));
		public void Remove(ICallable call) => list.Remove(call);
		public void Remove(Action call) => list.Remove(new ActionWrapper(call));
		public void Clear() => list.Clear();
		public void Set(ICallable call)
		{
			Clear();
			Add(call);
		}
		public void Set(Action call)
		{
			Clear();
			Add(call);
		}
		public void Invoke()
		{
			foreach (var call in list)
				call.Call(null, 0);
		}

		public struct AddProxy
		{
			internal readonly IList<ICallable> list;
			internal readonly ICallable call;
			public AddProxy(IList<ICallable> e, ICallable a)
			{
				list = e;
				call = a;
			}
		}
		public struct RemoveProxy
		{
			internal readonly IList<ICallable> list;
			internal readonly ICallable call;
			public RemoveProxy(IList<ICallable> e, ICallable a)
			{
				list = e;
				call = a;
			}
		}
		public static AddProxy operator +(Event e, IObject a)
			=> new AddProxy(e.list, a);
		public static AddProxy operator +(Event e, Action a)
			=> new AddProxy(e.list, new ActionWrapper(a));
		public static RemoveProxy operator -(Event e, IObject a)
			=> new RemoveProxy(e.list, a);
		public static RemoveProxy operator -(Event e, Action a)
			=> new RemoveProxy(e.list, new ActionWrapper(a));
		public Event(AddProxy p)
		{
			list = p.list;
			Add(p.call);
		}
		public Event(RemoveProxy p)
		{
			list = p.list;
			Remove(p.call);
		}
		public static implicit operator Event(AddProxy p)
			=> new Event(p);
		public static implicit operator Event(RemoveProxy p)
			=> new Event(p);

		public class ActionWrapper : ICallable, IEquatable<ActionWrapper>
		{
			Action action;
			public ActionWrapper(Action action)
				=> this.action = action;
			public Value Call(IObject self, int argc)
			{
				action();
				return new Value();
			}

			public bool Equals(ActionWrapper obj)
				=> action == obj.action;
			public override bool Equals(object obj)
				=> obj is ActionWrapper wrapper && Equals(wrapper);
			public override int GetHashCode()
				=> action?.GetHashCode() ?? ~0;
		}
	}
	public class EventObj : BasicObjects.BasicObject, IPropertyEx
	{
		protected Event it;
		public EventObj(IEngine engine, Event it)
			: base(engine, null, new Properties(StdProps))
			=> this.it = it;

		public static IDictionary<string, Value> StdProps { get; } = new Dictionary<string, Value>()
		{
			{ "add",	Value.Method<EventObj>((obj, value) => obj.it.Add(value.ToCallable())) },
			{ "remove",	Value.Method<EventObj>((obj, value) => obj.it.Remove(value.ToCallable())) },
			{ "set",	Value.Method<EventObj>((obj, value) => obj.it.Set(value.ToCallable(true))) },
			{ "clear",	Value.Method<EventObj>(obj => obj.it.Clear()) },
		};

		Value IProperty.Get(IObject self)
			=> new Value((IObject)this);
		bool IProperty.Set(IObject self, Value value)
		{
			it.Set(value.ToCallable(true));
			return true;
		}
		public bool Modify(IObject self, OpCode op, Value value)
		{
			if (op == OpCode.AddAssign)
			{
				it.Add(value.ToCallable());
				return true;
			}
			if (op == OpCode.SubAssign)
			{
				it.Remove(value.ToCallable());
				return true;
			}
			return false;
		}
	}
}
