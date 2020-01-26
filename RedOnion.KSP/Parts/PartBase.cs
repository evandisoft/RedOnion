using System;
using System.ComponentModel;
using RedOnion.Attributes;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	[Description("Type of part.")]
	public enum PartType
	{
		[Description("Uknown type.")]
		Unknown,
		[Description("Engine.")]
		Engine,
		[Description("Sensor.")]
		Sensor,
		[Description("Docking port.")]
		DockingPort,
		[Description("Decoupler (one-way).")]
		Decoupler,
		[Description("Separator (both-ways).")]
		Separator,
		[Description("Engine plate (EP-nn).")]
		EnginePlate,
		[Description("Launch clamp.")]
		LaunchClamp
	}

	[DisplayName("Part"), Description("Part of the ship (vehicle/vessel).")]
	[DocBuild(typeof(Engine), typeof(Sensor), typeof(DecouplerBase))]
	public class PartBase
	{
		[Unsafe, Description("Native `Part` - KSP API.")]
		public Part native { get; }
		[Description("Type of the part.")]
		public PartType type { get; }

		[Description("Ship (vehicle/vessel) this part belongs to.")]
		public Ship ship { get; }
		[Description("Parent part (this part is attached to).")]
		public PartBase parent { get; }
		[Description("Parts attached to this part.")]
		public PartChildren children => _children ?? (_children = new PartChildren(this));
		PartChildren _children;

		[Description("Decoupler that will decouple this part when staged.")]
		public DecouplerBase decoupler { get; internal set; }
		[Description("Stage number as provided by KSP API. (`native.inverseStage` - activating stage for engines, decouplers etc.)")]
		public int stage => native.inverseStage;
		[Description("Stage number where this part will be decoupled or -1. (`decoupler?.stage ?? -1`)")]
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

		[Description("Method to test the type of the part (e.g. `.istype(\"LaunchClamp\")`)."
			+ " Note that ROS has `is` operator and Lua has `isa` function that can be used togehter with"
			+ " `types.engine` etc.")]
		public virtual bool istype(string name) => false;

		[Description("Position of the part (relative to CoM of active ship/vessel).")]
		public Vector position => new Vector(native.partTransform.position - FlightGlobals.ActiveVessel.CoMD);

		[Description("Explode the part.")]
		public void explode() => native.explode();

		public override string ToString()
			=> native.ToString();

		protected internal PartBase(PartType type, Ship ship, Part native, PartBase parent, DecouplerBase decoupler)
		{
			this.type = type;
			this.ship = ship;
			this.native = native;
			this.parent = parent;
			this.decoupler = decoupler;
		}
	}
}
