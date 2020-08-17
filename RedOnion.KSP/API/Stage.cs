//#define DEBUG_STAGE_NOFUEL
//#define DEBUG_STAGE_XPARTS

using System;
using KSP.UI.Screens;
using RedOnion.Attributes;
using RedOnion.KSP.Parts;
using RedOnion.ROS;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using RedOnion.Debugging;

//TODO: redirect these to new `ship.stages` API

namespace RedOnion.KSP.API
{
	[Callable("activate")]
	[Description("Used to activate next stage and/or get various information about stage(s)."
	+ " Returns true on success, if used as function. False if stage was not ready.")]
	public static class Stage
	{
		[Description("Stage number.")]
		public static int number => StageManager.CurrentStage;
		[Description("Whether ready for activating next stage or not.")]
		public static bool ready => StageManager.CanSeparate;
		[Description("True when current stage number is the same as number of stages (LED is flashing).")]
		public static bool pending => StageManager.CurrentStage == StageManager.StageCount;

		[Description("Activate next stage (can call the stage object as a function as well (stage() instead of stage.activate())")]
		public static bool activate()
		{
			if (!HighLogic.LoadedSceneIsFlight)
				throw new InvalidOperationException("Can only activate stages in flight");
			if (!StageManager.CanSeparate)
				return false;
			StageManager.ActivateNextStage();
			return true;
		}

		[Description("Parts that will be separated by next decoupler")]
		public static PartSet<PartBase> parts { get; }
			= new PartSet<PartBase>(null, Refresh);

		[Description("Active engines (regardless of decouplers)."
			+ " `Engines.Resources` reflect total amounts of fuels"
			+ " inside boosters with fuel that cannot flow (like solid fuel).")]
		public static EngineSet engines { get; }
			= new StageEngineSet();
		//NOTE: checks for operational engines can still be seen in this file
		//..... even though we now use CheckFlameout(), that is for sure
		//..... and because we may choose to do the check less often
		class StageEngineSet : EngineSet
		{
			public StageEngineSet() : base(null, Stage.Refresh) { }
			protected override void Update()
			{
				CheckFlameout();
				base.Update();
			}
		}
		static TimeStamp lastFlameoutCheck = TimeStamp.never;
		internal static void CheckFlameout(bool force = false)
		{
			if (Dirty)
				return;
			var now = Time.now;
			if (!force && lastFlameoutCheck == now)
				return;
			lastFlameoutCheck = now;
			if (engines.anyFlameout)
				SetDirty("Flameout");
		}

		[Description("All accessible tanks upto next decoupler that can contain propellants."
			+ " `xparts.resources` reflect total amounts of fuels accessible to active engines,"
			+ " but only in parts that will be separated by next decoupler."
			+ " This includes liquid fuel and oxidizer and can be used for automated staging,"
			+ " Especially so called Asparagus and any design throwing off tanks (with or without engines).")]
		public static PartSet<PartBase> xparts { get; }
			= new StageCrossFeedParts();
		class StageCrossFeedParts : PartSet<PartBase>
		{
			public StageCrossFeedParts() : base(null, Stage.Refresh) { }
			protected override void Update()
			{
				CheckFlameout();
				base.Update();
			}
		}

		[Description("Amount of solid fuel available in active engines."
			+ " Similar to `engines.resources.getAmountOf(\"SolidFuel\")`"
			+ " but ignores engines/boosters not separated in next stage."
			+ " Useful when using central booster with smaller side-boosters (decoupled first)."
			+ " Stock-only version.")]
		public static double solidfuel
		{
			get
			{
				var ship = Ship.Active;
				var sum = 0.0;
				if (ship != null)
				{
					var nextDecoupler = ship.parts.nextDecouplerStage;
					foreach (var e in engines)
					{
						if (!e.booster || !e.operational || e.decoupledin < nextDecoupler)
							continue;
						var res = e.native.Resources["SolidFuel"];
						if (res != null)
							sum += res.amount;
					}
				}
				return sum;
			}
		}

