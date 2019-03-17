using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Ionic.Zip;

namespace RedOnion.UI
{
	public partial class Element
	{
		protected static readonly int UILayer = LayerMask.NameToLayer("UI");

		private static UISkinDef _Skin = UISkinManager.defaultSkin;
		public static UISkinDef Skin
		{
			get => _Skin;
			set => _Skin = value ?? UISkinManager.defaultSkin;
		}

		private static readonly Dictionary<string, ZipFile>
			ResourceZipFilesCache = new Dictionary<string, ZipFile>();
		private static MemoryStream ResourceStream(Assembly asm, string path)
		{
			var zipPath = asm.Location;
			var lastDot = zipPath.LastIndexOf('.');
			if (lastDot >= 0) zipPath = zipPath.Substring(0, lastDot);
			zipPath = zipPath + ".zip";
			if (!ResourceZipFilesCache.TryGetValue(zipPath, out var zip))
			{
				if (!File.Exists(zipPath))
				{
					Debug.Log("[RedOnion] File does not exist: " + zipPath);
					return null;
				}
				zip = ZipFile.Read(zipPath);
				ResourceZipFilesCache[zipPath] = zip;
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
