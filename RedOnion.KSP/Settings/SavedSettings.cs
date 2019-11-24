using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace RedOnion.KSP.Settings {
	static public class SavedSettings {
		// will need some changes about loading scripts if that gets changed!
		public static readonly string SettingsFile;
		public static readonly string BaseScriptsPath;
		static ConfigNode config;
		static ConfigNode Config
		{
			get
			{
				if (config==null)
				{
					if (!File.Exists(SettingsFile))
					{
						Directory.CreateDirectory(BaseScriptsPath);
						config = new ConfigNode();
						config.SetValue("settingsFileExists", true, true);
					}
					else
					{
						config=ConfigNode.Load(SettingsFile);
					}
				}
				//config.Save(SettingsFile);
				return config;
			}
		}

		public static void SaveToDisk()
		{
			Config.Save(SettingsFile);
			Debug.Log("Saving to disk "+Config);
		}

		static SavedSettings()
		{
			BaseScriptsPath = Path.Combine(KSPUtil.ApplicationRootPath, "GameData","RedOnion","Scripts");
			SettingsFile = Path.Combine(BaseScriptsPath, ".settings");
		}

		//static ConfigNode LoadConfig()
		//{
		//	//UnityEngine.Debug.Log("load config");
		//	ConfigNode configNode;
		//	if (!File.Exists(SettingsFile)) {
		//		Directory.CreateDirectory(BaseScriptsPath);
		//		configNode = new ConfigNode();
		//		configNode.SetValue("settingsFileExists", true,true);

		//		return configNode;
		//	}
		//	return ConfigNode.Load(SettingsFile);
		//}

		static public string LoadSetting(string settingName,string defaultValue)
		{
			if (Config.HasValue(settingName))
			{
				return Config.GetValue(settingName);
			}

			return defaultValue;
		}

		static public void SaveSetting(string settingName,string settingValue)
		{
			Config.SetValue(settingName, settingValue, true);
			//Config.Save(SettingsFile);
		}

		static public IList<string> LoadListSetting(string settingName)
		{;
			if (Config.HasNode(settingName)) {
				IList<string> values = Config.GetNode(settingName).GetValues();
				return values;
			}

			return new List<string>();
		}

		static public void SaveListSetting(string settingName,IList<string> values)
		{
			ConfigNode valuesNode = new ConfigNode();

			foreach(var value in values) {
				valuesNode.AddValue(settingName, value);
			}

			Config.SetNode(settingName, valuesNode,true);
			//Config.Save(SettingsFile);
		}
	}
}
