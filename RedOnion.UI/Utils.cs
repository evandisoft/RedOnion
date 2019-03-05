using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Ionic.Zip;

namespace RedOnion.UI
{
	public partial class Element
	{
		private static readonly Dictionary<string, ZipFile>
			ResourceZipFilesCache = new Dictionary<string, ZipFile>();
		private static MemoryStream ResourceStream(Assembly asm, string path)
		{
			var zipPath = Path.GetFileNameWithoutExtension(asm.Location) + ".zip";
			if (!ResourceZipFilesCache.TryGetValue(zipPath, out var zip))
			{
				if (!File.Exists(zipPath))
				{
					Debug.Log("[RedOnion] File does not exist: " + zipPath);
					return null;
				}
				zip = ZipFile.Read(path);
				ResourceZipFilesCache[path] = zip;
			}
			var entry = zip[path];
			if (entry == null)
			{
				Debug.Log("[RedOnion] Entry " + path + " does not exist in " + zipPath);
				return null;
			}
			var stream = new MemoryStream();
			entry.Extract(stream);
			return stream;
		}

		public static Texture2D LoadIcon(int width, int height, string path)
		{
			var stream = ResourceStream(Assembly.GetCallingAssembly(), path);
			if (stream == null)
				return null;
			var icon = new Texture2D(width, height, TextureFormat.BGRA32, false);
			icon.LoadImage(stream.ToArray());
			return icon;
		}
	}
}
