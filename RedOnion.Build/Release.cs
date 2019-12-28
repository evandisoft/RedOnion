using System;
using System.IO;
using System.IO.Compression;

namespace RedOnion.Build
{
	static class Release
	{
		public static string[] scriptWhitelist=
		{
			// Whitelisted ROS scripts
			"launch.ros","control.ros",
			// Whitelisted LUA scripts
			"majorMalfunction.lua","majorMalfunctionNative.lua","selfDestruct.lua","uibasics.lua",
		};

		internal static void Exec()
		{
			var data = Path.GetFullPath("GameData/RedOnion");
			var dlen1 = data.Length+1;

			void ZipFiles(ZipArchive zip, string name, bool indir, string[] whitelist=null)
			{
				foreach (var resource in Directory.GetFiles(name == null ? data : Path.Combine(data, name)))
				{
					var root = indir || name == null ? dlen1 : dlen1+1+name.Length;
					if (resource.Length <= root || resource[root] == '.')
						continue;
					var reduced = resource.Substring(root);
					if (whitelist!=null && whitelist.IndexOf(reduced)==-1)
					{
						continue;
					}
					Console.WriteLine("- " + reduced);
					var entry = zip.CreateEntry(reduced);
					using (var write = entry.Open())
					using (var read = new FileStream(resource, FileMode.Open))
						read.CopyTo(write);
				}
			}

			void CreateZip(string name,string[] whitelist=null)
			{
				Console.WriteLine(name + ".zip");
				using (var file = new FileStream(Path.Combine(data, name+".zip"), FileMode.Create))
				using (var zip = new ZipArchive(file, ZipArchiveMode.Create))
					ZipFiles(zip, name, false, whitelist);
			}

			CreateZip("Resources");
			CreateZip("Scripts",scriptWhitelist);

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
