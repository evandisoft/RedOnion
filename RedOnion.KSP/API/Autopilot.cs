using MoonSharp.Interpreter;
using RedOnion.KSP.Autopilot;
using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.API
{
	public class Autopilot : IDisposable
	{
		protected Ship ship;
		protected Vessel hooked;
		protected float throttle;

		protected internal Autopilot(Ship ship)
			=> this.ship = ship;

		~Autopilot() => Dispose(false);
		[Browsable(false), MoonSharpHidden]
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing)
		{
			ship = null;
			Unhook();
		}

		public float Throttle
		{
			get => throttle;
			set => Check(throttle = RosMath.Clamp(value, 0f, 1f));
		}

		protected void Hook()
		{
			if (hooked != null)
				return;
			hooked = ship?.native;
			if (hooked == null)
				return;
			hooked.OnFlyByWire += Callback;
		}
		protected void Unhook()
		{
			if (hooked == null)
				return;
			hooked.OnFlyByWire -= Callback;
			hooked = null;
		}
		protected void Check(float value)
		{
			if (!float.IsNaN(value))
				Hook();
			else if (float.IsNaN(throttle))
				Unhook();
		}

		protected virtual void Callback(FlightCtrlState st)
		{
			if (!float.IsNaN(throttle))
				st.mainThrottle = RosMath.Clamp(throttle, 0f, 1f);
		}

		void AnUpdate()
		{
			var angularVelocity = FlightControl.Instance.GetAngularVelocity(ship.native);

			var pidPitch = new PID();
			var pidRoll = new PID();
			var pidYaw = new PID();

			while (true)
			{
				angularVelocity = FlightControl.Instance.GetAngularVelocity(ship.native);

				pidPitch.Input = angularVelocity.x;
				pidPitch.Target = 0;
				// flightControlState.pitch = pidPitch.Output
				pidRoll.Input = angularVelocity.y;
				pidRoll.Target = 0;
				// flightControlState.roll = pidRoll.Output
				pidYaw.Input = angularVelocity.z;
				pidYaw.Target = 0;
				// flightControlState.yaw = pidYaw.Output
			}
		}
		// 
	}
}
