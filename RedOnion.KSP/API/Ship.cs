using System;
using RedOnion.ROS;
using KSP.UI.Screens;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.ComponentModel;
using RedOnion.KSP.Parts;
using RedOnion.ROS.Utilities;

namespace RedOnion.KSP.API
{
	[Description("Active vessel")]
	public class Ship : IDisposable
	{
		static Ship active;
		[Browsable(false), MoonSharpHidden]
		public static Ship Active
		{
			get
			{
				if (!HighLogic.LoadedSceneIsFlight)
				{
					if (active != null)
						ClearActive();
					return null;
				}
				var vessel = FlightGlobals.ActiveVessel;
				if (active?.Native != vessel)
				{
					ClearActive();
					if (vessel != null)
					{
						active = new Ship(vessel);
						Stage.SetDirty();
						GameEvents.onVesselChange.Add(active.VesselChange);
					}
				}
				return active;
			}
		}
		static void ClearActive(bool disposing = false)
		{
			if (active == null)
				return;
			GameEvents.onVesselChange.Remove(active.VesselChange);
			Stage.SetDirty();
			if (!disposing)
				active.Dispose();
			active = null;
		}
		// this would be static if EventData<T>.EvtDelegate
		// would not try to access evt.Target.GetType().Name
		// (evt.Target is null for static functions)
		void VesselChange(Vessel vessel)
			=> ClearActive();

		protected Ship(Vessel vessel)
		{
			Native = vessel;
			Parts = new ShipPartSet(this);
			GameEvents.onGameSceneLoadRequested.Add(SceneChange);
		}

		~Ship() => Dispose(false);
		[Browsable(false), MoonSharpHidden]
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (ReferenceEquals(active, this))
				ClearActive(disposing: true);
			if (Parts != null)
			{
				Parts.Dispose();
				Parts = null;
			}
			if (Native != null)
			{
				GameEvents.onGameSceneLoadRequested.Remove(SceneChange);
				Native = null;
			}
			if (autopilot != null)
			{
				autopilot.Dispose();
				autopilot = null;
			}
		}
		void SceneChange(GameScenes scene)
		{
			if (scene != GameScenes.FLIGHT)
				Dispose();
		}

		[Description("Autopilot of this ship (vehicle/wessel).")]
		public Autopilot Autopilot => autopilot ?? (autopilot = new Autopilot(this));
		protected Autopilot autopilot;
		[Description("Current throttle (assign redirects to `Autopilot`, reads control state if autopilot disabled)")]
		public float Throttle
		{
			get => autopilot == null || float.IsNaN(autopilot.Throttle)
				? Native.ctrlState.mainThrottle : autopilot.Throttle;
			set
			{
				if (autopilot == null && float.IsNaN(value))
					return;
				Autopilot.Throttle = value;
			}
		}

		[Description("Native `Vessel` for unrestricted access to KSP API."
			+ " Same as `FlightGlobals.ActiveVessel` if accessed through global `ship`.")]
		public Vessel Native { get; private set; }
		[Description("All parts of this ship/vessel/vehicle.")]
		public ShipPartSet Parts { get; private set; }
		[Description("Root part (same as `Parts.Root`).")]
		public PartBase Root => Parts.Root;
		[Description("One of the decouplers that will get activated by nearest stage. (Same as `Parts.NextDecoupler`.)")]
		public Decoupler NextDecoupler => Parts.NextDecoupler;
		[Description("Stage number of the nearest decoupler or -1. (Same as `Parts.NextDecouplerStage`.)")]
		public int NextDecouplerStage => Parts.NextDecouplerStage;

		[Description("List of all decouplers, separators, launch clamps and docks with staging."
			+ " (Docking ports without staging enabled not included.)")]
		public ReadOnlyList<Decoupler> Decouplers => Parts.Decouplers;
		[Description("List of all docking ports (regardless of staging).")]
		public ReadOnlyList<DockingPort> DockingPorts => Parts.DockingPorts;
		[Description("All engines (regardless of state).")]
		public EngineSet Engines => Parts.Engines;
		[Description("All sensors.")]
		public ReadOnlyList<Sensor> Sensors => Parts.Sensors;

