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
using Process = RedOnion.KSP.MunOS.Process;
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

		public readonly KerbaluaProcess kerbaluaProcess;

		public KerbaluaScript(KerbaluaProcess kerbaluaProcess) : base(CoreModules.Preset_Complete)
		{
			this.kerbaluaProcess=kerbaluaProcess;

			UserData.RegisterType<Button>(new LuaDescriptor(typeof(Button)));

			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;

			//GlobalOptions.CustomConverters
				//.SetClrToScriptCustomConversion(
					//(Script script, ModuleControlSurface m)
					//	=> DynValue.FromObject(script, new LuaProxy(m)) //DynValue.NewTable(new ModuleControlSurfaceProxyTable(this, m))
					//);

			GlobalOptions.CustomConverters
				.SetClrToScriptCustomConversion(
					(Script script, Vector3d vector3d)
						=> DynValue.FromObject(script, new Vector(vector3d)) //DynValue.NewTable(new ModuleControlSurfaceProxyTable(this, m))
					);

			GlobalOptions.CustomConverters
				.SetScriptToClrCustomConversion(DataType.Function, typeof(Action<Button>), (f) =>
				  {
					  var currentProcess=MunThread.ExecutingThread?.parentProcess as KerbaluaProcess;
					  if (currentProcess==null)
					  {
						  throw new Exception("Could not get current process in KerbaluaScript custom converter");
					  }
					  return new Action<Button>((button) =>
					  {
						  currentProcess.ExecuteFunctionInThread(ExecPriority.ONESHOT,f.Function);
						  //var script=this;
						  //var co = script.CreateCoroutine(f);
						  //co.Coroutine.AutoYieldCounter = 1000;
						  //co.Coroutine.Resume();
						  //if (co.Coroutine.State == CoroutineState.ForceSuspended)
						  //{
						  // script.PrintErrorAction?.Invoke("functions called in buttons must have a short runtime");
						  //}
					  });
				  });


			//UnityEngine.Debug.Log("sanity check");
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
			commonAPI["new"]=DoString(@"
return function(stat,...) 
	if type(stat)~='userdata' then
		error('First argument to `new` must be a CLR Static Class')
	end
	return stat.__new(...) 
end
");

			//commonAPI["dofile"]=new Func<string, DynValue>(dofile);

			//commonAPI["new"]=new newdel(@new);

			var coroutineTable=Globals["coroutine"] as Table;

			var yield = coroutineTable["yield"];//new Action<double>(sleep);
			Globals.Remove("coroutine");
			//commonAPI["setexeclimit"] = new Action<double>(setexeclimit);

			commonAPI["sleep"] = yield;
		}




		//DynValue dofile(string filename)
		//{
		//	return DoFile(Path.Combine(ProjectSettings.BaseScriptsDir, filename));
		//}

		//delegate object newdel(object obj, params DynValue[] args);
		//object @new(object typeStaticOrObject, params DynValue[] args)
		//{
		//	Type type=typeStaticOrObject as Type;
		//	if (type==null)
		//	{
		//		type=typeStaticOrObject.GetType();
		//	}


		//}

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
		Process process;


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

			Process.current = process;
			coroutine.Coroutine.AutoYieldCounter = execlimit;
			result = coroutine.Coroutine.Resume();
			process.UpdatePhysics();
			Process.current = null;

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
			if (process == null)
			{
				process = new Process();
				process.shutdown += Terminate;
			}
		}

		public void Terminate()
		{
			coroutine = null;

			// If SetCoroutine has an error, it is caught in MoonSharpReplEvaluator, which
			// calls Terminate, and this means that process could, in that situation be null.
			if (process!=null) 
			{
				process.shutdown -= Terminate;
				process.terminate();
				process = null;
			}
			sleepwatch.Reset();
			sleeptimeMillis=0;
		}
	}
}
