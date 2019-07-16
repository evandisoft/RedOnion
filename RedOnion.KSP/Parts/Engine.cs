using System;
using System.ComponentModel;
using RedOnion.KSP.API;
using RedOnion.ROS.Utilities;

namespace RedOnion.KSP.Parts
{
	public class EngineSet : PartSet<Engine>
	{
		protected internal EngineSet() { }
		protected internal EngineSet(Action refresh) : base(refresh) { }

		public PropellantList Propellants => propellants ?? (propellants = new PropellantList(this));
		protected PropellantList propellants;

		protected internal override void SetDirty()
		{
			base.SetDirty();
			if (propellants != null)
				propellants.SetDirty();
		}
	}

	[Description("Engine of a ship (wehicle/vessel).")]
	public class Engine : PartBase
	{
		[Description("KSP API. Module of multi-mode engine, if present (null otherwise).")]
		public MultiModeEngine MultiModule { get; protected set; }
		[Description("KSP API. Module of first engine.")]
		public ModuleEngines FirstModule { get; protected set; }
		[Description("KSP API. Module of second engine, if present (null otherwise).")]
		public ModuleEngines SecondModule { get; protected set; }
		[Description("Running primary engine (or the only one).")]
		public bool FirstIsActive => SecondModule == null || MultiModule.runningPrimary;
		[Description("Running secondary engine.")]
		public bool SecondIsActive => !FirstIsActive;
		[Description("KSP API. Active engine module.")]
		public ModuleEngines ActiveModule => FirstIsActive ? FirstModule : SecondModule;
		[Description("KSP API. Gimbal module, if present (null otherwise).")]
		public ModuleGimbal GimbalModule { get; private set; }
		[Description("Is multi-mode engine (or not).")]
		public bool MultiMode { get { return SecondModule != null; } }
		[Description("Has gimbal module.")]
		public bool HasGimbal { get { return GimbalModule != null; } }

		[Description("Accepts `sensor`. (Case insensitive)")]
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

		[DisplayName("Whether engine is operational (ignited and not flameout).")]
		public bool Operational => ActiveModule.isOperational;
		[DisplayName("Wheter engine is ignited.")]
		public bool Ignited => ActiveModule.EngineIgnited;
		[DisplayName("Wheter engine flamed out.")]
		public bool Flameout => ActiveModule.flameout;
		[DisplayName("Wheter engine is staged (activated by staging).")]
		public bool Staged => ActiveModule.staged;
		[DisplayName("Activate the engine.")]
		public void Activate() => ActiveModule.Activate();
		[DisplayName("Shutdown / deactivate the engine.")]
		public void Shutdown() => ActiveModule.Shutdown();

		[DisplayName("Current ISP. (Specific impulse)")]
		public float Isp => ActiveModule.realIsp;
		[DisplayName("Vacuum ISP.")]
		public float VIsp => ActiveModule.atmosphereCurve.Evaluate(0f);
		[DisplayName("Sea-level ISP.")]
		public float GIsp => ActiveModule.atmosphereCurve.Evaluate(1f);
		[DisplayName("Sea-level ISP.")]
		public float SLIsp => ActiveModule.atmosphereCurve.Evaluate(1f);
		[DisplayName("Vacuum ISP.")]
		public float VacuumIsp => ActiveModule.atmosphereCurve.Evaluate(0f);
		[DisplayName("Sea-level ISP.")]
		public float GroundIsp => ActiveModule.atmosphereCurve.Evaluate(1f);
		[DisplayName("Sea-level ISP.")]
		public float SeeLevelIsp => ActiveModule.atmosphereCurve.Evaluate(1f);

		[Description("Avalable thrust at current pressure, ignoring the limiter"
			+ " (`ThrustPercentage` and current throttle).")]
		public float Thrust => ActiveModule.finalThrust;
		[Description("Thrust limiter in percents.")]
		public float ThrustPercentage
		{
			get => ActiveModule.thrustPercentage;
			set => ActiveModule.thrustPercentage = RosMath.Clamp(value, 0f, 100f);
		}
		[Description("Get thrust at atmospheric pressure"
			+ " (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure)"
			+ " and throttle (default 1 = full throttle). Ignores `ThrustPercentage`.")]
		public float GetThrust(float atm = float.NaN, float throttle = 1f)
		{
			var module = ActiveModule;
			if (float.IsNaN(atm))
				atm = (float)module.part.staticPressureAtm;
			return module.GetEngineThrust(module.atmosphereCurve.Evaluate(
				RosMath.Clamp((float)atm, 0f, 10f)),
				RosMath.Clamp(throttle, 0f, 1f));
		}

		public double RatioSum => ActiveModule.ratioSum;
		public double MixtureDensity => ActiveModule.mixtureDensity;
		public double MixtureDensityRecip => ActiveModule.mixtureDensityRecip;

		ReadOnlyList<Propellant> propellants, propellants2;
		public ReadOnlyList<Propellant> Propellants => FirstIsActive ? Propellants1 : Propellants2;
		public ReadOnlyList<Propellant> Propellants1 => propellants ?? (propellants = new ReadOnlyList<Propellant>());
		public ReadOnlyList<Propellant> Propellants2 => propellants2 ?? (MultiMode ? propellants2 = new ReadOnlyList<Propellant>() : null);
	}
}
