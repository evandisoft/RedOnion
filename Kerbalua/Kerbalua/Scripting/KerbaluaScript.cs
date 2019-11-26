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
using MoonSharp.Interpreter.Compatibility;
using RedOnion.UI;
using Kerbalua.Events;

namespace Kerbalua.Scripting
{
	public class KerbaluaScript : Script
	{
		public Action<string> PrintErrorAction { get; set; }

		private int execlimit = defaultExecLimit;
		private const int defaultExecLimit=1000;
		private const int execLimitMin=100;
		private const int execLimitMax=5000;
		static KerbaluaScript _instance;
		public static KerbaluaScript Instance
		{
			get
			{
				if (_instance==null)
				{
					_instance=new KerbaluaScript();
				}
				return _instance;
			}
		}


		private KerbaluaScript() : base(CoreModules.Preset_Complete)
		{
			UserData.RegisterType<Button>(new LuaDescriptor(typeof(Button)));

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

			GlobalOptions.CustomConverters
				.SetScriptToClrCustomConversion(DataType.Function, typeof(Action<Button>), (f) =>
				  {
					  return new Action<Button>((button) =>
					  {
						  var script=Instance;
						  var co = script.CreateCoroutine(f);
						  co.Coroutine.AutoYieldCounter = 1000;
						  co.Coroutine.Resume();
						  if (co.Coroutine.State == CoroutineState.ForceSuspended)
						  {
							  script.PrintErrorAction?.Invoke("functions called in buttons cannot be long");
						  }
					  });
				  });

			//UnityEngine.Debug.Log("sanity check");
			var metatable=new Table(this);
			var commonAPI=new CommonAPITable(this);
			commonAPI.AddAPI(typeof(Globals));
			commonAPI.AddAPI(typeof(MoonSharpGlobals));

			metatable["__index"]=commonAPI;
			Globals.MetaTable=metatable;
			Globals.Remove("coroutine");

			commonAPI["sleep"] = new Action<double>(sleep);
			//commonAPI["setexeclimit"] = new Action<double>(setexeclimit);
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
