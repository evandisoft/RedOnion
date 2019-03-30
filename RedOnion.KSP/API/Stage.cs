using System;
using RedOnion.Script;
using KSP.UI.Screens;
using MoonSharp.Interpreter;

namespace RedOnion.KSP.API
{
	public class Stage : InteropObject
	{
		public static Stage Instance { get; } = new Stage();

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

		public Stage() : base(ObjectFeatures.Function, MemberList)
		{ }

		public override Value Call(IObject self, Arguments args)
			=> Activate();
		public override DynValue MetaIndex(MoonSharp.Interpreter.Script script, string metaname)
		{
			if (metaname != "__call") return null;
			return DynValue.FromObject(script, new Func<bool>(Activate));
		}

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
