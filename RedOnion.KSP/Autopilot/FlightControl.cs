using System;
using MoonSharp.Interpreter;
using UnityEngine;

namespace RedOnion.KSP.Autopilot {
	public class FlightControl {
		public Vector3 GetAvailableTorque()
		{
			Vessel vessel = FlightGlobals.ActiveVessel;
			if (vessel == null) {
				return new Vector3();
			}

			Vector3 torque = new Vector3();

			foreach(var part in vessel.Parts) {
				foreach(var module in part.Modules) {
					if(module is ITorqueProvider torqueProvider) {
						Vector3 pos, neg;
						torqueProvider.GetPotentialTorque(out pos, out neg);
						torque += pos;
					}
				}
			}

			return torque;
		}

		public void Init()
		{
			Reset();
			Disable();
		}

		FlightCtrlState userCtrlState = new FlightCtrlState();

		void RawControlCallback(FlightCtrlState flightCtrlState)
		{
			flightCtrlState.CopyFrom(userCtrlState);
		}

		public void SetWithTable(Table ctrlTable)
		{
			foreach(var setting in ctrlTable.Keys) {
				if(setting.Type==DataType.String && ctrlTable[setting] is Double value) {
					switch (setting.String) {
					case "roll":
						userCtrlState.roll = Mathf.Clamp((float)value, -1, 1);
						break;
					case "pitch":
						userCtrlState.pitch = Mathf.Clamp((float)value, -1, 1);
						break;
					case "yaw":
						userCtrlState.yaw = Mathf.Clamp((float)value, -1, 1);
						break;
					case "X":
						userCtrlState.X = Mathf.Clamp((float)value, -1, 1);
						break;
					case "Y":
						userCtrlState.Y = Mathf.Clamp((float)value, -1, 1);
						break;
					case "Z":
						userCtrlState.Z = Mathf.Clamp((float)value, -1, 1);
						break;
					case "mainThrottle":
						userCtrlState.mainThrottle = Mathf.Clamp((float)value, 0, 1);
						break;
					}
				}
			}
		}



		public void SetCtrlState(FlightCtrlState flightCtrlState)
		{
			userCtrlState.CopyFrom(flightCtrlState);
		}

		public void Reset()
		{
			userCtrlState = new FlightCtrlState();
		}

		public bool Enable()
		{
			var vessel = FlightGlobals.ActiveVessel;
			if (vessel == null) {
				return false;
			}

			vessel.OnFlyByWire -= RawControlCallback;
			vessel.OnFlyByWire += RawControlCallback;
			return true;
		}

		public bool Disable()
		{
			var vessel = FlightGlobals.ActiveVessel;
			if (vessel == null) {
				return false;
			}

			vessel.OnFlyByWire -= RawControlCallback;
			return true;
		}
	}
}
