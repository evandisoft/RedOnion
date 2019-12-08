using System;
using UnityEngine;
using KSP.UI.Screens;
using System.Collections.Generic;
using MunOS;
using System.Diagnostics;
using MunOS.Core;

namespace LiveRepl
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class LiveReplMain2 : MonoBehaviour
    {        
		static Texture2D toolbarTexture=null;

		bool guiActive = false;

		public const string title="Live REPL";

		public delegate void ToggleGUI();
        // ToggleGui has to be reset to a new relevant value each time we 
        // enter a new scene, otherwise its referencing stuff that
        // no longer has any relevance.
        static public ToggleGUI ToggleGui;

		bool highResolution;
        public void Awake()
        {
			highResolution=Stopwatch.IsHighResolution;
			if (!highResolution)
			{
				throw new Exception("Versions 0.5.0+ of LiveRepl require a high precision timer for MunOS. " +
					"It seems your computer does not have one. Please let the maintainers know, because we are under the " +
					"impression that pretty much every computer these days has one.");
			}

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

			CoreExecMgr.Initialize();
        }

		ScriptWindow scriptWindow;

		public void Start()
		{

		}

		void LocalToggleGui()
        {
			if (scriptWindow.inputIsLocked)
			{
				InputLockManager.RemoveControlLock("LiveRepl");
				scriptWindow.inputIsLocked=false;
			}
			guiActive = !guiActive;
        }

		void OnDestroy()
		{
			scriptWindow?.OnDestroy();
		}

		void FixedUpdate()
		{
			if (!highResolution) return;

			scriptWindow?.FixedUpdate();
			CoreExecMgr.Instance.FixedUpdate();
		}

		void OnGUI()
        {
			if (!highResolution) return;

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
				UnityEngine.Debug.Log(e);
			}
		}
	}
}
