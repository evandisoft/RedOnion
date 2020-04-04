using RedOnion.Debugging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CollectorQueue is separated to static class so that we can unit-test it

namespace RedOnion.UI
{
	/// <summary>
	/// Purpose of this collector is to dispose things on main thread.
	/// Desctructors (possibly called from different thread) can schedule things for destruction
	/// by calling Collector.Add(disposable).
	/// </summary>
	public static class CollectorQueue
	{
		static readonly object queueLock = new object();
		static List<IDisposable> queue = new List<IDisposable>();
		static List<IDisposable> queue2 = new List<IDisposable>();

		public static void Add(IDisposable item)
		{
			lock (queueLock)
			{
				queue.Add(item);
			}
		}

		public static void Collect()
		{
			List<IDisposable> process;
			lock (queueLock)
			{
				process = queue;
				if (process.Count == 0)
					return;
				queue = queue2;
				queue2 = process;
			}

			MainLogger.DebugLog("UI.Collector.Update: disposing " + process.Count);

			foreach (var item in process)
			{
				try
				{
					item.Dispose();
				}
				catch (Exception ex)
				{
					MainLogger.Log("[RedOnion] UI.Collector.Update: exception when disposing: " + ex.ToString());
				}
			}
			process.Clear();
		}
	}

	/// <summary>
	/// Purpose of this collector is to dispose things on main thread.
	/// Desctructors (possibly called from different thread) can schedule things for destruction
	/// by calling Collector.Add(disposable).
	/// </summary>
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public sealed class Collector : MonoBehaviour
	{
		public static void Add(IDisposable item)
			=> CollectorQueue.Add(item);

		public static void Coroutine(IEnumerator coroutine)
			=> instance.StartCoroutine(coroutine);

		static Collector instance;

		void Awake()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(this);
				MainLogger.LogListener = msg => Debug.Log("[RedOnion] " + msg);
			}
			else if (instance == this)
			{
				Debug.LogWarning("[RedOnion] UI.Collector.Awake: another call");
			}
			else
			{
				Debug.LogWarning("[RedOnion] UI.Collector.Awake: another instance!");
				Destroy(this);
			}
		}

		void Update()
		{
			CollectorQueue.Collect();
		}
	}
}
