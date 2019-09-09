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
		public void Clear() => UnityEvent.RemoveAllListeners();
		public void Set(UnityAction call)
		{
			Clear();
			Add(call);
		}
	}

	public struct Event<T>
	{
		readonly UnityEvent<T> UnityEvent;
		public Event(UnityEvent<T> unityEvent) => UnityEvent = unityEvent;
		public void Add(UnityAction<T> call) => UnityEvent.AddListener(call);
		public void Remove(UnityAction<T> call) => UnityEvent.RemoveListener(call);
		public void Clear() => UnityEvent.RemoveAllListeners();
		public void Set(UnityAction<T> call)
		{
			Clear();
			Add(call);
		}
	}

	public struct Event<T0, T1>
	{
		readonly UnityEvent<T0, T1> UnityEvent;
		public Event(UnityEvent<T0, T1> unityEvent) => UnityEvent = unityEvent;
		public void Add(UnityAction<T0, T1> call) => UnityEvent.AddListener(call);
		public void Remove(UnityAction<T0, T1> call) => UnityEvent.RemoveListener(call);
		public void Clear() => UnityEvent.RemoveAllListeners();
		public void Set(UnityAction<T0, T1> call)
		{
			Clear();
			Add(call);
		}
	}
}
