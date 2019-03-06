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
				Open, Close, null, null, null, null,
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
			if (terminal != null)
			{
				terminal.Dispose();
				terminal = null;
			}
		}

		Terminal terminal;
		private void Open()
		{
			Log("Open");
			if (terminal == null)
			{
				terminal = new Terminal();
				terminal.Closed += () => button.SetFalse();
			}
			terminal.Show();
		}

		private void Close()
		{
			Log("Close");
			terminal?.Hide();
		}

		private static void Log(string msg)
			=> Debug.Log("[RedOnion] Launcher." + msg);
		private static void Log(string msg, params object[] args)
			=> Debug.Log(string.Format("[RedOnion] Launcher." + msg, args));
	}
}
