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
using Process = RedOnion.KSP.OS.Process;
using RedOnion.KSP.MoonSharp.Descriptors;

namespace Kerbalua.Scripting
{
	public class KerbaluaScript : Script
	{
		public Action<string> PrintErrorAction { get; set; }

		public KerbaluaScript() : base(CoreModules.Preset_Complete)
		{
			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;

			GlobalOptions.CustomConverters
				.SetClrToScriptCustomConversion(
					(Script script, ModuleControlSurface m)
						=> DynValue.FromObject(script, new LuaProxy(m)) //DynValue.NewTable(new ModuleControlSurfaceProxyTable(this, m))
					);

			GlobalOptions.CustomConverters
				.SetClrToScriptCustomConversion(
					(Script script, Vector3d vector3d)
						=> DynValue.FromObject(script, new Vector(vector3d)) //DynValue.NewTable(new ModuleControlSurfaceProxyTable(this, m))
					);

			// This stuff is unsafe. User could pass a callback to List.Foreach and think it would run normally
			// but the overall execution of the callback on every item of the list would have to complete before
			// the game could return from fixed-update.
			//GlobalOptions.CustomConverters
			//	.SetScriptToClrCustomConversion(DataType.Function
			//		, typeof(Func<object, object>), (f) => new Func<object, object>((item) =>
			//		 {
			//			 var co = CreateCoroutine(f);
			//			 co.Coroutine.AutoYieldCounter = 10000;
			//			 object retval=co.Coroutine.Resume(item);
			//			 if (co.Coroutine.State == CoroutineState.ForceSuspended)
			//			 {
			//				 PrintErrorAction?.Invoke("Action<object> callback unable to finish");
			//				 return null;
			//			 }
			//			 return retval;
			//		 }));

			//GlobalOptions.CustomConverters
			//	.SetScriptToClrCustomConversion(DataType.Function
			//		, typeof(System.Action<object>), (f) => new Action<object>((item) =>
			//	{

			//		var co = CreateCoroutine(f);
			//		co.Coroutine.AutoYieldCounter = 10000;
			//		co.Coroutine.Resume(item);
			//		if (co.Coroutine.State == CoroutineState.ForceSuspended)
			//		{
			//			PrintErrorAction?.Invoke("Action<object> callback unable to finish");
			//		}
			//	}));

			//GlobalOptions.CustomConverters
			//.SetScriptToClrCustomConversion(DataType.Function
			//	, typeof(UnityAction), (f) => new UnityAction(() =>
			//{
			//	var co = CreateCoroutine(f);
			//	co.Coroutine.AutoYieldCounter = 10000;
			//	co.Coroutine.Resume();
			//	if (co.Coroutine.State == CoroutineState.ForceSuspended)
			//	{
			//		PrintErrorAction?.Invoke("UnityAction callback unable to finish");
			//	}
			//}));

			var metatable=new Table(this);
			var commonAPI=new CommonAPITable(this);
			commonAPI.AddAPI(typeof(Globals));
			commonAPI.AddAPI(typeof(MoonSharpGlobals));

			metatable["__index"]=commonAPI;
			Globals.MetaTable=metatable;
			Globals.Remove("coroutine");

			commonAPI["sleep"] = new Action<double>(sleep);
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
			coroutine.Coroutine.AutoYieldCounter = 1000;
			result = coroutine.Coroutine.Resume();
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
			process = new Process();
			process.shutdown += Terminate;
		}

		public void Terminate()
		{
			coroutine = null;
			process.shutdown -= Terminate;
			process.terminate();
			process = null;
			sleepwatch.Reset();
			sleeptimeMillis=0;
		}
	}
}
