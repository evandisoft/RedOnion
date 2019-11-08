using System;
using UnityEngine;


namespace LiveRepl.Main {
	public class KerbaluaRepl {
		public ScriptWindow scriptWindow;

		public KerbaluaRepl()
		{
			InputLockManager.ClearControlLocks();

			scriptWindow = new ScriptWindow(new Rect(100, 100, 1000, 600));
		}

		public void Print(string str)
		{
			scriptWindow.repl.outputBox.content.text += "\n" + str;
		}

		public void FixedUpdate()
		{
			scriptWindow.FixedUpdate();
		}

		public void Render(bool guiActive)
		{

			if (!guiActive) return;

			try {
				//TestWindow.TestGUI();
				scriptWindow.Update();
			} catch (Exception e) {
				//Debug.Log("yes");
				Debug.Log(e);
			}
		}

		public void OnDestroy()
		{
			scriptWindow.OnDestroy();
		}
	}
}
