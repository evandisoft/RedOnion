using System;
using System.ComponentModel;
using MoonSharp.Interpreter;
using RedOnion.ROS;
using RedOnion.ROS.Utilities;
using RedOnion.KSP.Parts;
using UnityEngine;
using KSP.Localization;
using RedOnion.KSP.Attributes;

namespace RedOnion.KSP.API
{
	[Description("Active vessel")]
	public class Ship : ISpaceObject, IDisposable
	{
		// for script cleanup
		[Browsable(false), MoonSharpHidden]
		public static void DisableAutopilot()
			=> active?._autopilot?.disable();

		#region Active Ship

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

		#endregion

		#region Create and destroy

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
			if (_autopilot != null)
			{
				_autopilot.Dispose();
				_autopilot = null;
			}
		}
		void SceneChange(GameScenes scene)
		{
			if (scene != GameScenes.FLIGHT)
				Dispose();
		}
		#endregion

		#region Autopilot

		[Description("Autopilot of this ship (vehicle/vessel).")]
		public Autopilot autopilot => _autopilot ?? (_autopilot = new Autopilot(this));
		protected Autopilot _autopilot;
		[Description("Current throttle (assign redirects to `Autopilot`, reads control state if autopilot disabled)")]
		public float throttle
		{
			get => _autopilot == null || float.IsNaN(_autopilot.throttle)
				? native.ctrlState.mainThrottle : _autopilot.throttle;
			set
			{
				if (_autopilot == null && float.IsNaN(value))
					return;
				autopilot.throttle = value;
			}
		}
		#endregion

		#region Native and name

		[Unsafe, Description("Native `Vessel` for unrestricted access to KSP API."
			+ " Same as `FlightGlobals.ActiveVessel` if accessed through global `ship`.")]
		public Vessel native { get; private set; }

		[Description("Name of the ship (vehicle/vessel).")]
		public string name
		{
			get
			{
				var name = native.vesselName;
				return name.StartsWith(autoLocMarker) ? Localizer.Format(name) : name;
			}
		}
		internal readonly static string autoLocMarker = "#autoLOC_";
		#endregion

		#region Parts

		[Description("All parts of this ship/vessel/vehicle.")]
		public ShipPartSet parts { get; private set; }
		[Description("Root part (same as `parts.root`).")]
		public PartBase root => parts.root;
		[Description("One of the decouplers that will get activated by nearest stage. (Same as `Parts.NextDecoupler`.)")]
		public Decoupler nextDecoupler => parts.nextDecoupler;
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

		#endregion

		#region Basic properties

		[Description("Unique identifier of the ship (vehicle/vessel). Can change when docking/undocking.")]
		public Guid ID => native.id;
		[Description("Unique identifier of the ship (vehicle/vessel). Should be same as it was before docking (after undocking).")]
		public uint PersistentID => native.persistentId;
		[Description("KSP API. Vessel type as selected by user (or automatically).")]
		public VesselType vesseltype => native.vesselType;
		[Description("Total mass of the ship (vehicle/vessel). [tons = 1000 kg]")]
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
		[Description("Dynamic pressure [atm = 101.325kPa]")]
		public double dynamicPressure => native.dynamicPressurekPa * (1.0/101.325);
		[Description("Dynamic pressure [atm = 101.325kPa]")]
		public double q => native.dynamicPressurekPa * (1.0/101.325);

		#endregion

		#region Orbit - basic

		[Description("KSP API. Orbited body.")]
		public SpaceBody body => Bodies.Instance[native.mainBody];
		ISpaceObject ISpaceObject.body => body;
		[Unsafe, Description("KSP API. Orbit parameters.")]
		public Orbit orbit => native.orbit;
		[Description("Eccentricity of current orbit.")]
		public double eccentricity => native.orbit.eccentricity;
		[Description("Semi-major axis of current orbit.")]
		public double semiMajorAxis => native.orbit.semiMajorAxis;
		[Description("Semi-minor axis of current orbit.")]
		public double semiMinorAxis => native.orbit.semiMinorAxis;
		[Description("Height above ground of highest point of current orbit).")]
		public double apoapsis => native.orbit.ApA;
		[Description("Height above ground of lowest point of current orbit).")]
		public double periapsis => native.orbit.PeA;
		[Description("Highest distance between center of orbited body and any point of current orbit.")]
		public double apocenter => native.orbit.ApR;
		[Description("Lowest distance between center of orbited body and any point of current orbit.")]
		public double pericenter => native.orbit.PeR;
		[Description("Eta to apoapsis in seconds.")]
		public double timeToAp => native.orbit.timeToAp;
		[Description("Eta to periapsis in seconds.")]
		public double timeToPe => native.orbit.timeToPe;
		[Description("Period of current orbit in seconds.")]
		public double period => native.orbit.period;
		[Description("Angle in degrees between the direction of periapsis and the current position.")]
		public double trueAnomaly => RosMath.Deg.Clamp360(native.orbit.trueAnomaly * RosMath.Rad2Deg);
		[Description("Angle in degrees between the direction of periapsis and the current position extrapolated on circular orbit.")]
		public double meanAnomaly => native.orbit.meanAnomaly;

