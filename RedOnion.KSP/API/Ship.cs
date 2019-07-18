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
				if (active?.native != vessel)
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
			native = vessel;
			parts = new ShipPartSet(this);
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
			if (parts != null)
			{
				parts.Dispose();
				parts = null;
			}
			if (native != null)
			{
				GameEvents.onGameSceneLoadRequested.Remove(SceneChange);
				native = null;
			}
			if (protectedAutopilot != null)
			{
				protectedAutopilot.Dispose();
				protectedAutopilot = null;
			}
		}
		void SceneChange(GameScenes scene)
		{
			if (scene != GameScenes.FLIGHT)
				Dispose();
		}

		[Description("Autopilot of this ship (vehicle/wessel).")]
		public Autopilot autopilot => protectedAutopilot ?? (protectedAutopilot = new Autopilot(this));
		protected Autopilot protectedAutopilot;
		[Description("Current throttle (assign redirects to `Autopilot`, reads control state if autopilot disabled)")]
		public float throttle
		{
			get => protectedAutopilot == null || float.IsNaN(protectedAutopilot.Throttle)
				? native.ctrlState.mainThrottle : protectedAutopilot.Throttle;
			set
			{
				if (protectedAutopilot == null && float.IsNaN(value))
					return;
				autopilot.Throttle = value;
			}
		}

		[Description("Native `Vessel` for unrestricted access to KSP API."
			+ " Same as `FlightGlobals.ActiveVessel` if accessed through global `ship`.")]
		public Vessel native { get; private set; }
		[Description("All parts of this ship/vessel/vehicle.")]
		public ShipPartSet parts { get; private set; }
		[Description("Root part (same as `Parts.Root`).")]
		public PartBase root => parts.Root;
		[Description("One of the decouplers that will get activated by nearest stage. (Same as `Parts.NextDecoupler`.)")]
		public Decoupler nextDecoupler => parts.NextDecoupler;
		[Description("Stage number of the nearest decoupler or -1. (Same as `Parts.NextDecouplerStage`.)")]
		public int nextDecouplerStage => parts.nextDecouplerStage;

		[Description("List of all decouplers, separators, launch clamps and docks with staging."
			+ " (Docking ports without staging enabled not included.)")]
		public ReadOnlyList<Decoupler> decouplers => parts.decouplers;
		[Description("List of all docking ports (regardless of staging).")]
		public ReadOnlyList<DockingPort> dockingports => parts.dockingports;
		[Description("All engines (regardless of state).")]
		public EngineSet engines => parts.engines;
		[Description("All sensors.")]
		public ReadOnlyList<Sensor> sensors => parts.sensors;

		[Description("Unique identifier of the ship (wehicle/vessel). Can change when docking/undocking.")]
		public Guid ID => native.id;
		[Description("Unique identifier of the ship (wehicle/vessel). Should be same as it was before docking (after undocking).")]
		public uint PersistentID => native.persistentId;
		[Description("KSP API. Vessel type as selected by user (or automatically).")]
		public VesselType vesseltype => native.vesselType;
		[Description("Total mass of the ship (wehicle/vessel).")]
		public float mass => native.GetTotalMass();
		[Description("Wheter the ship is still packed (reduced physics).")]
		public bool packed => native.packed;
		[Description("Wheter the ship is landed (on the ground or on/in water).")]
		public bool landed => native.Landed;
		[Description("Wheter the ship is in water.")]
		public bool splashed => native.Splashed;
		[Description("Longitude of current position in degrees.")]
		public double longitude => native.longitude;
		[Description("Latitude of current position in degrees.")]
		public double latitude => native.latitude;
		[Description("Altitude of current position (above sea level) in meters.")]
		public double altitude => native.altitude;
		[Description("True height above ground in meters.")]
		public double radarAltitude => native.radarAltitude;

		[Description("KSP API. Orbited body.")]
		public CelestialBody body => native.mainBody;
		[Description("KSP API. Orbit parameters.")]
		public Orbit orbit => native.orbit;
		[Description("Eccentricity of current orbit.")]
		public double eccentricity => native.orbit.eccentricity;
		[Description("Semi-major axis of current orbit.")]
		public double semimajoraxis => native.orbit.semiMajorAxis;
		[Description("Semi-minor axis of current orbit.")]
		public double semiminoraxis => native.orbit.semiMinorAxis;
		[Description("Height above ground of highest point of current orbit).")]
		public double apoapsis => native.orbit.ApA;
		[Description("Height above ground of lowest point of current orbit).")]
		public double periapsis => native.orbit.PeA;
		[Description("Highest distance between center of orbited body and any point of current orbit.")]
		public double apocenter => native.orbit.ApR;
		[Description("Lowest distance between center of orbited body and any point of current orbit.")]
		public double pericenter => native.orbit.PeR;
		[Description("Eta to apoapsis in seconds.")]
		public double timetoap => native.orbit.timeToAp;
		[Description("Eta to periapsis in seconds.")]
		public double timetope => native.orbit.timeToPe;
		[Description("Period of current orbit in seconds.")]
		public double period => native.orbit.period;
		[Description("Angle in degrees between the direction of periapsis and the current position.")]
		public double trueanomaly => RosMath.Deg.Clamp360(native.orbit.trueAnomaly * RosMath.Rad2Deg);
		[Description("Angle in degrees between the direction of periapsis and the current position extrapolated on circular orbit.")]
		public double meananomaly => native.orbit.meanAnomaly;

		[Convert(typeof(Vector)), Description("Current position.")]
		public Vector3d position => native.orbit.pos;
		[Convert(typeof(Vector)), Description("Current velocity.")]
		public Vector3d velocity => native.orbit.vel;
		[return: Convert(typeof(Vector)), Description("Predicted position at specified time")]
		public Vector3d positionat(double time) => native.orbit.getPositionAtUT(time);
		[return: Convert(typeof(Vector)), Description("Predicted velocity at specified time")]
		public Vector3d velocityat(double time) => native.orbit.getOrbitalVelocityAtUT(time);
	}
}
