using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using System;
using RedOnion.KSP.MoonSharp.Proxies;
using Kerbalua.Parsing;
using UnityEngine.Events;
using System.Diagnostics;
using RedOnion.KSP.API;
using RedOnion.KSP.MoonSharp.CommonAPI;
using System.ComponentModel;
using RedOnion.KSP.MoonSharp.MoonSharpAPI;
using MoonSharp.Interpreter.Compatibility;
using RedOnion.UI;
using Kerbalua.Events;
using System.IO;
using RedOnion.KSP.Settings;
using MunOS.ProcessLayer;
using MunOS.Core;

namespace Kerbalua.Scripting
{
	public class KerbaluaScript : Script
	{
		public Action<string> PrintErrorAction { get; set; }

		private int execlimit = defaultExecLimit;
		private const int defaultExecLimit=1000;
		private const int execLimitMin=100;
		private const int execLimitMax=5000;

		static KerbaluaScript()
		{
			GlobalOptions.CustomConverters
				.SetClrToScriptCustomConversion(
					(Script script, Vector3d vector3d)
						=> DynValue.FromObject(script, new Vector(vector3d)) //DynValue.NewTable(new ModuleControlSurfaceProxyTable(this, m))
					);
			UserData.RegisterType<Button>(new LuaDescriptor(typeof(Button)));

			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;

			GlobalOptions.CustomConverters
				.SetScriptToClrCustomConversion(DataType.Function, typeof(Action<Button>), (f) =>
				{
					return new Action<Button>((button) =>
					{
						var closure=f.Function;
						var kerbaluaScript=closure.OwnerScript as KerbaluaScript;
						if (kerbaluaScript==null)
						{
							throw new Exception("Ownerscript was not KerbaluaScript in LuaEventDescriptor");
						}

						var process=kerbaluaScript?.kerbaluaProcess;
						if (process==null)
						{
							throw new Exception("Could not get current process in LuaEventDescriptor");
						}

						process.Execute(MunOS.Core.ExecPriority.ONESHOT, closure);
					});
				});
		}

		public readonly KerbaluaProcess kerbaluaProcess;

		public const string LuaNew=@"
return function(stat,...) 
	if type(stat)~='userdata' then
		error('First argument to `new` must be a CLR Static Class')
	end
	local args={...}
	if #args>0 then
		return stat.__new(...)
	else
		return stat.__new()
	end
end
";

		public KerbaluaScript(KerbaluaProcess kerbaluaProcess) : base(CoreModules.Preset_Complete)
		{
			this.kerbaluaProcess=kerbaluaProcess;

			var metatable=new Table(this);
			var commonAPI=new CommonAPITable(this);
			commonAPI.AddAPI(typeof(Globals));
			commonAPI.AddAPI(typeof(MoonSharpGlobals));

			metatable["__index"]=commonAPI;
			Globals.MetaTable=metatable;

			Globals.Remove("dofile");
			//Globals.Remove("load");
			Globals.Remove("loadfilesafe");
			Globals.Remove("loadfile");
			//Globals.Remove("loadsafe");

			// This is the simplest way to define "new" to use __new.
			commonAPI["new"]=DoString(LuaNew);

			//commonAPI["dofile"]=new Func<string, DynValue>(dofile);

			//commonAPI["new"]=new newdel(@new);

			var coroutineTable=Globals["coroutine"] as Table;

			var yield = coroutineTable["yield"];//new Action<double>(sleep);
			Globals.Remove("coroutine");
			//commonAPI["setexeclimit"] = new Action<double>(setexeclimit);

			commonAPI["sleep"] = yield;
		}

		public void setexeclimit(double counterlimit)
		{
			try
			{
				execlimit=(int)Math.Max(execLimitMin, Math.Min(execLimitMax, counterlimit));
			}
			catch (Exception e)
			{
				PrintErrorAction?.Invoke(e.Message);
			}
		}

		[Description("Cause the script to sleep for waittimeSeconds seconds.")]
		public void sleep(double waittimeSeconds)
		{
			sleeptimeMillis=waittimeSeconds*1000;
			sleepwatch.Start();
			coroutine.Coroutine.AutoYieldCounter=0;
		}

		double sleeptimeMillis=0;
		Stopwatch sleepwatch=new Stopwatch();
		delegate Table Importer(string name);
		DynValue coroutine;
		//Process process;


		public bool Evaluate(out DynValue result)
		{
			if (coroutine == null)
			{
				throw new System.Exception("Coroutine not set in KerbaluaScript");
			}

			if (sleepwatch.IsRunning)
			{
				if (sleepwatch.ElapsedMilliseconds>sleeptimeMillis)
				{
					sleeptimeMillis=0;
					sleepwatch.Reset();
				}
				else
				{
					result=null;
					return false;
				}
			}

			coroutine.Coroutine.AutoYieldCounter = execlimit;
			result = coroutine.Coroutine.Resume();

			bool isComplete = false;
			if (coroutine.Coroutine.State == CoroutineState.Dead)
			{
				isComplete = true;
				coroutine = null;
			}

			return isComplete;
		}

		public void SetCoroutine(string source)
		{
			if (IncompleteLuaParsing.IsImplicitReturn(source))
			{
				source = "return " + source;
			}
			DynValue mainFunction = base.DoString("return function () " + source + "\n end");

			coroutine = CreateCoroutine(mainFunction);
		}

		public void Terminate()
		{
			coroutine = null;
			sleepwatch.Reset();
			sleeptimeMillis=0;
		}
	}
}
