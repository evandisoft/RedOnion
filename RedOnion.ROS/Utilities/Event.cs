using System;
using System.Collections.Generic;
using RedOnion.ROS.Objects;

//	Note that there is no Invoke/Call method, because functions need special treatement.
//	They cannot be called through Descriptor.Call, the Core has to use their code directly.

namespace RedOnion.ROS.Utilities
{
	/// <summary>
	/// Event handler helper that works well in scripts (e.g. LUA not having += operator)
	/// and still allow += operator if used in property with dummy set
	/// </summary>
	public struct Event : ISelfDescribing
	{
		readonly IList<Value> list;
		public Event(IList<Value> list) => this.list = list;
		public void Add(Value call) => list.Add(call);
		public void Add(Function call) => list.Add(new Value(call));
		public void Add(Action call) => list.Add(new Value(call));
		public void Remove(Value call) => list.Remove(call);
		public void Remove(Function call) => list.Remove(new Value(call));
		public void Remove(Action call) => list.Remove(new Value(call));
		public void Clear() => list.Clear();

		public void Set(Value call)
		{
			Clear();
			Add(call);
		}
		public void Set(Function call)
		{
			Clear();
			Add(call);
		}
		public void Set(Action call)
		{
			Clear();
			Add(call);
		}

		public struct AddProxy
		{
			internal readonly IList<Value> list;
			internal readonly Value call;
			internal AddProxy(IList<Value> e, Value a)
			{
				list = e;
				call = a;
			}
		}
		public struct RemoveProxy
		{
			internal readonly IList<Value> list;
			internal readonly Value call;
			internal RemoveProxy(IList<Value> e, Value a)
			{
				list = e;
				call = a;
			}
		}
		public static AddProxy operator +(Event e, Value a)
			=> new AddProxy(e.list, a);
		public static AddProxy operator +(Event e, Action a)
			=> new AddProxy(e.list, new Value(a));
		public static RemoveProxy operator -(Event e, Value a)
			=> new RemoveProxy(e.list, a);
		public static RemoveProxy operator -(Event e, Action a)
			=> new RemoveProxy(e.list, new Value(a));
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

		public Descriptor Descriptor => EventDescriptor.Instance;
		public class EventDescriptor : Descriptor.Simple
		{
			// keep Methods above Instance!
			protected static Values Methods = new Values(new Value[]
			{
				new Value(new Procedure1<Event>("add"),
					(Action<Event,Value>)((e, v) => e.Add(v))),
				new Value(new Procedure1<Event>("set"),
					(Action<Event,Value>)((e, v) => e.Set(v))),
				new Value(new Procedure1<Event>("rem"),
					(Action<Event,Value>)((e, v) => e.Remove(v))),
				new Value(new Procedure1<Event>("remove"),
					(Action<Event,Value>)((e, v) => e.Remove(v))),
				new Value(new Procedure0<Event>("clear"),
					(Action<Event>)(e => e.Clear())),
			});

			public static EventDescriptor Instance { get; } = new EventDescriptor();
			protected EventDescriptor() : base("Event", typeof(Event), Methods) { }
		}
	}
}
