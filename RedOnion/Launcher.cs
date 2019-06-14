using System;
using System.Collections;
using System.IO;
using UnityEngine;
using KSP.UI.Screens;
using RedOnion.KSP;
using RedOnion.KSP.ROS;
using RedOnion.ROS;

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

		RosCore core;
		private void RunScript()
		{
			var script = RosCore.LoadScript("launcher.ros");
			if (script == null)
			{
				Log("Could not load launcher.ros script");
				return;
			}
			try
			{
				core = new RosCore();
				core.Execute(script, "launcher.ros", 10000);
			}
			catch(Error err)
			{
				Log("Error at {0}: {1}", err.LineNumber+1, err.Message);
				Log("Content of the line: " + err.Line);
				StopScript();
			}
			catch (Exception err)
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
			core?.Dispose();
			core = null;
			stopping = false;
		}

		private void FixedUpdate()
		{
			core?.FixedUpdate();
		}

		private static void Log(string msg)
			=> Debug.Log("[RedOnion] Launcher." + msg);
		private static void Log(string msg, params object[] args)
			=> Debug.Log(string.Format("[RedOnion] Launcher." + msg, args));
	}
}
