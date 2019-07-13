using System;
using RedOnion.ROS;
using KSP.UI.Screens;
using MoonSharp.Interpreter;
using RedOnion.KSP.Parts;
using System.Collections.Generic;
using System.ComponentModel;

//NOTE: Never move Instance above MemberList ;)

namespace RedOnion.KSP.API
{
	public class Stage : InteropObject
	{
		public static MemberList MemberList { get; } = new MemberList(

@"Used to activate next stage and/or get various information about stage(s).
Returns true on success, if used as function. False if stage was not ready.",

		new IMember[]
		{
			new Int("number", "Stage number.",
				() => StageManager.CurrentStage),
			new Bool("ready", "Whether ready for activating next stage or not.",
				() => StageManager.CanSeparate),
			new Interop("parts", "PartSet", "Parts that belong to this stage, upto next decoupler",
				() => Parts),
			new Interop("crossParts", "PartSet", "Active engines and all accessible tanks upto next decoupler",
				() => CrossParts),
			new Interop("engines", "PartSet", "List of active engines",
				() => Engines),
			new Double("solidFuel", "Amount of solid fuel in active engines",
				() => SolidFuel),
			new Double("liquidFuel", "Amount of liquid fuel in tanks of current stage",
				() => LiquidFuel)
		});

		public static Stage Instance { get; } = new Stage();
		protected Stage() : base(MemberList) { }

		public override bool Call(ref Value result, object self, Arguments args, bool create)
		{
			result = Activate();
			return true;
		}
		public override DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
			=> DynValue.NewBoolean(Activate());

		public static int Number => StageManager.CurrentStage;
		public static bool Ready => StageManager.CanSeparate;

		public static bool Activate()
		{
			if (!HighLogic.LoadedSceneIsFlight)
				throw new InvalidOperationException("Can only activate stages in flight");
			if (!StageManager.CanSeparate)
				return false;
			StageManager.ActivateNextStage();
			return true;
		}

		[Description("Parts that belong to this stage, upto next decoupler")]
		public static PartSet<PartBase> Parts { get; }
			= new PartSet<PartBase>(Refresh);
		[Description("Active engines and all accessible tanks upto next decoupler")]
		public static PartSet<PartBase> CrossParts { get; }
			= new PartSet<PartBase>(Refresh);
		[Description("Active engines")]
		public static PartSet<Engine> Engines { get; }
			= new PartSet<Engine>(Refresh);

		public static double SolidFuel
			=> Engines.Resources.GetAmountOf("SolidFuel");
		public static double LiquidFuel
			=> CrossParts.Resources.GetAmountOf("LiquidFuel");			

		[Description("Propellants used by active engines"), ReadOnlyItems]
		public static ReadOnlyList<Propellant> Propellants { get; }
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
			GameEvents.onEngineActiveChange.Add(Instance.EngineChange);
			GameEvents.onStageActivate.Add(Instance.StageActivated);
			GameEvents.onStageSeparation.Add(Instance.StageSeparation);
			Value.DebugLog("Stage Refreshed (Decouple: {0}, Engines: {1})", nextDecoupler, Engines.Count);
		}
	}
}
