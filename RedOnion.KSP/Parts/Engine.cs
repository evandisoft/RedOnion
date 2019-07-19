using System;
using System.ComponentModel;
using RedOnion.KSP.API;
using RedOnion.ROS;
using RedOnion.ROS.Utilities;

namespace RedOnion.KSP.Parts
{
	[Description("Read-only set of engines.")]
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

		[Description("Whether any engine in the set is operational.")]
		public bool anyOperational
		{
			get
			{
				foreach (var e in this)
				{
					if (e.operational)
						return true;
				}
				return false;
			}
		}
		[Description("Whether all the engines in the set are operational.")]
		public bool allOperational
		{
			get
			{
				foreach (var e in this)
				{
					if (!e.operational)
						return false;
				}
				return true;
			}
		}
		[Description("Wheter any engine in the set flamed out.")]
		public bool anyFlameout
		{
			get
			{
				foreach (var e in this)
				{
					if (e.flameout)
						return true;
				}
				return false;
			}
		}
		[Description("Wheter all engines in the set flamed out.")]
		public bool allFlameout
		{
			get
			{
				foreach (var e in this)
				{
					if (!e.flameout)
						return false;
				}
				return true;
			}
		}
	}

	[Description("Engine of a ship (vehicle/vessel).")]
	public class Engine : PartBase
	{
		[Unsafe, Description("KSP API. Module of multi-mode engine, if present (null otherwise).")]
		public MultiModeEngine multiModule { get; protected set; }
		[Unsafe, Description("KSP API. Module of first engine.")]
		public ModuleEngines firstModule { get; protected set; }
		[Unsafe, Description("KSP API. Module of second engine, if present (null otherwise).")]
		public ModuleEngines secondModule { get; protected set; }
		[Description("Running primary engine (or the only one).")]
		public bool firstIsActive => secondModule == null || multiModule.runningPrimary;
		[Description("Running secondary engine.")]
		public bool secondIsActive => !firstIsActive;
		[Unsafe, Description("KSP API. Active engine module.")]
		public ModuleEngines activeModule => firstIsActive ? firstModule : secondModule;
		[Unsafe, Description("KSP API. Gimbal module, if present (null otherwise).")]
		public ModuleGimbal gimbalModule { get; private set; }
		[Description("Is multi-mode engine (or not).")]
		public bool multiMode { get { return secondModule != null; } }
		[Description("Has gimbal module.")]
		public bool hasGimbal { get { return gimbalModule != null; } }

		[Description("Accepts `sensor`. (Case insensitive)")]
		public override bool istype(string name)
			=> name.Equals("engine", StringComparison.OrdinalIgnoreCase);

		protected internal Engine(Ship ship, Part part, PartBase parent, Decoupler decoupler)
			: base(ship, part, parent, decoupler)
		{
			foreach (var module in part.Modules)
			{
				if (module is MultiModeEngine multi)
				{
					if (multiModule != null)
						multiModule = multi;
				}
				else if (module is ModuleEngines engines)
				{
					if (firstModule == null)
						firstModule = engines;
					else if (secondModule == null)
						secondModule = engines;
				}
				else if (module is ModuleGimbal gimbal)
				{
					if (gimbalModule == null)
						gimbalModule = gimbal;
				}
			}
			if (multiModule == null)
				secondModule = null;
			else if (secondModule != null && multiModule.primaryEngineID == secondModule.engineID)
			{
				var second = firstModule;
				firstModule = secondModule;
				secondModule = second;
			}
		}

		[DisplayName("Whether engine is operational (ignited and not flameout).")]
		public bool operational => activeModule.isOperational;
		[DisplayName("Wheter engine is ignited.")]
		public bool ignited => activeModule.EngineIgnited;
		[DisplayName("Wheter engine flamed out.")]
		public bool flameout => activeModule.flameout;
		[DisplayName("Wheter engine is staged (activated by staging).")]
		public bool staged => activeModule.staged;
		[DisplayName("Activate the engine.")]
		public void activate() => activeModule.Activate();
		[DisplayName("Shutdown / deactivate the engine.")]
		public void shutdown() => activeModule.Shutdown();

		[DisplayName("Current ISP. (Specific impulse)")]
		public float isp => activeModule.realIsp;
		[DisplayName("Vacuum ISP.")]
		public float visp => activeModule.atmosphereCurve.Evaluate(0f);
		[DisplayName("Sea-level ISP.")]
		public float gisp => activeModule.atmosphereCurve.Evaluate(1f);
		[DisplayName("Sea-level ISP.")]
		public float slisp => activeModule.atmosphereCurve.Evaluate(1f);
		[DisplayName("Vacuum ISP.")]
		public float vacuumIsp => activeModule.atmosphereCurve.Evaluate(0f);
		[DisplayName("Sea-level ISP.")]
		public float groundIsp => activeModule.atmosphereCurve.Evaluate(1f);
		[DisplayName("Sea-level ISP.")]
		public float seaLevelIsp => activeModule.atmosphereCurve.Evaluate(1f);

		[Description("Avalable thrust at current pressure, ignoring the limiter"
			+ " (`thrustPercentage` and current throttle).")]
		public float thrust => activeModule.finalThrust;
		[Description("Thrust limiter in percents.")]
		public float thrustPercentage
		{
			get => activeModule.thrustPercentage;
			set => activeModule.thrustPercentage = RosMath.Clamp(value, 0f, 100f);
		}
		[Description("Get thrust at atmospheric pressure"
			+ " (0 = vacuum, 1 = Kerbin sea-level pressure, NaN = current pressure)"
			+ " and throttle (default 1 = full throttle). Ignores `thrustPercentage`.")]
		public float getThrust(float atm = float.NaN, float throttle = 1f)
		{
			var module = activeModule;
			if (float.IsNaN(atm))
				atm = (float)module.part.staticPressureAtm;
			return module.GetEngineThrust(module.atmosphereCurve.Evaluate(
				RosMath.Clamp((float)atm, 0f, 10f)),
				RosMath.Clamp(throttle, 0f, 1f));
		}

		// TODO: find out the amount of fuel left in tanks by this engine
		// ..... there is some 'ignition threshold'

		public double ratioSum => activeModule.ratioSum;
		public double mixtureDensity => activeModule.mixtureDensity;
		public double mixtureDensityRecip => activeModule.mixtureDensityRecip;

		ReadOnlyList<Propellant> propellants, propellants2;
		public ReadOnlyList<Propellant> Propellants => firstIsActive ? Propellants1 : Propellants2;
		public ReadOnlyList<Propellant> Propellants1 => propellants ?? (propellants = new ReadOnlyList<Propellant>());
		public ReadOnlyList<Propellant> Propellants2 => propellants2 ?? (multiMode ? propellants2 = new ReadOnlyList<Propellant>() : null);
	}
}
