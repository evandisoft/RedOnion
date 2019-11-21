using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.Autopilot;
using UnityEngine;
using RedOnion.KSP.MathUtil;
using System;
using KSP.UI.Screens;
using RedOnion.KSP.MoonSharp.Proxies;
using Kerbalua.Parsing;
using RedOnion.UI;
using System.Reflection;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using API = RedOnion.KSP.API;
using RedOnion.KSP.ReflectionUtil;
using System.Diagnostics;
using RedOnion.KSP.API;
using System.Linq.Expressions;
using RedOnion.KSP.MoonSharp.CommonAPI;
using System.ComponentModel;
using RedOnion.KSP.MoonSharp.MoonSharpAPI;

namespace Kerbalua.MoonSharp
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
			//GlobalOptions.CustomConverters
				//.SetScriptToClrCustomConversion(DataType.UserData, typeof(Vector3d), (v) =>
				//{
				//	object obj=v.ToObject();
				//	if (obj is Vector vector)
				//	{
				//		return new Vector3d(vector.x, vector.y, vector.z);
				//	}
				//	throw new Exception("Can't convert from "+obj.GetType()+", to "+typeof(Vector3d));
				//});
			GlobalOptions.CustomConverters
				.SetScriptToClrCustomConversion(DataType.Function
					, typeof(Func<object, object>), (f) => new Func<object, object>((item) =>
					 {
						 var co = CreateCoroutine(f);
						 co.Coroutine.AutoYieldCounter = 10000;
						 object retval=co.Coroutine.Resume(item);
						 if (co.Coroutine.State == CoroutineState.ForceSuspended)
						 {
							 PrintErrorAction?.Invoke("Action<object> callback unable to finish");
							 return null;
						 }
						 return retval;
					 }));
			GlobalOptions.CustomConverters
				.SetScriptToClrCustomConversion(DataType.Function
					, typeof(System.Action<object>), (f) => new Action<object>((item) =>
				{

					var co = CreateCoroutine(f);
					co.Coroutine.AutoYieldCounter = 10000;
					co.Coroutine.Resume(item);
					if (co.Coroutine.State == CoroutineState.ForceSuspended)
					{
						PrintErrorAction?.Invoke("Action<object> callback unable to finish");
					}
				}));
			GlobalOptions.CustomConverters
				.SetScriptToClrCustomConversion(DataType.Function
					, typeof(UnityAction), (f) => new UnityAction(() =>
				{
					var co = CreateCoroutine(f);
					co.Coroutine.AutoYieldCounter = 10000;
					co.Coroutine.Resume();
					if (co.Coroutine.State == CoroutineState.ForceSuspended)
					{
						PrintErrorAction?.Invoke("UnityAction callback unable to finish");
					}
				}));
			//Globals.MetaTable = API.LuaGlobals.Instance;
			//Globals["Vessel"] = FlightGlobals.ActiveVessel;
			var metatable=new Table(this);
			var commonAPI=new CommonAPITable(this);
			commonAPI.AddAPI(typeof(Globals));
			commonAPI.AddAPI(typeof(MoonSharpGlobals));

			metatable["__index"]=commonAPI;
			Globals.MetaTable=metatable;
			Globals.Remove("coroutine");
			
			//var defaultMappings = NamespaceMappings.DefaultAssemblies;
			//commonAPI["new"] = new DelegateTypeNew(@new);

			//var reflection=new Table(this);
			//commonAPI["reflection"] = reflection;

			//reflection["getstatic"] = new Func<DynValue, DynValue>(getstatic);

			//reflection["isstatic"] = new Func<DynValue, DynValue>(isstatic);

			//reflection["isclrtype"] = new Func<DynValue, DynValue>(isclrtype);

			//reflection["getclrtype"] = new Func<DynValue, DynValue>(getclrtype);

			commonAPI["sleep"] = new Action<double>(sleep);

		}



		public int TestFunc(int a,double b,out string outstring)
		{
			outstring=a+","+b;
			return (int)(a + b);
		}

		//private Table ImporterImpl(string name)
		//{
		//	var a=Assembly.GetAssembly(GetType());

		//}
		double sleeptimeMillis=0;
		Stopwatch sleepwatch=new Stopwatch();
		delegate Table Importer(string name);

		//Stack<DynValue> coroutineStack=new Stack<DynValue>();
		DynValue coroutine;

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

			coroutine.Coroutine.AutoYieldCounter = 1000;
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
			DynValue mainFunction = base.DoString("return function () " + source + " end");

			coroutine = CreateCoroutine(mainFunction);
		}

		public void Terminate()
		{
			coroutine = null;
			sleepwatch.Reset();
			sleeptimeMillis=0;
		}

		[Description("Cause the script to sleep for waittimeSeconds seconds.")]
		public void sleep(double waittimeSeconds)
		{
			sleeptimeMillis=waittimeSeconds*1000;
			sleepwatch.Start();
			coroutine.Coroutine.AutoYieldCounter=0;
		}
	}
}
