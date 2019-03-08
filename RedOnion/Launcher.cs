using System;
using System.Collections;
using System.IO;
using UnityEngine;
using KSP.UI.Screens;

namespace RedOnion
{
	[KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
	public class Launcher : MonoBehaviour
	{
		public static Launcher Instance { get; private set; }

		private static Texture2D icon;
		private ApplicationLauncherButton button;
		private IEnumerator adder;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
			{
				Debug.Log("RedOnion.Launcher.Awake: Instance already exists", gameObject);
				Destroy(this);
			}
		}

		private void Start()
		{
			if (icon == null)
				icon = RedOnion.UI.Window.LoadIcon(38, 38, "AppIcon.png");

			if (adder != null)
				StopCoroutine(adder);
			StartCoroutine(adder = AddButton());
		}

		private void OnDestroy()
		{
			GameEvents.onGUIApplicationLauncherUnreadifying.Remove(RemoveButton);
		}

		private IEnumerator AddButton()
		{
			while (ApplicationLauncher.Instance == null || !ApplicationLauncher.Ready)
				yield return null;

			button = ApplicationLauncher.Instance.AddModApplication(
				RunScript, StopScript, null, null, null, null,
				ApplicationLauncher.AppScenes.SPACECENTER
				| ApplicationLauncher.AppScenes.FLIGHT
				| ApplicationLauncher.AppScenes.MAPVIEW
				| ApplicationLauncher.AppScenes.VAB
				| ApplicationLauncher.AppScenes.SPH
				| ApplicationLauncher.AppScenes.TRACKSTATION,
				icon);
			GameEvents.onGUIApplicationLauncherUnreadifying.Add(RemoveButton);
			adder = null;
		}

		private void RemoveButton(GameScenes scene)
		{
			if (button == null)
				return;

			ApplicationLauncher.Instance.RemoveModApplication(button);
			button = null;
		}

		Script.Engine engine;
		bool running = false;
		private void RunScript()
		{
			if(!File.Exists("scripts/launcher.ros"))
			{
				Log("Script scripts/launcher.ros does not exist");
				return;
			}
			if (engine == null)
				engine = new Script.Engine();
			try
			{
				engine.ExecutionCountdown = 10000;
				engine.Execute(File.ReadAllText("scripts/launcher.ros"));
				running = true;
			}
			catch(Script.Parsing.ParseError err)
			{
				Log("ParseError at {0}.{1}: {2}", err.LineNumber, err.Column, err.Message);
				Log("Content of the line: " + err.Line);
			}
			catch(Exception err)
			{
				Log("Exception in engine or parser: " + err.Message);
			}
		}
		private void StopScript()
			=> running = false;

		private void FixedUpdate()
			=> RunFunction("FixedUpdate");
		private void Update()
			=> RunFunction("Update");
		private void OnGUI()
			=> RunFunction("OnGUI");
		private void RunFunction(string name)
		{
			if (!running)
				return;
			if (!engine.Root.Get(name, out var value))
				return;
			if (value.Native is Script.BasicObjects.FunctionObj fn)
			{
				try
				{
					engine.ExecutionCountdown = 1000;
					fn.Call(null, 0);
				}
				catch (Script.Engine.TookTooLong)
				{
					Log(name + " took too long");
					running = false;
				}
				catch (Exception err)
				{
					Log("Exception in {0}: {1}", name, err.Message);
					running = false;
				}
			}
		}

		private static void Log(string msg)
			=> Debug.Log("[RedOnion] Launcher." + msg);
		private static void Log(string msg, params object[] args)
			=> Debug.Log(string.Format("[RedOnion] Launcher." + msg, args));
	}
}
