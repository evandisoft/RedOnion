using System;
using UnityEngine;

namespace RedOnion.KSP.API
{
	public static class VectorUtils
	{
		public static Vector3 scale(this Vector3 vec, Vector3 by)
			=> new Vector3(vec.x * by.x, vec.y * by.y, vec.z * by.z);
		public static Vector3d scale(this Vector3d vec, Vector3d by)
			=> new Vector3d(vec.x / by.x, vec.y / by.y, vec.z / by.z);

		public static Vector3 shrink(this Vector3 vec, Vector3 by)
			=> new Vector3(vec.x / by.x, vec.y / by.y, vec.z / by.z);
		public static Vector3d shrink(this Vector3d vec, Vector3d by)
			=> new Vector3d(vec.x / by.x, vec.y / by.y, vec.z / by.z);

		public static Vector3 abs(Vector3 v)
			=> new Vector3(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z));
		public static Vector3d abs(Vector3d v)
			=> new Vector3d(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z));

		public static float dot(this Vector3 lhs, Vector3 rhs)
			=> Vector3.Dot(lhs, rhs);
		public static double dot(this Vector3d lhs, Vector3d rhs)
			=> Vector3d.Dot(lhs, rhs);

		public static float angle(this Vector3 lhs, Vector3 rhs)
			=> Vector3.Angle(lhs, rhs);
		public static double angle(this Vector3d lhs, Vector3d rhs)
			=> Vector3d.Angle(lhs, rhs);

		public static float angle(this Vector3 lhs, Vector3 rhs, Vector3 axis)
		{
			var a = Vector3.Angle(lhs, rhs);
			if (Vector3.Angle(axis, Vector3.Cross(lhs, rhs)) > 90)
				a = -a;
			return a;
		}
		public static double angle(this Vector3d lhs, Vector3d rhs, Vector3d axis)
		{
			var a = Vector3d.Angle(lhs, rhs);
			if (Vector3d.Angle(axis, Vector3d.Cross(lhs, rhs)) > 90)
				a = -a;
			return a;
		}

		public static Vector3 cross(this Vector3 lhs, Vector3 rhs)
			=> Vector3.Cross(lhs, rhs);
		public static Vector3d cross(this Vector3d lhs, Vector3d rhs)
			=> Vector3d.Cross(lhs, rhs);

		public static Vector3 projectOnVector(this Vector3 vec, Vector3 normal)
			=> Vector3.Project(vec, normal);
		public static Vector3d projectOnVector(this Vector3d vec, Vector3d normal)
			=> Vector3d.Project(vec, normal);

		public static Vector3 projectOnPlane(this Vector3 vec, Vector3 normal)
			=> Vector3.ProjectOnPlane(vec, normal);
		public static Vector3d projectOnPlane(this Vector3d vec, Vector3d normal)
			=> Vector3d.Exclude(normal, vec);

		public static Vector3 project(this Vector3 vec, Vector3 normal)
			=> Vector3.Project(vec, normal);
		public static Vector3d project(this Vector3d vec, Vector3d normal)
			=> Vector3d.Project(vec, normal);

		public static Vector3 exclude(this Vector3 vec, Vector3 normal)
			=> Vector3.ProjectOnPlane(vec, normal);
		public static Vector3d exclude(this Vector3d vec, Vector3d normal)
			=> Vector3d.Exclude(normal, vec);

		public static Vector3 rotate(this Vector3 vec, float angle, Vector3 axis)
			=> Quaternion.AngleAxis(angle, axis) * vec;
		public static Vector3d rotate(this Vector3d vec, double angle, Vector3d axis)
			=> QuaternionD.AngleAxis(angle, axis) * vec;

	}
}
