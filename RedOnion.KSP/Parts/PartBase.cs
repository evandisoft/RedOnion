using System;
using System.ComponentModel;
using RedOnion.KSP.API;
using RedOnion.KSP.Attributes;
using RedOnion.ROS;

namespace RedOnion.KSP.Parts
{
	[DisplayName("Part"), Description("Part of the ship (vehicle/vessel).")]
	public class PartBase
	{
		[Unsafe, Description("Native `Part` - KSP API.")]
		public Part native { get; }

		[Description("Ship (vehicle/vessel) this part belongs to.")]
		public Ship ship { get; }
		[Description("Parent part (this part is attached to).")]
		public PartBase parent { get; }
		[Description("Parts attached to this part.")]
		public PartChildren children => _children ?? (_children = new PartChildren(this));
		PartChildren _children;

		[Description("Decoupler that will decouple this part when staged.")]
		public Decoupler decoupler { get; }
		[Description("Stage number as provided by KSP API. (`Native.inverseStage`)")]
		public int stage => native.inverseStage;
		[Description("Stage number where this part will be decoupled or -1. (`Decoupler?.Stage ?? -1`)")]
		public int decoupledin => decoupler?.stage ?? -1;

		[Description("Resources contained within this part.")]
		public ResourceList resources => _resources ?? (_resources = new ResourceList(this));
		ResourceList _resources;

		[Description("State of the part (IDLE, ACTIVE (e.g. engine), DEACTIVATED, DEAD, FAILED).")]
		public PartStates state => native.State;

		[Description("Name of the part (assigned by KSP).")]
		public string name => native.name;
		[Description("Title of the part (assigned by KSP).")]
		public string title => native.partInfo.title;

		// TODO: make this generic feature of ROS (`typeof` and `is` operators)
		[Description("Method to test the type of the part (e.g. `.IsType(\"LaunchClamp\")`)")]
		public virtual bool istype(string name) => false;

		[Description("Explode the part.")]
		public void explode() => native.explode();

		public override string ToString()
		{
			return native.ToString();
		}

		protected internal PartBase(Ship ship, Part native, PartBase parent, Decoupler decoupler)
		{
			this.ship = ship;
			this.native = native;
			this.parent = parent;
			this.decoupler = decoupler;
		}
	}
}
