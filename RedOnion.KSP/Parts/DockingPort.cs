using System;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	public class DockingPort : Decoupler
	{
		protected readonly ModuleDockingNode module;
		protected internal DockingPort(Ship ship, Part part, PartBase parent, Decoupler decoupler, ModuleDockingNode module)
			: base(ship, part, parent, decoupler)
			=> this.module = module;

		public ModuleDockingNode Module => module;
		public float AcquireForce => module.acquireForce;
		public float AcquireRange => module.acquireRange;
		public float AcquireTorque => module.acquireTorque;
		public float AcquireTorqueRoll => module.acquireTorqueRoll;
		public float CaptureRange => module.captureRange;
		public float CaptureVelocity => module.captureMaxRvel;
		public float ReEngageDistance => module.minDistanceToReEngage;

		//TODO: target, undock, direction
	}
}
