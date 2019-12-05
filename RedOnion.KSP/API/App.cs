using KSP.UI.Screens;
using RedOnion.Attributes;
using RedOnion.KSP.ROS;
using System;
using System.ComponentModel;
using UnityEngine;

using Scenes = KSP.UI.Screens.ApplicationLauncher.AppScenes;

namespace RedOnion.KSP.API
{
	[WorkInProgress, Description("Safe API for KSP Application Launcher (toolbar/buttons). WIP")]
	public static class App
	{
		[Description("Application launcher is ready for use.")]
		public static bool ready
			=> ApplicationLauncher.Ready
			&& ApplicationLauncher.Instance != null;

		[Description("Scenes in which to have the button.")]
		public static readonly Type scenes = typeof(Scenes);
		[Description("The button should be visible in Space Center.")]
		public const Scenes center = Scenes.SPACECENTER;
		[Description("The button should be visible in flight (does not include map view).")]
		public const Scenes flight = Scenes.FLIGHT;
		[Description("The button should be visible in map view.")]
		public const Scenes map = Scenes.MAPVIEW;
		[Description("The button should be visible in VAB.")]
		public const Scenes VAB = Scenes.VAB;
		[Description("The button should be visible in SPH.")]
		public const Scenes SPH = Scenes.SPH;
		[Description("The button should be visible in VAB and SPH.")]
		public const Scenes editor = Scenes.VAB|Scenes.SPH;
		[Description("The button should always be visible.")]
		public const Scenes always = Scenes.ALWAYS;
		[Description("The button should only be visible in current scene (flight|map or VAB|SPH used if appropriate).")]
		public const Scenes auto = Scenes.NEVER;

		public static Scenes current
		{
			get
			{
				switch (HighLogic.LoadedScene)
				{
				case GameScenes.SPACECENTER:
					return Scenes.SPACECENTER;
				case GameScenes.FLIGHT:
					return Scenes.FLIGHT|Scenes.MAPVIEW;
				case GameScenes.EDITOR:
					return Scenes.VAB|Scenes.SPH;
				case GameScenes.TRACKSTATION:
					return Scenes.TRACKSTATION;
				case GameScenes.MAINMENU:
					return Scenes.MAINMENU;
				default:
					return Scenes.NEVER;
				}
			}
		}

		[Description("Default Red Onion Icon.")]
		public static Texture2D defaultIcon
			=> _defaultIcon ?? (_defaultIcon
			= UI.Element.LoadIcon(38, 38, "RedOnionLauncherIcon.png"));
		private static Texture2D _defaultIcon;

		[Description("Button handle (keep it in variable).")]
		public sealed class Button : IDisposable
		{
			internal ApplicationLauncherButton it;
			internal Button(ApplicationLauncherButton it)
				=> this.it = it;
			~Button() => Dispose(false);
			void IDisposable.Dispose()
			{
				GC.SuppressFinalize(this);
				Dispose(true);
			}
			void Dispose(bool disposing)
			{
				if (it == null)
					return;
				if (disposing)
				{
					ApplicationLauncher.Instance?.RemoveApplication(it);
					it = null;
				}
				// see UI.Window/Vector.Draw for another example of this
				else UI.Collector.Add(this);
			}
		}

		[Description("Add new app launcher button. Keep the returned object in a variable,"
			+ " the button would eventually be removed otherwise.")]
		public static Button add(
			Scenes scenes, string iconPath,
			Callback onTrue, Callback onFalse,
			Callback onHover = null, Callback onHoverOut = null,
			Callback onEnable = null, Callback onDisable = null)
		=> add(onTrue, onFalse, onHover, onHoverOut, onEnable, onDisable,
			scenes, string.IsNullOrEmpty(iconPath) ? defaultIcon
			: UI.Element.LoadIcon(38, 38, iconPath) ?? defaultIcon);

		[Description("Add new app launcher button. Keep the returned object in a variable,"
			+ " the button would eventually be removed otherwise.")]
		public static Button add(
			Callback onTrue, Callback onFalse,
			Callback onHover = null, Callback onHoverOut = null,
			Callback onEnable = null, Callback onDisable = null,
			Scenes scenes = Scenes.NEVER,
			Texture texture = null)
		{
			if (scenes == auto)
				scenes = current;
			if (ApplicationLauncher.Ready)
			{
				var it = ApplicationLauncher.Instance;
				if (it != null)
				{
					return new Button(it.AddModApplication(
						onTrue, onFalse,
						onHover, onHoverOut,
						onEnable, onDisable,
						scenes, texture ?? defaultIcon
					));
				}
			}
			var btn = new Button(null);
			RosExecutor.Instance.StartCoroutine(CallbackUtil.WaitUntil(
			() => ApplicationLauncher.Ready && ApplicationLauncher.Instance != null,
			() => btn.it = ApplicationLauncher.Instance.AddModApplication(
				onTrue, onFalse,
				onHover, onHoverOut,
				onEnable, onDisable,
				scenes, texture ?? defaultIcon
			)));
			return btn;
		}
	}
}
