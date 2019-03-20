using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.Interpreter.Interop;
using UnityEngine;
//using RedOnion.Script;
using Kerbalua.MoonSharp;
using Kerbalua.Completion;
using MoonSharp.Interpreter.Loaders;
using Kerbalua.Utility;

namespace Kerbalua.Other
{
    public class MoonSharpReplEvaluator:ReplEvaluator
    {
		SimpleScript scriptEngine;

		CoreModules coreModules;
		KerbaluaExecutionManager kem = new KerbaluaExecutionManager();

		public MoonSharpReplEvaluator(CoreModules coreModules)
		{
			this.coreModules = coreModules;
			InternalResetEngine();
			//scriptEngine = new SimpleScript(coreModules);
		}

		protected override bool ProtectedEvaluate(string source,out string output)
		{
			output="";
			DynValue result;
			bool isComplete = false;
			try {
				if(scriptEngine.EvaluateWithCoroutine(source,out result)) {
					isComplete = true;

					if (result.UserData == null) {
						output += result;
					} else {
						output += result.UserData.Object;
						if (result.UserData.Object == null) {
							output += " (" + result.UserData.Object.GetType() + ")";
						}
					}

				} else {
					output = "";
				}

			} catch (Exception exception) {
				Debug.Log(exception);
				Terminate();
				isComplete = true;
			}

			return isComplete;
		}

		/// <summary>
		/// See the abstract version for complete comments.
		/// </summary>
		public override IList<string> GetCompletions(string source, int cursorPos,out int replaceStart,out int replaceEnd)
		{
			try {
				return LuaIntellisense.GetCompletions(scriptEngine.Globals, source, cursorPos, out replaceStart, out replaceEnd);
			} catch (Exception e) {
				Debug.Log(e);
				replaceStart = replaceEnd = cursorPos;
				return new List<string>();
			}
		}


		void InternalResetEngine()
		{
			scriptEngine = new SimpleScript(coreModules);
			scriptEngine.Options.DebugPrint = (string str) => {
				PrintAction?.Invoke(str);
			};
			scriptEngine.Options.ScriptLoader = new FileSystemScriptLoader();
			((ScriptLoaderBase)scriptEngine.Options.ScriptLoader).IgnoreLuaPathGlobal = true;
			((ScriptLoaderBase)scriptEngine.Options.ScriptLoader).ModulePaths = new string[] { Settings.BaseScriptsPath+"/?.lua" };

			//scriptEngine.AttachDebugger(kem);
		}

		public override void ResetEngine()
		{
			InternalResetEngine();
		}

		public override void Terminate()
		{
			scriptEngine.Terminate();
		}
	}
}
