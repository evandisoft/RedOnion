using RedOnion.Attributes;
using RedOnion.KSP.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.Parts
{
	[Description("Launch Clamp")]
	public class LaunchClamp : DecouplerBase
	{
		protected internal global::LaunchClamp module;
		protected internal LaunchClamp(Ship ship, Part part, PartBase parent, DecouplerBase decoupler, global::LaunchClamp module)
			: base(PartType.LaunchClamp, ship, part, parent, decoupler) => this.module = module;

		[Description("Accepts `clamp`, `launchClamp`, `decoupler` and `separator`. (Case insensitive)")]
		public override bool istype(string name)
			=> name.Equals("clamp", StringComparison.OrdinalIgnoreCase)
			|| name.Equals("launchClamp", StringComparison.OrdinalIgnoreCase)
			|| base.istype(name);

		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_launch_clamp.html)")]
		public global::LaunchClamp Module => module;
		protected override bool GetStagingEnabled() => module.StagingEnabled();
	}
}
