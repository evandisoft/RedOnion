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

		static protected internal ReadOnlyPartList<PartBase> parts;
		static protected internal ReadOnlyPartSet<PartBase> crossParts;
		static protected internal bool Dirty { get; private set; } = true;
		static protected internal void SetDirty()
		{
			if (Dirty)
				return;
			GameEvents.onEngineActiveChange.Remove(EngineChange);
			Dirty = true;
		}
		private static void EngineChange(ModuleEngines engine)
			=> SetDirty();
		[Description("Parts that belong to this stage, upto next decoupler")]
		public static IList<PartBase> Parts
		{
			get
			{
				if (Dirty)
					Refresh();
				return parts;
			}
		}
		[Description("Active engines and all accessible tanks upto next decoupler")]
		public static IList<PartBase> CrossParts
		{
			get
			{
				if (Dirty)
					Refresh();
				return crossParts;
			}
		}
		static protected void Refresh()
		{
			var ship = Ship.Active;
			if (ship == null)
			{
				parts?.Clear();
				crossParts?.Clear();
				Dirty = false;
				return;
			}
			if (parts == null)
				parts = new ReadOnlyPartList<PartBase>();
			if (crossParts == null)
				crossParts = new ReadOnlyPartSet<PartBase>();
			var shipParts = ship.Parts;
			var nextDecoupler = shipParts.NextDecouplerStage;
			foreach (var p in ship.Parts)
			{
				if (p.DecoupledIn >= nextDecoupler)
					parts.Add(p);
				if (p.State == PartStates.ACTIVE)
				{// active engine
					crossParts.Add(p);
					foreach (var crossPart in p.Native.crossfeedPartSet.GetParts())
					{
						var part = shipParts[crossPart];
						if (part.DecoupledIn >= nextDecoupler)
							crossParts.Add(part);
					}
				}
			}
			Dirty = false;
			GameEvents.onEngineActiveChange.Add(EngineChange);
		}
	}
}
