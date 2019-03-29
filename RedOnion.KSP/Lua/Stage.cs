using System;
using KSP.UI.Screens;
using MoonSharp.Interpreter;


namespace RedOnion.KSP.Lua
{
	// see RedOnion.KSP.ROS.Stage
	public class Stage : Table
	{
		public Stage(MoonSharp.Interpreter.Script script) : base(script)
		{
			var metatable = new Table(script);
			metatable["__call"] = new Func<Table, DynValue[], DynValue>(Activate);
			metatable["__index"] = new Func<Table, DynValue, DynValue>(IndexFunc);
			MetaTable = metatable;
		}

		DynValue Activate(Table self, params DynValue[] args)
		{
			if (!HighLogic.LoadedSceneIsFlight)
				throw new InvalidOperationException("Can only activate stages in flight");
			if (!StageManager.CanSeparate)
				return DynValue.False;
			StageManager.ActivateNextStage();
			return DynValue.True;
		}

		DynValue IndexFunc(Table table, DynValue key)
		{
			switch (key.String)
			{
			case "number":
				return DynValue.FromObject(table.OwnerScript, StageManager.CurrentStage);
			}
			return DynValue.Nil;
		}
	}
}
