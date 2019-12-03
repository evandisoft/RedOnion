using System;
namespace Kerbalui
{
	public static class KerbaluiSettings
	{
		public static event Action SettingsChange;
		public static float UI_SCALE => GameSettings.UI_SCALE;
		public const string DefaultFontname="";
		public const int DefaultFontsize=14;
		public static void ApplySettingsChange()
		{
			SettingsChange.Invoke();
		}
	}
}
