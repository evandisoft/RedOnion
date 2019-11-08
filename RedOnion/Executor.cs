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
	public class Executor : MonoBehaviour
	{
		public static Executor Instance { get; private set; }
		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
			{
				Debug.Log("[RedOnion] Executor.Awake: Instance already exists", gameObject);
				Destroy(this);
			}
		}

		private RosProcessor processor;

		private void Update()
			=> processor?.UpdateGraphic();
		private void FixedUpdate()
		{
			if (processor == null)
				return;
			processor.UpdatePhysics();
			if (!processor.Paused && !processor.HasEvents)
			{
				processor.Dispose();
				processor = null;
			}
		}

		private IEnumerator waiter;
		private void Start()
		{
			if (waiter != null)
				StopCoroutine(waiter);
			StartCoroutine(waiter = StartScript("os/main.ros"));
		}

		private void OnDestroy()
			=> StopScript();

		private void StopScript()
		{
			if (waiter != null)
			{
				StopCoroutine(waiter);
				waiter = null;
			}
			if (processor != null)
			{
				processor.Dispose();
				processor = null;
			}
		}
		private IEnumerator StartScript(string path)
		{
			while (ApplicationLauncher.Instance == null || !ApplicationLauncher.Ready)
				yield return null;
			waiter = null;
			StopScript();
			var script = RosProcessor.LoadScript(path);
			if (script == null)
				yield break;
			processor = new RosProcessor();
			processor.Execute(script, path, 0);
		}


		/*
		private static Texture2D icon;
		private ApplicationLauncherButton button;
		private IEnumerator adder;

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

		private void RunScript()
		{
			var script = RosProcessor.LoadScript("launcher.ros");
			if (script == null)
			{
				Log("Could not load launcher.ros script");
				return;
			}
			try
			{
				core = new RosProcessor();
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
		*/
	}
}