		[WorkInProgress, Description("Amount of solid-like fuel available in active engines."
			+ " Similar to `engines.resources.getAmountOf(engines.propellants.namesOfSolid)`"
			+ " but ignores engines/boosters not separated in next stage."
			+ " Useful when using central booster with smaller side-boosters (decoupled first)."
			+ " Universal version (should be compatible with mods - e.g. Karbonite).")]
		public static double solidlike
		{
			get
			{
				var ship = Ship.Active;
				var sum = 0.0;
				if (ship != null)
				{
					var nextDecoupler = ship.parts.nextDecouplerStage;
					foreach (var e in engines)
					{
						if (!e.booster || !e.operational || e.decoupledin < nextDecoupler)
							continue;
						foreach (var propellant in e.activeModule.propellants)
						{
							if (propellant.GetFlowMode() != ResourceFlowMode.NO_FLOW)
								continue;
							var res = e.native.Resources[propellant.name];
							if (res != null)
								sum += res.amount;
						}
					}
				}
				return sum;
			}
		}
		[Description("Amount of liquid fuel available in tanks of current stage to active engines."
			+ " Shortcut to `xparts.resources.getAmountOf(\"LiquidFuel\")`. Stock-only version.")]
		public static double liquidfuel
			=> xparts.resources.getAmountOf("LiquidFuel");

		static readonly HashSet<ResourceID> liquidFuels = new HashSet<ResourceID>();
		[WorkInProgress, Description("Amount of liquid fuel available in tanks of current stage to active engines."
			+ " Similar to `xparts.resources.getAmountOf(engines.propellants.namesOfLiquid)`."
			+ " Universal version (should be compatible with mods - e.g. Karbonite and CryoEngiens).")]
		public static double liquidlike
		{
			get
			{
				liquidFuels.Clear();
				foreach (var e in engines)
				{
					if (e.booster || !e.operational)
						continue;
					foreach (var propellant in e.activeModule.propellants)
					{
						if (propellant.GetFlowMode() == ResourceFlowMode.STACK_PRIORITY_SEARCH)
							liquidFuels.Add(new ResourceID(propellant.id));
					}
				}
				return xparts.resources.getAmountOf(liquidFuels);
			}
		}

		[WorkInProgress, Description("Total amount of fuel avialable for active engines in current stage."
			+ " Designed with Ion and Monopropellant engines as well as mods in mind,"
			+ " use `solidfuel + liquidfuel` as fallback, if this does not work.")]
		public static double fuel { get { UpdateBurnTime(); return _fuel; } }
		[WorkInProgress, Description("Indicator for staging - `fuel == 0.0`. Note that it could be better to use `fuel < 0.1` instead.")]
		public static bool nofuel => fuel == 0.0;

		static readonly Dictionary<ResourceID, PartResourceDefinition>
			flowingFuels = new Dictionary<ResourceID, PartResourceDefinition>();
		static double _isp, _sisp, _flow, _thrust, _fuel, _fuelMass, _dryMass, _deltaV;
		static double _nextStdIsp, _nextThrust, _nextMass;
		static TimeDelta _burnTime;
		public static double isp { get { UpdateBurnTime(); return _isp; } }
		public static double flow { get { UpdateBurnTime(); return _flow; } }
		public static double thrust { get { UpdateBurnTime(); return _thrust; } }
		public static double fuelMass { get { UpdateBurnTime(); return _fuelMass; } }
		public static double dryMass { get { UpdateBurnTime(); return _dryMass; } }
		public static double currDeltaV { get { UpdateBurnTime(); return _deltaV; } }
		public static TimeDelta currBurnTime { get { UpdateBurnTime(); return _burnTime; } }

