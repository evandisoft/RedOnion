using System;
using MoonSharp.Interpreter;
using UnityEngine;

namespace RedOnion.KSP.Autopilot {


	public class FlightControl {
		public enum Mode {
			RAW,
			STOP_SPIN,
			SET_DIR,
			SET_SPIN,
			STABILIZE,
			OFF,
		}

		public Mode CurrentMode { get; private set; }

		public Vector3 TargetDir { 
			get {
				return TargetDir;
			}
			set {
				CurrentMode = Mode.SET_DIR;
				TargetDir = value;
			}
		}

		public Vector3 TargetSpin {
			get {
				return TargetSpin;
			}
			set {
				CurrentMode = Mode.SET_SPIN;
				TargetSpin = value;
			}
		}

		public void SetTargetSpin(Table table) {

		}

		public FlightControl()
		{
			CurrentMode = Mode.OFF;
		}

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

		public void Shutdown()
		{
			Reset();
			Disable();
			CurrentMode = Mode.OFF;
		}

		FlightCtrlState userCtrlState = new FlightCtrlState();

		void ControlCallback(FlightCtrlState flightCtrlState)
		{
			if (CurrentMode != Mode.STABILIZE) {
				flightCtrlState.CopyFrom(userCtrlState);
				switch (CurrentMode) {
				case Mode.SET_SPIN:

					break;
				case Mode.SET_DIR:
					throw new NotImplementedException("SET_DIR mode not yet implemented");
					break;
				}
			}
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
			CurrentMode = Mode.RAW;
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

			vessel.OnFlyByWire -= ControlCallback;
			vessel.OnFlyByWire += ControlCallback;
			return true;
		}

		public bool Disable()
		{
			var vessel = FlightGlobals.ActiveVessel;
			if (vessel == null) {
				return false;
			}

			vessel.OnFlyByWire -= ControlCallback;
			return true;
		}
	}
}
