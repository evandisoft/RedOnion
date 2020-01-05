using MoonSharp.Interpreter;
using RedOnion.ROS;
using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RedOnion.KSP.API
{
	[Description("Autopilot (throttle and steering) for a ship (vehicle/vessel).")]
	public class Autopilot : IDisposable
	{
		protected Ship _ship;
		protected Vessel _hooked;

		// inputs
		protected float _throttle, _rawPitch, _rawYaw, _rawRoll;
		protected double _pitch, _heading, _roll;
		protected Vector _direction;
		protected bool _killRot;

		protected internal Autopilot(Ship ship)
		{
			_ship = ship;
			disable();
			reset();
		}

		[Description("Disable the autopilot, setting all values to NaN.")]
		public void disable()
		{
			_throttle = float.NaN;
			_rawPitch = float.NaN;
			_rawYaw = float.NaN;
			_rawRoll = float.NaN;
			_pitch = double.NaN;
			_heading = double.NaN;
			_roll = double.NaN;
			_direction = new Vector(double.NaN, double.NaN, double.NaN);
			_killRot = false;
			Unhook();
		}

		[Description("Reset the autopilot to default settings.")]
		public void reset()
			=> pids.reset();

		~Autopilot() => Dispose(false);
		[Browsable(false), MoonSharpHidden]
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing)
		{
			_ship = null;
			if (disposing)
				Unhook();
			else if (_hooked != null)
				UI.Collector.Add(this);
		}

		[Description("Throttle control (0..1). NaN for releasing the control.")]
		public float throttle
		{
			get => _throttle;
			set => Check(_throttle = RosMath.Clamp(value, 0f, 1f));
		}

		[Description("Raw pitch control (up-down, -1..+1). NaN for releasing the control.")]
		public float rawPitch
		{
			get => _rawPitch;
			set => Check(_rawPitch = RosMath.Clamp(value, -1f, +1f));
		}
		[Description("Raw yaw control (left-right, -1..+1). NaN for releasing the control.")]
		public float rawYaw
		{
			get => _rawYaw;
			set => Check(_rawYaw = RosMath.Clamp(value, -1f, +1f));
		}
		[Description("Raw roll control (rotation, -1..+1). NaN for releasing the control.")]
		public float rawRoll
		{
			get => _rawRoll;
			set => Check(_rawRoll = RosMath.Clamp(value, -1f, +1f));
		}

		[Description("Target direction vector."
			+ " NaN/vector.none for releasing the control.")]
		public Vector direction
		{
			get
			{
				if (double.IsNaN(_direction.x)
					|| double.IsNaN(_direction.y)
					|| double.IsNaN(_direction.z))
				{
					var pitch = _pitch;
					if (double.IsNaN(pitch))
						pitch = _ship.pitch;
					var heading = _heading;
					if (double.IsNaN(heading))
						heading = _ship.heading;
					return _ship.north.rotate(-pitch, _ship.east).rotate(heading, _ship.away);
				}
				return _direction;
			}
			set => Check((_direction = double.IsNaN(value.x)
				|| double.IsNaN(value.y) || double.IsNaN(value.z)
				? new Vector(double.NaN, double.NaN, double.NaN)
				: value).x);
		}

		[Description("Target heading [0..360]."
			+ " NaN for releasing the control.")]
		public double heading
		{
			get => _heading;
			set => Check(_heading = RosMath.Clamp360(value));
		}
		[Description("Target pitch/elevation [-180..+180]."
			+ " Values outside -90..+90 flip heading."
			+ " NaN for releasing the control.")]
		public double pitch
		{
			get => _pitch;
			set => Check(_pitch = RosMath.ClampS180(value));
		}
		[Description("Target roll/bank [-180..+180]."
			+ " NaN for releasing the control.")]
		public double roll
		{
			get => _roll;
			set => Check(_roll = RosMath.ClampS180(value));
		}
		[Description("Fix the roll of the ship. Note that this is not SAS and currently does not allow user override.")]
		public bool killRot
		{
			get => _killRot;
			set => Check((_killRot = value) ? 1.0 : double.NaN);
		}

		[Description("SAS: Stability Assist System. This is stock alternative to `killRot` which allows user override."
			+ " Can be used to allow user/player to adjust roll while autopilot controls direction.")]
		public bool sas
		{
			get => _ship.sas;
			set => _ship.sas = value;
		}
		[Description("RCS: Reaction Control System.")]
		public bool rcs
		{
			get => _ship.rcs;
			set => _ship.rcs = value;
		}

		public PIDs pids { get; } = new PIDs();
		protected PID pitchPID => pids._pitch;
		protected PID yawPID => pids._yaw;
		protected PID rollPID => pids._roll;
		public class PidParams : API.PidParams
		{
			// todo: maximal angular velocity and maximal stopping time
			public double angular { get; set; }
			public double time { get; set; }
			protected internal PidParams() => reset();
			public virtual void reset()
			{
				P = 1.0;    // direct control
				I = 0.3;    // error-correcting, *dt
				R = 0.25;   // cumulated change, *dt
				D = 0.05;   // change-resisting, /dt
				outputChangeLimit = 5; //*dt => 10% per std. tick
				targetChangeLimit = 5; //*dt => 10% per std. tick
				accumulatorLimit = 0.5; // abs(accu) <= 50%
				errorLimit = 1; // abs(err) <= 100%
				time = 3.0;
				angular = 10.0;
			}

			internal class Roll : PidParams
			{
				public override void reset()
				{
					base.reset();
					I = 0.0;
					R = 0.1;
				}
			}
		}
		public class PID : API.PID<PidParams>
		{
			protected internal PID() : base(new PidParams()) => Init();
			protected internal PID(PidParams param) : base(param) => Init();
			protected internal void Init()
			{
				param.reset();
				maxInput = +1.0;
				minInput = -1.0;
				maxOutput = +1.0;
				minOutput = -1.0;
				reset();
			}
			public double angular
			{
				get => param.angular;
				set => param.angular = value;
			}
			public double time
			{
				get => param.time;
				set => param.time = value;
			}
		}
		public class PIDs
		{
			protected internal PIDs() { }

			protected internal PID _pitch = new PID();
			protected internal PID _yaw = new PID();
			protected internal PID _roll = new PID(new PidParams.Roll());

			public PidParams pitch => _pitch.param;
			public PidParams yaw => _yaw.param;
			public PidParams roll => _roll.param;

			double combine(double a, double b)
				=> a == b ? b : double.NaN;
			double combine(double a, double b, double c)
				=> a == b && b == c ? c : double.NaN;

			public double P
			{
				get => combine(pitch.P, yaw.P, roll.P);
				set => roll.P = yaw.P = pitch.P = value;
			}
			public double I
			{
				get => combine(pitch.I, yaw.I, roll.I);
				set => roll.I = yaw.I = pitch.I = value;
			}
			public double D
			{
				get => combine(pitch.D, yaw.D, roll.D);
				set => roll.D = yaw.D = pitch.D = value;
			}
			public double R
			{
				get => combine(pitch.R, yaw.R, roll.R);
				set => roll.R = yaw.R = pitch.R = value;
			}
			public double time
			{
				get => combine(pitch.time, yaw.time, roll.time);
				set => roll.time = yaw.time = pitch.time = value;
			}

			public void reset()
			{
				_pitch.Init();
				_yaw.Init();
				_roll.Init();
			}
		}

		protected void Hook()
		{
			if (_hooked != null)
				return;
			_hooked = _ship?.native;
			if (_hooked == null)
				return;
			_hooked.OnFlyByWire += Callback;
			pitchPID.reset();
			yawPID.reset();
			rollPID.reset();
		}
		protected void Unhook()
		{
			if (_hooked == null)
				return;
			_hooked.OnFlyByWire -= Callback;
			_hooked = null;
		}
		protected void Check(double value)
		{
			if (!double.IsNaN(value))
				Hook();
			else if (double.IsNaN(_throttle)
				&& double.IsNaN(_pitch) && double.IsNaN(_heading) && double.IsNaN(_roll)
				&& double.IsNaN(_direction.x * _direction.y * _direction.z)
				&& float.IsNaN(_rawPitch) && float.IsNaN(_rawYaw) && float.IsNaN(_rawRoll))
				Unhook();
		}

		protected virtual void Callback(FlightCtrlState st)
		{
			if (_hooked == null)
				return;
			if (!float.IsNaN(_throttle))
				st.mainThrottle = RosMath.Clamp(_throttle, 0f, 1f);

			// could probably use InputLockManager.lockStack.ContainsKey("TimeWarp") instead
			if (TimeWarp.index > 0 && TimeWarp.high)
			{
				pitchPID.resetAccu();
				yawPID.resetAccu();
				rollPID.resetAccu();
				return;
			}

			var angvel = _ship.angularVelocity;
			var maxang = _ship.maxAngular;
			if (!double.IsNaN(_direction.x)
				|| !double.IsNaN(_pitch)
				|| !double.IsNaN(_heading))
			{
				// translate the direction into local space
				// (like if we were looking at the vector from cockpit)
				var want = _ship.local(direction).normalized;
				// now get the angles in respective planes and feed it into the PIDs
				// ship.forward => (0,1,0)  <= transform.up
				// ship.right   => (1,0,0)  <= transform.right
				// ship.up      => (0,0,-1) <= -transform.forward
				var pitchDiff = RosMath.Deg.Atan2(-want.z, want.y);
				var yawDiff = RosMath.Deg.Atan2(want.x, want.y);
				// compute control inputs (X=pitch, Z=yaw - the one not used in the atan2 above)
				var pitch = (float)AngularControl(pitchPID, pitchDiff, angvel.x, maxang.x);
				var yaw = (float)AngularControl(yawPID, yawDiff, angvel.z, maxang.z);
				// set the controls
				if (!float.IsNaN(pitch))
					st.pitch = RosMath.Clamp(pitch, -1f, +1f);
				if (!float.IsNaN(yaw))
					st.yaw = RosMath.Clamp(yaw, -1f, +1f);
			}
			else if (killRot)
			{
				// compute control inputs
				var pitch = (float)AngularControl(pitchPID, 0, angvel.x, maxang.x);
				var yaw = (float)AngularControl(yawPID, 0, angvel.z, maxang.z);
				// set the controls
				if (!float.IsNaN(pitch))
					st.pitch = RosMath.Clamp(pitch, -1f, +1f);
				if (!float.IsNaN(yaw))
					st.yaw = RosMath.Clamp(yaw, -1f, +1f);
			}
			if (!double.IsNaN(_roll) || killRot)
			{
				var rollDiff = 0.0;
				if (!double.IsNaN(_roll))
				{
					if (Math.Abs(_ship.pitch) <= 89.0)
						rollDiff = RosMath.ClampS180(_roll - _ship.roll);
					else if (!double.IsNaN(_heading))
						rollDiff = RosMath.ClampS180(_roll - 180.0
							- _ship.up.angle(_ship.north.rotate(_heading, _ship.away), _ship.away));
				}
				var roll = (float)AngularControl(rollPID, rollDiff, angvel.y, maxang.y);
				if (!float.IsNaN(roll))
					st.roll = RosMath.Clamp(roll, -1f, +1f);
			}
			if (!float.IsNaN(_rawPitch))
				st.pitch = RosMath.Clamp(_rawPitch, -1f, +1f);
			if (!float.IsNaN(_rawYaw))
				st.yaw = RosMath.Clamp(_rawYaw, -1f, +1f);
			if (!float.IsNaN(_rawRoll))
				st.roll = RosMath.Clamp(_rawRoll, -1f, +1f);
		}
		/// <summary>
		/// Calculate PYR control input for given parameters. 
		/// </summary>
		/// <param name="pid">Associated PID(R) controller.</param>
		/// <param name="angle">Angle towards target (signed).</param>
		/// <param name="speed">Angular speed (deg/s, signed).</param>
		/// <param name="accel">Angular acceleration (deg/s/s, unsigned) at full control.</param>
		/// <returns>New control fraction (0-100%).</returns>
		protected virtual double AngularControl(PID pid, double angle, double speed, double accel)
		{
			// double the angle we will still travel if we try to stop immediately.
			// abs(speed)/accel is the time needed, 0.5*speed would be average speed,
			// we use double of that because we always add the angle difference.
			var stop = speed * (Math.Abs(speed)/accel + 0.3 + 10 * pid.dt);
			// TODO: recheck the signs and what they mean, should be angle-stop
			pid.input = 10 * (angle + stop) / accel;
			return pid.update();
		}
	}
}
