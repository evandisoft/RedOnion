using RedOnion.Attributes;
using RedOnion.KSP.Completion;
using RedOnion.KSP.Utilities;
using RedOnion.ROS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.API
{
	[Description("Properties that all targetable space-objects have to implement.")]
	public interface ISpaceObject
	{
		[Description("Position of the object relative to active ship.")]
		Vector position { get; }
		[Description("Space body this object orbits.")]
		ISpaceObject body { get; }
	}

	[DocBuild(typeof(SpaceBody)), Description(
@"Collection of [celestial bodies](SpaceBody.md). Can be indexed (`bodies[""kerbin""]`)
and elements are also properties (`bodies.kerbin`, `bodies.mun`).")]
	public class Bodies : Properties<SpaceBody>.WithMap<CelestialBody>, ICompletable
	{
		public static Bodies Instance { get; } = new Bodies();
		// this is only temporary, `Properties` need some redesign
		IList<string> ICompletable.PossibleCompletions => dict.Keys.ToList();

		protected Bodies()
		{
			var sb = new StringBuilder();
			foreach (var body in FlightGlobals.Bodies)
			{
				var it = new SpaceBody(body);
				map[body] = it;
				strict[body.bodyName] = list.size;
				sb.Append(body.bodyName.Trim());
				if (sb.Length > 0 && char.IsUpper(sb[0]))
					sb[0] = char.ToLowerInvariant(sb[0]);
				for (int i = 0; i < sb.Length;)
				{
					if (sb[i] == ' ')
					{
						sb.Remove(i, 1);
						if (char.IsLower(sb[i]))
							sb[i] = char.ToUpper(sb[i]);
					}
					else i++;
				}
				var name = sb.ToString();
				dict[name] = list.size;
				list.Add(new KeyValuePair<string, SpaceBody>(name, it));
				sb.Length = 0;
			}
		}
	}
	[Description("Celestial body. (`SpaceBody` selected not to conflict with KSP `CelestialBody`.)")]
	public class SpaceBody : ISpaceObject
	{
		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_celestial_body.html)")]
		public CelestialBody native { get; private set; }
		protected internal SpaceBody(CelestialBody body)
		{
			native = body;
			bodies = new ReadOnlyList<SpaceBody>(FillBodies);
		}

		[Description("Name of the body.")]
		public string name => native.bodyName;
		public override string ToString() => name;
		[Description("Celestial body this body is orbiting.")]
		public SpaceBody body => native.referenceBody == null || native.referenceBody == native ? null :
			Bodies.Instance[native.referenceBody];
		ISpaceObject ISpaceObject.body => body;

		[Description("Position of the body (relative to active ship).")]
		public Vector position => new Vector(native.position - FlightGlobals.ActiveVessel.CoMD);
		[Description("Current orbital velocity (relative to parent body, zero for Sun/Kerbol).")]
		public Vector velocity => new Vector(native.orbit?.vel.xzy ?? Vector3d.zero);
		[WorkInProgress, Description("Predicted position at specified time.")]
		public Vector positionAt(TimeStamp time) => new Vector(native.getPositionAtUT(time) - FlightGlobals.ActiveVessel.CoMD);
		[WorkInProgress, Description("Predicted velocity at specified time.")]
		public Vector velocityAt(TimeStamp time) => new Vector(native.orbit?.getOrbitalVelocityAtUT(time).xzy ?? Vector3d.zero);

		[Description("Orbiting celestial bodies.")]
		public ReadOnlyList<SpaceBody> bodies { get; }
		void FillBodies()
		{
			bodies.Clear();
			foreach (var child in native.orbitingBodies)
				bodies.Add(Bodies.Instance[child]);
		}

		[Description("Radius of the body [m].")]
		public double radius => native.Radius;
		[Description("Mass of the body [kg].")]
		public double mass => native.Mass;
		[Description("Standard gravitational parameter (μ = GM) [m³/s²]")]
		public double gravParameter => native.gravParameter;
		[Description("Standard gravitational parameter (μ = GM) [m³/s²]")]
		public double mu => native.gravParameter;

		[Description("Atmosphere parameters of the body.")]
		public Atmosphere atmosphere => new Atmosphere(native);
		[Description("Atmosphere parameters of the body. (Alias to `atmosphere`)")]
		public Atmosphere atm => new Atmosphere(native);

		[Description("Atmosphere parameters of a body.")]
		public struct Atmosphere
		{
			readonly CelestialBody native;
			internal Atmosphere(CelestialBody native)
				=> this.native = native;

			[Description("Is there any atmosphere (true on Kerbin, false on Moon).")]
			public bool exists => native?.atmosphere ?? false;
			[Description("Is there oxygen in the atmosphere.")]
			public bool oxygen => native?.atmosphereContainsOxygen ?? false;
			[Description("Depth/height of the atmosphere.")]
			public double depth => native?.atmosphereDepth ?? double.NaN;
			[Description("Depth/height of the atmosphere.")]
			public double height => native?.atmosphereDepth ?? double.NaN;

			[Description("Used when there is no body/ship. Returns false/NaN in properties.")]
			public static readonly Atmosphere none = new Atmosphere();
		}

		OrbitInfo _orbit;
		[WorkInProgress, Description("Orbit parameters. Null for Sun/Kerbol.")]
		public OrbitInfo orbit
		{
			get
			{
				var norbit = native.orbit;
				if (norbit == null)
					return null; // Sun/Kerbol
				if (_orbit == null)
					_orbit = new OrbitInfo(norbit);
				else _orbit.native = norbit;
				return _orbit;
			}
		}

		[Description("Period of current orbit in seconds. Alias to `orbit.period`. `NaN/none` for Sun/Kerbol.")]
		public TimeDelta period => new TimeDelta(native.orbit?.period ?? double.NaN);
		[Description("Eta to apoapsis in seconds. Alias to `orbit.timeToAp`. `NaN/none` for Sun/Kerbol.")]
		public TimeDelta timeToAp => orbit?.timeToAp ?? TimeDelta.none;
		[Description("Eta to periapsis in seconds. Alias to `orbit.timeToPe`. `NaN/none` for Sun/Kerbol.")]
		public TimeDelta timeToPe => orbit?.timeToPe ?? TimeDelta.none;
		[Description("Time at apoapsis. Alias to `orbit.timeAtAp`. `NaN/none` for Sun/Kerbol.")]
		public TimeStamp timeAtAp => orbit?.timeAtAp ?? TimeStamp.none;
		[Description("Time at periapsis. Alias to `orbit.timeAtPe`. `NaN/none` for Sun/Kerbol.")]
		public TimeStamp timeAtPe => orbit?.timeAtPe ?? TimeStamp.none;

		[Description("Eccentricity of the orbit. \\[0, +inf) `NaN` for Sun/Kerbol.")]
		public double eccentricity => native.orbit?.eccentricity ?? double.NaN;
		[Description("Inclination of the orbit. `NaN` for Sun/Kerbol.")]
		public double inclination => native.orbit?.inclination ?? double.NaN;
		[Description("Semi-major axis of the orbit. `NaN` for Sun/Kerbol.")]
		public double semiMajorAxis => native.orbit?.semiMajorAxis ?? double.NaN;
		[Description("Semi-minor axis of the orbit. `NaN` for Sun/Kerbol.")]
		public double semiMinorAxis => native.orbit?.semiMinorAxis ?? double.NaN;

		[Description("Height above ground of highest point of the orbit. `NaN` for Sun/Kerbol.")]
		public double apoapsis => native.orbit?.ApA ?? double.NaN;
		[Description("Height above ground of lowest point of the orbit. `NaN` for Sun/Kerbol.")]
		public double periapsis => native.orbit?.PeA ?? double.NaN;
		[Description("Highest distance between center of orbited body and any point of the orbit. `NaN` for Sun/Kerbol.")]
		public double apocenter => native.orbit?.ApR ?? double.NaN;
		[Description("Lowest distance between center of orbited body and any point of the orbit. `NaN` for Sun/Kerbol.")]
		public double pericenter => native.orbit?.PeR ?? double.NaN;

		[Description("Angle in degrees between the direction of periapsis and the current position. Zero at periapsis, 180 at apoapsis. `NaN` for Sun/Kerbol.")]
		public double trueAnomaly => native.orbit == null ? double.NaN :
			RosMath.Deg.Clamp360(native.orbit.trueAnomaly * RosMath.Rad2Deg);
		[Description("Angle in degrees between the direction of periapsis and the current position extrapolated on circular orbit. `NaN` for Sun/Kerbol.")]
		public double meanAnomaly => native.orbit == null ? double.NaN :
			RosMath.Deg.Clamp360(native.orbit.meanAnomaly * RosMath.Rad2Deg);
		[Description("Longitude of ascending node. `NaN` for Sun/Kerbol.")]
		public double lan => native.orbit?.LAN ?? double.NaN;
		[Description("Argument of periapsis. Angle from ascending node to periapsis. `NaN` for Sun/Kerbol.")]
		public double argumentOfPeriapsis => native.orbit?.argumentOfPeriapsis ?? double.NaN;
		[Description("Argument of periapsis. Angle from ascending node to periapsis. `NaN` for Sun/Kerbol.")]
		public double aop => native.orbit?.argumentOfPeriapsis ?? double.NaN;

		[WorkInProgress, Description("Get time at true anomaly (absolute time of angle from direction of periapsis). `NaN/none` for Sun/Kerbol.")]
		public TimeStamp timeAtTrueAnomaly(double trueAnomaly)
			=> Time.now + timeToTrueAnomaly(trueAnomaly);
		[WorkInProgress, Description("Get time to true anomaly (relative time of angle from direction of periapsis). [0, period) `NaN/none` for Sun/Kerbol.")]
		public TimeDelta timeToTrueAnomaly(double trueAnomaly)
			=> native.orbit == null ? TimeDelta.none : new TimeDelta(
				native.orbit.GetDTforTrueAnomaly(trueAnomaly * RosMath.Deg2Rad, 0.0) % native.orbit.period);
	}
}
