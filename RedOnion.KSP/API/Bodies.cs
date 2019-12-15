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
		[Unsafe, Description("KSP API. Native `CelestialBody`.")]
		public CelestialBody native { get; private set; }
		protected internal SpaceBody(CelestialBody body)
		{
			native = body;
			bodies = new ReadOnlyList<SpaceBody>(FillBodies);
		}

		[Description("Name of the body.")]
		public string name => native.bodyName;
		[Description("Celestial body this body is orbiting.")]
		public SpaceBody body => native.referenceBody == null || native.referenceBody == native ? null :
			Bodies.Instance[native.referenceBody];
		ISpaceObject ISpaceObject.body => body;

		[Description("Position of the body (relative to active ship).")]
		public Vector position => new Vector(native.position);
		[WorkInProgress, Description("Predicted position at specified time.")]
		public Vector positionAt(double time) => new Vector(native.getPositionAtUT(time) - FlightGlobals.ActiveVessel.CoMD);
		[WorkInProgress, Description("Predicted velocity at specified time.")]
		public Vector velocityAt(double time)
		{
			// suppose that Y and Z are swapped as in `Ship.velocityAt`
			var v = native.orbit.getOrbitalVelocityAtUT(time);
			return new Vector(v.x, v.z, v.y);
		}


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

		[Unsafe, WorkInProgress, Description("KSP API. Orbit parameters. May get replaced by safe wrapper in the future.")]
		public Orbit orbit => native.orbit;
		[Description("Eccentricity of the orbit.")]
		public double eccentricity => native.orbit.eccentricity;
		[Description("Semi-major axis of the orbit.")]
		public double semiMajorAxis => native.orbit.semiMajorAxis;
		[Description("Semi-minor axis of the orbit.")]
		public double semiMinorAxis => native.orbit.semiMinorAxis;
		[Description("Inclination of the orbit.")]
		public double inclination => native.orbit.inclination;
		[Description("Height above ground of highest point of the orbit.")]
		public double apoapsis => native.orbit.ApA;
		[Description("Height above ground of lowest point of the orbit.")]
		public double periapsis => native.orbit.PeA;
		[Description("Highest distance between center of orbited body and any point of the orbit.")]
		public double apocenter => native.orbit.ApR;
		[Description("Lowest distance between center of orbited body and any point of the orbit.")]
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
		public double meanAnomaly => RosMath.Deg.Clamp360(native.orbit.meanAnomaly * RosMath.Rad2Deg);
		[Description("Longitude of ascending node.")]
		public double lan => native.orbit.LAN;
		[Description("Argument of periapsis.")]
		public double argumentOfPeriapsis => native.orbit.argumentOfPeriapsis;

		[WorkInProgress, Description("Get time at true anomaly (absolute time).")]
		public double timeAtTrueAnomaly(double trueAnomaly)
			=> orbit.GetUTforTrueAnomaly(trueAnomaly * RosMath.Deg2Rad, 0.0);
		[WorkInProgress, Description("Get time to true anomaly (relative time).")]
		public double timeToTrueAnomaly(double trueAnomaly)
			=> orbit.GetDTforTrueAnomaly(trueAnomaly * RosMath.Deg2Rad, 0.0);
	}
}
