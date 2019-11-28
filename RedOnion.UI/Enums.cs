using System;
using System.ComponentModel;

namespace RedOnion.UI
{
	[Description("How child elements are placed.")]
	public enum Layout
	{
		[Description("No layout selected.")]
		None = 0,
	//	Stack = 1,
		[Description("Lay elements horizontally (left-to-right).")]
		Horizontal = 2,
		[Description("Lay elements verticallt (top-down).")]
		Vertical = 3,
	//	FlowHorizontal = 4,
	//	FlowVertical = 5,
	//	Table = 6
	}

	[Flags, Description("Flags for KSP scenes.")]
	public enum SceneFlags
	{
		[Description("No scene enabled.")]
		None = 0,

		[Description("Main menu - `1 << GameScenes.MAINMENU`.")]
		MainMenu = 1 << GameScenes.MAINMENU,
		[Description("Game settings - `1 << GameScenes.SETTINGS`.")]
		Settings = 1 << GameScenes.SETTINGS,
		[Description("Kerbal Space Center - `1 << GameScenes.SPACECENTER`.")]
		SpaceCenter = 1 << GameScenes.SPACECENTER,
		[Description("VAB or SPH - `1 << GameScenes.EDITOR`.")]
		Editor = 1 << GameScenes.EDITOR,
		[Description("Flight or map - `1 << GameScenes.FLIGHT`.")]
		Flight = 1 << GameScenes.FLIGHT,
		[Description("Tracking station - `1 << GameScenes.TRACKSTATION`.")]
		TrackingStation = 1 << GameScenes.TRACKSTATION,

		[Description("All game scenes")]
		All = MainMenu|Settings|SpaceCenter|Editor|Flight|TrackingStation
	}
}
