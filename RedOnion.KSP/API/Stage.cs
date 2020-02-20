//#define DEBUG_STAGE_NOFUEL

using System;
using RedOnion.ROS;
using KSP.UI.Screens;
using MoonSharp.Interpreter;
using RedOnion.KSP.Parts;
using System.Collections.Generic;
using System.ComponentModel;
using RedOnion.KSP.Utilities;
using RedOnion.Attributes;
using System.Linq;
using RedOnion.Collections;
using System.Text;

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
			= new EngineSet(null, Refresh);
		[Description("All accessible tanks upto next decoupler that can contain propellants."
			+ " `xparts.resources` reflect total amounts of fuels accessible to active engines,"
			+ " but only in parts that will be separated by next decoupler."
			+ " This includes liquid fuel and oxidizer and can be used for automated staging,"
			+ " Especially so called Asparagus and any design throwing off tanks (with or without engines).")]
		public static PartSet<PartBase> xparts { get; }
			= new PartSet<PartBase>(null, Refresh);

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
						if (e.flameout || !e.booster || e.decoupledin < nextDecoupler)
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
						if (e.flameout || !e.booster || e.decoupledin < nextDecoupler)
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

		static HashSet<ResourceID> liquidFuels = new HashSet<ResourceID>();
		[WorkInProgress, Description("Amount of liquid fuel available in tanks of current stage to active engines."
			+ " Similar to `xparts.resources.getAmountOf(engines.propellants.namesOfLiquid)`"
			+ " but also ignores flameout engines.")]
		public static double liquidlike
		{
			get
			{
				liquidFuels.Clear();
				foreach (var e in engines)
				{
					if (e.flameout || e.booster)
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

		static HashSet<ResourceID> flowingFuels = new HashSet<ResourceID>();
		[WorkInProgress, Description("Total amount of fuel avialable for active engines in current stage."
			+ " Designed with Ion and Monopropellant engines as well as mods in mind,"
			+ " use `solidfuel + liquidfuel` as fallback, if this does not work.")]
		public static double fuel
		{
			get
			{
				var ship = Ship.Active;
				var sum = 0.0;
				if (ship != null)
				{
					var nextDecoupler = ship.parts.nextDecouplerStage;
					flowingFuels.Clear();
					foreach (var e in engines)
					{
						if (e.flameout)
							continue;
						if (e.booster)
						{
							if (e.decoupledin >= nextDecoupler)
							{
								foreach (var propellant in e.activeModule.propellants)
									if (propellant.GetFlowMode() == ResourceFlowMode.NO_FLOW)
										sum += propellant.totalResourceAvailable;
							}
							continue;
						}
						foreach (var propellant in e.activeModule.propellants)
							if (propellant.GetFlowMode() != ResourceFlowMode.NO_FLOW)
								flowingFuels.Add(new ResourceID(propellant.id));
					}
					sum += xparts.resources.getAmountOf(flowingFuels);
#if DEBUG && DEBUG_STAGE_NOFUEL
					if (sum == 0.0)
					{
						Value.DebugLog($"No Fuel. Flowing Fuels: {flowingFuels.Count}, XParts: {xparts.count}");
						foreach (var ff in flowingFuels)
						{
							var name = PartResourceLibrary.Instance.GetDefinition(ff).name;
							Value.DebugLog($"{name}: {xparts.resources.getAmountOf(ff):F2}/{xparts.resources.getAmountOf(name):F2}");
						}
						Value.DebugLog("All resources:");
						foreach (var res in xparts.resources)
							Value.DebugLog($"{res.name}: {res.amount:F2}/{res.maxAmount:F2}");
					}
#endif
				}
				return sum;
			}
		}
		[WorkInProgress, Description("Indicator for staging - `fuel == 0.0`. Note that it could be better to use `fuel < 0.1` instead.")]
		public static bool nofuel => fuel == 0.0;

		[WorkInProgress, Description("Estimate burn time for given delta-v.")]
		public static TimeDelta burntime(double deltaV) => engines.burnTime(deltaV);
		// TODO: burnTime even if current stage cannot handle it

		static internal bool Dirty { get; private set; } = true;
		static internal void SetDirty(string reason = null)
		{
			if (Dirty) return;
			Value.DebugLog(reason == null ? "Stage Dirty" : "Stage Dirty: " + reason);
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
#if DEBUG
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
#if DEBUG
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
			Value.DebugLog($"Stage #{ship.currentStage} Refreshed (Decouple: {nextDecoupler}, Engines: {engines.count}, Parts: {parts.count}/X:{xparts.count})");
		}
	}
}
