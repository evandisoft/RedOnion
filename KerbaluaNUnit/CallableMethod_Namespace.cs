using System;
using RedOnion.ROS;

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
