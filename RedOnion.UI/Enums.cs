using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public enum Layout
	{
		None = 0,
	//	Stack = 1,
		Horizontal = 2,
		Vertical = 3,
	//	FlowHorizontal = 4,
	//	FlowVertical = 5,
	//	Table = 6
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
