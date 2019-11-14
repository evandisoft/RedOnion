using MoonSharp.Interpreter;
using RedOnion.ROS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.API
{
	[Callable("now")]
	[Description("The simulation time, in seconds, since the game or this save was started.")]
	public static class Time
	{
		[Description("The simulation time, in seconds, since the game or this save was started.")]
		public static double now => Planetarium.GetUniversalTime();
		[Description("Seconds since some previous time.")]
		public static double since(double time) => double.IsNaN(time) ? double.PositiveInfinity : now - time;

		[Description("Seconds in one tick. (Script engine always runs in physics ticks.)")]
		public static double tick => TimeWarp.fixedDeltaTime;
	}
}