		[Description("Unique identifier of the ship (wehicle/vessel). Can change when docking/undocking.")]
		public Guid ID => Native.id;
		[Description("Unique identifier of the ship (wehicle/vessel). Should be same as it was before docking (after undocking).")]
		public uint PersistentID => Native.persistentId;
		[Description("KSP API. Vessel type as selected by user (or automatically).")]
		public VesselType VesselType => Native.vesselType;
		[Description("Total mass of the ship (wehicle/vessel).")]
		public float Mass => Native.GetTotalMass();
		[Description("Wheter the ship is still packed (reduced physics).")]
		public bool Packed => Native.packed;
		[Description("Wheter the ship is landed (on the ground or on/in water).")]
		public bool Landed => Native.Landed;
		[Description("Wheter the ship is in water.")]
		public bool Splashed => Native.Splashed;
		[Description("Longitude of current position in degrees.")]
		public double Longitude => Native.longitude;
		[Description("Latitude of current position in degrees.")]
		public double Latitude => Native.latitude;
		[Description("Altitude of current position (above sea level) in meters.")]
		public double Altitude => Native.altitude;
		[Description("True height above ground in meters.")]
		public double RadarAltitude => Native.radarAltitude;

		[Description("KSP API. Orbit parameters.")]
		public Orbit Orbit => Native.orbit;
		[Description("KSP API. Orbited body.")]
		public CelestialBody Body => Native.orbit.referenceBody;
		[Description("Eccentricity of current orbit.")]
		public double Eccentricity => Native.orbit.eccentricity;
		[Description("Semi-major axis of current orbit.")]
		public double SemiMajorAxis => Native.orbit.semiMajorAxis;
		[Description("Semi-minor axis of current orbit.")]
		public double SemiMinorAxis => Native.orbit.semiMinorAxis;
		[Description("Height above ground of highest point of current orbit).")]
		public double Apoapsis => Native.orbit.ApA;
		[Description("Height above ground of lowest point of current orbit).")]
		public double Periapsis => Native.orbit.PeA;
		[Description("Highest distance between center of orbited body and any point of current orbit.")]
		public double Apocenter => Native.orbit.ApR;
		[Description("Lowest distance between center of orbited body and any point of current orbit.")]
		public double Pericenter => Native.orbit.PeR;
		[Description("Eta to apoapsis in seconds.")]
		public double TimeToAp => Native.orbit.timeToAp;
		[Description("Eta to periapsis in seconds.")]
		public double TimeToPe => Native.orbit.timeToPe;
		[Description("Period of current orbit in seconds.")]
		public double Period => Native.orbit.period;
		[Description("Angle in degrees between the direction of periapsis and the current position.")]
		public double TrueAnomaly => RosMath.Deg.Clamp360(Native.orbit.trueAnomaly * RosMath.Rad2Deg);
		[Description("Angle in degrees between the direction of periapsis and the current position extrapolated on circular orbit.")]
		public double MeanAnomaly => Native.orbit.meanAnomaly;

		[Convert(typeof(Vector)), Description("Current position.")]
		public Vector3d Position => Native.orbit.pos;
		[Convert(typeof(Vector)), Description("Current velocity.")]
		public Vector3d Velocity => Native.orbit.vel;
		[return: Convert(typeof(Vector)), Description("Predicted position at specified time")]
		public Vector3d PositionAt(double time) => Native.orbit.getPositionAtUT(time);
		[return: Convert(typeof(Vector)), Description("Predicted velocity at specified time")]
		public Vector3d VelocityAt(double time) => Native.orbit.getOrbitalVelocityAtUT(time);
	}
}
