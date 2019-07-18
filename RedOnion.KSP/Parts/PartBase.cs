using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	[DisplayName("Part"), Description("Part of the ship (vehicle/vessel).")]
	public class PartBase
	{
		[Description("Ship (vehicle/vessel) this part belongs to.")]
		public Ship ship { get; }
		[Description("Native `Part` - KSP API.")]
		public Part native { get; }
		[Description("Parent part (this part is attached to).")]
		public PartBase parent { get; }
		[Description("Decoupler that will decouple this part when staged.")]
		public Decoupler decoupler { get; }
		[Description("Stage number as provided by KSP API. (`Native.inverseStage`)")]
		public int stage => native.inverseStage;
		[Description("Stage number where this part will be decoupled or -1. (`Decoupler?.Stage ?? -1`)")]
		public int decoupledin => decoupler?.stage ?? -1;

		[Description("Resources contained within this part.")]
		public ResourceList resources => privateResources ?? (privateResources = new ResourceList(this));
		ResourceList privateResources;

		[Description("State of the part (IDLE, ACTIVE (e.g. engine), DEACTIVATED, DEAD, FAILED).")]
		public PartStates state => native.State;

		// TODO: make this generic feature of ROS (`typeof` and `is` operators)
		[Description("Method to test the type of the part (e.g. `.IsType(\"LaunchClamp\")`)")]
		public virtual bool istype(string name) => false;

		protected internal PartBase(Ship ship, Part native, PartBase parent, Decoupler decoupler)
		{
			this.ship = ship;
			this.native = native;
			this.parent = parent;
			this.decoupler = decoupler;
		}
	}
}
