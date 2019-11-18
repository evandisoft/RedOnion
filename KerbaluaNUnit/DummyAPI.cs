using System;
using RedOnion.KSP.API;

namespace KerbaluaNUnit
{
	public static class DummyAPI
	{
		public static readonly Type pid = typeof(PID);
		public static double a => 1.5;
		public static int getint(int b,out int c)
		{
			c=b+1;
			return b;
		}
	}
}
