using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RedOnion.KSP.Autopilot
{
	public class PID
	{
		[Description("Proportional factor (direct control)")]
		public double P { get; set; } = 1.0;
		[Description("Integral factor (error-correction)")]
		public double I { get; set; } = 0.1;
		[Description("Derivative factor (dumpening - applied to output)")]
		public double D { get; set; } = 0.05;
		[Description("Reduction factor for accumulator"
			+ " (dumpening - applied to accumulator used by integral factor)")]
		public double R { get; set; } = 0.05;

		[Description("Feedback (true state - e.g. current pitch)")]
		public double Input { get; set; }
		[Description("Desired state (set point - e.g. desired/wanted pitch)")]
		public double Target { get; set; }

		[Description("Last computed output value (control signal,"
			+ " call Update() after changing Input/Desired)")]
		public double Output => output;
		[Description("Highest output allowed")]
		public double MaxOutput { get; set; } = double.PositiveInfinity;
		[Description("Lowest output allowed")]
		public double MinOutput { get; set; } = double.NegativeInfinity;

		[Description("Limit of abs(input-output) per second for both the output"
			+ " and error calculation (for integral and reduction factors)."
			+ " Helps preventing overshooting especially after change of Target (anti-windup).")]
		public double ChangeLimit { get; set; } = double.PositiveInfinity;
		[Description("Limit of abs(accumulator) used by I and R factors."
			+ " Another anti-windup measure to prevent overshooting.")]
		public double AccuLimit { get; set; } = double.PositiveInfinity;

		protected double stamp = double.NaN;
		protected double output = double.NaN;
		protected double accu, prevInput = double.NaN;

		[Description("Reset internal state of the regulator (won't change PIDR and limits)")]
		public void Reset()
		{
			stamp = double.NaN;
			output = double.NaN;
			accu = 0.0;
			prevInput = double.NaN;
		}

		[Description("Update output according to time elapsed (and Input and Target)")]
		public double Update()
		{
			var now = Planetarium.GetUniversalTime();
			if (now != stamp)
			{
				Update(now - stamp);
				stamp = now;
			}
			return output;
		}
		[Description("Set input and update output according to time elapsed (provided as dt)")]
		public double Update(
			[Description("Time elapsed since last update (in seconds)")]
			double dt,
			[Description("New input/feedback")]
			double input)
		{
			Input = input;
			return Update(dt);
		}
		[Description("Set input and target and update output according to time elapsed (provided as dt)")]
		public double Update(
			[Description("Time elapsed since last update (in seconds)")]
			double dt,
			[Description("New input/feedback")]
			double input,
			[Description("New target / desired state")]
			double target)
		{
			Input = input;
			Target = target;
			return Update(dt);
		}
		[Description("Update output according to time elapsed (provided as dt, using current Input and Target)")]
		public double Update(
			[Description("Time elapsed since last update (in seconds)")]
			double dt)
		{
			if (double.IsNaN(dt))
				output = Input;
			else
			{

				double error = Target - Input;
				if (Math.Abs(error) > ChangeLimit)
					error = error < 0 ? -ChangeLimit : ChangeLimit;
				double result = 0;
				if (!double.IsNaN(error))
				{
					if (!double.IsNaN(P))
						result = P * error;
					if (!double.IsNaN(I))
						accu += I * error * dt;
					if (!double.IsNaN(prevInput))
					{
						double change = Input - prevInput;
						if (!double.IsNaN(D))
							result -= D * change / dt;
						if (!double.IsNaN(R))
							accu -= R * change * dt;
					}
				}
				if (Math.Abs(accu) > AccuLimit)
					accu = accu < 0 ? -AccuLimit : AccuLimit;
				result += accu;
				var limit = ChangeLimit * dt;
				if (Math.Abs(result) > limit)
					result = result < 0 ? -limit : limit;
				output = Input + result;
			}
			prevInput = Input;
			if (output > MaxOutput)
				output = MaxOutput;
			if (output < MinOutput)
				output = MinOutput;
			return output;
		}
	}
}
