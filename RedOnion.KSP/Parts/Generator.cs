using System;
using System.ComponentModel;
using RedOnion.Attributes;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	[Description("Resource generator.")]
	public class Generator : PartBase
	{
		protected readonly ModuleGenerator module;
		protected internal Generator(Ship ship, Part part, PartBase parent, LinkPart decoupler, ModuleGenerator module)
			: base(PartType.Generator, ship, part, parent, decoupler)
			=> this.module = module;

		[Description("Activate/start the generator.")]
		public void activate() => module.Activate();
		[Description("Stop/shutdown the generator.")]
		public void shutdown() => module.Shutdown();
	}
}
