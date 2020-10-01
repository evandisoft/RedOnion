using System;
using System.ComponentModel;
using RedOnion.Attributes;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	[WorkInProgress, Description("Type of part.")]
	public enum PartType : byte
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
		LaunchClamp,
		[Description("Solar panel.")]
		SolarPanel,
		[Description("Resource generator.")]
		Generator,
	}

	[DisplayName("Part"), Description("Part of the ship (vehicle/vessel).")]
	[DocBuild(typeof(Engine), typeof(Sensor), typeof(LinkPart), typeof(SolarPanel), typeof(Generator))]
	public class PartBase
	{
		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_part.html)")]
		public Part native { get; }
		public static implicit operator Part(PartBase it) => it?.native;
		[WorkInProgress, Description("Type of the part.")]
		public PartType type { get; }

		[Description("Science available through this part, `null` if none.")]
		public PartScience science => scienceQueried ? _science : _science = FindScience();
		private bool scienceQueried;
		private PartScience _science;
		private PartScience FindScience()
		{
			scienceQueried = true;
			var mod = native.FindModuleImplementing<ModuleScienceExperiment>();
			return mod == null ? null : PartScience.Create(this, mod);
		}

		[Description("Ship (vehicle/vessel) this part belongs to.")]
		public Ship ship { get; }
		[Description("Parent part (this part is attached to).")]
		public PartBase parent { get; }
		[Description("Parts attached to this part.")]
		public PartChildren children => _children ?? (_children = new PartChildren(this));
		PartChildren _children;
		[Description("Custom values and tags attached to this part.")]
		public PartValues values => _values ?? (_values = new PartValues(this));
		[Description("Custom values and tags attached to this part. (alias to `values`)")]
		public PartValues tags => _values ?? (_values = new PartValues(this));
		PartValues _values;

		[Description("Decoupler that will decouple this part when staged.")]
		public LinkPart decoupler { get; internal set; }
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
		public override string ToString() => native.ToString();
		[Description("Title of the part (assigned by KSP).")]
		public string title => native.partInfo.title;

		[Description("Method to test the type of the part (e.g. `.istype(\"LaunchClamp\")`)."
			+ " Note that ROS has `is` operator and Lua has `isa` function that can be used togehter with"
			+ " `types.engine` etc. Another classification is through `type` property.")]
		public virtual bool istype(string name) => false;

		[Description("Position of the part (relative to CoM of active ship/vessel).")]
		public Vector position => new Vector(native.partTransform.position - FlightGlobals.ActiveVessel.CoMD);

		[Description("Mass of the part including resources.")]
		public double mass => native.mass + native.GetResourceMass();
		[Description("Mass of the resources contained.")]
		public double resourceMass => native.GetResourceMass();

		[Description("Explode the part.")]
		public void explode() => native.explode();

		protected internal PartBase(PartType type, Ship ship, Part native, PartBase parent, LinkPart decoupler)
		{
			this.type = type;
			this.ship = ship;
			this.native = native;
			this.parent = parent;
			this.decoupler = decoupler;
		}

		[Unsafe, Description("Modules of this part.")]
		public PartModules modules => _modules ?? (_modules = new PartModules(this));
		PartModules _modules;
	}
}
