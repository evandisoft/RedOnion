using System;
using System.Collections.Generic;
using RedOnion.ROS;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace RedOnion.KSP.API
{
	public abstract class FunctionBase : Descriptor, IUserDataType
	{
		public FunctionBase(string name = "interop function", Type type = null)
			: base(name, type ?? typeof(FunctionBase)) { }

		public virtual DynValue MetaIndex(Script script, string metaname)
			=> metaname == "__call" ? DynValue.FromObject(script, new CallbackFunction(Call)) : null;

		public virtual DynValue Call(ScriptExecutionContext ctx, CallbackArguments args)
		{
			var result = Value.Void;
			var self = args.ToRos(out var ros);
			return Call(ref result, self, ros, false) ? result.ToLua() : DynValue.Void;
		}
		DynValue IUserDataType.Index(Script script, DynValue index, bool isDirectIndexing)
			=> null;
		bool IUserDataType.SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
			=> false;
	}
}
