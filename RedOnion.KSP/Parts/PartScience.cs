using Experience.Effects;
using RedOnion.Attributes;
using RedOnion.ROS;
using RedOnion.KSP.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_science_experiment.html)")]
		public ScienceExperiment experiment => native.experiment;
		[Description("Experiment ID.")]
		public string experimentId => experiment.id;
		[Description("Experiment title.")]
		public string experimentTitle => experiment.experimentTitle;

		Science.Subject _subject;
		[WorkInProgress, Description("Science subject for the experiment and current situation.")]
		public Science.Subject subject
		{
			get
			{
				if (_subject == null)
					_subject = new Science.Subject(experiment);
				else _subject.Update();
				return _subject;
			}
		}

		// note: may need to create linked list for multiple modules
		protected PartScience(PartBase part, ModuleScienceExperiment module)
		{
			this.part = part;
			native = module;
		}
		internal static PartScience Create(PartBase part, ModuleScienceExperiment module)
			=> module.ClassName == "DMModuleScienceAnimate"
			? new DMagic(part, module, DMagic.API1(module))
			: module.ClassName == "DMModuleScienceAnimateGeneric"
			? new DMagic(part, module, DMagic.API2(module))
			: new PartScience(part, module);

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
			public class API
			{
				public readonly Type type;
				public readonly Func<ModuleScienceExperiment, bool> canConduct;
				public readonly Action<ModuleScienceExperiment, bool> gatherScienceData;

				public API(Type type)
				{
					this.type = type;
					const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
					var module = Expression.Parameter(
						typeof(ModuleScienceExperiment), "module");
					try
					{
						canConduct = Expression.Lambda<Func<ModuleScienceExperiment, bool>>(
							Expression.Call(Expression.Convert(module, type),
							type.GetMethod("canConduct", flags, null, new Type[0], null)),
							module).Compile();
					}
					catch (Exception ex)
					{
						Value.Log($"Could not reflect DMagic's `canConduct` in {type.Name}: {ex}");
					}

					var silent = Expression.Parameter(
						typeof(bool), "silent");
					try
					{
						gatherScienceData = Expression.Lambda<Action<ModuleScienceExperiment, bool>>(
							Expression.Call(Expression.Convert(module, type),
							type.GetMethod("gatherScienceData", flags, null, new Type[] { typeof(bool) }, null),
							silent), module, silent).Compile();
					}
					catch (Exception ex)
					{
						Value.Log($"Could not reflect DMagic's `gatherScienceData` in {type.Name}: {ex}");
					}
				}
			}
			protected static API api1, api2;
			public static API API1(ModuleScienceExperiment module)
				=> api1 ?? (api1 = new API(module.GetType()));
			public static API API2(ModuleScienceExperiment module)
				=> api2 ?? (api2 = new API(module.GetType()));

			protected readonly Func<ModuleScienceExperiment, bool> _canConduct;
			protected readonly Action<ModuleScienceExperiment, bool> _gatherScienceData;

			public DMagic(PartBase part, ModuleScienceExperiment module, API api)
				: base(part, module)
			{
				_canConduct = api.canConduct;
				_gatherScienceData = api.gatherScienceData;
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
