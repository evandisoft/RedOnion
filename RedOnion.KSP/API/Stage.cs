using System;
using RedOnion.Script;
using KSP.UI.Screens;
using MoonSharp.Interpreter;

//NOTE: Never move Instance above MemberList ;)

namespace RedOnion.KSP.API
{
	public class Stage : InteropObject
	{
		public static MemberList MemberList { get; } = new MemberList(
		ObjectFeatures.Function,

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
		public Stage() : base(MemberList) { }

		public override Value Call(IObject self, Arguments args)
			=> Activate();
		public override DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
			=> DynValue.NewBoolean(Activate());

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
