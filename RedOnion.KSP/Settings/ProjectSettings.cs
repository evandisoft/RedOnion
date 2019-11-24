using System;
using System.IO;

namespace RedOnion.KSP.Settings
{
	public static class ProjectSettings
	{
		public static readonly string BaseProjectDir;

		static ProjectSettings()
		{
			BaseProjectDir=Path.Combine(KSPUtil.ApplicationRootPath, "GameData", "RedOnion");
		}
	}
}
