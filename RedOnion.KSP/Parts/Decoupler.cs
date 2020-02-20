using System;
using System.ComponentModel;
using RedOnion.Attributes;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	[Description("Decoupler, separator, engine plate, launch clamp or docking port.")]
	[DocBuild(typeof(Decoupler), typeof(DockingPort), typeof(LaunchClamp))]
	public abstract class LinkPart : PartBase
	{
		protected LinkPart(PartType type, Ship ship, Part part, PartBase parent, LinkPart decoupler)
			: base(type, ship, part, parent, decoupler) { }

		[Description("Accepts `link`. (Case insensitive)")]
		public override bool istype(string name)
			=> name.Equals("link", StringComparison.OrdinalIgnoreCase);

		[Description("Part is staged.")]
		public bool staged => GetStagingEnabled();
		protected abstract bool GetStagingEnabled();
	}

	[Description("Decoupler, separator or engine plate.")]
	public class Decoupler : LinkPart
	{
		protected internal ModuleDecouplerBase module;
		protected internal Decoupler(Ship ship, Part part, PartBase parent, LinkPart decoupler, ModuleDecouplerBase module)
			: base(module.isEnginePlate ? PartType.EnginePlate :
				  module.isOmniDecoupler ? PartType.Separator :
				  PartType.Decoupler, ship, part, parent, decoupler)
			=> this.module = module;

		[Description("Accepts `link` and `decoupler`/`separator`/`enginePlate` according to `type`. (Case insensitive)")]
		public override bool istype(string name)
			=> name.Equals(type.ToString(), StringComparison.OrdinalIgnoreCase)
			|| base.istype(name);

		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_module_decoupler_base.html)")]
		public ModuleDecouplerBase Module => module;
		protected override bool GetStagingEnabled() => module.StagingEnabled();

		[Description("Decouple the decoupler / separator / engine plate.")]
		public void decouple()
			=> module.Decouple();
	}
}
