using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Kerbalua.Gui {
	public class RecentFilesList:AutoLayoutBox {
		private string baseFolderPath = Path.Combine(KSPUtil.ApplicationRootPath, "scripts");
		const int ioDelayMillis = 5000;

		Action<string> loadAction;
		public RecentFilesList(Action<string> loadAction)
		{
			this.loadAction = loadAction;
		}

		Stopwatch ioDelayWatch = new Stopwatch();
		List<string> scriptList;
		public override void Update(Rect rect, bool vertical = true,GUIStyle style=null)
		{
			if (scriptList == null || ioDelayWatch.ElapsedMilliseconds > ioDelayMillis) {
				scriptList = new List<string>(Directory.GetFiles(baseFolderPath));
				scriptList.Sort((string s1, string s2) => {
					var t1 = Directory.GetLastWriteTime(s1);
					var t2 = Directory.GetLastWriteTime(s2);
					if (t1 < t2) return 1;
					if (t1 > t2) return -1;
					return 0;
				});

				renderables.Clear();
				renderables.Add(new Label("Recent Files:"));
				int i = 0;
				foreach (var script in scriptList) {
					string filename = Path.GetFileName(script);
					if (filename.StartsWith(".")) continue;
					if (++i > 5) break;

					renderables.Add(new Button(filename, () => loadAction.Invoke(filename)));
				}
				ioDelayWatch.Reset();
				ioDelayWatch.Start();
			}

			base.Update(rect, vertical,style);
		}

		

	}
}
