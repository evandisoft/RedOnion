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
		SimpleScript scriptEngine;

		KerbaluaMain.KSPRaw kspApi;
		ScriptWindow scriptWindow;


		public class FlightControl {
			Vessel vessel;
			DynValue callback;
			SimpleScript script;

			void FlightCallback(FlightCtrlState st)
			{
				script.Call(callback, st);
			}

			public FlightControl(Vessel vessel, SimpleScript script)
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
			scriptEngine = new SimpleScript(CoreModules.Preset_Complete);
			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;

			scriptEngine.Globals["ksp"] = kspApi;
			scriptEngine.Globals["flight"] = new FlightControl(FlightGlobals.ActiveVessel, scriptEngine);

			scriptEngine.Options.DebugPrint = Print;
			InputLockManager.ClearControlLocks();

			scriptWindow = new ScriptWindow(scriptEngine, new Rect(500, 100, 400, 500));
		}

		public void Print(string str)
		{
			scriptWindow.repl.outputBox.content.text += Environment.NewLine + str;
		}

		public void Render(bool guiActive)
		{
			if (!guiActive) return;

			try {
				scriptWindow.Render();
			} catch (Exception e) {
				Debug.Log(e);
			}
		}
	}
}
