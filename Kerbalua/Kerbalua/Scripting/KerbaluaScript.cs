using MunSharp.Interpreter;
using MunSharp.Interpreter.Interop;
using System;
using Kerbalua.Parsing;
using UnityEngine.Events;
using System.Diagnostics;
using System.ComponentModel;
using MunSharp.Interpreter.Compatibility;
using RedOnion.UI;
using System.IO;
using MunOS;
using Kerbalua.CommonAPI;

namespace Kerbalua.Scripting
{
	public class KerbaluaScript : Script
	{
		//EVANTODO: This seems to not be needed
		public Action<string> PrintErrorAction { get; set; }

		public readonly MunProcess kerbaluaProcess;

		public const string LuaNew=@"
return function(stat,...)
	if type(stat)~='userdata' then
		error('First argument to `new` must be a type.')
	end
	local args={...}
	if #args>0 then
		return stat.__new(...)
	else
		return stat.__new()
	end
end
";

		public void AddAPI(Type apiType)
		{
			commonAPITable.AddAPI(apiType);
		}

		public CommonAPITable commonAPITable;

		public KerbaluaScript(MunProcess kerbaluaProcess) : base(CoreModules.Preset_Complete)
		{
			this.kerbaluaProcess=kerbaluaProcess;

			var metatable=new Table(this);
			commonAPITable=new CommonAPITable(this);

			metatable["__index"]=commonAPITable;
			Globals.MetaTable=metatable;
			
			Globals.Remove("dofile");
			Globals.Remove("loadfilesafe");
			Globals.Remove("loadfile");

			// This is the simplest way to define "new" to use __new.
			Globals["new"]=DoString(LuaNew);
			
			//var coroutineTable=Globals["coroutine"] as Table;

			//var yield = coroutineTable["yield"];
			//Globals.Remove("coroutine");
			//Globals["sleep"] = yield;
		}
	}
}
