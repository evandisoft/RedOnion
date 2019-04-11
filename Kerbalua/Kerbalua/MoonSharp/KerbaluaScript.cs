using System.Runtime.Remoting.Proxies;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.Autopilot;
using UnityEngine;
using RedOnion.KSP.MathUtil;
using System;
using KSP.UI.Screens;
using RedOnion.KSP.Lua.Proxies;
using Kerbalua.Parsing;
using RedOnion.UI;
using System.Reflection;
using System.Globalization;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using API = RedOnion.KSP.API;
using RedOnion.KSP.ReflectionUtil;
using RedOnion.KSP.API;
using static RedOnion.KSP.API.Reflect;

namespace Kerbalua.MoonSharp
{
	public class KspApi
	{
		public FlightControl FlightControl = FlightControl.GetInstance();
		public FlightGlobals FlightGlobals = FlightGlobals.fetch;
		public Time Time = new Time();
		public Mathf Mathf = new Mathf();
		public Scalar Scalar = new Scalar();
		public Vec Vec = new Vec();
		public EditorPanels EditorPanels = EditorPanels.Instance;
		public EditorLogic EditorLogic = EditorLogic.fetch;
		public ShipConstruction ShipConstruction = new ShipConstruction();
		public UnityEngine.Random Random = new UnityEngine.Random();
		public FlightDriver FlightDriver = FlightDriver.fetch;
		public HighLogic HighLogic = HighLogic.fetch;
		public StageManager StageManager = StageManager.Instance;
		public PartLoader PartLoader = PartLoader.Instance;
	}

	public class UI
	{
		public Type Window = typeof(Window);
		public Type Panel = typeof(Panel);
		public Type Button = typeof(Button);
		public Anchors Anchors = new Anchors();
		public Layout Layout = new Layout();
	}

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
			Globals.MetaTable = API.Globals.Instance;
			//Globals["Vessel"] = FlightGlobals.ActiveVessel;
			var allMappings = NamespaceMappings.ForAllAssemblies;
			//Globals["KSP"] = new KspApi();
			Globals["new"] = Constructor.Instance;
			Globals["static"] = new Func<object, DynValue>((o) =>
			{
				if (o is Type t)
				{
					return UserData.CreateStatic(t);
				}
				return UserData.CreateStatic(o.GetType());
			});
			Globals["gettype"] = new Func<object, DynValue>((o) =>
			{
				if (o is DynValue d && d.Type==DataType.UserData)
				{
					return DynValue.FromObject(this,d.UserData.Descriptor.Type);
				}
				if (o is Type t)
				{
					return DynValue.FromObject(this,t);
				}
				return DynValue.FromObject(this,o.GetType());
			});
			Globals["loadedassemblies"] = new Func<Assembly[]>(() =>
			{
				return AppDomain.CurrentDomain.GetAssemblies();
			});

			Globals["assembly"] = new GetMappings();

			Globals["printall"] = DoString(
			@"
				return function(lst) for i=0,lst.Count-1 do print(i..' '..lst[i].ToString()) end end
				");
			//Globals["unity"] = Assembly.GetAssembly(typeof(Vector3));
			//Globals["Assembly"] = typeof(Assembly);
			//Assembly blah;
			Globals["import"] = allMappings.GetNamespace("");
			//Globals["Coll"] = allMappings.GetNamespace("System.Collections.Generic");
			Globals["reldir"] = new Func<double, double, RelativeDirection>((heading, pitch) => new RelativeDirection(heading, pitch));
			//Globals["AppDomain"] = UserData.CreateStatic(typeof(AppDomain));
			//Globals["AssemblyStatic"] = UserData.CreateStatic(typeof(Assembly));
			//Globals["UI"] = new UI();
			//UserData.RegisterExtensionType(typeof(System.Linq.Enumerable));
			//UserData.RegisterAssembly(Assembly.GetAssembly(typeof(System.Linq.Enumerable)),true);
		}

		//private Table ImporterImpl(string name)
		//{
		//	var a=Assembly.GetAssembly(GetType());

		//}

		delegate Table Importer(string name);

		DynValue coroutine;

		public bool Evaluate(out DynValue result)
		{
			if (coroutine == null)
			{
				throw new System.Exception("Coroutine not set in KerbaluaScript");
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
		}
	}
}
