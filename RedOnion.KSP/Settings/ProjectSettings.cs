using System;
using System.IO;

namespace RedOnion.KSP.Settings
{
	public static class ProjectSettings
	{
		public static readonly string BaseProjectDir;
		public static readonly string BaseScriptsDir;

		static ProjectSettings()
		{
			BaseProjectDir=Path.Combine(KSPUtil.ApplicationRootPath, "GameData", "RedOnion");
			BaseScriptsDir=Path.Combine(BaseProjectDir, "Scripts");
		}
	}
}
