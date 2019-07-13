using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	[DisplayName("Part")]
	public class PartBase
	{
		public Ship Ship { get; }
		public Part Native { get; }
		public PartBase Parent { get; }
		public Decoupler Decoupler { get; }
		public int Stage => Native.inverseStage;
		public int DecoupledIn => Decoupler?.Stage ?? -1;
		public PartStates State => Native.State;

		// TODO: make this generic feature of ROS (typeof operator)
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
