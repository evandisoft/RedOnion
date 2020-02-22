using System;
using System.ComponentModel;
using RedOnion.Attributes;
using RedOnion.KSP.API;
using RedOnion.ROS.Utilities;

namespace RedOnion.KSP.Parts
{
	[Description("Read-only set of engines.")]
	public class EngineSet : PartSet<Engine>
	{
		protected internal EngineSet(Ship ship) : base(ship) { }
		protected internal EngineSet(Ship ship, Action refresh) : base(ship, refresh) { }

		[WorkInProgress, Description("Propellants consumed by the engines.")]
		public PropellantList propellants => _propellants ?? (_propellants = new PropellantList(this));
		protected PropellantList _propellants;

		protected internal override void SetDirty(bool value)
		{
			if (value) _propellants?.SetDirty(value);
			base.SetDirty(value);
		}

		[Description("Whether any engine in the set is operational.")]
		public bool anyOperational
		{
			get
			{
				foreach (var e in this)
				{
					if (e.operational)
						return true;
				}
				return false;
			}
		}
		[Description("Whether all the engines in the set are operational.")]
		public bool allOperational
		{
			get
			{
				foreach (var e in this)
				{
					if (!e.operational)
						return false;
				}
				return true;
			}
		}
		[Description("Wheter any engine in the set flamed out.")]
		public bool anyFlameout
		{
			get
			{
				foreach (var e in this)
				{
					if (e.flameout)
						return true;
				}
				return false;
			}
		}
		[Description("Wheter all engines in the set flamed out.")]
		public bool allFlameout
		{
			get
			{
				foreach (var e in this)
				{
					if (!e.flameout)
						return false;
				}
				return true;
			}
		}
		[Description("Current thrust \\[kN] (at current pressure, with current `thrustPercentage` and current throttle).")]
		public double thrust
		{
			get
			{
				var thrust = 0.0;
				foreach (var e in this)
				{
					if (e.operational)
						thrust += (double)e.thrust * e.thrustPercentage * 0.01;
				}
				return thrust;
			}
		}
		[Description("Get thrust \\[kN] of all operational engines at atmospheric pressure"
			+ " (0 = vacuum, 1 = Kerbin sea-level pressure, default NaN = current pressure)"
			+ " and throttle (default 1 = full throttle).")]
		public double getThrust(float atm = float.NaN, float throttle = 1f)
		{
			var thrust = 0.0;
			foreach (var e in this)
			{
				if (e.operational)
					thrust += e.getThrust(atm, throttle) * e.thrustPercentage * 0.01;
			}
			return thrust;
		}
		[Description("Get average specific impulse \\[seconds] of operational engines at atmospheric pressure"
			+ " (0 = vacuum, 1 = Kerbin sea-level pressure, default NaN = current pressure).")]
		public double getIsp(double atm = double.NaN)
		{
			var thrust = 0.0;
			var flow = 0.0;
			foreach (var e in this)
			{
				if (!e.operational)
					continue;
				var isp = e.isp;
				if (isp <= minIsp)
					continue;
				var eth = e.getThrust(atm) * e.thrustPercentage * 0.01;
				thrust += eth;
				flow += eth / isp;
			}
			return flow <= minFlow ? 0.0 : thrust/flow;
		}

		public static double g0 = 9.81;
		internal const double minIsp = 0.001;
		internal const double minFlow = 0.0001;
		[Description("Estimate burn time for given delta-v (assuming it can be done without staging).")]
		public TimeDelta burntime(double deltaV, double mass = double.NaN)
		{// see Stage.UpdateBurnTime
			var thrust = 0.0;
			var flow = 0.0;
			foreach (var e in this)
			{
				if (!e.operational)
					continue;
				var eisp = e.isp;
				if (eisp < minIsp)
					continue;
				var ethr = e.getThrust() * e.thrustPercentage * 0.01;
				thrust += ethr;
				flow += ethr / eisp;
			}
			if (flow < minFlow)
				return TimeDelta.none;
			var stdIsp = g0 * thrust / flow;
			if (double.IsNaN(mass)) mass = _ship.mass;
			return new TimeDelta(stdIsp * mass * (1.0 - Math.Pow(Math.E, -deltaV / stdIsp)) / thrust);
		}
	}
}
