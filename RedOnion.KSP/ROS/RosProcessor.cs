using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Ionic.Zip;
using RedOnion.KSP.API;
using RedOnion.ROS;
using RedOnion.ROS.Objects;
using RedOnion.ROS.Utilities;
using UE = UnityEngine;

namespace RedOnion.KSP.ROS
{
	public class RosProcessor : Processor, IProcessor
	{
		protected OS.Process _process;
		public OS.Process Process
		{
			get
			{
				if (_process == null)
				{
					_process = new OS.Process();
					_process.shutdown += Terminate;
				}
				return _process;
			}
		}

		protected override RedOnion.ROS.Objects.Globals GetGlobals()
			=> new RosGlobals();
		public override void Terminate()
		{
			if (_process != null)
			{
				_process.shutdown -= Terminate;
				_process.terminate();
				_process = null;
			}
			base.Terminate();
		}
		protected void ProcessorReset()
			=> base.Reset();

		public override bool Execute(int countdown = 1000)
		{
			OS.Process.current = Process;
			var result = base.Execute(countdown);
			OS.Process.current = null;
			return result;
		}
		public override void UpdateGraphic()
		{
			OS.Process.current = Process;
			base.UpdateGraphic();
			OS.Process.current = null;
		}
		public override void UpdatePhysics()
		{
			OS.Process.current = Process;
			base.UpdatePhysics();
			_process.UpdatePhysics();
			OS.Process.current = null;
		}

		public override void Log(string msg)
			=> UE.Debug.Log("[RedOnion] " + msg);
		static RosProcessor()
			=> Value.LogListener = msg => UE.Debug.Log("[RedOnion] " + msg);

		private static Ionic.Zip.ZipFile ScriptsZip;
		public static string LoadScript(string path)
		{
			var data = UI.Element.ResourceFileData(Assembly.GetCallingAssembly(),
				"Scripts", ref ScriptsZip, path);
			return data == null ? null : Encoding.UTF8.GetString(data);
		}
		public override string ReadScript(string path)
			=> LoadScript(path);
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
	}
}
