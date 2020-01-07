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
	[WorkInProgress, Description("Orbit parameters")]
	public readonly struct OrbitInfo
	{
		[Unsafe, Description("KSP API. Orbit parameters.")]
		public readonly Orbit native;
		internal OrbitInfo(Orbit orbit)
			=> native = orbit;
		public static implicit operator Orbit(OrbitInfo info)
			=> info.native;

		[Description("Eccentricity of current orbit.")]
		public double eccentricity => native.eccentricity;
		[Description("Inclination of current orbit [0, 180).")]
		public double inclination => native.inclination;
		[Description("Semi-major axis of current orbit.")]
		public double semiMajorAxis => native.semiMajorAxis;
		[Description("Semi-minor axis of current orbit.")]
		public double semiMinorAxis => native.semiMinorAxis;
		[Description("Height above ground of highest point of current orbit.")]
		public double apoapsis => native.ApA;
		[Description("Height above ground of lowest point of current orbit.")]
		public double periapsis => native.PeA;
		[Description("Highest distance between center of orbited body and any point of current orbit.")]
		public double apocenter => native.ApR;
		[Description("Lowest distance between center of orbited body and any point of current orbit.")]
		public double pericenter => native.PeR;
		[Description("Eta to apoapsis in seconds.")]
		public double timeToAp => native.timeToAp;
		[Description("Eta to periapsis in seconds.")]
		public double timeToPe => native.timeToPe;
		[Description("Period of current orbit in seconds.")]
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
