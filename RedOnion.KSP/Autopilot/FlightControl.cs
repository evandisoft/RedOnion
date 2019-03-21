using System;
using MoonSharp.Interpreter;

namespace RedOnion.KSP.Autopilot {
	public class FlightControl {
		FlightCtrlState userCtrlState = new FlightCtrlState();

		void RawControlCallback(FlightCtrlState flightCtrlState)
		{
			if (userCtrlState != null) {
				flightCtrlState.CopyFrom(userCtrlState);
			}
		}

		public void SetWithTable(Table ctrlTable)
		{
			//userCtrlState.pitch=ctrlTable["pitch"]
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
