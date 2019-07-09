using System;
using RedOnion.KSP.Autopilot;
using KSP.UI.Screens;
using UE = UnityEngine;
using System.ComponentModel;

namespace RedOnion.KSP.Namespaces
{
	[DisplayName("KSP")]
	public static class KSP_Namespace
	{
		public static Type Vessel = typeof(Vessel);
		public static Type VesselType = typeof(VesselType);
		public static Type FlightCtrlState = typeof(FlightCtrlState);
		public static FlightGlobals FlightGlobals = FlightGlobals.fetch;
		public static FlightControl FlightControl => FlightControl.GetInstance();
		public static FlightDriver FlightDriver => FlightDriver.fetch;
		public static HighLogic HighLogic => HighLogic.fetch;
		public static Type InputLockManager = typeof(InputLockManager);
		public static Type InputLock = typeof(InputLockManager);
		public static Type StageManager = typeof(StageManager);
		public static Type EditorLogic = typeof(EditorLogic);
		public static Type EditorPanels = typeof(EditorPanels);
		public static Type ShipConstruction = typeof(ShipConstruction);
		public static Type GameScenes = typeof(GameScenes);
		public static Type PartLoader = typeof(PartLoader);
		public static PartResourceLibrary PartResourceLibrary => PartResourceLibrary.Instance;
		public static Type Time = typeof(UE.Time);
		public static Type Random = typeof(UE.Random);
		public static Type Mathf = typeof(UE.Mathf);
	}
}
