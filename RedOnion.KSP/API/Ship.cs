using System;
using RedOnion.ROS;
using KSP.UI.Screens;
using MoonSharp.Interpreter;
using System.Collections.Generic;

//NOTE: Never move Instance above MemberList ;)

namespace RedOnion.KSP.API
{
	public class Ship : InteropObject
	{
		public static MemberList MemberList { get; } = new MemberList(

@"Active vessel (in flight) or ship construct (in editor).",

		new IMember[]
		{
			new Native(
				"native", "Vessel|ShipConstruct",
				"Native Vessel or ShipConstruct according to scene.",
				() => HighLogic.LoadedSceneIsFlight ? (object)FlightGlobals.ActiveVessel
				: HighLogic.LoadedSceneIsEditor ? EditorLogic.fetch.ship : null),
			new Native(
				"vessel", "Vessel",
				"Native Vessel (in flight only, null otherwise).",
				() => HighLogic.LoadedSceneIsFlight ? FlightGlobals.ActiveVessel : null),
			new Native(
				"construct", "ShipConstruct",
				"Native ShipConstruct (in editor only, null otherwise).",
				() => HighLogic.LoadedSceneIsEditor ? EditorLogic.fetch.ship : null),

			new String(
				"name",
				"Name of the ship/vessel.",
				() => Instance.Name),
			new Native(
				"id", "Guid",
				"Unique identifier of the vessel (Guid.Empty if not in flight).",
				() => Instance.ID),
			new UInt(
				"persistentId",
				"Persistent ID (provided by KSP, should be same for all ships of same design).",
				() => Instance.PersistentID),

			new Native(
				"parts", "List<Part>",
				"All parts (may get changed to our future API class).",
				() => Instance.Parts),
			new Native(
				"root", "Part",
				"Root part (usually command module).",
				() => Instance.Root),
			new Native(
				"deltaV", "VesselDeltaV",
				"KSP's native ΔV calculations.",
				() => Instance.DeltaV),
			new Native(
				"resources", "PartSet",
				"KSP's native resource part set (may get changed to our future API class).",
				() => Instance.Resources),

			new Float(
				"mass",
				"Total mass of the ship.",
				() => Instance.Mass),
			new Native(
				"type", "VesselType",
				"Vessel type (flight only, returns VesselType.Ship otherwise).",
				() => Instance.VesselType),
			new Native(
				"control", "FlightCtrlState",
				"Ship's controls (KSP native).",
				() => Instance.Control),
			new Native(
				"state", "Vessel.State",
				"Current state of the ship (0 = inactive, 1 = active, 2 = dead).",
				() => Instance.State),
			new Bool(
				"landed", "Wheter the ship is landed or not.", () => Instance.Landed),
			new Bool(
				"splashed", "Wheter the ship is splashed or not.", () => Instance.Splashed),
			new Interop(
				"stage", "Stage", "Redirects to global stage function.", () => Stage.Instance),

			new Double(
				"latitude", "Ship's latitude on current body.", () => Instance.Latitude),
			new Double(
				"longitude", "Ship's longitude on current body.", () => Instance.Longitude),
			new Double(
				"altitude", "Ship's mean-sea altitude.", () => Instance.Altitude),
			new Double(
				"radarAltitude", "Ship's altitude above ground.", () => Instance.RadarAltitude),
			new Double(
				"lat", "Alias to latitude.", () => Instance.Latitude),
			new Double(
				"lon", "Alias to longitude.", () => Instance.Longitude),
			new Double(
				"alt", "Alias to altitude.", () => Instance.Altitude),
			new Double(
				"radar", "Alias to radarAltitude.", () => Instance.RadarAltitude),
		});

		public static Ship Instance { get; } = new Ship();
		public Ship() : base(MemberList) { }

		public IShipconstruct Native
			=> HighLogic.LoadedSceneIsFlight ? (IShipconstruct)FlightGlobals.ActiveVessel
				: HighLogic.LoadedSceneIsEditor ? EditorLogic.fetch.ship : null;
		public Vessel Vessel
			=> HighLogic.LoadedSceneIsFlight ? FlightGlobals.ActiveVessel : null;
		public ShipConstruct Construct
			=> HighLogic.LoadedSceneIsEditor ? EditorLogic.fetch.ship : null;

		public Guid ID
			=> Vessel?.id ?? Guid.Empty;
		public uint PersistentID
			=> Vessel?.persistentId ?? Construct.persistentId;

		public List<Part> Parts
			=> Native.Parts;
		public Part Root
		{
			get
			{
				if (HighLogic.LoadedSceneIsFlight)
					return FlightGlobals.ActiveVessel.rootPart;
				if (HighLogic.LoadedSceneIsEditor)
				{
					var ship = EditorLogic.fetch.ship;
					if (ship != null && ship.Count > 0)
						return ship[0];
				}
				return null;
			}
		}
		public VesselDeltaV DeltaV
			=> Vessel?.VesselDeltaV ?? Construct?.vesselDeltaV;
		public PartSet Resources
			=> Vessel?.resourcePartSet ?? Construct?.resourcePartSet;
		public float Mass
			=> Vessel?.GetTotalMass() ?? Construct?.GetTotalMass() ?? float.NaN;
		public VesselType VesselType
			=> Vessel?.vesselType ?? VesselType.Ship;
		public FlightCtrlState Control
			=> Vessel?.ctrlState;
		public Vessel.State State
			=> Vessel?.state ?? Vessel.State.INACTIVE;
		public bool Packed
			=> Vessel?.packed ?? true;
		public bool Landed
			=> Vessel?.Landed ?? true;
		public bool Splashed
			=> Vessel?.Splashed ?? false;
		public double Longitude
			=> Vessel?.longitude ?? double.NaN;
		public double Latitude
			=> Vessel?.latitude ?? double.NaN;
		public double Altitude
			=> Vessel?.altitude ?? double.NaN;
		public double RadarAltitude
			=> Vessel?.radarAltitude ?? double.NaN;
	}
}
