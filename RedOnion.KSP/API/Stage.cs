using System;
using RedOnion.Script;
using KSP.UI.Screens;
using MoonSharp.Interpreter;

namespace RedOnion.KSP.API
{
	public class Stage : InteropObject
	{
		public static Stage Instance { get; } = new Stage();
		public override string Help =>
@"Used to activate next stage and/or get various information about stage(s).
Returns true on success, if used as function. False if stage was not ready.";

		public Stage() : base(ObjectFeatures.Function, new Member[]
		{
			new Member("number", "int", "Stage number.",
				() => StageManager.CurrentStage),
			new Member("ready", "bool", "Whether ready for activating next stage or not.",
				() => StageManager.CanSeparate),
		})
		{
			MetaTable["__call"] = new Func<bool>(Activate);
		}
		public override Value Call(IObject self, Arguments args)
			=> Activate();

		public bool Activate()
		{
			if (!HighLogic.LoadedSceneIsFlight)
				throw new InvalidOperationException("Can only activate stages in flight");
			if (!StageManager.CanSeparate)
				return false;
			StageManager.ActivateNextStage();
			return true;
		}
	}
}
