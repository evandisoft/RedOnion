using System;
using UnityEngine;
using KSP.UI.Screens;
using System.Collections.Generic;
using MunOS;

namespace LiveRepl
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class LiveReplMain : MonoBehaviour
    {        
		static Texture2D toolbarTexture=null;

		bool guiActive = false;

		public const string title="Live REPL";

		public delegate void ToggleGUI();
        // ToggleGui has to be reset to a new relevant value each time we 
        // enter a new scene, otherwise its referencing stuff that
        // no longer has any relevance.
        static public ToggleGUI ToggleGui;

        public void Awake()
        {
			toolbarTexture = RedOnion.UI.Element.LoadIcon(38, 38, "LiveRepl.png");
            if (ToggleGui == null) {
                ApplicationLauncher.Instance.AddModApplication(
                    () => { ToggleGui(); },
                    () => { ToggleGui(); },
                null, null, null, null,
                ApplicationLauncher.AppScenes.ALWAYS,
                toolbarTexture
                );
            }

            // We have to connect it with a function that has relevance in this
            // particular scene. If ToggleGui was already set, it was with
            // a delegate from another scene.
            ToggleGui = LocalToggleGui;

			ExecutionManager.Initialize();
        }

		ScriptWindow scriptWindow;

		public void Start()
		{

		}

		void LocalToggleGui()
        {
			if (scriptWindow.inputIsLocked)
			{
				InputLockManager.ClearControlLocks();
				scriptWindow.inputIsLocked=false;
			}
			guiActive = !guiActive;
        }

		void OnDestroy()
		{
			scriptWindow.OnDestroy();
		}

		void FixedUpdate()
		{
			scriptWindow?.FixedUpdate();
			ExecutionManager.Instance.FixedUpdate();
		}

		void OnGUI()
        {

			if (scriptWindow==null)
			{
				scriptWindow=new ScriptWindow(title);
			}

			if (!guiActive) return;

			try
			{
				scriptWindow.Update();
			}
			catch (Exception e)
			{
				Debug.Log(e);
			}
		}
    }
}
