using System.Runtime.CompilerServices;
using System.IO;
using System.Reflection;
using UnityEngine;
using Ionic.Zip;
using System.Globalization;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.ComponentModel;
using RedOnion.Attributes;

[assembly: InternalsVisibleTo("RedOnion.KSP")]

namespace RedOnion.UI
{
	public partial class Element
	{
		protected internal static void Log(string msg)
			=> Debug.Log("[RedOnion] " + msg);
		protected internal static void Log(string msg, params object[] args)
			=> Debug.Log(string.Format(CultureInfo.InvariantCulture, "[RedOnion] " + msg, args));
		[Conditional("DEBUG")]
		protected internal static void DebugLog(string msg)
			=> Log(msg);
		[Conditional("DEBUG")]
		protected internal static void DebugLog(string msg, params object[] args)
			=> Log(msg, args);

		protected static readonly int UILayer = LayerMask.NameToLayer("UI"); // should be 5

		private static UISkinDef _Skin = UISkinManager.defaultSkin;
		protected internal static UISkinDef Skin
		{
			get => _Skin;
			set => _Skin = value ?? UISkinManager.defaultSkin;
		}

		protected internal static byte[] ResourceFileData(Assembly asm, string kind, ref ZipFile zip, string path)
		{
			// assume asm.Location points to GameData/RedOnion/Plugis/RedOnion.dll (or any other dll)
			var root = Path.Combine(Path.GetDirectoryName(asm.Location), "..");
			var filePath = Path.Combine(root, Path.Combine(kind, path));
			if (File.Exists(filePath))
				return File.ReadAllBytes(filePath);

			// if the file does not exists, try GameData/RedOnion/Resources.zip
			if (zip == null)
			{
				var zipPath = Path.Combine(root, kind + ".zip");
				if (!File.Exists(zipPath))
				{
					Log("Neither {0} nor {1} exists", path, zipPath);
					return null;
				}
				zip = ZipFile.Read(zipPath);
			}
			var entry = zip[path];
			if (entry == null)
			{
				Log("File {0} does not exist", path);
				return null;
			}
			var stream = new MemoryStream();
			entry.Extract(stream);
			return stream.ToArray();
		}

		private static ZipFile ResourcesZip;
		protected internal static byte[] ResourceData(Assembly asm, string path)
			=> ResourceFileData(asm, "Resources", ref ResourcesZip, path);

		[Unsafe, Description("Load icon of specified dimensions as `Texture2D` from a file (from `Resources` directory or `Resources.zip` ).")]
		public static Texture2D LoadIcon(int width, int height, string path)
		{
			var data = ResourceData(Assembly.GetCallingAssembly(), path);
			var icon = new Texture2D(width, height, TextureFormat.BGRA32, false);
			icon.LoadImage(data);
			return icon;
		}
	}
}
