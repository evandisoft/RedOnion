using System;
using RedOnion.ROS;
using KSP.UI.Screens;
using MoonSharp.Interpreter;
using RedOnion.KSP.Parts;
using System.Collections.Generic;
using System.ComponentModel;

namespace RedOnion.KSP.API
{
	[Description("Used to activate next stage and/or get various information about stage(s)."
	+ " Returns true on success, if used as function. False if stage was not ready.")]
	public class Stage : ICallable
	{
		[Browsable(false), MoonSharpHidden]
		public static Stage Instance { get; } = new Stage();
		protected Stage() { }

		bool ICallable.Call(ref Value result, object self, Arguments args, bool create)
		{
			result = activate();
			return true;
		}
		[MoonSharpUserDataMetamethod("__call"), Browsable(false)]
		public DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
			=> DynValue.NewBoolean(activate());

		[Description("Stage number.")]
		public static int number => StageManager.CurrentStage;
		[Description("Whether ready for activating next stage or not.")]
		public static bool ready => StageManager.CanSeparate;

		[Description("Activate next stage (can simply call stage() instead)")]
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
		[Description("Active engines and all accessible tanks upto next decoupler."
			+ " `xparts.resources` reflect total amounts of fuels accessible to active engines,"
			+ " but only in parts that will be separated by next decoupler."
			+ " This includes liquid fuel and oxidizer and can be used for automated staging,"
			+ " Especially so called Asparagus and any design throwing off tanks (with or without engiens).")]
		public static PartSet<PartBase> xparts { get; }
			= new PartSet<PartBase>(null, Refresh);

		[Description("Amount of solid fuel available in active engines."
			+ " Shortcut to `engines.resources.getAmountOf(\"SolidFuel\")`.")]
		public static double solidfuel
			=> engines.resources.getAmountOf("SolidFuel");
		[Description("Amount of liquid fuel available in tanks of current stage to active engines."
			+ " Shortcut to `xparts.resources.getAmountOf(\"LiquidFuel\")`.")]
		public static double liquidfuel
			=> xparts.resources.getAmountOf("LiquidFuel");

		[Description("Total amount of fuel avialable in active engines.")]
		public static double fuel => solidfuel + liquidfuel;

		[Description("Estimate burn time for given delta-v.")]
		public static double burnTime(double deltaV) => engines.burnTime(deltaV);
		// TODO: burnTime even if current stage cannot handle it


		static protected internal bool Dirty { get; private set; } = true;
		static protected internal void SetDirty(string reason = null)
		{
			if (Dirty) return;
			Value.DebugLog(reason == null ? "Stage Dirty" : "Stage Dirty: " + reason);
			GameEvents.onEngineActiveChange.Remove(Instance.EngineChange);
			GameEvents.onStageActivate.Remove(Instance.StageActivated);
			GameEvents.onStageSeparation.Remove(Instance.StageSeparation);
			GameEvents.StageManager.OnGUIStageSequenceModified.Remove(Instance.StageSequenceModified);
			GameEvents.StageManager.OnStagingSeparationIndices.Remove(Instance.StagingSeparationIndices);
			GameEvents.StageManager.OnGUIStageAdded.Remove(Instance.StagesChanged);
			GameEvents.StageManager.OnGUIStageRemoved.Remove(Instance.StagesChanged);
			Dirty = true;
			parts.SetDirty();
			xparts.SetDirty();
			engines.SetDirty();
			parts.Clear();
			xparts.Clear();
			engines.Clear();

		}
		void EngineChange(ModuleEngines engine)
			=> SetDirty("EngineChange");
		void StageActivated(int stage)
			=> SetDirty("StageActivated");
		void StageSeparation(EventReport e)
			=> SetDirty("StageSeparation");
		void StageSequenceModified()
			=> SetDirty("StageSequenceModified");
		void StagingSeparationIndices()
			=> SetDirty("StagingSeparationIndices");
		void StagesChanged(int stage)
			=> SetDirty("StagesChanged");

		static protected void Refresh()
		{
			parts.Clear();
			xparts.Clear();
			engines.Clear();
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
			foreach (var e in ship.engines)
			{
				if (e.state != PartStates.ACTIVE)
					continue;
				engines.Add(e);
				xparts.Add(e);
				foreach (var crossPart in e.native.crossfeedPartSet.GetParts())
				{
					var part = shipParts[crossPart];
					if (part.decoupledin >= nextDecoupler)
						xparts.Add(part);
				}
			}
			Dirty = false;
			parts.Dirty = false;
			xparts.Dirty = false;
			engines.Dirty = false;
			GameEvents.onEngineActiveChange.Add(Instance.EngineChange);
			GameEvents.onStageActivate.Add(Instance.StageActivated);
			GameEvents.onStageSeparation.Add(Instance.StageSeparation);
			GameEvents.StageManager.OnGUIStageSequenceModified.Add(Instance.StageSequenceModified);
			GameEvents.StageManager.OnStagingSeparationIndices.Add(Instance.StagingSeparationIndices);
			GameEvents.StageManager.OnGUIStageAdded.Add(Instance.StagesChanged);
			GameEvents.StageManager.OnGUIStageRemoved.Add(Instance.StagesChanged);
			Value.DebugLog("Stage Refreshed (Decouple: {0}, Engines: {1})", nextDecoupler, engines.Count);
		}
	}
}
