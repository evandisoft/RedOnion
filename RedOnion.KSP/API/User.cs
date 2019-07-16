using MoonSharp.Interpreter;
using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.API
{
	public class User
	{
		[Browsable(false), MoonSharpHidden]
		public static User Instance { get; } = new User();
		protected User() { }

		public float Throttle
		{
			get => FlightInputHandler.state.mainThrottle;
			set
			{
				if (!float.IsNaN(value))
					FlightInputHandler.state.mainThrottle = RosMath.Clamp(value, 0f, 1f);
			}
		}
	}
}
