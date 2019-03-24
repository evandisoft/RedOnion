using System;
using UnityEngine.Events;

namespace RedOnion.UI
{
	public struct Event
	{
		readonly UnityEvent UnityEvent;
		public Event(UnityEvent unityEvent) => UnityEvent = unityEvent;
		public void Add(UnityAction call) => UnityEvent.AddListener(call);
		public void Remove(UnityAction call) => UnityEvent.RemoveListener(call);
		public void Invoke() => UnityEvent.Invoke();
		public void Clear() => UnityEvent.RemoveAllListeners();
		public void Set(UnityAction call)
		{
			Clear();
			Add(call);
		}

		public struct AddProxy
		{
			internal readonly UnityEvent Event;
			internal readonly UnityAction Action;
			public AddProxy(UnityEvent e, UnityAction a)
			{
				Event = e;
				Action = a;
			}
		}
		public struct RemoveProxy
		{
			internal readonly UnityEvent Event;
			internal readonly UnityAction Action;
			public RemoveProxy(UnityEvent e, UnityAction a)
			{
				Event = e;
				Action = a;
			}
		}
		public static AddProxy operator +(Event e, UnityAction a)
			=> new AddProxy(e.UnityEvent, a);
		public static RemoveProxy operator -(Event e, UnityAction a)
			=> new RemoveProxy(e.UnityEvent, a);
		public Event(AddProxy add)
		{
			UnityEvent = add.Event;
			Add(add.Action);
		}
		public Event(RemoveProxy remove)
		{
			UnityEvent = remove.Event;
			Remove(remove.Action);
		}
		public static implicit operator Event(AddProxy add)
			=> new Event(add);
		public static implicit operator Event(RemoveProxy remove)
			=> new Event(remove);
	}

	public struct Event<T>
	{
		readonly UnityEvent<T> UnityEvent;
		public Event(UnityEvent<T> unityEvent) => UnityEvent = unityEvent;
		public void Add(UnityAction<T> call) => UnityEvent.AddListener(call);
		public void Remove(UnityAction<T> call) => UnityEvent.RemoveListener(call);
		public void Invoke(T arg) => UnityEvent.Invoke(arg);
		public void Clear() => UnityEvent.RemoveAllListeners();
		public void Set(UnityAction<T> call)
		{
			Clear();
			Add(call);
		}

		public struct AddProxy
		{
			internal readonly UnityEvent<T> Event;
			internal readonly UnityAction<T> Action;
			public AddProxy(UnityEvent<T> e, UnityAction<T> a)
			{
				Event = e;
				Action = a;
			}
		}
		public struct RemoveProxy
		{
			internal readonly UnityEvent<T> Event;
			internal readonly UnityAction<T> Action;
			public RemoveProxy(UnityEvent<T> e, UnityAction<T> a)
			{
				Event = e;
				Action = a;
			}
		}
		public static AddProxy operator +(Event<T> e, UnityAction<T> a)
			=> new AddProxy(e.UnityEvent, a);
		public static RemoveProxy operator -(Event<T> e, UnityAction<T> a)
			=> new RemoveProxy(e.UnityEvent, a);
		public Event(AddProxy add)
		{
			UnityEvent = add.Event;
			Add(add.Action);
		}
		public Event(RemoveProxy remove)
		{
			UnityEvent = remove.Event;
			Remove(remove.Action);
		}
		public static implicit operator Event<T>(AddProxy add)
			=> new Event<T>(add);
		public static implicit operator Event<T>(RemoveProxy remove)
			=> new Event<T>(remove);
	}
}
