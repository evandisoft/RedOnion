using System;
using MoonSharp.Interpreter;
using UnityEngine;

namespace RedOnion.KSP.Autopilot
{
	public class FlightControl
	{
		static FlightControl instance;
		static public FlightControl GetInstance()
		{
			if (instance == null)
			{
				instance = new FlightControl();
			}
			return instance;
		}

		public enum SpinMode
		{
			SET_DIR,
			SET_SPIN,
			RAW,
			OFF,
		}

		public SpinMode CurrentSpinMode { get; private set; }

		Vector3 targetDir = new Vector3();
		public Vector3 TargetDir
		{
			get
			{
				return targetDir;
			}
			set
			{
				Enable();
				CurrentSpinMode = SpinMode.SET_DIR;
				targetDir = value;
			}
		}

		Vector3 targetSpin = new Vector3();
		public Vector3 TargetSpin
		{
			get
			{
				return targetSpin;
			}
			set
			{
				Enable();
				CurrentSpinMode = SpinMode.SET_SPIN;
				targetSpin = value;
			}
		}

		public void SetSpin(float x, float y, float z)
		{
			TargetSpin = new Vector3(x, y, z);
		}

		public void SetSpin(Vector3 vector)
		{
			TargetSpin = vector;
		}

		public void StopSpin()
		{
			TargetSpin = new Vector3();
		}

		protected FlightControl()
		{
			CurrentSpinMode = SpinMode.OFF;
		}

