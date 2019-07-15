using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	[DisplayName("Part"), Description("Part of the ship (wehicle/vessel).")]
	public class PartBase
	{
		[Description("Ship (wehicle/vessel) this part belongs to.")]
		public Ship Ship { get; }
		[Description("Native `Part` - KSP API.")]
		public Part Native { get; }
		[Description("Parent part (this part is attached to).")]
		public PartBase Parent { get; }
		[Description("Decoupler that will decouple this part when staged.")]
		public Decoupler Decoupler { get; }
		[Description("Stage number as provided by KSP API. (`Native.inverseStage`)")]
		public int Stage => Native.inverseStage;
		[Description("Stage number where this part will be decoupled or -1. (`Decoupler?.Stage ?? -1`)")]
		public int DecoupledIn => Decoupler?.Stage ?? -1;

		public PartStates State => Native.State;

		// TODO: make this generic feature of ROS (typeof operator)
		[Description("Method to test the type of the part (e.g. `.IsType(\"LaunchClamp\")`)")]
		public virtual bool IsType(string name) => false;

		protected internal PartBase(Ship ship, Part native, PartBase parent, Decoupler decoupler)
		{
			Ship = ship;
			Native = native;
			Parent = parent;
			Decoupler = decoupler;
		}
	}
}
