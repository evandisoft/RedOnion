using System;
using UnityEngine;
using KSP.UI.Screens;
using System.Collections.Generic;

namespace LiveRepl.Main
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
        }

		ScriptWindow scriptWindow;

		public void Start()
		{
			scriptWindow=new ScriptWindow(title);
		}

		void LocalToggleGui()
        {
			guiActive = !guiActive;
        }

		void OnDestroy()
		{
			scriptWindow.OnDestroy();
		}

		void FixedUpdate()
		{
			scriptWindow.FixedUpdate();
		}

		void OnGUI()
        {

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
