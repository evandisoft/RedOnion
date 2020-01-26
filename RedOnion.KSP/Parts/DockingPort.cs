using System;
using System.ComponentModel;
using RedOnion.Attributes;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	[Description(
@"Docking port. Used to dock a ship in a station or to join various parts together.
Can also function as separator when staging is enabled.")]
	public class DockingPort : DecouplerBase
	{
		protected readonly ModuleDockingNode module;
		protected internal DockingPort(Ship ship, Part part, PartBase parent, DecouplerBase decoupler, ModuleDockingNode module)
			: base(PartType.DockingPort, ship, part, parent, decoupler)
			=> this.module = module;

		[Description("Accepts `port` and `dockingPort`. (Case insensitive)")]
		public override bool istype(string name)
			=> name.Equals("port", StringComparison.OrdinalIgnoreCase)
			|| name.Equals("dockingPort", StringComparison.OrdinalIgnoreCase)
			|| base.istype(name);

		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_module_docking_node.html)")]
		public ModuleDockingNode Module => module;
		protected override bool GetStagingEnabled() => module.StagingEnabled();

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