		#endregion

		#region Orbit - vectors

		[Convert(typeof(Vector)), Description("Current position relative to active ship (so `ship.position` always reads zero).")]
		public Vector3d position => native.transform.position;
		[Convert(typeof(Vector)), Description("Current orbital velocity.")]
		public Vector3d velocity => native.obt_velocity;
		[Convert(typeof(Vector)), Description("Current surface velocity.")]
		public Vector3d surfaceVelocity => native.srf_velocity;
		[Convert(typeof(Vector)), Description("Current surface velocity (Alias to `surfaceVelocity`).")]
		public Vector3d srfVelocity => native.srf_velocity;
		[Convert(typeof(Vector)), Description("Current surface velocity (Alias to `surfaceVelocity`).")]
		public Vector3d srfvel => native.srf_velocity;
		[Description("Predicted position at specified time.")]
		[return: Convert(typeof(Vector))]
		public Vector3d positionAt(double time) => native.orbit.getPositionAtUT(time);
		[Description("Predicted velocity at specified time.")]
		[return: Convert(typeof(Vector))]
		public Vector3d velocityAt(double time) => native.orbit.getOrbitalVelocityAtUT(time);

		[Convert(typeof(Vector)), Description("Vector pointing forward (from cockpit - in the direction of the 'nose').")]
		public Vector3d forward => native.transform.up;
		[Convert(typeof(Vector)), Description("Vector pointing backward (from cockpit - in the direction of the 'tail').")]
		public Vector3d back => -native.transform.up;
		[Convert(typeof(Vector)), Description("Vector pointing up (from cockpit).")]
		public Vector3d up => -native.transform.forward;
		[Convert(typeof(Vector)), Description("Vector pointing down (from cockpit).")]
		public Vector3d down => native.transform.forward;
		[Convert(typeof(Vector)), Description("Vector pointing left (from cockpit).")]
		public Vector3d left => -native.transform.right;
		[Convert(typeof(Vector)), Description("Vector pointing right (from cockpit).")]
		public Vector3d right => native.transform.right;

		// see https://en.wikipedia.org/wiki/Axes_conventions#Ground_reference_frames_for_attitude_description
		[Convert(typeof(Vector)), Description("Vector pointing north in the plane that is tangent to sphere centered in orbited body.")]
		public Vector3d north => native.north;
		[Convert(typeof(Vector)), Description("Vector pointing east (tangent to sphere centered in orbited body).")]
		public Vector3d east => native.east;
		[Convert(typeof(Vector)), Description("Vector pointing away from orbited body (aka *up*, but we use `up` for cockpit-up).")]
		public Vector3d away => native.up;

		#endregion

		#region Pitch, heading and roll

		[Description("Current pitch / elevation (the angle between forward vector and tangent plane) [-90..+90]")]
		public double pitch
		{
			get => 90.0 - Vector3d.Angle(forward, away);
			set
			{
				if (_autopilot == null && double.IsNaN(value))
					return;
				autopilot.pitch = value;
			}
		}
		[Description("Current heading / yaw (the angle between forward and north vectors"
			+ " in tangent plane) [0..360]. Note that it can change violently around the poles.")]
		public double heading
		{
			get
			{
				var forward = this.forward;
				var north = this.north;
				var away = this.away;
				var a = Vector3d.Angle(north,
					Vector3d.Exclude(away, forward));
				if (Vector3d.Angle(north,
					Vector3d.Cross(away, forward)) < 90.0)
					a = 360.0-a;
				return a;
			}
			set
			{
				if (_autopilot == null && double.IsNaN(value))
					return;
				autopilot.heading = value;
			}
		}
		[Description("Current roll / bank (the angle between up and away vectors"
			+ " in the plane perpendicular to forward vector) [-180..+180]."
			+ " \nNote that it can change violently when facing up or down.")]
		public double roll
		{
			get
			{
				var forward = this.forward;
				var away = this.away;
				var up = this.up;
				var a = Vector3d.Angle(up,
					Vector3d.Exclude(forward, away));
				if (Vector3d.Angle(up,
					Vector3d.Cross(forward, away)) < 90.0)
					a = -a;
				return a;
			}
			set
			{
				if (_autopilot == null && double.IsNaN(value))
					return;
				autopilot.roll = value;
			}
		}

