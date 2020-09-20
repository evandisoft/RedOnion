using System;
using System.ComponentModel;
using MunSharp.Interpreter;
using RedOnion.Attributes;
using RedOnion.ROS.Utilities;
using RedOnion.KSP.Parts;
using UnityEngine;
using KSP.Localization;
using System.Collections.Generic;
using RedOnion.ROS;
using RedOnion.Collections;
using RedOnion.Debugging;

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
				var vessel = HighLogic.LoadedSceneIsFlight ? FlightGlobals.ActiveVessel : null;
				if (!vessel)
				{
					if (active != null)
						ClearActive();
					return null;
				}
				if (active?.native != vessel)
				{
					ClearActive();
					if (vessel != null)
						FromVessel(vessel);
				}
				return active;
			}
		}
		static void ClearActive()
		{
			if (active == null)
				return;
			GameEvents.onVesselChange.Remove(active.VesselChange);
			Stage.SetDirty("Ship.ClearActive");
			active = null;
		}
		void VesselChange(Vessel vessel)
			=> ClearActive();
		#endregion

		#region Create and destroy

		// could use ConditionalWeakTable here, but we track game events and want to keep expensive Ship objects alive
		protected static readonly Dictionary<Vessel, Ship>
			cache = new Dictionary<Vessel, Ship>();
		protected static Hooks hooks;
		protected class Hooks : IDisposable
		{
			public Hooks()
			{
				hooks = this;
				GameEvents.onGameSceneLoadRequested.Add(SceneChange);
				GameEvents.onVesselDestroy.Add(VesselDestroy);
				GameEvents.onVesselWillDestroy.Add(VesselDestroy);
			}
			~Hooks() => Dispose(false);
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			protected virtual void Dispose(bool disposing)
			{
				hooks = null;
				GameEvents.onGameSceneLoadRequested.Remove(SceneChange);
				GameEvents.onVesselDestroy.Remove(VesselDestroy);
				GameEvents.onVesselWillDestroy.Remove(VesselDestroy);
			}
			public void SceneChange(GameScenes scene)
			{
				if (scene == GameScenes.FLIGHT)
					return;
				var ships = new ListCore<Ship>();
				foreach (var pair in cache)
					ships.Add(pair.Value);
				cache.Clear();
				foreach (var ship in ships)
					ship.Dispose();
				Dispose();
			}
			public void VesselDestroy(Vessel vessel)
			{
				if (cache.TryGetValue(vessel, out var ship))
				{
					ship.Dispose();
					if (cache.Count == 0)
						Dispose();
				}
			}
		}

		public static Ship FromVessel(Vessel vessel)
		{
			if (cache.TryGetValue(vessel, out var ship))
			{
				if (active == null && ship.native == FlightGlobals.ActiveVessel)
				{
					active = ship;
					GameEvents.onVesselChange.Add(ship.VesselChange);
					MainLogger.DebugLog("Reusing active vessel {0}/{1} named {2}.", ship.native.id, ship.native.persistentId, ship.name);
				}
				return ship;
			}
			return new Ship(vessel);
		}
		protected Ship(Vessel vessel)
		{
			native = vessel;
			parts = new ShipPartSet(this);
			cache[vessel] = this;
			if (hooks == null)
				new Hooks();

			if (FlightGlobals.ActiveVessel != native)
			{
				MainLogger.DebugLog("Creating Ship for vessel id {0}/{1} named {2}.", native.id, native.persistentId, name);
				return;
			}
			MainLogger.DebugLog("Creating Ship for active vessel {0}/{1} named {2}.", native.id, native.persistentId, name);
			active = this;
			GameEvents.onVesselChange.Add(VesselChange);
		}

		~Ship() => Dispose(false);
		[Browsable(false), MoonSharpHidden]
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
			{
				UI.Collector.Add(this);
				return;
			}
			MainLogger.DebugLog("Disposing Ship for vessel id {0}/{1} named {2}.", native.id, native.persistentId, name);
			cache.Remove(native);
			if (ReferenceEquals(active, this))
				ClearActive();
			if (parts != null)
			{
				parts.Dispose();
				parts = null;
			}
			if (_autopilot != null)
			{
				_autopilot.Dispose();
				_autopilot = null;
			}
			native = null;
		}
		#endregion

		#region Native, name and target

		[Unsafe, Description("Native `Vessel` for unrestricted access to [KSP API](https://kerbalspaceprogram.com/api/class_vessel.html)."
			+ " Same as `FlightGlobals.ActiveVessel` if accessed through global `ship`.")]
		public Vessel native { get; private set; }
		public static implicit operator Vessel(Ship ship) => ship?.native;

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

		[WorkInProgress, Description("Target of active ship. Null if none.")]
		public object target
		{
			get
			{
				if (!HighLogic.LoadedSceneIsFlight)
					return null;
				var target = native.targetObject;
				if (target is CelestialBody body)
					return Bodies.Instance[body];
				if (target is Vessel vessel)
					return FromVessel(vessel);
				if (target is PartModule dock)
				{
					var part = dock.part;
					return FromVessel(part.vessel).parts[part];
				}
				return null;
			}
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
		[Description("Indicator that ship is controllable.")]
		public bool controllable => native.IsControllable;
		#endregion

		#region Parts

		[Description("All parts of this ship/vessel/vehicle.")]
		public ShipPartSet parts { get; private set; }
		[Description("Root part (same as `parts.root`).")]
		public PartBase root => parts.root;
		[Description("Controlling part (same as `parts.control`).")]
		public PartBase controlPart => parts.control;
		[Description("One of the decouplers that will get activated by nearest stage. (Same as `Parts.NextDecoupler`.)")]
		public LinkPart nextDecoupler => parts.nextDecoupler;
		[Description("Stage number of the nearest decoupler or -1. (Same as `Parts.NextDecouplerStage`.)")]
		public int nextDecouplerStage => parts.nextDecouplerStage;
		[Description("Current stage number. (Same as `stage.number` if active ship.)")]
		public int currentStage => native.currentStage;
		[WorkInProgress, Description("Parts per stage (by `decoupledin+1`).")]
		public Stages stages => parts.stages;

		[Description("List of all decouplers, separators, launch clamps and docks with staging."
			+ " (Docking ports without staging enabled not included.)")]
		public ReadOnlyList<LinkPart> decouplers => parts.decouplers;
		[Description("List of all docking ports (regardless of staging).")]
		public ReadOnlyList<DockingPort> dockingports => parts.dockingports;
		[Description("All engines (regardless of state).")]
		public EngineSet engines => parts.engines;
		[Description("All sensors.")]
		public ReadOnlyList<Sensor> sensors => parts.sensors;
		[WorkInProgress, Description("All science modules.")]
		public ReadOnlyList<PartScience> science => parts.science;

		#endregion

		#region Basic properties

		[Description("Unique identifier of the ship (vehicle/vessel). Can change when docking/undocking.")]
		public Guid id => native.id;
		[Description("Unique identifier of the ship (vehicle/vessel). Should be same as it was before docking (after undocking).")]
		public uint persistentId => native.persistentId;
		[Description("KSP API. Vessel type as selected by user (or automatically).")]
		public VesselType vesseltype => native.vesselType;
		[Description("Total mass of the ship (vehicle/vessel). [tons = 1000 kg]")]
		public double mass => native.totalMass;

		[Description("Wheter the ship is still packed (reduced physics).")]
		public bool packed => native.packed;
		[Description("Wheter the ship is landed (on the ground or on/in water).")]
		public bool landed => native.Landed;
		[Description("Wheter the ship is in water.")]
		public bool splashed => native.Splashed;
		[Description("Ship is on launch-pad.")]
		public bool prelaunch => situation == Vessel.Situations.PRELAUNCH;
		[Description("KSP API. Current situation.")]
		public Vessel.Situations situation => native.situation;

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

		[Description("Orbited body.")]
		public SpaceBody body => Bodies.Instance[native.mainBody];
		ISpaceObject ISpaceObject.body => body;

		OrbitInfo _orbit;
		[WorkInProgress, Description("Orbit parameters.")]
		public OrbitInfo orbit
		{
			get
			{
				if (_orbit == null)
					_orbit = new OrbitInfo(native.orbit);
				else _orbit.native = native.orbit;
				return _orbit;
			}
		}

		[Description("Period of current orbit in seconds. Alias to `orbit.period`.")]
		public TimeDelta period => new TimeDelta(native.orbit.period);
		[Description("Eta to apoapsis in seconds. Alias to `orbit.timeToAp`.")]
		public TimeDelta timeToAp => orbit.timeToAp;
		[Description("Eta to periapsis in seconds. Alias to `orbit.timeToPe`.")]
		public TimeDelta timeToPe => orbit.timeToPe;
		[Description("Time at apoapsis. Alias to `orbit.timeAtAp`.")]
		public TimeStamp timeAtAp => orbit.timeAtAp;
		[Description("Time at periapsis. Alias to `orbit.timeAtPe`.")]
		public TimeStamp timeAtPe => orbit.timeAtPe;

		[Description("Eccentricity of current orbit. \\[0, +inf)")]
		public double eccentricity => native.orbit.eccentricity;
		[Description("Inclination of current orbit. \\[0, 180)")]
		public double inclination => native.orbit.inclination;
		[Description("Semi-major axis of current orbit.")]
		public double semiMajorAxis => native.orbit.semiMajorAxis;
		[Description("Semi-minor axis of current orbit.")]
		public double semiMinorAxis => native.orbit.semiMinorAxis;

		[Description("Height above ground of highest point of current orbit.")]
		public double apoapsis => native.orbit.ApA;
		[Description("Height above ground of lowest point of current orbit.")]
		public double periapsis => native.orbit.PeA;
		[Description("Highest distance between center of orbited body and any point of current orbit. `(1 + eccentricity) * semiMajorAxis`")]
		public double apocenter => native.orbit.ApR;
		[Description("Lowest distance between center of orbited body and any point of current orbit. `(1 - eccentricity) * semiMajorAxis`")]
		public double pericenter => native.orbit.PeR;

		[Description("Angle in degrees between the direction of periapsis and the current position. Zero at periapsis, 180 at apoapsis.")]
		public double trueAnomaly => RosMath.Deg.Clamp360(native.orbit.trueAnomaly * RosMath.Rad2Deg);
		[Description("Angle in degrees between the direction of periapsis and the current position extrapolated on circular orbit.")]
		public double meanAnomaly => RosMath.Deg.Clamp360(native.orbit.meanAnomaly * RosMath.Rad2Deg);
		[Description("Longitude of ascending node.")]
		public double lan => native.orbit.LAN;
		[Description("Argument of periapsis. Angle from ascending node to periapsis.")]
		public double argumentOfPeriapsis => native.orbit.argumentOfPeriapsis;
		[Description("Argument of periapsis. Angle from ascending node to periapsis.")]
		public double aop => native.orbit.argumentOfPeriapsis;

		#endregion

		#region Orbit - vectors

		[Description("Center of mass relative to (CoM of) active ship (zero for active ship).")]
		public Vector position => new Vector(native.CoMD - FlightGlobals.ActiveVessel.CoMD);
		[Description("Current orbital velocity.")]
		public Vector velocity => new Vector(native.obt_velocity);
		[Description("Current surface velocity.")]
		public Vector surfaceVelocity => new Vector(native.srf_velocity);
		[Description("Current surface velocity (Alias to `surfaceVelocity`).")]
		public Vector srfVelocity => new Vector(native.srf_velocity);
		[Description("Current surface velocity (Alias to `surfaceVelocity`).")]
		public Vector srfvel => new Vector(native.srf_velocity);

		[Description("Vector pointing forward (from cockpit - in the direction of the 'nose').")]
		public Vector forward => new Vector(native.ReferenceTransform.up);
		[Description("Vector pointing backward (from cockpit - in the direction of the 'tail').")]
		public Vector back => new Vector(-native.ReferenceTransform.up);
		[Description("Vector pointing up (from cockpit).")]
		public Vector up => new Vector(-native.ReferenceTransform.forward);
		[Description("Vector pointing down (from cockpit).")]
		public Vector down => new Vector(native.ReferenceTransform.forward);
		[Description("Vector pointing left (from cockpit).")]
		public Vector left => new Vector(-native.ReferenceTransform.right);
		[Description("Vector pointing right (from cockpit).")]
		public Vector right => new Vector(native.ReferenceTransform.right);

		// see https://en.wikipedia.org/wiki/Axes_conventions#Ground_reference_frames_for_attitude_description
		[Description("Vector pointing north in the plane that is tangent to sphere centered in orbited body.")]
		public Vector north => new Vector(native.north);
		[Description("Vector pointing east (tangent to sphere centered in orbited body).")]
		public Vector east => new Vector(native.east);
		[Description("Vector pointing away from orbited body (aka *up*, but we use `up` for cockpit-up).")]
		public Vector away => new Vector(native.up);

		#endregion

		#region Pitch, heading and roll

		[Description("Current pitch / elevation (the angle between forward vector and tangent plane) \\[-90..+90]")]
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
			+ " in tangent plane) \\[0..360]. Note that it can change violently around the poles.")]
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
			+ " in the plane perpendicular to forward vector) \\[-180..+180]."
			+ " Note that it can change violently when facing up or down.")]
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

		[Description("Angular velocity \\[ω, deg/s], how fast the ship rotates")]
		public Vector angularVelocity => new Vector(native.angularVelocityD * RosMath.Rad2Deg);
		[Description("Angular momentum \\[L = Iω, kg⋅m²⋅deg/s=N⋅m⋅s⋅deg] aka moment of momentum or rotational momentum.")]
		public Vector angularMomentum => new Vector((Vector3d)native.angularMomentum * RosMath.Rad2Deg);
		[Description("Moment of inertia \\[I, kg⋅m²=N⋅m⋅s²] aka angular mass or rotational inertia.")]
		public Vector momentOfInertia => new Vector(native.MOI);

		protected TimeStamp _torqueStamp;
		protected Vector _maxTorque, _maxVacuumTorque;
		protected Vector _maxAngular, _maxVacuumAngular;
		protected void UpdateTorque()
		{
			var torque = Vector.zero;
			var vacuum = Vector.zero;
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
			_maxAngular = new Vector(VectorCreator.shrink(_maxTorque, momentOfInertia));
			_maxVacuumAngular = new Vector(VectorCreator.shrink(_maxVacuumTorque, momentOfInertia));
		}

		[Description("Maximal ship torque \\[N⋅m⋅deg=deg⋅kg⋅m²/s²] (aka moment of force or turning effect, maximum of positive and negative).")]
		public Vector maxTorque
		{
			get
			{
				if (Time.since(_torqueStamp) > 0)
					UpdateTorque();
				return _maxTorque;
			}
		}
		[Description("Maximal ship torque in vacuum \\[N⋅m⋅deg=deg⋅kg⋅m²/s²] (ignoring control surfaces).")]
		public Vector maxVacuumTorque
		{
			get
			{
				if (Time.since(_torqueStamp) > 0)
					UpdateTorque();
				return _maxVacuumTorque;
			}
		}
		[Description("Maximal angular acceleration. \\[deg/s²]")]
		public Vector maxAngular
		{
			get
			{
				if (Time.since(_torqueStamp) > 0)
					UpdateTorque();
				return _maxAngular;
			}
		}
		[Description("Maximal angular acceleration in vacuum (ignoring control surfaces).")]
		public Vector maxVacuumAngular
		{
			get
			{
				if (Time.since(_torqueStamp) > 0)
					UpdateTorque();
				return _maxVacuumAngular;
			}
		}

		#endregion

		#region Action Groups, SAS, RCS, ...

		[Description("SAS: Stability Assist System.")]
		public bool sas
		{
			get => native.ActionGroups[KSPActionGroup.SAS];
			set => native.ActionGroups.SetGroup(KSPActionGroup.SAS, value);
		}

		[Description("RCS: Reaction Control System.")]
		public bool rcs
		{
			get => native.ActionGroups[KSPActionGroup.RCS];
			set => native.ActionGroups.SetGroup(KSPActionGroup.RCS, value);
		}

		#endregion

		#region Tools

		[WorkInProgress, Description("Get orbit info relevant for given time. See [orbit.png](orbit.png).")]
		public OrbitInfo orbitAt(TimeStamp time)
		{
			var orbit = this.orbit;
			while (time >= orbit.endTime)
			{
				var next = orbit.next;
				if (next == null)
					break;
				orbit = next;
			}
			return orbit;
		}
		[WorkInProgress, Description(
			"Predicted position at specified time."
			+ " Includes the movement of bodies (e.g. Mun or Ike) when ship is currently orbiting the (grand)parent (e.g. Kerbin or Sun/Kerbol)."
			+ " This method is trying to be reasonably smooth/continuous, use `orbitAt(time).positionAt(time)` if that is not desired."
			+ " See [orbit.png](orbit.png).")]
		public Vector positionAt(TimeStamp time)
		{
			var orbit = native.orbit.GetOrbitAt(time);
			var pos = orbit.getPositionAtUT(time);
			var our = native.orbit.referenceBody;
			var its = orbit.referenceBody;
			if (our != its)
			{// need to adjust the position by the motion of the reference body
			 // note: Sun/Kerbol's reference body may be itself, the logic accounts for this
				var inner = its.referenceBody;
				if (inner == our)
				{// target body is orbiting current body (e.g. from Kerbin to Mun)
					pos += its.getPositionAtUT(time) - its.position;
				}
				else if (inner != null)
				{
					var deep = inner.referenceBody;
					if (deep == our)
					{// target body is orbiting a body that is orbitin current body (e.g. Ike or Gilly when still in Sun/Kerbol's SOI)
						pos += its.getPositionAtUT(time) - its.position;
						pos += deep.getPositionAtUT(time) - deep.position;
					}
				}
			}
			return new Vector(pos - FlightGlobals.ActiveVessel.CoMD);
		}
		[WorkInProgress, Description(
			"Predicted velocity at specified time."
			+ " Includes the movement of bodies (e.g. Mun or Ike) when ship is currently orbiting the (grand)parent (e.g. Kerbin or Sun/Kerbol)."
			+ " This method is trying to be reasonably smooth/continuous, use `orbitAt(time).velocityAt(time)` if that is not desired."
			+ " See [orbit.png](orbit.png).")]
		public Vector velocityAt(TimeStamp time)
		{
			var orbit = native.orbit.GetOrbitAt(time);
			var vel = orbit.getOrbitalVelocityAtUT(time);
			var our = native.orbit.referenceBody;
			var its = orbit.referenceBody;
			if (our != its)
			{// need to adjust the velocity by the motion of the reference body
			 // note: Sun/Kerbol's reference body may be itself, the logic accounts for this
				var inner = its.referenceBody;
				if (inner == our)
				{// target body is orbiting current body (e.g. from Kerbin to Mun)
					vel += its.orbit.getOrbitalVelocityAtUT(time);
				}
				else if (inner != null)
				{
					var deep = inner.referenceBody;
					if (deep == our)
					{// target body is orbiting a body that is orbiting current body (e.g. Ike or Gilly when still in Sun/Kerbol's SOI)
						vel += its.orbit.getOrbitalVelocityAtUT(time);
						vel += deep.orbit.getOrbitalVelocityAtUT(time);
					}
				}
			}
			return new Vector(vel.xzy);
		}

		[Description("Translate vector/direction into ship-local coordinates (like looking at it from the cockpit - or rather from the controlling part).")]
		public Vector local(Vector v)
			=> new Vector(native.ReferenceTransform.InverseTransformDirection(v));
		[Description("Translate vector/direction into world coordinates (reverse the `local` transformation).")]
		public Vector world(Vector v)
			=> new Vector(native.ReferenceTransform.TransformDirection(v));

		[WorkInProgress, Description("Get time at true anomaly (absolute time of angle from direction of periapsis).")]
		public TimeStamp timeAtTrueAnomaly(double trueAnomaly)
			=> Time.now + timeToTrueAnomaly(trueAnomaly);
		[WorkInProgress, Description("Get time to true anomaly (relative time of angle from direction of periapsis). [0, period)")]
		public TimeDelta timeToTrueAnomaly(double trueAnomaly)
			=> new TimeDelta(native.orbit.GetDTforTrueAnomaly(trueAnomaly * RosMath.Deg2Rad, 0.0) % native.orbit.period);

		#endregion
	}
}
