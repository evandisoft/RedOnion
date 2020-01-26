using System;
using System.ComponentModel;
using RedOnion.Attributes;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	[Description("Decoupler, separator, engine plate, launch clamp or docking port.")]
	[DocBuild(typeof(Decoupler), typeof(DockingPort), typeof(LaunchClamp))]
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

	[Description("Decoupler, separator or engine plate.")]
	public class Decoupler : DecouplerBase
	{
		protected internal ModuleDecouplerBase module;
		protected internal Decoupler(Ship ship, Part part, PartBase parent, DecouplerBase decoupler, ModuleDecouplerBase module)
			: base(module.isEnginePlate ? PartType.EnginePlate :
				  module.isOmniDecoupler ? PartType.Separator :
				  PartType.Decoupler, ship, part, parent, decoupler)
			=> this.module = module;

		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_module_decoupler_base.html)")]
		public ModuleDecouplerBase Module => module;
		protected override bool GetStagingEnabled() => module.StagingEnabled();

		[Description("Decouple the decoupler / separator / engine plate.")]
		public void decouple()
			=> module.Decouple();
	}
}
