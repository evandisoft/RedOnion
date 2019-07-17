using System;
using System.Collections.Generic;
using RedOnion.KSP.Settings;

namespace RedOnion.KSP.API
{
	public class AutoRun
	{
		const string AutoRunSettingName = "AutoRun";

		public AutoRun()
		{
		}

		public static AutoRun Instance = new AutoRun();


#pragma warning disable IDE1006 // Naming Styles

		public IList<string> scripts() => SavedSettings.LoadListSetting(AutoRunSettingName);


		public void save(IList<string> scripts)
		{
			SavedSettings.SaveListSetting(AutoRunSettingName, scripts);
		}

		public void add(string scriptname)
		{
			var s = scripts();
			s.Add(scriptname);
			save(s);
		}

#pragma warning restore IDE1006 // Naming Styles
	}
}
