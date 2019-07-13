using RedOnion.KSP.API;
using RedOnion.ROS;
using RedOnion.ROS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.Parts
{
	public class Engine : PartBase
	{
		public MultiModeEngine MultiModule { get; protected set; }
		public ModuleEngines FirstModule { get; protected set; }
		public ModuleEngines SecondModule { get; protected set; }
		public ModuleEngines ActiveModule
			=> SecondModule == null || MultiModule.runningPrimary ? FirstModule : SecondModule;
		public ModuleGimbal GimbalModule { get; private set; }

		public bool MultiMode { get { return SecondModule != null; } }
		public bool HasGimbal { get { return GimbalModule != null; } }

		public override bool IsType(string name)
			=> name.Equals("engine", StringComparison.OrdinalIgnoreCase);

		protected internal Engine(Ship ship, Part part, PartBase parent, Decoupler decoupler)
			: base(ship, part, parent, decoupler)
		{
			foreach (var module in part.Modules)
			{
				if (module is MultiModeEngine multi)
				{
					if (MultiModule != null)
						MultiModule = multi;
				}
				else if (module is ModuleEngines engines)
				{
					if (FirstModule == null)
						FirstModule = engines;
					else if (SecondModule == null)
						SecondModule = engines;
				}
				else if (module is ModuleGimbal gimbal)
				{
					if (GimbalModule == null)
						GimbalModule = gimbal;
				}
			}
			if (MultiModule == null)
				SecondModule = null;
			else if (SecondModule != null && MultiModule.primaryEngineID == SecondModule.engineID)
			{
				var second = FirstModule;
				FirstModule = SecondModule;
				SecondModule = second;
			}
		}

		public bool Operational => ActiveModule.isOperational;
		public bool Ignited => ActiveModule.EngineIgnited;
		public bool Flameout => ActiveModule.flameout;
		public bool Staged => ActiveModule.staged;
		public void Activate() => ActiveModule.Activate();
		public void Shutdown() => ActiveModule.Shutdown();

		public float Isp => ActiveModule.realIsp;
		public float VIsp => ActiveModule.atmosphereCurve.Evaluate(0);
		public float GIsp => ActiveModule.atmosphereCurve.Evaluate(1);
		public float SLIsp => ActiveModule.atmosphereCurve.Evaluate(1);
		public float VacuumIsp => ActiveModule.atmosphereCurve.Evaluate(0);
		public float GroundIsp => ActiveModule.atmosphereCurve.Evaluate(1);
		public float SeeLevelIsp => ActiveModule.atmosphereCurve.Evaluate(1);

		public float Thrust => ActiveModule.finalThrust;
		public float ThrustPercentage
		{
			get => ActiveModule.thrustPercentage;
			set => ActiveModule.thrustPercentage = RosMath.Clamp(value, 0f, 100f);
		}

		public double RatioSum => ActiveModule.ratioSum;
		public double MixtureDensity => ActiveModule.mixtureDensity;
		public double MixtureDensityRecip => ActiveModule.mixtureDensityRecip;
		[ReadOnlyContent, ReadOnlyItems]
		public List<Propellant> Propellants => ActiveModule.propellants;
	}
}
