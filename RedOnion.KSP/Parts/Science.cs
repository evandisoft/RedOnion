using RedOnion.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedOnion.KSP.Parts
{
	[WorkInProgress, Description("Available science through one part.")]
	public class Science
	{
		[Description("The part this science belongs to.")]
		public PartBase part { get; }
		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_module_science_experiment.html)")]
		public ModuleScienceExperiment native { get; }

		protected internal Science(PartBase part, ModuleScienceExperiment module)
		{
			this.part = part;
			native = module;
		}
		public override string ToString()
			=> part.name + ".science";

		[Description("Science module inoperable (cannot deploy).")]
		public bool inoperable => native.Inoperable;
		[Description("Science module is deployed (that usually means it contains data and cannot be deployed again).")]
		public bool deployed => native.Deployed;

		[Description("Deploy the experiment (may take some time). Shows the dialog by default."
			+ " Note that suppressing the dialog is rather experimental and may not work for some parts.")]
		public bool deploy(bool dialog = true)
		{
			if (inoperable)
				return false;
			if (dialog)
				native.DeployExperiment();
			else if (native.useStaging)
				native.OnActive();
			else
			{
				native.useStaging = true;
				native.OnActive();
				native.useStaging = false;
			}
			return true;
		}
	}
}
