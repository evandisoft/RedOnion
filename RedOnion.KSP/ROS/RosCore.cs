using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Ionic.Zip;
using RedOnion.ROS;
using RedOnion.ROS.Objects;
using RedOnion.ROS.Utilities;
using UE = UnityEngine;

namespace RedOnion.KSP.ROS
{
	public class RosCore : Core
	{
		public override void Log(string msg)
			=> UE.Debug.Log("[RedOnion] " + msg);

		private static Ionic.Zip.ZipFile ScriptsZip;
		public static string LoadScript(string path)
		{
			var data = UI.Element.ResourceFileData(Assembly.GetCallingAssembly(),
				"Scripts", ref ScriptsZip, path);
			return data == null ? null : Encoding.UTF8.GetString(data);
		}
		public static List<string> EnumerateScripts()
		{
			// assume asm.Location points to GameData/RedOnion/Plugis/RedOnion.dll (or any other dll)
			var root = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(
				Assembly.GetCallingAssembly().Location), "../Scripts"));
			var raw = new List<string>();
			foreach (var path in Directory.GetFiles(root))
			{
				if (path.Length <= root.Length+1 || path[root.Length+1] == '.')
					continue;
				raw.Add(path.Substring(root.Length+1));
			}

			// GameData/RedOnion/Scripts.zip
			if (ScriptsZip == null)
			{
				var zipPath = Path.GetFullPath(Path.Combine(root, "../Scripts.zip"));
				if (!File.Exists(zipPath))
					return raw;
				ScriptsZip = ZipFile.Read(zipPath);
			}
			var map = new HashSet<string>(raw);
			foreach (var entry in ScriptsZip.Entries)
			{
				map.Add(entry.FileName);
			}
			raw.Clear();
			raw.AddRange(map);
			return raw;
		}

		protected EventList updateList = new EventList();
		protected EventList idleList = new EventList();

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
		public event Func<Core, bool> Shutdown;
		public void Reset()
		{
			var shutdown = Shutdown;
			if (shutdown != null)
			{
				foreach (Func<Core, bool> handler in shutdown.GetInvocationList())
				{
					try
					{
						if (!handler(this))
							Shutdown -= handler;
					}
					catch (Exception ex)
					{
						Log("Exception in Shutdown: " + ex.Message);
						if (ex is Error err)
						{
							var line = err.Line;
							Log(line == null ? "Error at line {0}."
								: "Error at line {0}: {1}", err.LineNumber, line);
						}
						Shutdown -= handler;
					}
				}
			}
			ClearEvents();
			Globals = new RosGlobals();
		}
		public RosCore()
			=> Globals = new RosGlobals();
		public RosCore(RosGlobals globals)
			=> Globals = globals;

		public int UpdateCountdown { get; set; } = 1000;
		public int StepCountdown { get; set; } = 100;
		public int UpdatePercent { get; set; } = 50;
		public int IdlePercent { get; set; } = 80;
		public TimeSpan UpdateTimeout { get; set; } = TimeSpan.FromMilliseconds(10.0);

		public int TotalCountdown { get; protected set; }
		public int CountdownPercent => 100 * TotalCountdown / UpdateCountdown;
		public double AverageMillis { get; protected set; }
		public double PeakMillis { get; protected set; }
		Stopwatch watch = new Stopwatch();
		int idleSkipped;
		public void FixedUpdate()
		{
			TotalCountdown = UpdateCountdown;
			watch.Reset();
			watch.Start();
			if (updateList.Count > 0)
			{
				do
				{
					ref var call = ref updateList.GetNext();
					if (!Call(ref call))
						updateList.Remove(call.Value);
				} while (!updateList.AtEnd
				&& CountdownPercent > UpdatePercent
				&& watch.Elapsed < UpdateTimeout);
			}
			if (idleList.IsEmpty)
				idleSkipped = 0;
			else if (CountdownPercent <= IdlePercent && idleSkipped < 10)
				idleSkipped++;
			else
			{
				idleSkipped = 0;
				do
				{
					ref var call = ref idleList.GetNext();
					if (!Call(ref call))
						idleList.Remove(call.Value);
				} while (!idleList.AtEnd
				&& CountdownPercent > IdlePercent
				&& watch.Elapsed < UpdateTimeout);
			}
			do
			{
				if (Exit != ExitCode.Countdown && Exit != ExitCode.Yield)
					break;
				var countdown = Math.Min(StepCountdown, TotalCountdown);
				Execute(countdown);
				TotalCountdown -= (countdown - Countdown);
			} while (TotalCountdown > 0 && watch.Elapsed < UpdateTimeout);
			watch.Stop();
			var milli = watch.Elapsed.TotalMilliseconds;
			if (AverageMillis <= 0)
				AverageMillis = milli;
			else AverageMillis = (AverageMillis * 9 + milli) * 0.1;
			if (milli > PeakMillis)
				PeakMillis = milli;
		}
		private bool Call(ref EventList.Element e)
		{
			try
			{
				if (!e.Value.IsFunction)
				{
					var result = Value.Void;
					e.Value.desc.Call(ref result, null, new Arguments(Arguments, 0));
				}
				else if (e.Core != null)
				{
					do
					{
						if (Exit != ExitCode.Countdown && Exit != ExitCode.Yield)
							break;
						var countdown = Math.Min(StepCountdown, TotalCountdown);
						e.Core.Execute(countdown);
						TotalCountdown -= (countdown - Countdown);
					} while (TotalCountdown > 0 && watch.Elapsed < UpdateTimeout);
					if (Exit != ExitCode.Countdown && Exit != ExitCode.Yield)
					{
						// TODO: pool
						e.Core = null;
					}
				} else
				{
					e.Core = new Core(Globals);
					var countdown = Math.Min(StepCountdown, TotalCountdown);
					var fn = e.Value.obj as Function;
					e.Core.Execute(fn.Code, countdown);
				}
			}
			catch (Exception ex)
			{
				Log("Exception in FixedUpdate: " + ex.Message);
				if (ex is Error err)
				{
					var line = err.Line;
					Log(line == null ? "Error at line {0}."
						: "Error at line {0}: {1}", err.LineNumber, line);
				}
				return false;
			}
			return true;
		}
	}

	/// <summary>
	/// List of callable objects that tracks removals during enumeration
	/// </summary>
	public class EventList : IList<Value>
	{
		//TODO: doubly-linked list (for fast remove) with free-list (pool to save GC)
		public struct Element: IEquatable<Element>
		{
			/// <summary>
			/// Function or Action
			/// </summary>
			public readonly Value Value;
			/// <summary>
			/// Core it is currently running on (if yielding)
			/// </summary>
			public Core Core;

			public Element(Value value)
			{
				Value = value;
				Core = null;
			}

			public bool Equals(Element e)
				=> Value.Equals(e.Value);
			public override bool Equals(Object o)
				=> o is Element e && Equals(e);
			public override int GetHashCode()
				=> Value.GetHashCode();
		}
		protected ListCore<Element> list;
		protected internal int index;

		public int Count => list.size;
		public bool IsEmpty => list.size == 0;
		public bool AtEnd => index >= list.size;

		public ref Element GetNext()
		{
			if (index >= list.size)
				index = 0;
			return ref list.items[index++];
		}

		public void Add(Value item)
			=> list.Add(new Element(item));
		public void Insert(int index, Value item)
		{
			if (index <= this.index && index >= 0)
				this.index++;
			list.Insert(index, new Element(item));
		}
		public void RemoveAt(int index)
		{
			if (index < this.index && index >= 0)
				this.index--;
			list.RemoveAt(index);
		}
		public bool Remove(Value item)
		{
			int index = IndexOf(item);
			if (index < 0)
				return false;
			RemoveAt(index);
			return true;
		}

		public Value this[int index]
		{
			get => list[index].Value;
			set => list[index] = new Element(value);
		}
		public void Clear()
		{
			list.Clear();
			index = 0;
		}

		public bool Contains(Value item)
		{
			foreach (var e in list)
				if (e.Value.Equals(item))
					return true;
			return false;
		}
		public void CopyTo(Value[] array, int index)
		{
			foreach (var e in list)
				array[index++] = e.Value;
		}
		public int IndexOf(Value item)
		{
			for (int i = 0; i < list.size; i++)
				if (list.items[i].Value.Equals(item))
					return i;
			return -1;
		}
		public IEnumerator<Value> GetEnumerator()
		{
			foreach (var e in list)
				yield return e.Value;
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		bool ICollection<Value>.IsReadOnly => false;
	}
}
