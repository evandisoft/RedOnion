using RedOnion.Attributes;
using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedOnion.KSP.API
{
	public static class OrbitExtensions
	{
		public static Orbit GetNext(this Orbit orbit)
			=> (orbit.patchEndTransition == Orbit.PatchTransitionType.ENCOUNTER
			|| orbit.patchEndTransition == Orbit.PatchTransitionType.ESCAPE)
			&& orbit.nextPatch?.activePatch == true
			? orbit.nextPatch : null;
		public static Orbit GetOrbitAt(this Orbit orbit, double time)
		{
			while (time > orbit.EndUT)
			{
				var next = orbit.GetNext();
				if (next == null)
					break;
				orbit = orbit.nextPatch;
			}
			return orbit;
		}
	}

	[WorkInProgress, Description("Orbit/patch parameters.")]
	public class OrbitInfo
	{
		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_orbit.html): Orbit parameters.")]
		public Orbit native { get; internal set; }
		internal OrbitInfo(Orbit orbit)
			=> native = orbit;
		public static implicit operator Orbit(OrbitInfo info)
			=> info?.native;

		[Description("Orbited body.")]
		public SpaceBody body => Bodies.Instance[native.referenceBody];

		[WorkInProgress, Description("This orbit-patch ends by encounter with another celestial body.")]
		public bool encounter => native.patchEndTransition == Orbit.PatchTransitionType.ENCOUNTER;
		[WorkInProgress, Description("This orbit-patch ends by escaping the SOI.")]
		public bool escape => native.patchEndTransition == Orbit.PatchTransitionType.ESCAPE;

		OrbitInfo _next;
		[Description("Next patch if there is some transition (null otherwise).")]
		public OrbitInfo next
		{
			get
			{
				var next = native.GetNext();
				if (next == null)
					_next = null;
				else
				{
					if (_next == null)
						_next = new OrbitInfo(native.nextPatch);
					else _next.native = native.nextPatch;
				}
				return _next;
			}
		}

		[Description("Time of start of this patch, if it is continuation. Usually in the past for current orbit without a transition.")]
		public double startTime => native.StartUT;
		[Description("Time of end of this patch, if there is transition. `period = endTime - startTime` for current orbit without a transition.")]
		public double endTime => native.EndUT;

		[Description("Eccentricity of current orbit. \\[0, +inf)")]
		public double eccentricity => native.eccentricity;
		[Description("Inclination of current orbit. \\[0, 180)")]
		public double inclination => native.inclination;
		[Description("Semi-major axis of current orbit.")]
		public double semiMajorAxis => native.semiMajorAxis;
		[Description("Semi-minor axis of current orbit.")]
		public double semiMinorAxis => native.semiMinorAxis;

		[Description("Height above ground of highest point of current orbit. `apocenter - body.radius`")]
		public double apoapsis => native.ApA;
		[Description("Height above ground of lowest point of current orbit. `pericenter - body.radius`")]
		public double periapsis => native.PeA;
		[Description("Highest distance between center of orbited body and any point of current orbit. `(1 + eccentricity) * semiMajorAxis`")]
		public double apocenter => native.ApR;
		[Description("Lowest distance between center of orbited body and any point of current orbit. `(1 - eccentricity) * semiMajorAxis`")]
		public double pericenter => native.PeR;

		[Description("Eta to apoapsis in seconds.")]
		public double timeToAp => native.timeToAp;
		[Description("Eta to periapsis in seconds.")]
		public double timeToPe => native.timeToPe;
		[Description("Period of the orbit in seconds.")]
		public double period => native.period;

		[Description("Angle in degrees between the direction of periapsis and the current position. Zero at periapsis, 180 at apoapsis.")]
		public double trueAnomaly => RosMath.Deg.Clamp360(native.trueAnomaly * RosMath.Rad2Deg);
		[Description("Angle in degrees between the direction of periapsis and the current position extrapolated on circular orbit.")]
		public double meanAnomaly => RosMath.Deg.Clamp360(native.meanAnomaly * RosMath.Rad2Deg);
		[Description("Longitude of ascending node.")]
		public double lan => native.LAN;
		[Description("Argument of periapsis. Angle from ascending node to periapsis.")]
		public double argumentOfPeriapsis => native.argumentOfPeriapsis;

		[WorkInProgress, Description("Predicted position at specified time.")]
		public Vector positionAt(double time) => new Vector(native.getPositionAtUT(time) - FlightGlobals.ActiveVessel.CoMD);
		[WorkInProgress, Description("Predicted velocity at specified time.")]
		public Vector velocityAt(double time) => new Vector(native.getOrbitalVelocityAtUT(time).xzy);
	}
}
