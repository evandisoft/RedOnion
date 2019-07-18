using System;
using MoonSharp.Interpreter;
using UnityEngine;

namespace RedOnion.KSP.Autopilot
{
	public class FlightControl
	{
		// I just decided on a whim to make it thread safe.
		// Probably never going to matter. :)
		static readonly object instanceLock = new object();

		static FlightControl instance;
		static public FlightControl Instance
		{
			get
			{
				lock (instanceLock)
				{
					if (instance == null)
					{
						instance = new FlightControl();
					}
					return instance;
				}
			}
		}
		
		static public FlightControl GetInstance()
		{
			return Instance;
		}

		public enum SpinMode
		{
			SET_RELDIR,
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

		RelativeDirection targetRel = new RelativeDirection();
		public RelativeDirection TargetRel
		{
			get
			{
				return targetRel;
			}
			set
			{
				Enable();
				CurrentSpinMode = SpinMode.SET_RELDIR;
				targetRel = value;
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

		public void SetRel(double heading,double pitch)
		{
			TargetRel = new RelativeDirection(heading, pitch);
		}

		/// <summary>
		/// TODO: We don't propery handle both possible torque situations
		/// (pos/neg)
		/// </summary>
		/// <returns>The available torque.</returns>
		/// <param name="vessel">Vessel.</param>
		public void GetAllTorque(Vessel vessel,out Vector3 pos, out Vector3 neg)
		{
			pos = new Vector3();
			neg = new Vector3();

			foreach (var part in vessel.Parts)
			{
				foreach (var module in part.Modules)
				{
					if (module is ITorqueProvider torqueProvider)
					{
						Vector3 newPos, newNeg;
						torqueProvider.GetPotentialTorque(out newPos, out newNeg);
						if(torqueProvider is ModuleControlSurface s)
						{
							//newPos *= 10;
							//newNeg *= 10;
							Vector3 currentTorque = Vector3.Cross(s.liftForce,vessel.localCoM - module.transform.position);

						}
						pos += newPos;
						neg += newNeg;
						if(module.GUIName=="Control Surface")
						{
							//Debug.Log("torque is "+newPos+","+newNeg);
						}
					}
				}
			}
		}

		public void GetNonControlSurfaceTorque(Vessel vessel, out Vector3 pos, out Vector3 neg)
		{
			pos = new Vector3();
			neg = new Vector3();

			foreach (var part in vessel.Parts)
			{
				foreach (var module in part.Modules)
				{
					if (module is ITorqueProvider torqueProvider)
					{
						Vector3 newPos, newNeg;
						torqueProvider.GetPotentialTorque(out newPos, out newNeg);
						if (torqueProvider is ModuleControlSurface s)
						{
							//newPos *= 10;
							//newNeg *= 10;
							Vector3 currentTorque = Vector3.Cross(s.liftForce, vessel.localCoM - module.transform.position);

						}
						else
						{
							pos += newPos;
							neg += newNeg;
						}

						if (module.GUIName == "Control Surface")
						{
							//Debug.Log("torque is "+newPos+","+newNeg);
						}
					}
				}
			}
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
			Vessel vessel = FlightGlobals.ActiveVessel;
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
				case SpinMode.SET_RELDIR:
					if(targetRel.TryGetCurrentDir(vessel,out Vector3 dir))
					{
						targetDir = dir;
					}
					SetDir(flightCtrlState);
					break;
			}
		}

		const float maxAngularSpeed = Mathf.PI * 2 / 15;

		public Vector3 CurrentDistanceAxis(Vessel vessel,Vector3 targetDir)
		{
			Vector3 currentDir = vessel.transform.up;
			Vector3 distanceAxis = vessel.transform.worldToLocalMatrix * Vector3.Cross(currentDir.normalized, targetDir.normalized);
			return distanceAxis;
		}
		const float fudgeFactor = 0.9f;
		/// <summary>
		/// Gets spin needed to approach target direction
		/// </summary>
		/// <returns>The needed spin.</returns>
		/// <param name="targetDir">Target dir.</param>
		public Vector3 TargetSpinNeeded(Vector3 targetDir)
		{
			Vessel vessel = FlightGlobals.ActiveVessel;
			if (vessel == null)
			{
				return new Vector3();
			}
			// vessel.transform.up is actually the forward vector for the vessel
			Vector3 currentDir = vessel.transform.up;
			Vector3 angularVelocity = GetAngularVelocity(vessel);
			// angularVelocity gives a local direction for its axis.
			// worldSpaceAngularVelocity is that direction in world space.
			Vector3 worldSpaceAngularVelocity = vessel.transform.localToWorldMatrix * angularVelocity;

			// I aim for a halfway point because otherwise it fails to stop oscillating
			Vector3 halfWayDir = Quaternion.AngleAxis(worldSpaceAngularVelocity.magnitude * Time.deltaTime / 2, worldSpaceAngularVelocity) * currentDir;

			/// We take the cross of currentDir and halfWayDir to get the axis along which
			/// we need to rotate to bring currentDir to halfWayDir. Then we translate it into local coordinates.
			/// in order to bring it into the same coordinate system as angularVelocity
			Vector3 currentDistanceAxis = vessel.transform.worldToLocalMatrix * Vector3.Cross(halfWayDir, targetDir);

			// just angle between halfWayDir and targetDir
			float angularDistance = Vector3.Angle(halfWayDir, targetDir) / 360 * 2 * Mathf.PI;
			//Vector3 currentDistanceAxis = vessel.transform.worldToLocalMatrix * Vector3.Cross(currentDir, target);
			//float angularDistance = Vector3.Angle(currentDir, target) / 360 * 2 * Mathf.PI;
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


			// angular speed at which we are rotating 
			float spinMagnitude = Math.Min(angularDistance / Time.deltaTime, maxAngularSpeed);

			// 
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

		/// <summary>
		/// Find the input needed to either get/stay on course to the desired
		/// direction
		/// return (angularSpeed - maxAngularSpeed * Mathf.Sign(distance)) / (maxAccel * Time.deltaTime);
		/// 
		/// or check if it is time to stop and return the input less than 1
		/// (minStopDistance / distance), that is needed.
		/// 
		/// I don't know the units of angularDistance/angularSpeed. But they are all
		/// in the same units, whatever those units may be.
		/// </summary>
		/// <returns>The needed.</returns>
		/// <param name="angularDistance">Distance.</param>
		/// <param name="angularSpeed">Angular speed.</param>
		/// <param name="maxAccel">Max accel.</param>
		float InputNeeded(float angularDistance, float angularSpeed, float maxAccel)
		{
			// Only check if we are headed toward the desired direction.
			if (angularDistance * angularSpeed > 0)
			{
				// The minimum amount of angular distance we will be able to stop 
				// rotating in.
				float minStopDistance = MinStoppingDistance(angularSpeed, maxAccel);
				// if the angular distance remaining is less than twice the minimum needed to stop
				// we will start providing a response designed to slow us down to a stop
				// right on the desired point.
				// If I did this at less than the minStopDistance we wouldn't be able to stop in time.
				if (minStopDistance > Math.Abs(angularDistance) / 2)
				{
					// Since minStopDistance corresponds to maxAccel (max input)
					// this ratio corresponds to neededInput/MaxInput. (max input is 1)
					return minStopDistance / angularDistance;
				}
			}

			// take any deviation from maxAngularSpeed and return an input that will correct for it
			return (angularSpeed - maxAngularSpeed * Mathf.Sign(angularDistance)) / (maxAccel * Time.deltaTime);
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

			GetNonControlSurfaceTorque(vessel, out Vector3 posInstantTorque, out Vector3 negInstantTorque);
			GetAllTorque(vessel, out Vector3 posAllTorque, out Vector3 negAllTorque);

			// Only use our super precise mode if we have enough instant torque for it to work
			// Works perfectly so far in space, and works well for craft in atmospheric flight
			// that do not have control surfaces. Oscillates indefinitely when used with
			// Craft whose current torque is dominated by control surfaces.
			if (posAllTorque.magnitude < posInstantTorque.magnitude * 2)
			{
				// Remember. X = pitch, Y = roll, and Z = yaw. And we're dealing with
				// rotational speed, not absolute orientation with those values.

				// vessel.transform.up is what I consider the forward vector of the vessel
				Vector3 currentDir = vessel.transform.up;

				Vector3 angularVelocity = GetAngularVelocity(vessel);

				/// We take the cross of currentDir and targetDir to get the axis along which
				/// we need to rotate to bring currentDir to targetDir. Then we translate it into local coordinates.
				/// in order to bring it into the same coordinate system as angularVelocity
				Vector3 distanceAxis = vessel.transform.worldToLocalMatrix * Vector3.Cross(currentDir.normalized, targetDir.normalized);

				// We figure out how much potential rotational acceleration we have on all axis of rotations
				// This is same coordinate system as angularVelocity and distanceAxis
				GetMaxAcceleration(vessel, out Vector3 posAccel, out Vector3 negAccel);

				Vector3 inputNeeded = new Vector3();

				// We are separately finding the inputs needed for each of pitch (x), roll (y), and yaw (z)
				// we chose to not specify roll. We just give the input needed to stop it.
				inputNeeded.x = InputNeeded(distanceAxis.x, angularVelocity.x, posAccel.x);
				inputNeeded.y = angularVelocity.y / (posAccel.y * Time.deltaTime); ;//InputNeeded(distanceAxis.y, angularVelocity.y, maxAccel.y);
				inputNeeded.z = InputNeeded(distanceAxis.z, angularVelocity.z, posAccel.z);

				// We set these, and they need to be clamped in SetPitchRollYaw because
				// our inputNeeded values are often much greater than 1 or less than -1.
				SetPitchRollYaw(flightCtrlState, inputNeeded);
			}
			// TargetSpinNeeded + setSpin works way better for handling control surfaces.
			// When control surface torque is a high percentage of the torque we need to
			// use this method.
			else
			{
				targetSpin = TargetSpinNeeded(targetDir);
				//Debug.Log(targetDir+","+targetSpin);
				SetSpin(flightCtrlState);
			}



			//Debug.Log(distanceAxis+","+angularVelocity+","+pryNeeded);
			//SetPitchRollYaw(flightCtrlState, inputNeeded);


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
			GetMaxAcceleration(vessel,out Vector3 posAccel,out Vector3 negAccel);
			Vector3 pry = MathUtil.Vec.Div(angularVelocity - targetSpin, posAccel * Time.deltaTime);
			SetPitchRollYaw(flightCtrlState, pry);
		}



		public void GetMaxAcceleration(Vessel vessel,out Vector3 posAccel,out Vector3 negAccel)
		{
			GetAllTorque(vessel,out Vector3 posTorque,out Vector3 negTorque);
			posAccel=MathUtil.Vec.Div(posTorque, vessel.MOI);
			negAccel = MathUtil.Vec.Div(negTorque, vessel.MOI);
		}

		public Vector3 GetAngularVelocity(Vessel vessel)
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
