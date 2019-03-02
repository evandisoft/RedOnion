using System;
using UnityEngine;
using KSP.UI.Screens;
using Kerbalua.Gui;
using System.Collections.Generic;

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

        static Texture2D toolbarTexture;
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

        void LocalToggleGui()
        {
            //if (guiActive) {
            //    InputLockManager.ClearControlLocks();
            //} else {
            //    InputLockManager.SetControlLock(ControlTypes.KEYBOARDINPUT, "kerbalua");
            //}

            guiActive = !guiActive;
        }


        public void OnGUI()
        {
            repl.Render(guiActive);
        }
    }
}
