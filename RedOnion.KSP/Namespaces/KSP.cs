using System;
using RedOnion.KSP.Autopilot;
using KSP.UI.Screens;
using UE = UnityEngine;
using System.ComponentModel;
using RedOnion.KSP.MathUtil;

namespace RedOnion.KSP.Namespaces
{
	[DisplayName("KSP")]
	public static class KSP_Namespace
	{
		public static readonly Type Vessel = typeof(Vessel);
		public static readonly Type VesselType = typeof(VesselType);
		public static readonly Type FlightCtrlState = typeof(FlightCtrlState);
		public static FlightGlobals FlightGlobals => FlightGlobals.fetch;
		public static FlightControl FlightControl => FlightControl.GetInstance();
		public static FlightDriver FlightDriver => FlightDriver.fetch;
		public static HighLogic HighLogic => HighLogic.fetch;
		public static readonly Type InputLockManager = typeof(InputLockManager);
		public static readonly Type InputLock = typeof(InputLockManager);
		public static readonly Type StageManager = typeof(StageManager);
		public static readonly Type EditorLogic = typeof(EditorLogic);
		public static readonly Type EditorPanels = typeof(EditorPanels);
		public static readonly Type ShipConstruction = typeof(ShipConstruction);
		public static readonly Type GameScenes = typeof(GameScenes);
		public static readonly Type PartLoader = typeof(PartLoader);
		public static PartResourceLibrary PartResourceLibrary => PartResourceLibrary.Instance;
		public static readonly Type Time = typeof(UE.Time);
		public static readonly Type Random = typeof(UE.Random);
		public static readonly Type Mathf = typeof(UE.Mathf);

		public static readonly Scalar Scalar = new Scalar();
		public static readonly Vec Vec = new Vec();
	}
}
