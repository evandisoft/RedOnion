using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using RedOnion.KSP.Settings;

namespace Kerbalui.Gui {
	public class RecentFilesList:AutoLayoutBox {
		const int ioDelayMillis = 1000;
		public int MaxFiles = 10;

		Action<string> loadAction;
		public RecentFilesList(Action<string> loadAction)
		{
			this.loadAction = loadAction;
		}

		Stopwatch ioDelayWatch = new Stopwatch();
		List<string> scriptList;
		public void Update(Rect rect, string fileExtension="")
		{

			if (scriptList == null || ioDelayWatch.ElapsedMilliseconds > ioDelayMillis) {
				List<string> recentFiles = new List<string>(SavedSettings.LoadListSetting("recentFiles"));
				recentFiles.RemoveAll((string filename) => !File.Exists(Path.Combine(SavedSettings.BaseScriptsPath, filename)));
				scriptList = recentFiles;
				////scriptList = new List<string>(Directory.GetFiles(baseFolderPath));
				scriptList.Sort((string s1, string s2) => {
					var t1 = Directory.GetLastWriteTime(Path.Combine(SavedSettings.BaseScriptsPath, s1));
					var t2 = Directory.GetLastWriteTime(Path.Combine(SavedSettings.BaseScriptsPath, s2));
					if (t1 < t2) return 1;
					if (t1 > t2) return -1;
					return 0;
				});
				////
				//scriptList.RemoveAll((string str) => !recentFiles.Contains(str));

				renderables.Clear();
				//renderables.Add(new Label("Recent Files:"));
				int i = 0;
				foreach (var script in scriptList) {
					string filename = Path.GetFileName(script);
					//if (!filename.EndsWith(fileExtension)) continue;
					if (filename.StartsWith(".")) continue;
					if (++i > MaxFiles) break;

					renderables.Add(new Button(filename, () => loadAction.Invoke(filename)));
				}
				ioDelayWatch.Reset();
				ioDelayWatch.Start();
			}

			base.Update(rect);
		}

		

	}
}
