using System;
namespace RedOnion.KSP.MathUtil {
	public class Scalar
	{
		static public float Clampf(float a,float min,float max)
		{
			return Math.Min(Math.Max(a, min), max);
		}
	}
}
