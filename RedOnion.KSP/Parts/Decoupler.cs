using System;
using System.ComponentModel;
using RedOnion.Attributes;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	[Description("Decoupler, separator, launch clamp or docking port.")]
	public abstract class DecouplerBase : PartBase
	{
		protected DecouplerBase(PartType type, Ship ship, Part part, PartBase parent, DecouplerBase decoupler)
			: base(type, ship, part, parent, decoupler) { }

		[Description("Accepts `decoupler` and `separator`. (Case insensitive)")]
		public override bool istype(string name)
			=> name.Equals("decoupler", StringComparison.OrdinalIgnoreCase)
			|| name.Equals("separator", StringComparison.OrdinalIgnoreCase);

		public bool staged => GetStagingEnabled();
		protected abstract bool GetStagingEnabled();
	}

	[Description("Decoupler or separator.")]
	public class Decoupler : DecouplerBase
	{
		protected internal ModuleDecouplerBase module;
		protected internal Decoupler(Ship ship, Part part, PartBase parent, DecouplerBase decoupler, ModuleDecouplerBase module)
			: base(module.isOmniDecoupler ? PartType.Separator : PartType.Decoupler, ship, part, parent, decoupler)
			=> this.module = module;

		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_module_decoupler_base.html)")]
		public ModuleDecouplerBase Module => module;
		protected override bool GetStagingEnabled() => module.StagingEnabled();

		[Description("Decouple the decoupler/separator.")]
		public void decouple()
			=> module.Decouple();
	}
}
