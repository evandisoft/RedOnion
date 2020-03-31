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
		ready,
		[Description("Module is full of data.")]
		full,
		[Description("Module inoperable.")]
		inoperable,
		[Description("Module is shielded and cannot perform experiments now.")]
		shielded,
		[Description("Ship is currently not controllable (and module requires it).")]
		noControl,
		[Description("No crew in ship or the part (as required by the module).")]
		noCrew,
		[Description("No scientist in ship or the part (as required by the module).")]
		noScientist,
		[Description("Module needs sime time before operation.")]
		cooldown,
		[Description("Unknown state, probably cannot perform experiments now.")]
		uknown
	}

	[WorkInProgress, Description("Available science through one part.")]
	public class PartScience
	{
		[Description("The part this science belongs to.")]
		public PartBase part { get; }
		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_module_science_experiment.html)")]
		public ModuleScienceExperiment native { get; }
		public static implicit operator ModuleScienceExperiment(PartScience sci) => sci?.native;

		// note: may need to create linked list for multiple modules
		protected PartScience(PartBase part, ModuleScienceExperiment module)
		{
			this.part = part;
			native = module;
		}
		internal static PartScience Create(PartBase part, ModuleScienceExperiment module)
			=> module.ClassName == "DMModuleScienceAnimate"
			? new DMagic(part, module) : new PartScience(part, module);

		public override string ToString()
			=> Value.Format($"{part.name}/{native.experimentID}");

		[WorkInProgress, Description("Ready to perform experiment.")]
		public bool ready => state == ScienceState.ready;
		[WorkInProgress, Description("State of science module.")]
		public virtual ScienceState state
		{
			get
			{
				if (native.Deployed)
					return ScienceState.full;
				if (native.Inoperable)
					return ScienceState.inoperable;
				if (!native.availableShielded && native.part.ShieldedFromAirstream)
					return ScienceState.shielded;
				var usage = (ExperimentUsageReqs)native.usageReqMaskInternal;
				string msg = null;
				if (!ScienceUtil.RequiredUsageInternalAvailable(
					native.vessel, native.part, usage, native.experiment, ref msg))
				{
					if (usage == ExperimentUsageReqs.Never)
						return ScienceState.uknown;
					if ((usage & ExperimentUsageReqs.VesselControl) != 0
						&& !native.vessel.IsControllable)
						return ScienceState.noControl;
					if ((usage & ExperimentUsageReqs.CrewInVessel) != 0
						&& !native.vessel.GetVesselCrew().Any(x =>
						x.type == ProtoCrewMember.KerbalType.Crew))
						return ScienceState.noCrew;
					if ((usage & ExperimentUsageReqs.CrewInPart) != 0
						&& !native.part.protoModuleCrew.Any(x =>
						x.type == ProtoCrewMember.KerbalType.Crew))
						return ScienceState.noCrew;
					if ((usage & ExperimentUsageReqs.ScientistCrew) != 0
						&& !((usage & ExperimentUsageReqs.CrewInPart) != 0
						? native.part.protoModuleCrew
						: native.vessel.GetVesselCrew()).Any(x =>
						x.HasEffect<SpecialExperimentSkill>()))
						return ScienceState.noScientist;
					return ScienceState.uknown;
				}
				if (native.useCooldown && native.cooldownTimer > 0.0)
					return ScienceState.cooldown;
				return ScienceState.ready;
			}
		}

		[Description("Perform the experiment (may take some time). Shows the dialog by default."
			+ " Note that suppressing the dialog is rather experimental and may not work for some parts.")]
		public virtual bool perform(bool dialog = true)
		{
			if (state != ScienceState.ready)
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

		protected class DMagic : PartScience
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
						return ScienceState.full;
					if (native.Inoperable)
						return ScienceState.inoperable;
					if (!native.availableShielded && native.part.ShieldedFromAirstream)
						return ScienceState.shielded;
					return _canConduct(native) ? ScienceState.ready : ScienceState.uknown;
				}
			}

			public override bool perform(bool dialog = true)
			{
				if (state != ScienceState.ready)
					return false;
				_gatherScienceData(native, !dialog);
				return true;
			}
		}
	}
}
