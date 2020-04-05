using System;
using System.IO;
using System.IO.Compression;

namespace RedOnion.Build
{
	static class Release
	{
		/// <summary>
		/// A list of scripts to be distributed with a release. This is to allow us to use test-scripts and
		/// our own playthrough scripts without having to delete them every time we release.
		/// 
		/// No script will be distributed for release without being in this whitelist.
		/// 
		/// Small scripts that are just for tutorials should just have their code be in the tutorial text, 
		/// which can be copy-pasted in to run it, rather than being whitelisted here.
		/// </summary>
		public static string[] scriptWhitelist=
		{
			// Whitelisted ROS scripts
			"launch.ros","control.ros",
			// Whitelisted LUA scripts
		};

		internal static void Exec()
		{
			var data = Path.GetFullPath("GameData/RedOnion");
			var dlen1 = data.Length+1; // includes final dir separator
			var sln = Path.GetFullPath(".");
			var slnlen1 = sln.Length+1; // includes final dir separator

			void ZipFiles(ZipArchive zip, string name, bool indir, string[] whitelist = null)
			{
				// "/GameData/RedOnion/Resources" first
				var dir = name == null ? data : Path.Combine(data, name);
				bool second = false;
			again:
				foreach (var resource in Directory.Exists(dir) ? Directory.GetFiles(dir) : new string[0])
				{
					var root = indir || name == null ? dlen1 : (second ? slnlen1 : dlen1) + name.Length+1;
					// ignore hidden files
					if (resource.Length <= root || resource[root] == '.')
						continue;
					// get pure name (and convert to zip/linux notation)
					var reduced = resource.Substring(root).Replace('\\', '/');
					// ignore scripts that are not explicitly white-listed
					if (whitelist != null && whitelist.IndexOf(reduced) < 0)
						continue;
					// add to zip
					Console.WriteLine("- " + reduced);
					var entry = zip.CreateEntry(reduced);
					using (var write = entry.Open())
					using (var read = new FileStream(resource, FileMode.Open))
						read.CopyTo(write);
				}
				// "/Resources" as second
				if (!second && !indir && name != null)
				{
					second = true;
					dir = Path.Combine(sln, name);
					goto again;
				}
			}

			void CreateZip(string name, string[] whitelist = null)
			{
				Console.WriteLine(name + ".zip");
				using (var file = new FileStream(Path.Combine(data, name+".zip"), FileMode.Create))
				using (var zip = new ZipArchive(file, ZipArchiveMode.Create))
					ZipFiles(zip, name, false, whitelist);
			}

			CreateZip("Resources");
			CreateZip("Scripts", scriptWhitelist);

			Console.WriteLine("RedOnion.zip");
			using (var file = new FileStream("RedOnion.zip", FileMode.Create))
			using (var zip = new ZipArchive(file, ZipArchiveMode.Create))
			{
				ZipFiles(zip, null, true);
				ZipFiles(zip, "Plugins", true);
				ZipFiles(zip, "Licenses", true);
			}
		}
	}
}
