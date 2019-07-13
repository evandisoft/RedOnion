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
		[Description("Proportional factor (strength of direct control)")]
		public double P { get; set; } = 1.0;
		[Description("Integral factor (dynamic error-correction, causes oscillation as side-effect)")]
		public double I { get; set; } = 0.1;
		[Description("Derivative factor (dumpening - applied to output, reduces the oscillation)")]
		public double D { get; set; } = 0.05;
		[Description("Reduction factor for accumulator"
			+ " (dumpening - applied to accumulator used by integral factor,"
			+ " works well against both oscillation and windup)")]
		public double R { get; set; } = 0.05;

		[Description("Feedback (true state - e.g. current pitch;"
			+ " error/difference if Target is NaN)")]
		public double Input { get; set; } = double.NaN;
		[Description("Desired state (set point - e.g. desired/wanted pitch;"
			+ " NaN for pure error/difference mode, which is the default)."
			+ " The computed control signal is added to Input if Target is valid,"
			+ " use error/difference mode if you want to add it to Target.")]
		public double Target { get; set; } = double.NaN;

		[Description("Last computed output value (control signal,"
			+ " call Update() after changing Input/Target)")]
		public double Output => output;
		[Description("Highest output allowed")]
		public double MaxOutput { get; set; } = double.PositiveInfinity;
		[Description("Lowest output allowed")]
		public double MinOutput { get; set; } = double.NegativeInfinity;

		[Description("Maximal abs(Target - previous Target) per second."
			+ " NaN or +Inf means no limit (which is default)."
			+ " This can make the output smoother (more human-like control)"
			+ " and help prevent oscillation after target change (windup).")]
		public double TargetChangeLimit { get; set; } = double.PositiveInfinity;
		[Description("Maximal abs(output-input)"
			+ " and also abs(target-input) for integral and reduction factors)."
			+ " Helps preventing overshooting especially after change of Target (windup)."
			+ " NaN or +Inf means no limit (which is default)")]
		public double OutputChangeLimit { get; set; } = double.PositiveInfinity;
		[Description("Limit of abs(accumulator) used by I and R factors."
			+ " Another anti-windup measure to prevent overshooting.")]
		public double AccumulatorLimit { get; set; } = double.PositiveInfinity;

		protected double stamp, input, target, output, accu;
		public PID() => Reset();
		[Description("Reset internal state of the regulator (won't change PIDR and limits)")]
		public void Reset()
		{
			stamp = double.NaN;
			input = double.NaN;
			target = double.NaN;
			output = double.NaN;
			accu = 0.0;
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
				var targetLimit = TargetChangeLimit * dt;
				target = Math.Abs(Target - target) > targetLimit ? Target < 0
					? target - targetLimit
					: target + targetLimit
					: Target; // this accounts for any of Target, this.target or targetLimit being NaN
				double error = double.IsNaN(target) ? Input : target - Input;
				double result = 0;
				if (!double.IsNaN(error))
				{
					if (!double.IsNaN(P))
						result = P * error;
					if (!double.IsNaN(I))
						accu += I * error * dt;
					if (!double.IsNaN(input))
					{
						double change = Input - input;
						if (!double.IsNaN(D))
							result -= D * change / dt;
						if (!double.IsNaN(R))
							accu -= R * change * dt;
					}
				}
				if (Math.Abs(accu) > AccumulatorLimit)
					accu = accu < 0 ? -AccumulatorLimit : AccumulatorLimit;
				result += accu;
				var outputLimit = OutputChangeLimit * dt;
				if (Math.Abs(result) > outputLimit)
					result = result < 0 ? -outputLimit : outputLimit;
				output = double.IsNaN(target) ? result : Input + result;
			}
			input = Input;
			if (output > MaxOutput)
				output = MaxOutput;
			if (output < MinOutput)
				output = MinOutput;
			return output;
		}
	}
}
