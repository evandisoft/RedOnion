using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public enum Layout
	{
		None,
		Stack,
		Horizontal,
		Vertical,
		FlowHorizontal,
		FlowVertical
	}

	[Flags]
	public enum SceneFlags
	{
		None = 0,

		MainMenu = 1 << GameScenes.MAINMENU,
		Settings = 1 << GameScenes.SETTINGS,
		SpaceCenter = 1 << GameScenes.SPACECENTER,
		Editor = 1 << GameScenes.EDITOR,
		Flight = 1 << GameScenes.FLIGHT,
		TrackingStation = 1 << GameScenes.TRACKSTATION,

		All = MainMenu|Settings|SpaceCenter|Editor|Flight|TrackingStation
	}
}
