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
