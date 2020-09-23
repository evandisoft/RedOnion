using System;
using System.ComponentModel;
using RedOnion.Attributes;
using RedOnion.KSP.API;
using RedOnion.ROS.Utilities;

namespace RedOnion.KSP.Parts
{
	[Description("Engine of a ship (vehicle/vessel).")]
	public class Engine : PartBase
	{
		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_multi_mode_engine.html). Module of multi-mode engine, if present (null otherwise).")]
		public MultiModeEngine multiModule { get; protected set; }
		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_module_engines.html). Module of first engine.")]
		public ModuleEngines firstModule { get; protected set; }
		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_module_engines.html). Module of second engine, if present (null otherwise).")]
		public ModuleEngines secondModule { get; protected set; }
		[Description("Running primary engine (or the only one).")]
		public bool firstIsActive => secondModule == null || multiModule.runningPrimary;
		[Description("Running secondary engine.")]
		public bool secondIsActive => !firstIsActive;
		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_module_engines.html). Active engine module.")]
		public ModuleEngines activeModule => firstIsActive ? firstModule : secondModule;
		[Unsafe, Description("[KSP API](https://kerbalspaceprogram.com/api/class_module_gimbal.html). Gimbal module, if present (null otherwise).")]
		public ModuleGimbal gimbalModule { get; private set; }
		[Description("Is multi-mode engine (or not).")]
		public bool multiMode => secondModule != null;
		[Description("Has gimbal module.")]
		public bool hasGimbal => gimbalModule != null;

		[Description("Accepts `engine`. (Case insensitive)")]
		public override bool istype(string name)
			=> name.Equals("engine", StringComparison.OrdinalIgnoreCase);

		protected internal Engine(Ship ship, Part part, PartBase parent, LinkPart decoupler)
			: base(PartType.Engine, ship, part, parent, decoupler)
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
			var propellants = activeModule?.propellants;
			if (propellants != null)
			{
				foreach (var propellant in propellants)
				{
					if (propellant.GetFlowMode() == ResourceFlowMode.NO_FLOW)
					{
						booster = true;
						break;
					}
				}
			}
		}

		[Description("Whether engine is operational (ignited and not flameout).")]
		public bool operational => activeModule.isOperational;
		[Description("Wheter engine is ignited.")]
		public bool ignited => activeModule.EngineIgnited;
		[Description("Wheter engine flamed out.")]
		public bool flameout => activeModule.flameout;

		[Description("Activate the engine.")]
		public void activate() => activeModule.Activate();
		[Description("Shutdown / deactivate the engine.")]
		public void shutdown() => activeModule.Shutdown();

		[Description("Get specific impulse \\[kN] at atmospheric pressure"
			+ " (0 = vacuum, 1 = Kerbin sea-level pressure, default NaN = current pressure).")]
		public double getIsp(double atm = double.NaN)
			=> activeModule.atmosphereCurve.Evaluate((float)(
				double.IsNaN(atm) ? activeModule.part.staticPressureAtm : atm));

		[Description("Get thrust \\[kN] at atmospheric pressure"
			+ " (0 = vacuum, 1 = Kerbin sea-level pressure, default NaN = current pressure)"
			+ " and throttle (default 1 = full throttle). Ignores `thrustPercentage`.")]
		public double getThrust(double atm = float.NaN, double throttle = 1f)
		{
			var module = activeModule;
			if (double.IsNaN(atm))
				atm = module.part.staticPressureAtm;
			return module.GetEngineThrust(module.atmosphereCurve.Evaluate(
				RosMath.Clamp((float)atm, 0f, 10f)),
				RosMath.Clamp((float)throttle, 0f, 1f));
		}

		[Description("Current ISP (Specific impulse). \\[seconds]")]
		public double isp => activeModule.realIsp;
		[Description("Vacuum ISP. \\[seconds]")]
		public double vacuumIsp => activeModule.atmosphereCurve.Evaluate(0f);
		[Description("Sea-level ISP. \\[seconds]")]
		public double seaLevelIsp => activeModule.atmosphereCurve.Evaluate(1f);

		[Description("Current thrust [kN] (at current pressure, with current `thrustPercentage` and current throttle).")]
		public double thrust => activeModule.finalThrust;
		[Description("Thrust limiter in percents.")]
		public double thrustPercentage
		{
			get => activeModule.thrustPercentage;
			set => activeModule.thrustPercentage = RosMath.Clamp((float)value, 0f, 100f);
		}
		public double ratioSum => activeModule.ratioSum;
		public double mixtureDensity => activeModule.mixtureDensity;
		public double mixtureDensityRecip => activeModule.mixtureDensityRecip;
		public double ignitionThreshold => activeModule.ignitionThreshold;

		PropellantList _propellants, _propellants2;
		[Description("List of propellants used by the engine (by currently active mode).")]
		public PropellantList propellants => firstIsActive ? propellants1 : propellants2;
		[Description("List of propellants used by first mode.")]
		public PropellantList propellants1 => _propellants ?? (_propellants = new PropellantList(firstModule));
		[Description("List of propellants used by second mode (null for single-mode engines).")]
		public PropellantList propellants2 => _propellants2 ?? (multiMode ? _propellants2 = new PropellantList(secondModule) : null);

		[Description("Indicator that the engines is (probably) solid rocket booster (contains propellant that does not flow).")]
		public bool booster { get; }
	}
}
