using RedOnion.KSP.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.Parts
{
	public class LaunchClamp : Decoupler
	{
		protected internal LaunchClamp(Ship ship, Part part, PartBase parent, Decoupler decoupler)
			: base(ship, part, parent, decoupler) { }

		public override bool istype(string name)
			=> name.Equals("clamp", StringComparison.OrdinalIgnoreCase)
			|| name.Equals("launchClamp", StringComparison.OrdinalIgnoreCase)
			|| base.istype(name);
	}
}
