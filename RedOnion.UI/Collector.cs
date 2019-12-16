using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedOnion.UI
{
	/// <summary>
	/// Purpose of this collector is to dispose things on main thread.
	/// Desctructors (possibly called from different thread) can schedule things for destruction
	/// by calling Collector.Add(disposable).
	/// </summary>
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public sealed class Collector : MonoBehaviour
	{
		public static void Add(IDisposable item)
		{
			lock (queueLock)
			{
				queue.Add(item);
			}
		}
		public static void Coroutine(IEnumerator coroutine)
		{
			instance.StartCoroutine(coroutine);
		}

		static Collector instance;
		Collector() { }

		void Awake()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(this);
				Debug.Log("[RedOnion] UI.Collector.Awake");
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

		void Start()
		{
			Debug.Log("[RedOnion] UI.Collector.Start");
		}

		static readonly object queueLock = new object();
		static List<IDisposable> queue = new List<IDisposable>();
		static List<IDisposable> queue2 = new List<IDisposable>();
		void Update()
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
#if DEBUG
			Debug.Log("[RedOnion] UI.Collector.Update: disposing " + process.Count);
#endif
			foreach (var item in process)
			{
				try
				{
					item.Dispose();
				}
				catch (Exception ex)
				{
					Debug.LogError("[RedOnion] UI.Collector.Update: exception when disposing: " + ex.ToString());
				}
			}
			process.Clear();
		}
	}
}