		static TimeStamp burnTimeUpdate = Time.never;
		static TimeStamp burnTimeUpdate2 = Time.never;
		static TimeDelta burnTimeUpdateDelta = new TimeDelta(0.2);
		static void UpdateBurnTime()
		{
			CheckFlameout();
			if (Dirty) Refresh();
			var now = Time.now;
			if (now - burnTimeUpdate < burnTimeUpdateDelta)
				return;
			burnTimeUpdate = now;
			_isp = 0;
			_sisp = 0;
			_flow = 0;
			_thrust = 0;
			_fuel = 0;
			_fuelMass = 0;
			_dryMass = 0;
			_deltaV = 0;
			_burnTime = TimeDelta.none;
			flowingFuels.Clear();
			var ship = Ship.Active;
			if (ship == null)
				return;
			var nextDecoupler = ship.parts.nextDecouplerStage;
			foreach (var e in engines)
			{// see EngineSet.burntime
				if (!e.operational)
					continue;
				var eisp = e.isp;
				if (eisp < EngineSet.minIsp)
					continue;
				var ethr = e.getThrust() * e.thrustPercentage * 0.01;
				_thrust += ethr;
				_flow += ethr / eisp;

				if (!e.booster)
				{
					foreach (var propellant in e.activeModule.propellants)
						if (propellant.GetFlowMode() != ResourceFlowMode.NO_FLOW)
							flowingFuels[new ResourceID(propellant.id)] = propellant.resourceDef;
				}
				else if (e.decoupledin >= nextDecoupler)
				{
					foreach (var propellant in e.activeModule.propellants)
					{
						if (propellant.GetFlowMode() != ResourceFlowMode.NO_FLOW)
							continue;
						var amount = propellant.totalResourceAvailable;
						_fuel += amount;
						_fuelMass += amount * propellant.resourceDef.density;
					}
				}
			}
			_isp = _flow < EngineSet.minFlow ? 0.0 : _thrust / _flow;
			foreach (var flowing in flowingFuels)
			{
				var amount = xparts.resources.getAmountOf(flowing.Key);
				_fuel += amount;
				_fuelMass += amount * flowing.Value.density;
			}
			// rather update the mass right now
			ship.native.GetTotalMass();
			_dryMass = Math.Max(0.0, ship.mass - _fuelMass);
			if (_isp >= EngineSet.minIsp)
			{
				_sisp = EngineSet.g0 * _isp;
				_deltaV = _sisp * Math.Log(ship.mass / _dryMass);
				_burnTime = new TimeDelta(_sisp * _fuelMass / _thrust);
			}

#if DEBUG && DEBUG_STAGE_NOFUEL
			if (_fuel == 0.0)
			{
				Value.DebugLog($"No Fuel. Flowing Fuels: {flowingFuels.Count}, XParts: {xparts.count}");
				foreach (var ff in flowingFuels)
				{
					var name = ff.Value.name;
					Value.DebugLog($"{name}: {xparts.resources.getAmountOf(ff.Key):F2}/{xparts.resources.getAmountOf(name):F2}");
				}
				Value.DebugLog("All resources:");
				foreach (var res in xparts.resources)
					Value.DebugLog($"{res.name}: {res.amount:F2}/{res.maxAmount:F2}");
			}
#endif
		}

		[WorkInProgress, Description("Estimate burn time for given delta-v.")]
		public static TimeDelta burntime(double deltaV)
		{
			if (!(deltaV >= 0.0) || double.IsInfinity(deltaV))
				return TimeDelta.none; // !(deltaV >= 0.0) checks for NaN as well
			if (deltaV == 0.0)
				return TimeDelta.zero;
			UpdateBurnTime();
			if (_isp < EngineSet.minIsp)
				return TimeDelta.none;
			if (deltaV > _deltaV)
			{
				//could do the following recursively but this should do for now
				var now = Time.now;
				if (now - burnTimeUpdate2 >= burnTimeUpdateDelta)
				{
					burnTimeUpdate2 = now;
					var ship = Ship.Active;
					_nextStdIsp = 0;
					_nextThrust = 0;
					_nextMass = 0;
					double nxflow = 0;
					double nxstage = ship.nextDecouplerStage;
					foreach (var e in ship.engines)
					{
						if (e.stage < nxstage)
							continue;
						if (e.decoupledin >= nxstage)
							continue;
						var eisp = e.vacuumIsp; // assume vacuum for next stage
						if (eisp < EngineSet.minIsp)
							continue;
						var ethr = e.getThrust(atm: 0.0) * e.thrustPercentage * 0.01;
						//Value.DebugLog($"BT2: {e.name} thrust={ethr}, isp={eisp}");
						_nextThrust += ethr;
						nxflow += ethr / eisp;
					}
					if (nxflow >= EngineSet.minFlow)
					{
						_nextStdIsp = _nextThrust/nxflow * EngineSet.g0;
						foreach (var p in ship.parts)
						{
							if (p.decoupledin < nxstage)
								_nextMass += p.mass;
						}
					}
				}
				if (_nextStdIsp > 0 && _nextMass > 0 && _nextThrust > 0)
					return new TimeDelta(_burnTime + 0.5 + _nextMass * _nextStdIsp *
						(1.0 - Math.Pow(Math.E, -(deltaV - _deltaV) / _nextStdIsp)) / _nextThrust);
/*#if DEBUG
				if (burnTimeUpdate2 == now)
					Value.DebugLog($"BT2: sisp={_nextStdIsp}, mass={_nextMass}, thrust={_nextThrust}");
#endif*/
			}
			return new TimeDelta(Ship.Active.mass * _sisp * (1.0 - Math.Pow(Math.E, -deltaV / _sisp)) / _thrust);
		}

