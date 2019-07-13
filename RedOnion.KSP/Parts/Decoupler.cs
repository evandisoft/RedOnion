using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	public class Decoupler : PartBase
	{
		protected internal Decoupler(Ship ship, Part part, PartBase parent, Decoupler decoupler)
			: base(ship, part, parent, decoupler) { }

		public override bool IsType(string name)
			=> name.Equals("decoupler", StringComparison.OrdinalIgnoreCase)
			|| name.Equals("separator", StringComparison.OrdinalIgnoreCase);
	}
}
