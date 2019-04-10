using System;
using UnityEngine;

namespace RedOnion.KSP.Autopilot
{
	/// <summary>
	/// A specification of a direction relative to the nearest body.
	/// </summary>
	public struct RelativeDirection
	{
		/// <summary>
		/// Degrees clockwise from north.
		/// 
		/// Ranges from 0 to 360.
		/// </summary>
		public double Heading;
		/// <summary>
		/// The pitch above the tangent to the ground under the craft assuming
		/// the body was perfectly spherical.
		/// 
		/// Ranges from -90 to 90.
		/// </summary>
		public double Pitch;

		public RelativeDirection(double heading,double pitch)
		{
			Heading = heading;
			Pitch = pitch;
		}

		/// <summary>
		/// Tries to get the current dir based on the vessel and the nearest body
		/// to it. Can fail if vessel is directly above the north or south pole.
		/// </summary>
		/// <returns><c>true</c>, if get current dir was tryed, <c>false</c> otherwise.</returns>
		/// <param name="vessel">Vessel.</param>
		/// <param name="dir">Dir.</param>
		public bool TryGetCurrentDir(Vessel vessel,out Vector3 dir)
		{
			var body = vessel.mainBody;
			var invec = body.transform.position - vessel.transform.position;
			invec.Normalize();
			var up = Vector3.up;
			var west = Vector3.Cross(invec, up).normalized;

			var north = Vector3.Cross(west, invec);
			var pitched = Quaternion.AngleAxis((float)Pitch, west)*north;
			var headed = Quaternion.AngleAxis((float)Heading, -invec) * pitched;

			dir = headed;
			return true;
		}
	}
}
