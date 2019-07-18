using System;
using System.Collections.Generic;
using System.ComponentModel;
using RedOnion.KSP.Settings;

namespace RedOnion.KSP.API
{
	[Description("Used to get, set, or modify the current list of scripts that are to be autoran "
	+" whenever an engine is initialized or reset.")]
	public class AutoRun
	{
		const string AutoRunSettingName = "AutoRun";

		public AutoRun()
		{
		}

		public static AutoRun Instance = new AutoRun();


#pragma warning disable IDE1006 // Naming Styles

		[Description("Returns a list of the current autorun scripts")]
		public IList<string> scripts() => SavedSettings.LoadListSetting(AutoRunSettingName);

		[Description("Saves the given list of scripts as the new list of autorun scripts.")]
		public void save(IList<string> scripts)
		{
			SavedSettings.SaveListSetting(AutoRunSettingName, scripts);
		}

		[Description("Adds a new scriptname to the list of autorun scripts")]
		public void add(string scriptname)
		{
			var s = new List<string>(scripts());
			s.Add(scriptname);
			save(s);
		}

		[Description("Removes the given scriptname from the list of autorun sccripts")]
		public void remove(string scriptname)
		{
			var s = new List<string>(scripts());
			s.Remove(scriptname);
			save(s);
		}

#pragma warning restore IDE1006 // Naming Styles
	}
}