		/// <summary>
		/// TODO: We don't propery handle both possible torque situations
		/// (pos/neg)
		/// </summary>
		/// <returns>The available torque.</returns>
		/// <param name="vessel">Vessel.</param>
		public Vector3 GetAvailableTorque(Vessel vessel)
		{
			Vector3 torque = new Vector3();

			foreach (var part in vessel.Parts)
			{
				foreach (var module in part.Modules)
				{
					if (module is ITorqueProvider torqueProvider)
					{
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
			switch (CurrentSpinMode)
			{
				case SpinMode.SET_SPIN:
					SetSpin(flightCtrlState);
					break;
				case SpinMode.SET_DIR:
					SetDir(flightCtrlState);
					break;
				case SpinMode.RAW:
					SetPitchRollYaw(flightCtrlState, new Vector3(
						userCtrlState.pitch, userCtrlState.roll, userCtrlState.yaw));
					break;
			}
		}

		const float maxAngularSpeed = Mathf.PI * 2 / 15;
		const float fudgeFactor = 0.9f;
		/// <summary>
		/// Gets spin needed to approach target direction
		/// </summary>
		/// <returns>The needed spin.</returns>
		/// <param name="target">Target dir.</param>
		public Vector3 TargetSpinNeeded(Vector3 target)
		{
			Vessel vessel = FlightGlobals.ActiveVessel;
			if (vessel == null)
			{
				return new Vector3();
			}
			Vector3 currentDir = vessel.transform.up;
			Vector3 angularSpeed = GetAngularVelocity(vessel);
			Vector3 worldSpaceAngularSpeed = vessel.transform.localToWorldMatrix * angularSpeed;
			Vector3 halfWayDir = Quaternion.AngleAxis(worldSpaceAngularSpeed.magnitude * Time.deltaTime / 2, worldSpaceAngularSpeed) * currentDir;

			Vector3 currentDistanceAxis = vessel.transform.worldToLocalMatrix * Vector3.Cross(halfWayDir, target);
			float angularDistance = Vector3.Angle(halfWayDir, target) / 360 * 2 * Mathf.PI;
			//(d-vt)*2/t^2=a
			//accel=(angularDistance-angularSpeed*Time.deltaTime)/Time.deltaTime^2
			// angularSpeedFinal/2*Time.deltaTime=angularDistanceFinal
			// angularSpeedFinal=angularSpeedStart+accel*Time.deltaTime
			// 0=angularSpeedFinal=angularSpeedStart+accel*Time.deltaTime
			// 0=angularDistanceFinal=desiredSpinAxis+angularSpeedStart*Time.deltaTime+1/2*accel*Time.deltaTime*Time.deltaTime
			// currentDistance=angularSpeedStart/2*Time.deltaTime
			// if(maxAccel*Time.deltaTime>angularSpeed and 
			// v.x+a.x*t=0, v.y+a.y*t=0, v.z+a.z*t=0 
			// d.x+v.x*t+



			float spinMagnitude = Math.Min(angularDistance / Time.deltaTime, maxAngularSpeed);
			return currentDistanceAxis * spinMagnitude; //*fudgeFactor;
		}

		float Accel(float distance, float velocity)
		{
			return (distance - velocity * Time.deltaTime) * 2 / Mathf.Pow(Time.deltaTime, 2);
		}


		float MinStoppingDistance(float speed, float maxAccel)
		{
			float absSpeed = Math.Abs(speed);
			float minStoppingTime = absSpeed / maxAccel;
			return absSpeed / 2 * minStoppingTime;
		}

		float InputNeeded(float distance, float angularSpeed, float maxAccel)
		{
			if (distance * angularSpeed > 0)
			{
				float minStopDistance = MinStoppingDistance(angularSpeed, maxAccel);
				if (minStopDistance > Math.Abs(distance) / 2)
				{
					return minStopDistance / distance;
				}
			}

			return (angularSpeed - maxAngularSpeed * Mathf.Sign(distance)) / (maxAccel * Time.deltaTime);
		}

		void SetDir(FlightCtrlState flightCtrlState)
		{
			//Debug.Log("SetDir");
			Vessel vessel = FlightGlobals.ActiveVessel;
			if (vessel == null)
			{
				Shutdown();
				return;
			}

			Vector3 currentDir = vessel.transform.up;
			Vector3 angularVelocity = GetAngularVelocity(vessel);
			float angularDistance = Vector3.Angle(currentDir, targetDir) / 360 * 2 * Mathf.PI;
			////(d-vt)*2/t^2=a
			////accel=(angularDistance-angularSpeed*Time.deltaTime)/Time.deltaTime^2
			Vector3 distanceAxis = vessel.transform.worldToLocalMatrix * Vector3.Cross(currentDir.normalized, targetDir.normalized);
			Vector3 maxAccel = GetMaxAcceleration(vessel);
			//Vector3 desiredAccel=(-distanceAxis + angularSpeed * Time.deltaTime) * 2 / (float)Math.Pow(Time.deltaTime,2);


			Vector3 inputNeeded = new Vector3();

			inputNeeded.x = InputNeeded(distanceAxis.x, angularVelocity.x, maxAccel.x);
			inputNeeded.y = angularVelocity.y / (maxAccel.y * Time.deltaTime); ;//InputNeeded(distanceAxis.y, angularVelocity.y, maxAccel.y);
			inputNeeded.z = InputNeeded(distanceAxis.z, angularVelocity.z, maxAccel.z);

			//Debug.Log(distanceAxis+","+angularVelocity+","+pryNeeded);
			SetPitchRollYaw(flightCtrlState, inputNeeded);


			//Debug.Log(flightCtrlState.pitch + "," + flightCtrlState.roll + "," + flightCtrlState.yaw);
			//if (angularVelocity.magnitude < maxAngularSpeed+0.01) {

			//} else {
			//	targetSpin = TargetSpinNeeded(targetDir);
			//	SetSpin(flightCtrlState);
			//}
			//float pitch = Accel(distanceAxis.x, angularVelocity.x) / maxAccel.x;
			//if(Accel(distanceAxis.x, angularVelocity.x) <= maxAccel.x) {

			//}
			//float roll = Accel(distanceAxis.y, angularVelocity.y) / maxAccel.y;
			//float yaw = Accel(distanceAxis.z, angularVelocity.z) / maxAccel.z;

			//Vector3 pry = MathUtil.Vec.Div(desiredAccel, maxAccel);
			//Debug.Log(pry);
			//SetPitchRollYaw(flightCtrlState, pry);
			//targetSpin = TargetSpinNeeded(targetDir);
			//Debug.Log(targetDir+","+targetSpin);
			//SetSpin(flightCtrlState);


		}

		void SetSpin(FlightCtrlState flightCtrlState)
		{
			Vessel vessel = FlightGlobals.ActiveVessel;
			if (vessel == null)
			{
				Shutdown();
				return;
			}

			Vector3 angularVelocity = GetAngularVelocity(vessel);
			Vector3 maxAccel = GetMaxAcceleration(vessel);
			Vector3 pry = MathUtil.Vec.Div(angularVelocity - targetSpin, maxAccel * Time.deltaTime);
			SetPitchRollYaw(flightCtrlState, pry);
		}



		Vector3 GetMaxAcceleration(Vessel vessel)
		{
			Vector3 torque = GetAvailableTorque(vessel);
			return MathUtil.Vec.Div(torque, vessel.MOI);
		}

		Vector3 GetAngularVelocity(Vessel vessel)
		{
			return MathUtil.Vec.Div(vessel.angularMomentum, vessel.MOI);
		}

		/// <summary>
		/// Set each setting only if they are not already set and clamp the values
		/// to the accepted -1 to 1 range.
		/// </summary>
		/// <param name="flightCtrlState">Flight ctrl state.</param>
		/// <param name="pry">Pry.</param>
		void SetPitchRollYaw(FlightCtrlState flightCtrlState, Vector3 pry)
		{
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			if (flightCtrlState.pitch == 0.0)
			{
				flightCtrlState.pitch = Mathf.Clamp(pry.x, -1, 1);
			}
			if (flightCtrlState.roll == 0.0)
			{
				flightCtrlState.roll = Mathf.Clamp(pry.y, -1, 1);
			}
			if (flightCtrlState.yaw == 0.0)
			{
				flightCtrlState.yaw = Mathf.Clamp(pry.z, -1, 1);
			}
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
		}

		public void SetWithTable(Table ctrlTable)
		{
			foreach (var setting in ctrlTable.Keys)
			{
				if (setting.Type == DataType.String && ctrlTable[setting] is Double value)
				{
					switch (setting.String)
					{
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
			CurrentSpinMode = SpinMode.RAW;
			Enable();
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
			if (vessel == null)
			{
				return false;
			}

			vessel.OnFlyByWire -= ControlCallback;
			vessel.OnFlyByWire += ControlCallback;
			return true;
		}

		public bool Disable()
		{
			var vessel = FlightGlobals.ActiveVessel;
			if (vessel == null)
			{
				return false;
			}

			vessel.OnFlyByWire -= ControlCallback;
			return true;
		}
	}
}
