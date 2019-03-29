using KSP.UI.Screens;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;
using RedOnion.Script.ReflectedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.ROS
{
	// see RedOnion.KSP.Lua.Stage
	public class Stage : SimpleObject
	{
		public Stage(IEngine engine)
			: base(engine, new Properties(StdProps)) { }
		public override ObjectFeatures Features => ObjectFeatures.Function;
		public override Value Call(IObject self, int argc)
		{
			if (!HighLogic.LoadedSceneIsFlight)
				throw new InvalidOperationException("Can only activate stages in flight");
			if (!StageManager.CanSeparate)
				return false;
			StageManager.ActivateNextStage();
			return true;
		}

		public static IDictionary<string, Value> StdProps { get; } = new Dictionary<string, Value>()
		{
			{ "number", Value.ReadOnly(stage => StageManager.CurrentStage) }
		};
	}
}
