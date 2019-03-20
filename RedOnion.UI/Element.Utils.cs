using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Ionic.Zip;
using System.Globalization;
using System.Diagnostics;

namespace RedOnion.UI
{
	public partial class Element
	{
		internal static void Log(string msg)
			=> Debug.Log("[RedOnion] " + msg);
		public static void Log(string msg, params object[] args)
			=> Debug.Log(string.Format(CultureInfo.InvariantCulture, "[RedOnion] " + msg, args));
		[Conditional("DEBUG")]
		public static void DebugLog(string msg)
			=> Log(msg);
		[Conditional("DEBUG")]
		public static void DebugLog(string msg, params object[] args)
			=> Log(msg, args);

		protected static readonly int UILayer = LayerMask.NameToLayer("UI");

		private static UISkinDef _Skin = UISkinManager.defaultSkin;
		public static UISkinDef Skin
		{
			get => _Skin;
			set => _Skin = value ?? UISkinManager.defaultSkin;
		}

		private static ZipFile ResourcesZip;
		private static byte[] ResourceData(Assembly asm, string path)
		{
			// assume asm.Location points to GameData/RedOnion/Plugis/RedOnion.dll (or any other dll)
			var root = Path.Combine(Path.GetDirectoryName(asm.Location), "..");
			var filePath = Path.Combine(root, path);
			if (File.Exists(filePath))
				return File.ReadAllBytes(filePath);

			// if the file does not exists, try GameData/RedOnion/Resources.zip
			if (ResourcesZip == null)
			{
				var zipPath = Path.Combine(root, "Resources.zip");
				if (!File.Exists(zipPath))
				{
					Log("Neither {0} nor {1} exists", path, zipPath);
					return null;
				}
				ResourcesZip = ZipFile.Read(zipPath);
			}
			var entry = ResourcesZip[path];
			if (entry == null)
			{
				Log("Resource {0} does not exist", path);
				return null;
			}
			var stream = new MemoryStream();
			entry.Extract(stream);
			return stream.ToArray();
		}

		public static Texture2D LoadIcon(int width, int height, string path)
		{
			var data = ResourceData(Assembly.GetCallingAssembly(), path);
			var icon = new Texture2D(width, height, TextureFormat.BGRA32, false);
			icon.LoadImage(data);
			return icon;
		}
	}
}
