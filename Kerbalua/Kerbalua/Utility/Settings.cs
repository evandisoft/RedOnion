using System;
using System.IO;
using System.Collections.Generic;

namespace Kerbalua.Utility {
	static public class Settings {
		public static string BaseScriptsPath;
		public static string SettingsFile;

		static Settings()
		{
			BaseScriptsPath = Path.Combine(KSPUtil.ApplicationRootPath, "GameData/RedOnion/Scripts");
			SettingsFile = Path.Combine(BaseScriptsPath, ".settings");
		}

		static ConfigNode LoadConfig()
		{
			//UnityEngine.Debug.Log("load config");
			ConfigNode configNode;
			if (!File.Exists(SettingsFile)) {
				Directory.CreateDirectory(BaseScriptsPath);
				configNode = new ConfigNode();
				configNode.SetValue("settingsFileExists", true,true);
				configNode.Save(SettingsFile);
				return configNode;
			}
			return ConfigNode.Load(SettingsFile);
		}

		static public string LoadSetting(string settingName,string defaultValue)
		{
			ConfigNode config = LoadConfig();
			if (config.HasValue(settingName)) {
				return config.GetValue(settingName);
			}

			config.SetValue(settingName, defaultValue, true);
			config.Save(SettingsFile);
			return defaultValue;
		}

		static public void SaveSetting(string settingName,string settingValue)
		{
			ConfigNode config = LoadConfig();
			config.SetValue(settingName, settingValue, true);
			config.Save(SettingsFile);
		}

		static public IList<string> LoadListSetting(string settingName)
		{
			ConfigNode config = LoadConfig();

			if (config.HasNode(settingName)) {
				IList<string> values = config.GetNode(settingName).GetValues();
				return values;
			}

			return new List<string>();
		}

		static public void SaveListSetting(string settingName,IList<string> values)
		{
			ConfigNode config = LoadConfig();
			ConfigNode valuesNode = new ConfigNode();

			foreach(var value in values) {
				valuesNode.AddValue(settingName, value);
			}

			config.SetNode(settingName, valuesNode,true);
			config.Save(SettingsFile);
		}
	}
}
