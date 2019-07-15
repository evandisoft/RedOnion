using System;
using System.ComponentModel;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	[Description("Decoupler, separator, launch clamp or docking port.")]
	public class Decoupler : PartBase
	{
		protected internal Decoupler(Ship ship, Part part, PartBase parent, Decoupler decoupler)
			: base(ship, part, parent, decoupler) { }

		[Description("Accepts `decoupler` and `separator`. (Case insensitive)")]
		public override bool IsType(string name)
			=> name.Equals("decoupler", StringComparison.OrdinalIgnoreCase)
			|| name.Equals("separator", StringComparison.OrdinalIgnoreCase);
	}
}
