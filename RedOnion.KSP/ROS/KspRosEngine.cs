using System;
using System.Collections;
using System.Collections.Generic;
using RedOnion.Script;
using RedOnion.Script.Utilities;
using UE = UnityEngine;

namespace RedOnion.KSP.ROS
{
	public class KspRosEngine : Engine
	{
		public KspRosEngine(Func<IEngine, IEngineRoot> createRoot)
			: base(createRoot) { }
		public KspRosEngine(Func<KspRosEngine, IEngineRoot> createRoot)
			: base(engine => createRoot((KspRosEngine)engine)) { }
		public override void Log(string msg)
			=> UE.Debug.Log("[RedOnion] " + msg);

		private static Ionic.Zip.ZipFile ScriptsZip;
		public static string LoadScript(string path)
		{
			var data = UI.Element.ResourceFileData(System.Reflection.Assembly.GetCallingAssembly(),
				"Scripts", ref ScriptsZip, path);
			return data == null ? null : System.Text.Encoding.UTF8.GetString(data);
		}

		protected UpdateList updateList = new UpdateList();
		protected UpdateList idleList = new UpdateList();

		public Event Update
		{
			get => new Event(updateList);
			set { }
		}
		public Event Idle
		{
			get => new Event(idleList);
			set { }
		}

		public bool HasEvents
			=> updateList.Count > 0 || idleList.Count > 0;
		public void ClearEvents()
		{
			updateList.Clear();
			idleList.Clear();
		}

		int counter;
		public void FixedUpdate(int countdown = 1000, int percent = 30, int idlePercent = 50)
		{
			counter++;
			ExecutionCountdown = countdown;
			if (updateList.Count > 0)
			{
				do
				{
					DebugLog("FixedUpdate #{1}: updateList.index = {0}", updateList.index, counter);
					var call = updateList.GetNext();
					try
					{
						call.Call(null, 0);
					}
					catch (Exception ex)
					{
						updateList.Remove(call);
						Log("Exception in FixedUpdate: " + ex.Message);
						if (ex is IErrorWithLine ln)
						{
							var line = ln.Line;
							Log(line == null ? "Error at line {0}."
								: "Error at line {0}: {1}", ln.LineNumber, line);
						}
					}
				} while (!updateList.AtEnd && CountdownPercent > percent);
			}
			if (idleList.Count > 0 && CountdownPercent > idlePercent)
			{
				do
				{
					DebugLog("FixedUpdate #{1}: idleList.index = {0}", idleList.index, counter);
					var call = idleList.GetNext();
					try
					{
						call.Call(null, 0);
					}
					catch (Exception ex)
					{
						updateList.Remove(call);
						Log("Exception in FixedUpdate: " + ex.Message);
						if (ex is IErrorWithLine ln)
						{
							var line = ln.Line;
							Log(line == null ? "Error at line {0}."
								: "Error at line {0}: {1}", ln.LineNumber, line);
						}
					}
				} while (!idleList.AtEnd && CountdownPercent > idlePercent);
			}
		}
	}

	/// <summary>
	/// List of callable objects that tracks removals during enumeration
	/// </summary>
	public class UpdateList : IList<ICallable>
	{
		protected ListCore<ICallable> list;
		protected internal int index;

		public int Count => list.size;
		public bool IsEmpty => list.size == 0;
		public bool AtEnd => index >= list.size;

		public ICallable GetNext()
		{
			if (index >= list.size)
				index = 0;
			return list[index++];
		}

		public void Add(ICallable item)
			=> list.Add(item);
		public void Insert(int index, ICallable item)
		{
			if (index <= this.index && index >= 0)
				this.index++;
			list.Insert(index, item);
		}
		public void RemoveAt(int index)
		{
			if (index < this.index && index >= 0)
				this.index--;
			list.RemoveAt(index);
		}
		public bool Remove(ICallable item)
		{
			int index = IndexOf(item);
			if (index < 0)
				return false;
			RemoveAt(index);
			return true;
		}

		public ICallable this[int index]
		{
			get => list[index];
			set => list[index] = value;
		}
		public void Clear()
		{
			list.Clear();
			index = 0;
		}

		public bool Contains(ICallable item) => list.Contains(item);
		public void CopyTo(ICallable[] array, int index) => list.CopyTo(array, index);
		public int IndexOf(ICallable item) => list.IndexOf(item);
		public IEnumerator<ICallable> GetEnumerator() => list.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		bool ICollection<ICallable>.IsReadOnly => false;
	}
}
