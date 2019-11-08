using MoonSharp.Interpreter;
using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.API
{
	public class Player
	{
		[Browsable(false), MoonSharpHidden]
		public static Player Instance { get; } = new Player();
		protected Player() { }

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
