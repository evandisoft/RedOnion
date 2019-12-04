using MoonSharp.Interpreter;
using RedOnion.ROS;
using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using KSPTW = global::TimeWarp;

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
		public static double tick => global::TimeWarp.fixedDeltaTime;

		public static readonly Type warp = typeof(TimeWarp);
	}

	[Description("Time warping utilities")]
	public static class TimeWarp
	{
		[Description("Indicator that warping utilities are ready for commands. (`to` will return false otherwise.)")]
		public static bool ready
			=> KSPTW.fetch?.setAutoWarp == false
			&& !InputLockManager.lockStack.ContainsKey("TimeWarpLock");

		[Description("Warp mode set to low aka physics warp. Note that it can only be changed on zero rate-index (and when `ready`).")]
		public static bool low
		{
			get => KSPTW.WarpMode == KSPTW.Modes.LOW;
			set => setMode(KSPTW.Modes.LOW);
		}
		[Description("Warp mode set to high aka on-rails warp. Note that it can only be changed on zero rate-index (and when `ready`).")]
		public static bool high
		{
			get => KSPTW.WarpMode == KSPTW.Modes.HIGH;
			set => setMode(KSPTW.Modes.HIGH);
		}
		private static void setMode(KSPTW.Modes mode)
		{
			if (!ready || KSPTW.fetch.Mode == mode || KSPTW.CurrentRateIndex > 0)
				return;
			KSPTW.fetch.Mode = mode;
		}

		[Description("Current rate.")]
		public static float rate
		{
			get => KSPTW.CurrentRate;
			set => setRate(value);
		}
		[Description("Current rate index.")]
		public static int index
		{
			get => KSPTW.CurrentRateIndex;
			set => setIndex(value);
		}

		private static float[] rates
			=> low ? KSPTW.fetch.physicsWarpRates : KSPTW.fetch.warpRates;
		[Description("Set rate index. Returns false if not possible now.")]
		public static bool setIndex(int value)
		{
			if (!ready)
				return false;
			var rates = TimeWarp.rates;
			value = RosMath.Clamp(value, 0, rates.Length-1);
			KSPTW.SetRate(value, false);
			return true;
		}
		[Description("Set rate. Returns false if not possible now.")]
		public static bool setRate(float value)
		{
			if (!ready)
				return false;
			var rates = TimeWarp.rates;
			float closest = float.MaxValue;
			int i = 0;
			while (i < rates.Length)
			{
				float diff = Math.Abs(value - rates[i]);
				if (diff > closest)
					break;
				closest = diff;
				i++;
			}
			return setIndex(i-1);
		}

		[Description("Warp to specified time. Returns true if engaged, false if not ready.")]
		public static bool to(double time)
		{
			if (!ready)
				return false;
			KSPTW.fetch.WarpTo(time);
			return KSPTW.fetch.setAutoWarp;
		}

		[Description("Cancel any warp-to in progress. Returns true if it was canceled, false if no warp was in progress.")]
		public static bool cancel()
		{
			if (KSPTW.fetch == null)
				return false;
			KSPTW.fetch.setAutoWarp = false;
			return KSPTW.fetch.CancelAutoWarp();
		}
	}
}