		static internal bool Dirty { get; private set; } = true;
		static internal void SetDirty(string reason)
		{
			if (Dirty) return;
			MainLogger.DebugLog("Stage Dirty: " + reason);
			GameEvents.onEngineActiveChange.Remove(hooks.EngineChange);
			GameEvents.onStageActivate.Remove(hooks.StageActivated);
			GameEvents.onStageSeparation.Remove(hooks.StageSeparation);
			GameEvents.StageManager.OnGUIStageSequenceModified.Remove(hooks.StageSequenceModified);
			GameEvents.StageManager.OnStagingSeparationIndices.Remove(hooks.StagingSeparationIndices);
			GameEvents.StageManager.OnGUIStageAdded.Remove(hooks.StagesChanged);
			GameEvents.StageManager.OnGUIStageRemoved.Remove(hooks.StagesChanged);
			Dirty = true;
			parts.SetDirty(true);
			xparts.SetDirty(true);
			engines.SetDirty(true);

		}
		static readonly Hooks hooks = new Hooks();
		class Hooks
		{
			public void EngineChange(ModuleEngines engine)
				=> SetDirty("EngineChange");
			public void StageActivated(int stage)
				=> SetDirty("StageActivated");
			public void StageSeparation(EventReport e)
				=> SetDirty("StageSeparation");
			public void StageSequenceModified()
				=> SetDirty("StageSequenceModified");
			public void StagingSeparationIndices()
				=> SetDirty("StagingSeparationIndices");
			public void StagesChanged(int stage)
				=> SetDirty("StagesChanged");
		}

		static void Refresh()
		{
			var ship = Ship.Active;
			if (ship == null)
			{
				Dirty = false;
				parts.Dirty = false;
				xparts.Dirty = false;
				engines.Dirty = false;
				parts.ship = null;
				xparts.ship = null;
				engines.ship = null;
				return;
			}
			parts.ship = ship;
			xparts.ship = ship;
			engines.ship = ship;
			var shipParts = ship.parts;
			var nextDecoupler = shipParts.nextDecouplerStage;
			foreach (var p in shipParts)
			{
				if (p.decoupledin >= nextDecoupler)
					parts.Add(p);
			}
#if DEBUG && DEBUG_STAGE_XPARTS
			var sb = new StringBuilder();
#endif
			foreach (var e in ship.engines)
			{
				if (!e.operational)
					continue;
				engines.Add(e);
				if (e.decoupledin >= nextDecoupler && e.native.Resources.Count > 0)
					xparts.Add(e);
				foreach (var crossPart in e.native.crossfeedPartSet.GetParts())
				{
					if (crossPart.Resources.Count == 0)
						continue;
					var part = shipParts[crossPart];
					if (part.decoupledin >= nextDecoupler && xparts.Add(part))
					{
#if DEBUG && DEBUG_STAGE_XPARTS
						sb.AppendFormat("Part {0} added to xparts. [", part.name);
						var first = true;
						foreach (var res in crossPart.Resources)
						{
							if (!first) sb.Append(", ");
							sb.AppendFormat("{0}: {1:F2}/{2:F0}", res.resourceName, res.amount, res.maxAmount);
							first = false;
						}
						Value.DebugLog(sb.Append(']').ToString());
						sb.Length = 0;
#endif
					}
				}
			}
			lastFlameoutCheck = Time.now;
			burnTimeUpdate = Time.never;
			burnTimeUpdate2 = Time.never;
			Dirty = false;
			parts.Dirty = false;
			xparts.Dirty = false;
			engines.Dirty = false;
			GameEvents.onEngineActiveChange.Add(hooks.EngineChange);
			GameEvents.onStageActivate.Add(hooks.StageActivated);
			GameEvents.onStageSeparation.Add(hooks.StageSeparation);
			GameEvents.StageManager.OnGUIStageSequenceModified.Add(hooks.StageSequenceModified);
			GameEvents.StageManager.OnStagingSeparationIndices.Add(hooks.StagingSeparationIndices);
			GameEvents.StageManager.OnGUIStageAdded.Add(hooks.StagesChanged);
			GameEvents.StageManager.OnGUIStageRemoved.Add(hooks.StagesChanged);
			MainLogger.DebugLog($"Stage #{ship.currentStage} Refreshed (Decouple: {nextDecoupler}, Engines: {engines.count}, Parts: {parts.count}/X:{xparts.count})");
		}
	}
}
