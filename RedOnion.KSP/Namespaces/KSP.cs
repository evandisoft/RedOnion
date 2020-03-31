using System;
using RedOnion.KSP.Autopilot;
using KSP.UI.Screens;
using UE = UnityEngine;
using System.ComponentModel;
using RedOnion.Attributes;
using RedOnion.KSP.MathUtil;
using RedOnion.KSP.UnsafeAPI;

namespace RedOnion.KSP.Namespaces
{
	[DisplayName("KSP"), DocBuild("RedOnion.KSP/Namespaces/KSP")]
	[SafeProps,Unsafe, Description("Unsafe KSP API - see [CommonScriptApi](../../CommonScriptApi.md)")]
	public static class KSP_Namespace
	{
		[Description("UnityEngine.Time")]
		public static readonly Type Time = typeof(UE.Time);
		[Description("UnityEngine.Random")]
		public static readonly Type Random = typeof(UE.Random);
		[Description("UnityEngine.Mathf")]
		public static readonly Type Mathf = typeof(UE.Mathf);

		[Description("Math utilities.")]
		public static readonly Scalar Scalar = new Scalar();
		[Description("Vector utilities.")]
		public static readonly Vec Vec = new Vec();

		[Description("A map of planet names to planet bodies. (Unsafe API)")]
		public static BodiesDictionary bodies => BodiesDictionary.Instance;

		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_vessel.html): Vessel (class).")]
		public static readonly Type Vessel = typeof(Vessel);
		[Description("[KSP API](https://kerbalspaceprogram.com/api/_vessel_8cs.html#afa39c7ec7cc0926b332fcd2d77425edb): Vessel Type (enum).")]
		public static readonly Type VesselType = typeof(VesselType);

		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_flight_ctrl_state.html): Flight Control State (class for fly-by-wire/autopilot).")]
		public static readonly Type FlightCtrlState = typeof(FlightCtrlState);
		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_flight_globals.html): Flight Globals (for autopilot).")]
		public static FlightGlobals FlightGlobals => FlightGlobals.fetch;
		[Description("Custom autopilot.")]
		public static FlightControl FlightControl => FlightControl.Instance;
		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_flight_driver.html)")]
		public static FlightDriver FlightDriver => FlightDriver.fetch;

		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_k_s_p_1_1_u_i_1_1_screens_1_1_stage_manager.html): Staging logic.")]
		public static readonly Type StageManager = typeof(StageManager);

		[Description("[KSP API](https://kerbalspaceprogram.com/api/_high_logic_8cs.html#a0687e907db3af3681f90377d69f32090): Game scenes (enum).")]
		public static readonly Type GameScenes = typeof(GameScenes);
		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_high_logic.html): LoadedScene indicator and other global state.")]
		public static HighLogic HighLogic => HighLogic.fetch;
		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_game.html): State of the game.")]
		public static Game CurrentGame => HighLogic.CurrentGame;
		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_game_parameters.html): Parameters of the game.")]
		public static GameParameters GameParameters => CurrentGame.Parameters;

		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_game_parameters_1_1_career_params.html): Career parameters.")]
		public static GameParameters.CareerParams Career => GameParameters.Career;

		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_editor_logic.html): For use in editor (VAB/SPH).")]
		public static readonly Type EditorLogic = typeof(EditorLogic);
		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_k_s_p_1_1_u_i_1_1_screens_1_1_editor_panels.html)")]
		public static readonly Type EditorPanels = typeof(EditorPanels);
		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_ship_construction.html)")]
		public static readonly Type ShipConstruction = typeof(ShipConstruction);
		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_part_loader.html)")]
		public static readonly Type PartLoader = typeof(PartLoader);
		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_part_resource_library.html)")]
		public static PartResourceLibrary PartResourceLibrary => PartResourceLibrary.Instance;

		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_input_lock_manager.html): For locking input.")]
		public static readonly Type InputLockManager = typeof(InputLockManager);
		[Description("Alias to `InputLockManager`.")]
		public static readonly Type InputLock = typeof(InputLockManager);

		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_game_settings.html): Various KSP settings.")]
		public static readonly Type GameSettings = typeof(GameSettings);
		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_game_events.html): Various KSP events.")]
		public static readonly Type GameEvents = typeof(GameEvents);
		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_game_variables.html): Various KSP variables.")]
		public static GameVariables GameVariables => GameVariables.Instance;

		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_research_and_development.html): Science stuff.")]
		public static ResearchAndDevelopment ResearchAndDevelopment => ResearchAndDevelopment.Instance;
		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_research_and_development.html): Science stuff.")]
		public static ResearchAndDevelopment RnD => ResearchAndDevelopment.Instance;
		[Description("[KSP API](https://kerbalspaceprogram.com/api/class_science_util.html): Science utilities.")]
		public static readonly Type ScienceUtil = typeof(ScienceUtil);
		[Description("[KSP API](https://kerbalspaceprogram.com/api/_science_8cs.html): Experiment situation flags.")]
		public static readonly Type ExperimentSituations = typeof(ExperimentSituations);
	}
}
