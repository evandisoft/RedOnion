using RedOnion.KSP.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.Parts
{
	[Description("Launch Clamp")]
	public class LaunchClamp : Decoupler
	{
		protected internal LaunchClamp(Ship ship, Part part, PartBase parent, Decoupler decoupler)
			: base(ship, part, parent, decoupler) { }

		[Description("Accepts `clamp`, `launchClamp`, `decoupler` and `separator`. (Case insensitive)")]
		public override bool istype(string name)
			=> name.Equals("clamp", StringComparison.OrdinalIgnoreCase)
			|| name.Equals("launchClamp", StringComparison.OrdinalIgnoreCase)
			|| base.istype(name);
	}
}
