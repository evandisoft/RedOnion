using MoonSharp.Interpreter;
using RedOnion.KSP.Autopilot;
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
		protected Vector3d _direction;

		// these manipulate the direction (and do the hard work)
		protected PID pidPitch = new PID();
		protected PID pidYaw = new PID();
		protected PID pidRoll = new PID();

		protected internal Autopilot(Ship ship)
		{
			_ship = ship;
			Disable();
			Reset();
		}

		[Description("Disable the autopilot, setting all values to NaN.")]
		public void Disable()
		{
			_throttle = float.NaN;
			_rawPitch = float.NaN;
			_rawYaw = float.NaN;
			_rawRoll = float.NaN;
			_pitch = double.NaN;
			_heading = double.NaN;
			_roll = double.NaN;
			_direction = new Vector3d(double.NaN, double.NaN, double.NaN);
			Unhook();
		}

		[Description("Reset the autopilot to default settings.")]
		public void Reset()
		{
			ResetPID(pidPitch);
			ResetPID(pidYaw);
			ResetPID(pidRoll);
			pidPitch.scale = 0.3;
			pidYaw.scale = 0.1;
			pidRoll.scale = 0.1;
		}
		void ResetPID(PID pid)
		{
			pid.P = 0.9;
			pid.I = 0.1;
			pid.D = 0.5;
			pid.R = 0.2;
			pid.maxOutput = +1.0;
			pid.minOutput = -1.0;
			pid.outputChangeLimit = 10;
			pid.targetChangeLimit = 10;
			pid.reset();
		}

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
			Unhook();
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

		[Convert(typeof(Vector)), Description("Target direction vector."
			+ " NaN/vector.none for releasing the control.")]
		public Vector3d direction
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
					return QuaternionD.AngleAxis(heading, _ship.away) *
						(QuaternionD.AngleAxis(-pitch, _ship.east) * _ship.north);
				}
				return _direction;
			}
			set
			{
				_direction = double.IsNaN(value.x) || double.IsNaN(value.y) || double.IsNaN(value.z)
					? new Vector3d(double.NaN, double.NaN, double.NaN) : value;
				Check(value.x);
			}
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
			+ "NaN for releasing the control.")]
		public double pitch
		{
			get => _pitch;
			set => Check(_pitch = RosMath.ClampS180(value));
		}
		[Description("Target roll/bank [-180..+180]."
			+ "NaN for releasing the control.")]
		public double roll
		{
			get => _roll;
			set => Check(_roll = RosMath.ClampS180(value));
		}

		protected void Hook()
		{
			if (_hooked != null)
				return;
			_hooked = _ship?.native;
			if (_hooked == null)
				return;
			_hooked.OnFlyByWire += Callback;
			pidPitch.reset();
			pidYaw.reset();
			pidRoll.reset();
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
			if (!double.IsNaN(_direction.x)
				|| !double.IsNaN(_pitch)
				|| !double.IsNaN(_heading))
			{
				// translate the direction into local space
				// (like if we were looking at the vector from cockpit)
				var want = _ship.native.transform.InverseTransformDirection(
					((Vector3)direction).normalized);
				// now get the angles in respective planes and feed it into the PIDs
				// ship.forward => (0,1,0)  <= transform.up
				// ship.right   => (1,0,0)  <= transform.right
				// ship.up      => (0,0,-1) <= -transform.forward
				pidPitch.input = RosMath.Deg.Atan2(-want.z, want.y);
				pidYaw.input = RosMath.Deg.Atan2(want.x, want.y);
				var pitch = (float)pidPitch.Update();
				var yaw = (float)pidYaw.Update();
				if (!float.IsNaN(pitch) && !float.IsNaN(yaw))
				{
					st.pitch = RosMath.Clamp(pitch, -1f, +1f);
					st.yaw = RosMath.Clamp(yaw, -1f, +1f);
				}
			}
			if (!double.IsNaN(_roll))
			{
				if (Math.Abs(_ship.pitch) >= 89.9)
					st.roll = 0;
				else
				{
					pidRoll.input = _roll - _ship.roll;
					var roll = (float)pidRoll.Update();
					if (!float.IsNaN(roll))
						st.roll = RosMath.Clamp(roll, -.1f, +.1f);
				}
			}
			if (!float.IsNaN(_rawPitch))
				st.pitch = RosMath.Clamp(_rawPitch, -1f, +1f);
			if (!float.IsNaN(_rawYaw))
				st.yaw = RosMath.Clamp(_rawYaw, -1f, +1f);
			if (!float.IsNaN(_rawRoll))
				st.roll = RosMath.Clamp(_rawRoll, -1f, +1f);
		}
	}
}
