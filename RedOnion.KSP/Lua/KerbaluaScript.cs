using System.Runtime.Remoting.Proxies;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.Autopilot;
using UnityEngine;
using RedOnion.KSP.MathUtil;
using KSP.UI.Screens;
using RedOnion.KSP.Lua.Proxies;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Lua
{
	public class KspApi
	{
		public FlightControl FlightControl = FlightControl.GetInstance();
		public FlightGlobals FlightGlobals = new FlightGlobals();
		public Time Time = new Time();
		public Mathf Mathf = new Mathf();
		public Scalar Scalar = new Scalar();
		public Vec Vec = new Vec();
		public EditorPanels EditorPanels = EditorPanels.Instance;
		public EditorLogic EditorLogic = EditorLogic.fetch;
		public ShipConstruction ShipConstruction = new ShipConstruction();
		public Random Random = new Random();
		public FlightDriver FlightDriver = new FlightDriver();
		public HighLogic HighLogic = new HighLogic();
		public StageManager StageManager = StageManager.Instance;
		public PartLoader PartLoader = PartLoader.Instance;
	}

	public class KerbaluaScript : MoonSharp.Interpreter.Script
	{
		public KerbaluaScript() : base(CoreModules.Preset_Complete)
		{
			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;

			GlobalOptions.CustomConverters
				.SetClrToScriptCustomConversion(
					(MoonSharp.Interpreter.Script script, ModuleControlSurface m)
						=> DynValue.NewTable(new ProxyTable(this, m))
					);
			Globals.MetaTable = API.Globals.Instance;
			//Globals["Vessel"] = FlightGlobals.ActiveVessel;
			Globals["Ksp"] = new KspApi();
		}

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
			if (source.StartsWith("="))
			{
				source = "return " + source.Substring(1);
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
