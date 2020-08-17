using System;
using RedOnion.ROS;
using RedOnion.Attributes;

namespace KerbaluaNUnit
{
	[Callable("callablemethod")]
	public static class CallableMethod_Namespace
	{
		public static double callablemethod(double a)
		{
			return a*3;
		}
	}
}
