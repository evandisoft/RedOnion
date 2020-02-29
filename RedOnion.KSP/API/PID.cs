using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.API
{
	[Description("PID(R) regulator parameters.")]
	public class PidParams
	{
		public PidParams() { }
		public PidParams(PidParams copyFrom)
		{
			P = copyFrom.P;
			I = copyFrom.I;
			D = copyFrom.D;
			R = copyFrom.R;
			targetChangeLimit = copyFrom.targetChangeLimit;
			outputChangeLimit = copyFrom.outputChangeLimit;
			accumulatorLimit = copyFrom.accumulatorLimit;
		}
		public PidParams(
			double P = 1.0,
			double I = 0.0,
			double D = 0.0,
			double R = 0.0)
		{
			this.P = P;
			this.I = I;
			this.D = D;
			this.R = R;
		}

		[Description("Proportional factor (strength of direct control).")]
		public double P = 1.0;
		[Description("Integral factor (dynamic error-correction, causes oscillation as side-effect).")]
		public double I = 0.0;
		[Description("Derivative factor (dampening - applied to output, reduces the oscillation).")]
		public double D = 0.0;
		[Description("Reduction factor for accumulator"
			+ " (dampening - applied to accumulator used by integral factor,"
			+ " works well against both oscillation and windup).")]
		public double R = 0.0;

		[Description("Difference scaling factor")]
		public double scale = 1.0;

		[Description("Maximal abs(Target - previous Target) per second."
			+ " NaN or +Inf means no limit (which is default)."
			+ " This can make the output smoother (more human-like control)"
			+ " and help prevent oscillation after target change (windup).")]
		public double targetChangeLimit = double.PositiveInfinity;

		[Description("Maximal abs(output-input)"
			+ " and also abs(target-input) for integral and reduction factors."
			+ " Helps preventing overshooting especially after change of Target (windup)."
			+ " NaN or +Inf means no limit (which is default).")]
		public double outputChangeLimit = double.PositiveInfinity;

		[Description("Limit of abs(target-input) used by P and I factors."
			+ " Prevents over-reactions and also reducing windup.")]
		public double errorLimit = double.PositiveInfinity;

		[Description("Limit of abs(accumulator) used by I and R factors."
			+ " Another anti-windup measure to prevent overshooting.")]
		public double accumulatorLimit = double.PositiveInfinity;
	}
	[Description("PID(R) regulator.")]
	public class PID : PID<PidParams>
	{
		public PID() : base(new PidParams()) { }
		public PID(double scale) : this() => this.scale = scale;
		public PID(double p, double i, double d = 0.0, double r = 0.0) : this()
		{
			P = p;
			I = i;
			D = d;
			R = r;
		}
	}
	[Description("PID(R) regulator (with extra parameters).")]
	public class PID<Params> where Params : PidParams
	{
		protected Params _param;
		protected TimeStamp _stamp;
		protected double _input, _target, _output, _accu;

		[Description("All the parameters.")]
		public Params param => _param;

		[Description("Proportional factor (strength of direct control).")]
		public double P { get => _param.P; set => _param.P = value; }
		[Description("Integral factor (dynamic error-correction, causes oscillation as side-effect).")]
		public double I { get => _param.I; set => _param.I = value; }
		[Description("Derivative factor (dampening - applied to output, reduces the oscillation).")]
		public double D { get => _param.D; set => _param.D = value; }
		[Description("Reduction factor for accumulator"
			+ " (dampening - applied to accumulator used by integral factor,"
			+ " works well against both oscillation and windup).")]
		public double R { get => _param.R; set => _param.R = value; }

		[Description("Difference scaling factor.")]
		public double scale { get => _param.scale; set => _param.scale = value; }

		[Description("Maximal abs(target - previous target) per second."
			+ " NaN or +Inf means no limit (which is default)."
			+ " This can make the output smoother (more human-like control)"
			+ " and help prevent oscillation after target change (windup).")]
		public double targetChangeLimit
		{
			get => _param.targetChangeLimit;
			set => _param.targetChangeLimit = value;
		}
		[Description("Maximal abs(output-input)"
			+ " and also abs(target-input) for integral and reduction factors)."
			+ " Helps preventing overshooting especially after change of Target (windup)."
			+ " NaN or +Inf means no limit (which is default).")]
		public double outputChangeLimit
		{
			get => _param.outputChangeLimit;
			set => _param.outputChangeLimit = value;
		}
		[Description("Limit of abs(target-input) used by P and I factors."
			+ " Prevents over-reactions and also reducing windup.")]
		public double errorLimit
		{
			get => _param.errorLimit;
			set => _param.errorLimit = value;
		}
		[Description("Limit of abs(accumulator) used by I and R factors."
			+ " Another anti-windup measure to prevent overshooting.")]
		public double accumulatorLimit
		{
			get => _param.accumulatorLimit;
			set => _param.accumulatorLimit = value;
		}

		[Description("Feedback (true state - e.g. current pitch;"
			+ " error/difference if Target is NaN).")]
		public double input { get; set; } = double.NaN;
		[Description("Highest input allowed.")]
		public double maxInput { get; set; } = double.PositiveInfinity;
		[Description("Lowest input allowed.")]
		public double minInput { get; set; } = double.NegativeInfinity;

		[Description("Desired state (set point - e.g. desired/wanted pitch;"
			+ " NaN for pure error/difference mode, which is the default)."
			+ " The computed control signal is added to Input if Target is valid,"
			+ " use error/difference mode if you want to add it to Target.")]
		public double target { get; set; } = double.NaN;
		[Description("Highest target allowed.")]
		public double maxTarget { get; set; } = double.PositiveInfinity;
		[Description("Lowest target allowed.")]
		public double minTarget { get; set; } = double.NegativeInfinity;

		[Description("Last computed output value (control signal,"
			+ " call Update() after changing Input/Target).")]
		public double output => _output;
		[Description("Highest output allowed.")]
		public double maxOutput { get; set; } = double.PositiveInfinity;
		[Description("Lowest output allowed.")]
		public double minOutput { get; set; } = double.NegativeInfinity;

		public PID(Params param)
		{
			_param = param;
			reset();
		}

		[Description("Reset internal state of the regulator (won't change PIDR and limits).")]
		public void reset()
		{
			_stamp = TimeStamp.none;
			_input = double.NaN;
			_target = double.NaN;
			_output = double.NaN;
			_accu = 0.0;
			input = double.NaN;
			target = double.NaN;
		}
		[Description("Reset accumulator to zero.")]
		public void resetAccu()
			=> _accu = 0.0;
		[Description("Time elapsed since last update (in seconds), Time.tick after reset.")]
		public double dt
			=> double.IsNaN(_stamp) ? Time.tick : Time.since(_stamp);
		[Description("Time elapsed between last and previous update.")]
		public double lastDt { get; protected set; }

		[Description("Update output according to time elapsed (and Input and Target).")]
		public double update()
		{
			var now = Time.now;
			if (now != _stamp)
			{
				update(double.IsNaN(_stamp) ? Time.tick : now - _stamp);
				_stamp = now;
			}
			return _output;
		}
		[Description("Set input and update output according to time elapsed (provided as dt).")]
		public double update(
			[Description("Time elapsed since last update (in seconds).")]
			double dt,
			[Description("New input/feedback")]
			double input)
		{
			this.input = input;
			return update(dt);
		}
		[Description("Set input and target and update output according to time elapsed (provided as dt).")]
		public double Update(
			[Description("Time elapsed since last update (in seconds).")]
			double dt,
			[Description("New input/feedback.")]
			double input,
			[Description("New target / desired state.")]
			double target)
		{
			this.input = input;
			this.target = target;
			return update(dt);
		}
		[Description("Update output according to time elapsed (provided as dt, using current Input and Target).")]
		public virtual double update(
			[Description("Time elapsed since last update (in seconds).")]
			double dt)
		{
			_stamp = Time.now;
			lastDt = dt;
			var input = RosMath.Clamp(this.input, minInput, maxInput);
			if (double.IsNaN(dt))
				_output = input;
			else
			{
				var targetLimit = targetChangeLimit * dt;
				_target = RosMath.Clamp(RosMath.Clamp(
					target, minTarget, maxTarget),
					_target - targetLimit, _target + targetLimit);
				double error = RosMath.Clamp(
					double.IsNaN(_target) ? input : _target - input,
					-errorLimit, errorLimit);
				double result = 0;
				if (!double.IsNaN(error))
				{
					if (!double.IsNaN(P))
						result = P * error;
					if (!double.IsNaN(I))
						_accu += I * error * dt;
					if (!double.IsNaN(_input))
					{
						double change = input - _input;
						if (double.IsNaN(_target))
							change = -change;
						if (!double.IsNaN(D))
							result -= D * change / dt;
						if (!double.IsNaN(R))
							_accu -= R * change * dt;
					}
				}
				if (double.IsNaN(_accu))
					_accu = 0.0;
				if (Math.Abs(_accu) > accumulatorLimit)
					_accu = _accu < 0 ? -accumulatorLimit : accumulatorLimit;
				result += _accu;
				if (!double.IsNaN(scale))
					result *= scale;
				var outputLimit = outputChangeLimit * dt;
				if (double.IsNaN(_target))
					_output = double.IsNaN(_output) ? result : RosMath.Clamp(
						result, _output - outputLimit, _output + outputLimit);
				else
				{
					if (Math.Abs(result) > outputLimit)
						result = result < 0 ? -outputLimit : outputLimit;
					_output = input + result;
				}
			}
			_input = input;
			if (_output > maxOutput)
				_output = maxOutput;
			if (_output < minOutput)
				_output = minOutput;
			return _output;
		}
	}
}
