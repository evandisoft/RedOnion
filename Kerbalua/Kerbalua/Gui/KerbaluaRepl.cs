using System;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using Kerbalua.Other;
using Kerbalua.Completion;
using Kerbalua;

namespace Kerbalua.Gui {
	public class KerbaluaRepl {
		MoonSharpReplEvaluator scriptEngine;

		KerbaluaMain.KSPRaw kspApi;
		public ScriptWindow scriptWindow;


		public class FlightControl {
			Vessel vessel;
			DynValue callback;
			MoonSharpReplEvaluator script;

			void FlightCallback(FlightCtrlState st)
			{
				//script.Call(callback, st);
			}

			public FlightControl(Vessel vessel, MoonSharpReplEvaluator script)
			{
				this.vessel = vessel;
				this.script = script;
			}

			public void SetCallback(DynValue callback)
			{
				this.callback = callback;
				vessel.OnFlyByWire += FlightCallback;
			}
		}

		public KerbaluaRepl(KerbaluaMain.KSPRaw kspApi)
		{
			this.kspApi = kspApi;
			scriptEngine = new MoonSharpReplEvaluator();


			//scriptEngine.Globals["ksp"] = kspApi;
			//scriptEngine.Globals["flight"] = new FlightControl(FlightGlobals.ActiveVessel, scriptEngine);

			//scriptEngine.Options.DebugPrint = Print;
			InputLockManager.ClearControlLocks();

			scriptWindow = new ScriptWindow(new Rect(100, 100, 1000, 600));
		}

		public void Print(string str)
		{
			scriptWindow.repl.outputBox.content.text += "\n" + str;
		}

		public void FixedUpdate()
		{
			scriptWindow.FixedUpdate();
		}

		public void Render(bool guiActive)
		{

			if (!guiActive) return;

			try {
				scriptWindow.Update();
			} catch (Exception e) {
				//Debug.Log("yes");
				Debug.Log(e);
			}
		}
	}
}
