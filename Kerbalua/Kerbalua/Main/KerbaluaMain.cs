using System;
using UnityEngine;
using KSP.UI.Screens;
using Kerbalua.Gui;
using System.Collections.Generic;
using Kerbalua.Utility;

namespace Kerbalua
{

    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class KerbaluaMain : MonoBehaviour
    {
        public static void Printall(string message, IEnumerable<object> os)
        {
            foreach (var o in os) {
                Console.WriteLine(message + " " + o);
            }

        }
        public static void Printall(string message, List<string> os)
        {
            foreach (var o in os) {
                Console.WriteLine(message + " " + o);
            }
        }
        
		static Texture2D toolbarTexture=null;

		bool guiActive = false;
        KerbaluaRepl repl;

        public class KSPRaw {
            // Most of these don't work. Just testing this out.
            public FlightGlobals flightGlobals = new FlightGlobals();
            //public EditorDriver editorDriver = FindObjectOfType<EditorDriver>();
            ////public EditorFacility editorFacility = FindObjectOfType<EditorFacility>();
            //public EditorLogic editorLogic = FindObjectOfType<EditorLogic>();
            //public FlightDriver flightDriver = FindObjectOfType<FlightDriver>();
            //public Planetarium planetarium = FindObjectOfType<Planetarium>();
            //public Reputation reputation = FindObjectOfType<Reputation>();
            //public TimeWarp timeWarp = FindObjectOfType<TimeWarp>();

        }

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


            //Found on thread
            //HighLogic.SaveFolder = "test_save";
            //GamePersistence.LoadGame("persistent", HighLogic.SaveFolder, true, false);
            //EditorDriver.StartEditor(EditorFacility.SPH);
        }

        public void Start(){
            repl = new KerbaluaRepl(new KSPRaw());
        }

		// Doesn't seem to work
		//public void OnSave(ConfigNode node)
		//{
		//	node.AddValue("saveLoadFilename", repl.scriptWindow.saveLoadFilename);
		//}

		//public void OnLoad(ConfigNode node)
		//{
		//	repl.scriptWindow.saveLoadFilename = node.GetValue("saveLoadFilename");
		//}

		void LocalToggleGui()
        {
			//if (guiActive) {
			//    InputLockManager.ClearControlLocks();
			//} else {
			//    InputLockManager.SetControlLock(ControlTypes.KEYBOARDINPUT, "kerbalua");
			//}
			//IList<string> newSettings = new List<string>();
			//newSettings.Add("asdf");
			//newSettings.Add("fdas");
			//newSettings.Add("asdf1");
			//newSettings.Add("fdas1");
			//newSettings.Add("asdf2");
			//newSettings.Add("fdas2");
			//IList<string> settings = Settings.LoadListSetting("recentFiles");
			//Debug.Log("OldSettings");
			//foreach(var setting in settings) {
			//	Debug.Log(setting);
			//}
			//Settings.SaveListSetting("recentFiles", newSettings);

			guiActive = !guiActive;
        }

		void OnDestroy()
		{
			repl.OnDestroy();
		}

		void FixedUpdate()
		{
			repl.FixedUpdate();
		}

		void OnGUI()
        {

			repl.Render(guiActive);
        }
    }
}