		#endregion

		#region Properties for autopilot

		[Convert(typeof(Vector)), Description("Center of mass.")]
		public Vector3d centerOfMass => native.CoMD;
		Vector3d ISpaceObject.position => centerOfMass;
		[Convert(typeof(Vector)), Description("Angular velocity (ω, deg/s), how fast the ship rotates")]
		public Vector3d angularVelocity => native.angularVelocityD * RosMath.Rad2Deg;
		[Convert(typeof(Vector)), Description("Angular momentum (L = Iω, kg⋅m²⋅deg/s=N⋅m⋅s⋅deg) aka moment of momentum or rotational momentum.")]
		public Vector3d angularMomentum => (Vector3d)native.angularMomentum * RosMath.Rad2Deg;
		[Convert(typeof(Vector)), Description("Moment of inertia (I, kg⋅m²=N⋅m⋅s²) aka angular mass or rotational inertia.")]
		public Vector3d momentOfInertia => native.MOI;

		protected double _torqueStamp;
		protected Vector3d _maxTorque, _maxVacuumTorque;
		protected Vector3d _maxAngular, _maxVacuumAngular;
		protected void UpdateTorque()
		{
			var torque = Vector3d.zero;
			var vacuum = Vector3d.zero;
			foreach (var part in native.parts)
			{
				foreach (var module in part.Modules)
				{
					if (!(module is ITorqueProvider provider))
						continue;
					provider.GetPotentialTorque(out var pos, out var neg);
					torque.x += 0.5 * (Math.Abs(pos.x) + Math.Abs(neg.x));
					torque.y += 0.5 * (Math.Abs(pos.y) + Math.Abs(neg.y));
					torque.z += 0.5 * (Math.Abs(pos.z) + Math.Abs(neg.z));
					if (!(provider is ModuleControlSurface))
					{
						vacuum.x += 0.5 * (Math.Abs(pos.x) + Math.Abs(neg.x));
						vacuum.y += 0.5 * (Math.Abs(pos.y) + Math.Abs(neg.y));
						vacuum.z += 0.5 * (Math.Abs(pos.z) + Math.Abs(neg.z));
					}
				}
			}
			_torqueStamp = Time.now;
			_maxTorque = torque * RosMath.Rad2Deg;
			_maxVacuumTorque = vacuum * RosMath.Rad2Deg;
			_maxAngular = VectorCreator.shrink(_maxTorque, momentOfInertia);
			_maxVacuumAngular = VectorCreator.shrink(_maxVacuumTorque, momentOfInertia);
		}

		[Convert(typeof(Vector)), Description("Maximal ship torque [N⋅m⋅deg=deg⋅kg⋅m²/s²] (aka moment of force or turning effect, maximum of positive and negative).")]
		public Vector3d maxTorque
		{
			get
			{
				if (Time.since(_torqueStamp) > 0)
					UpdateTorque();
				return _maxTorque;
			}
		}
		[Convert(typeof(Vector)), Description("Maximal ship torque in vacuum [N⋅m⋅deg=deg⋅kg⋅m²/s²] (ignoring control surfaces).")]
		public Vector3d maxVacuumTorque
		{
			get
			{
				if (Time.since(_torqueStamp) > 0)
					UpdateTorque();
				return _maxVacuumTorque;
			}
		}
		[Convert(typeof(Vector)), Description("Maximal angular acceleration (deg/s²)")]
		public Vector3d maxAngular
		{
			get
			{
				if (Time.since(_torqueStamp) > 0)
					UpdateTorque();
				return _maxAngular;
			}
		}
		[Convert(typeof(Vector)), Description("Maximal angular acceleration in vacuum (ignoring control surfaces).")]
		public Vector3d maxVacuumAngular
		{
			get
			{
				if (Time.since(_torqueStamp) > 0)
					UpdateTorque();
				return _maxVacuumAngular;
			}
		}

		#endregion

		#region Tools

		[Description("Translate vector/direction into local coordinates.")]
		public Vector local(ConstVector v)
			=> new Vector(native.transform.InverseTransformDirection(v));
		public Vector3d local(Vector3d v)
			=> native.transform.InverseTransformDirection(v);
		public Vector3 local(Vector3 v)
			=> native.transform.InverseTransformDirection(v);

		#endregion
	}
}
