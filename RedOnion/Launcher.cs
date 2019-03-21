using System;
using System.Collections;
using System.IO;
using UnityEngine;
using KSP.UI.Screens;
using RedOnion.KSP;

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
				icon = UI.Element.LoadIcon(38, 38, "RedOnionLauncherIcon.png");

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

		RuntimeEngine engine;
		private void RunScript()
		{
			var script = RuntimeEngine.LoadScript("launcher.ros");
			if (script == null)
			{
				Log("Could not load launcher.ros script");
				return;
			}
			try
			{
				engine = new RuntimeEngine();
				engine.ExecutionCountdown = 10000;
				engine.Execute(script);
			}
			catch(Script.Parsing.ParseError err)
			{
				Log("ParseError at {0}.{1}: {2}", err.LineNumber, err.Column, err.Message);
				Log("Content of the line: " + err.Line);
				StopScript();
			}
			catch(Exception err)
			{
				Log("Exception in engine or parser: " + err.Message);
				StopScript();
			}
		}

		private bool stopping;
		private void StopScript()
		{
			if (stopping)
				return;
			stopping = true;
			RunFunction("shutdown");
			engine = null;
			stopping = false;
		}

		private void FixedUpdate()
			=> RunFunction("fixedUpdate");
		private void Update()
			=> RunFunction("update");
		private void OnGUI()
			=> RunFunction("onGUI");
		private void RunFunction(string name)
		{
			if (engine == null)
				return;
			if (!engine.Root.Get(name, out var value))
				return;
			if (value.Object is Script.BasicObjects.FunctionObj fn)
			{
				try
				{
					engine.ExecutionCountdown = 1000;
					fn.Call(null, 0);
				}
				catch (Script.Engine.TookTooLong)
				{
					Log(name + " took too long");
					StopScript();
				}
				catch (Exception err)
				{
					Log("Exception in {0}: {1}", name, err.Message);
					StopScript();
				}
			}
		}

		private static void Log(string msg)
			=> Debug.Log("[RedOnion] Launcher." + msg);
		private static void Log(string msg, params object[] args)
			=> Debug.Log(string.Format("[RedOnion] Launcher." + msg, args));
	}
}
