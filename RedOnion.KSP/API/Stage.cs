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
			result = Activate();
			return true;
		}
		[MoonSharpUserDataMetamethod("__call"), Browsable(false)]
		public DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
			=> DynValue.NewBoolean(Activate());

		[Description("Stage number.")]
		public static int Number => StageManager.CurrentStage;
		[Description("Whether ready for activating next stage or not.")]
		public static bool Ready => StageManager.CanSeparate;

		[Description("Activate next stage (can simply call stage() instead)")]
		public static bool Activate()
		{
			if (!HighLogic.LoadedSceneIsFlight)
				throw new InvalidOperationException("Can only activate stages in flight");
			if (!StageManager.CanSeparate)
				return false;
			StageManager.ActivateNextStage();
			return true;
		}

		[Description("Parts that will be separated by next decoupler")]
		public static PartSet<PartBase> Parts { get; }
			= new PartSet<PartBase>(Refresh);
		[Description("Active engines (regardless of decouplers)."
			+ " `Engines.Resources` reflect total amounts of fuels"
			+ " inside boosters with fuel that cannot flow (like solid fuel).")]
		public static PartSet<Engine> Engines { get; }
			= new PartSet<Engine>(Refresh);
		[Description("Active engines and all accessible tanks upto next decoupler."
			+ " `CrossParts.Resources` reflect total amounts of fuels accessible to active engines,"
			+ " but only in parts that will be separated by next decoupler."
			+ " This includes liquid fuel and oxidizer and can be used for automated staging,"
			+ " Especially so called Asparagus and any design throwing off tanks (with or without engiens).")]
		public static PartSet<PartBase> CrossParts { get; }
			= new PartSet<PartBase>(Refresh);

		[Description("Amount of solid fuel in active engines."
			+ " Shortcut to Engines.Resources.GetAmountOf(\"SolidFuel\")")]
		public static double SolidFuel
			=> Engines.Resources.GetAmountOf("SolidFuel");
		[Description("Amount of liquid fuel in tanks of current stage."
			+ " Shortcut to CrossParts.Resources.GetAmountOf(\"LiquidFuel\")")]
		public static double LiquidFuel
			=> CrossParts.Resources.GetAmountOf("LiquidFuel");

		// TODO: aggregate to custom PropellantInfo
		[Description("Propellants used by active engines"), ReadOnlyItems]
		static ReadOnlyList<Propellant> Propellants { get; }
			= new ReadOnlyList<Propellant>(Refresh);
		static readonly HashSet<string> propellantNames
			= new HashSet<string>();

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
			Parts.SetDirty();
			CrossParts.SetDirty();
			Engines.SetDirty();
			Propellants.SetDirty();
			Parts.Clear();
			CrossParts.Clear();
			Engines.Clear();
			Propellants.Clear();
			propellantNames.Clear();

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
			Parts.Clear();
			CrossParts.Clear();
			Engines.Clear();
			Propellants.Clear();
			propellantNames.Clear();
			var ship = Ship.Active;
			if (ship == null)
			{
				Dirty = false;
				Parts.Dirty = false;
				CrossParts.Dirty = false;
				Engines.Dirty = false;
				return;
			}
			var shipParts = ship.Parts;
			var nextDecoupler = shipParts.NextDecouplerStage;
			foreach (var p in shipParts)
			{
				if (p.DecoupledIn >= nextDecoupler)
					Parts.Add(p);
			}
			foreach (var e in ship.Engines)
			{
				if (e.State != PartStates.ACTIVE)
					continue;
				Engines.Add(e);
				foreach (var propellant in e.Propellants)
					if (propellantNames.Add(propellant.name))
						Propellants.Add(propellant);
				CrossParts.Add(e);
				foreach (var crossPart in e.Native.crossfeedPartSet.GetParts())
				{
					var part = shipParts[crossPart];
					if (part.DecoupledIn >= nextDecoupler)
						CrossParts.Add(part);
				}
			}
			Dirty = false;
			Parts.Dirty = false;
			CrossParts.Dirty = false;
			Engines.Dirty = false;
			Propellants.Dirty = false;
			GameEvents.onEngineActiveChange.Add(Instance.EngineChange);
			GameEvents.onStageActivate.Add(Instance.StageActivated);
			GameEvents.onStageSeparation.Add(Instance.StageSeparation);
			GameEvents.StageManager.OnGUIStageSequenceModified.Add(Instance.StageSequenceModified);
			GameEvents.StageManager.OnStagingSeparationIndices.Add(Instance.StagingSeparationIndices);
			GameEvents.StageManager.OnGUIStageAdded.Add(Instance.StagesChanged);
			GameEvents.StageManager.OnGUIStageRemoved.Add(Instance.StagesChanged);
			Value.DebugLog("Stage Refreshed (Decouple: {0}, Engines: {1})", nextDecoupler, Engines.Count);
		}
	}
}
