using System;
using MoonSharp.Interpreter;
using UnityEngine;

namespace RedOnion.KSP.Autopilot {


	public class FlightControl {
		public enum SpinMode {
			SET_DIR,
			SET_SPIN,
			RAW,
			OFF,
		}

		public SpinMode CurrentSpinMode { get; private set; }

		Vector3 targetDir;
		public Vector3 TargetDir { 
			get {
				return targetDir;
			}
			set {
				Enable();
				CurrentSpinMode = SpinMode.SET_DIR;
				targetDir = value;
			}
		}

		Vector3 targetSpin;
		public Vector3 TargetSpin {
			get {
				return targetSpin;
			}
			set {
				Enable();
				CurrentSpinMode = SpinMode.SET_SPIN;
				targetSpin = value;
			}
		}

		public void StopSpin()
		{
			TargetSpin = new Vector3();
		}

		public FlightControl()
		{
			CurrentSpinMode = SpinMode.OFF;
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
			CurrentSpinMode = SpinMode.OFF;
		}

		FlightCtrlState userCtrlState = new FlightCtrlState();

		void ControlCallback(FlightCtrlState flightCtrlState)
		{
			//flightCtrlState.CopyFrom(userCtrlState);
			switch (CurrentSpinMode) {
			case SpinMode.SET_SPIN:
				SetSpin(flightCtrlState);
				break;
			case SpinMode.SET_DIR:
				throw new NotImplementedException("SET_DIR mode not yet implemented");
				break;
			case SpinMode.RAW:
				SetPitchRollYaw(flightCtrlState, new Vector3(
					userCtrlState.pitch, userCtrlState.roll, userCtrlState.yaw));
				break;
			}
		}

		void SetSpin(FlightCtrlState flightCtrlState)
		{
			Vessel vessel = FlightGlobals.ActiveVessel;
			if (vessel == null) {
				Shutdown();
				return;
			}

			Vector3 torque = GetAvailableTorque();

			Vector3 angularSpeed = MathUtil.Vec.Div(vessel.angularMomentum, vessel.MOI);
			Vector3 accel = MathUtil.Vec.Div(torque, vessel.MOI);
			Vector3 pry = MathUtil.Vec.Div(angularSpeed-TargetSpin, accel * Time.deltaTime);
			SetPitchRollYaw(flightCtrlState, pry);
		}

		/// <summary>
		/// Set each setting only if they are not already set and clamp the values
		/// to the accepted -1 to 1 range.
		/// </summary>
		/// <param name="flightCtrlState">Flight ctrl state.</param>
		/// <param name="pry">Pry.</param>
		void SetPitchRollYaw(FlightCtrlState flightCtrlState,Vector3 pry)
		{
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			if (flightCtrlState.pitch == 0.0) {
				flightCtrlState.pitch = Mathf.Clamp(pry.x,-1,1);
			}
			if (flightCtrlState.roll == 0.0) {
				flightCtrlState.roll = Mathf.Clamp(pry.y, -1, 1);
			}
			if (flightCtrlState.yaw == 0.0) {
				flightCtrlState.yaw = Mathf.Clamp(pry.z,-1,1);
			}

#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
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
			//CurrentSpinMode = Mode.RAW;
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
