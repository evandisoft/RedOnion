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
		public Part Part { get; }
		public PartBase Parent { get; }
		public Decoupler Decoupler { get; }
		
		protected internal PartBase(Ship ship, Part part, PartBase parent, Decoupler decoupler)
		{
			Ship = ship;
			Part = part;
			Parent = parent;
			Decoupler = decoupler;
		}
	}
}
