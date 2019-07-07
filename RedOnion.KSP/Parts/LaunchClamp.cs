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
	}
}
