using System;
using UnityEngine;

namespace RedOnion.KSP.MathUtil {
	public class Vec {
		static public Vector3 Div(Vector3 vec1, Vector3 vec2)
		{
			return new Vector3(vec1.x / vec2.x, vec1.y / vec2.y, vec1.z / vec2.z);
		}

		static public Vector3 Abs(Vector3 vec)
		{
			return new Vector3(Math.Abs(vec.x), Math.Abs(vec.y), Math.Abs(vec.z));
		}

		static public Vector3 New(float x,float y,float z)
		{
			return new Vector3(x, y, z);
		}

		static public Vector3 New()
		{
			return new Vector3(0, 0, 0);
		}
	}
}
