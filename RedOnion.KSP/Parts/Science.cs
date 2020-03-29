using Experience.Effects;
using RedOnion.Attributes;
using RedOnion.ROS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RedOnion.KSP.Parts
{
	[WorkInProgress, Description("State of science module.")]
	public enum ScienceState
	{
		[Description("Can perform experiment now.")]
		Ready,
		[Description("Module is full of data.")]
		Full,
		[Description("Module inoperable.")]
		Inoperable,
		[Description("Module is shielded and cannot perform experiments now.")]
		Shielded,
		[Description("Ship is currently not controllable (and module requires it).")]
		NoControl,
		[Description("No crew in ship or the part (as required by the module).")]
		NoCrew,
		[Description("No scientist in ship or the part (as required by the module).")]
		NoScientist,
		[Description("Module needs sime time before operation.")]
		Cooldown,
		[Description("Unknown state, probably cannot perform experiments now.")]
		Uknown
	}

	[WorkInProgress, Description("Available science through one part.")]
	public class Science
	{
		[Description("The part this science belongs to.")]
		public PartBase part { get; }
		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_module_science_experiment.html)")]
		public ModuleScienceExperiment native { get; }

		// note: may need to create linked list for multiple modules
		protected Science(PartBase part, ModuleScienceExperiment module)
		{
			this.part = part;
			native = module;
		}
		internal static Science Create(PartBase part, ModuleScienceExperiment module)
			=> module.ClassName == "DMModuleScienceAnimate"
			? new DMagic(part, module) : new Science(part, module);

		public override string ToString()
		{
			var title = native.experiment.experimentTitle;
			if (string.IsNullOrEmpty(title))
				title = part.title;
			return Value.Format($"{part.name}/{title}");
		}

		[WorkInProgress, Description("Ready to perform experiment.")]
		public bool ready => state == ScienceState.Ready;
		[WorkInProgress, Description("State of science module.")]
		public virtual ScienceState state
		{
			get
			{
				if (native.Deployed)
					return ScienceState.Full;
				if (native.Inoperable)
					return ScienceState.Inoperable;
				if (!native.availableShielded && native.part.ShieldedFromAirstream)
					return ScienceState.Shielded;
				var usage = (ExperimentUsageReqs)native.usageReqMaskInternal;
				string msg = null;
				if (!ScienceUtil.RequiredUsageInternalAvailable(
					native.vessel, native.part, usage, native.experiment, ref msg))
				{
					if (usage == ExperimentUsageReqs.Never)
						return ScienceState.Uknown;
					if ((usage & ExperimentUsageReqs.VesselControl) != 0
						&& !native.vessel.IsControllable)
						return ScienceState.NoControl;
					if ((usage & ExperimentUsageReqs.CrewInVessel) != 0
						&& !native.vessel.GetVesselCrew().Any(x =>
						x.type == ProtoCrewMember.KerbalType.Crew))
						return ScienceState.NoCrew;
					if ((usage & ExperimentUsageReqs.CrewInPart) != 0
						&& !native.part.protoModuleCrew.Any(x =>
						x.type == ProtoCrewMember.KerbalType.Crew))
						return ScienceState.NoCrew;
					if ((usage & ExperimentUsageReqs.ScientistCrew) != 0
						&& !((usage & ExperimentUsageReqs.CrewInPart) != 0
						? native.part.protoModuleCrew
						: native.vessel.GetVesselCrew()).Any(x =>
						x.HasEffect<SpecialExperimentSkill>()))
						return ScienceState.NoScientist;
					return ScienceState.Uknown;
				}
				if (native.useCooldown && native.cooldownTimer > 0.0)
					return ScienceState.Cooldown;
				return ScienceState.Ready;
			}
		}

		[Description("Perform the experiment (may take some time). Shows the dialog by default."
			+ " Note that suppressing the dialog is rather experimental and may not work for some parts.")]
		public virtual bool perform(bool dialog = true)
		{
			if (state != ScienceState.Ready)
				return false;
			if (dialog)
				native.DeployExperiment();
			else if (native.useStaging && native.stagingEnabled)
				native.OnActive();
			else
			{
				var useStaging = native.useStaging;
				var stagingEnabled = native.stagingEnabled;
				native.useStaging = true;
				native.stagingEnabled = true;
				native.OnActive();
				native.useStaging = useStaging;
				native.stagingEnabled = stagingEnabled;
			}
			return true;
		}

		protected class DMagic : Science
		{
			protected static Type _moduleType;
			protected static Func<ModuleScienceExperiment, bool> _canConduct;
			protected static Action<ModuleScienceExperiment, bool> _gatherScienceData;

			public DMagic(PartBase part, ModuleScienceExperiment module)
				: base(part, module)
			{
				if (_moduleType == null)
				{
					_moduleType = module.GetType();

					var nativeParameter = Expression.Parameter(
						typeof(ModuleScienceExperiment), "native");
					try
					{
						_canConduct = Expression.Lambda<Func<ModuleScienceExperiment, bool>>(
							Expression.Call(Expression.Convert(nativeParameter, _moduleType),
							_moduleType.GetMethod("canConduct")),
							nativeParameter).Compile();

					}
					catch (Exception ex)
					{
						Value.Log("Could not reflect DMagic's `canConduct`: " + ex.ToString());
						return;
					}

					var silentParameter = Expression.Parameter(
						typeof(bool), "silent");
					try
					{
						_gatherScienceData = Expression.Lambda<Action<ModuleScienceExperiment, bool>>(
							Expression.Call(Expression.Convert(nativeParameter, _moduleType),
							_moduleType.GetMethod("gatherScienceData"), silentParameter),
							nativeParameter, silentParameter).Compile();
					}
					catch (Exception ex)
					{
						Value.Log("Could not reflect DMagic's `gatherScienceData`: " + ex.ToString());
					}
				}
			}

			public override ScienceState state
			{
				get
				{
					if (native.Deployed)
						return ScienceState.Full;
					if (native.Inoperable)
						return ScienceState.Inoperable;
					if (!native.availableShielded && native.part.ShieldedFromAirstream)
						return ScienceState.Shielded;
					return _canConduct(native) ? ScienceState.Ready : ScienceState.Uknown;
				}
			}

			public override bool perform(bool dialog = true)
			{
				if (state != ScienceState.Ready)
					return false;
				_gatherScienceData(native, !dialog);
				return true;
			}
		}
	}
}
