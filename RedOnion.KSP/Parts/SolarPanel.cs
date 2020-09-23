using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RedOnion.Attributes;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	[Description("List/set of solar panels.")]
	public class SolarPanelList : ReadOnlyList<SolarPanel>
	{
		internal SolarPanelList(Action refresh) : base(refresh) { }
		[Description("Extend all solar panels.")]
		public bool extend()
		{
			bool did = false;
			foreach (var p in this)
			{
				if (p.extend())
					did = true;
			}
			return did;
		}
		[Description("Retract all solar panels.")]
		public bool retract()
		{
			bool did = false;
			foreach (var p in this)
			{
				if (p.retract())
					did = true;
			}
			return did;
		}
	}

	[Description("Solar panel.")]
	public class SolarPanel : PartBase
	{
		protected readonly ModuleDeployableSolarPanel module;
		protected internal SolarPanel(Ship ship, Part part, PartBase parent, LinkPart decoupler, ModuleDeployableSolarPanel module)
			: base(PartType.Generator, ship, part, parent, decoupler)
			=> this.module = module;

		[Description("Indicates retractable (non-broken) solar panel.")]
		public bool retractable => module.retractable && module.useAnimation && deployState != ModuleDeployablePart.DeployState.BROKEN;
		[Description("Deploy state.")]
		public ModuleDeployablePart.DeployState deployState => module.deployState;
		[Description("Panel is fully extended.")]
		public bool extended => deployState == ModuleDeployablePart.DeployState.EXTENDED;
		[Description("Panel is fully retracted (and not broken).")]
		public bool retracted => deployState == ModuleDeployablePart.DeployState.RETRACTED;
		[Description("Panel is broken.")]
		public bool broken => deployState == ModuleDeployablePart.DeployState.BROKEN;
		[Description("Panel is extending.")]
		public bool extending => deployState == ModuleDeployablePart.DeployState.EXTENDING;
		[Description("Panel is retracting.")]
		public bool retracting => deployState == ModuleDeployablePart.DeployState.RETRACTING;

		[Description("Panel type.")]
		public ModuleDeployableSolarPanel.PanelType panelType => module.panelType;

		[Description("Extend the panel.")]
		public bool extend()
		{
			if (!module.useAnimation || broken)
				return false;
			if (!module.Actions["ExtendAction"].active)
				return false;
			module.Extend();
			return true;
		}
		[Description("Retract the panel.")]
		public bool retract()
		{
			// WARNING: OX-STAT reports module.retractable && extended, but calling its Retract() results in endless NREs!
			// ........ therefore both module.useAnimation and action checks were added
			if (!retractable)
				return false;
			if (!module.Actions["RetractAction"].active)
				return false;
			module.Retract();
			return true;
		}
	}
}
