using System;
using System.IO;

namespace Kerbalua.Utility {
	static public class Settings {
		static string baseFolderPath;
		static string settingsFile;

		static Settings()
		{
			baseFolderPath = Path.Combine(KSPUtil.ApplicationRootPath, "scripts");
			settingsFile = Path.Combine(baseFolderPath, ".settings");
		}

		static ConfigNode LoadConfig()
		{
			ConfigNode configNode;
			if (!File.Exists(settingsFile)) {
				Directory.CreateDirectory(baseFolderPath);
				configNode = new ConfigNode();
				configNode.Save(settingsFile);
				return configNode;
			}
			return ConfigNode.Load(settingsFile);
		}

		static public string LoadSetting(string settingName,string defaultValue)
		{
			ConfigNode config = LoadConfig();
			if (config.HasValue(settingName)) {
				return config.GetValue(settingName);
			}

			config.SetValue(settingName, defaultValue, true);
			config.Save(settingsFile);
			return defaultValue;
		}

		static public void SaveSetting(string settingName,string settingValue)
		{
			ConfigNode config = LoadConfig();
			config.SetValue(settingName, settingValue, true);
			config.Save(settingsFile);
		}
	}
}
