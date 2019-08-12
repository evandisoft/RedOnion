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
		protected float _throttle, _elevation, _heading, _bank;

		// for change limiting
		protected float prevPitch;  // for elevation
		protected float prevYaw;    // for heading
		protected float prevRoll;   // for bank

		// these manipulate the direction (and do the hard work)
		protected PID pidElevation = new PID();
		protected PID pidHeading = new PID();
		protected PID pidBank = new PID();

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
			_elevation = float.NaN;
			_heading = float.NaN;
			_bank = float.NaN;
			Unhook();
		}

		public void Reset()
		{
			prevPitch = 0f;
			prevYaw = 0f;
			prevRoll = 0f;
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
		[Description("Target elevation (aka pitch, -180..+180)."
			+ " Values outside -90..+90 flip heading."
			+ "NaN for releasing the control.")]
		public float elevation
		{
			get => _elevation;
			set => Check(_elevation = RosMath.ClampS180(value));
		}
		[Description("Target heading (aka yaw, -180..+180)."
			+ "NaN for releasing the control.")]
		public float heading
		{
			get => _heading;
			set => Check(_heading = RosMath.ClampS180(value));
		}
		[Description("Target bank (aka roll, -180..+180)."
			+ "NaN for releasing the control.")]
		public float bank
		{
			get => _bank;
			set => Check(_bank = RosMath.ClampS180(value));
		}

		protected void Hook()
		{
			if (_hooked != null)
				return;
			_hooked = _ship?.native;
			if (_hooked == null)
				return;
			_hooked.OnFlyByWire += Callback;
			pidElevation.reset();
			pidHeading.reset();
			pidBank.reset();
		}
		protected void Unhook()
		{
			if (_hooked == null)
				return;
			_hooked.OnFlyByWire -= Callback;
			_hooked = null;
		}
		protected void Check(float value)
		{
			if (!float.IsNaN(value))
				Hook();
			else if (float.IsNaN(_throttle)
				&& float.IsNaN(_elevation)
				&& float.IsNaN(_heading)
				&& float.IsNaN(_bank))
				Unhook();
		}

		protected virtual void Callback(FlightCtrlState st)
		{
			if (_hooked == null)
				return;
			if (!float.IsNaN(_throttle))
				st.mainThrottle = RosMath.Clamp(_throttle, 0f, 1f);
			/*
			// TODO: get current elevation/pitch and heading/yaw if one is NaN
			if (!float.IsNaN(_elevation) && !float.IsNaN(_heading))
			{
				var pos = _ship.relative;
				var npos = pos.normalized;
				var north = Vector3.Cross(Vector3.forward, npos).normalized;
				var pitched = Quaternion.AngleAxis(_elevation, west) * north;
				var target = Quaternion.AngleAxis(_heading, pos) * pitched;

				var input = vessel.transform.up;
				var axis = vessel.transform.worldToLocalMatrix
					* Vector3.Cross(input.normalized, target.normalized);
				var speed = (Vector3)_ship.angularVelocity;
			}
			*/
		}
	}
}
