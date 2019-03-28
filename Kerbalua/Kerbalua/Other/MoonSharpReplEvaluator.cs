using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;
using Kerbalua.MoonSharp;
using Kerbalua.Completion;
using MoonSharp.Interpreter.Loaders;
using Kerbalua.Utility;
using RedOnion.KSP.Lua;
using RedOnion.KSP.Autopilot;

namespace Kerbalua.Other
{
    public class MoonSharpReplEvaluator:ReplEvaluator
    {
		KerbaluaScript scriptEngine;

		KerbaluaExecutionManager kem = new KerbaluaExecutionManager();

		public MoonSharpReplEvaluator()
		{
			InternalResetEngine();
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
			scriptEngine = new KerbaluaScript();
			scriptEngine.Options.DebugPrint = (string str) => {
				PrintAction?.Invoke(str);
			};
			
			scriptEngine.Options.ScriptLoader = new FileSystemScriptLoader();
			((ScriptLoaderBase)scriptEngine.Options.ScriptLoader).IgnoreLuaPathGlobal = true;
			((ScriptLoaderBase)scriptEngine.Options.ScriptLoader).ModulePaths = new string[] { Settings.BaseScriptsPath+"/?.lua" };
		}

		public override void ResetEngine()
		{
			InternalResetEngine();
			FlightControl.GetInstance().Shutdown();
			Terminate();
		}

		public override void Terminate()
		{
			scriptEngine.Terminate();
		}

		public override bool Evaluate(out string result)
		{
			result = "";
			DynValue dynResult;
			bool isComplete = false;
			try
			{
				if (scriptEngine.Evaluate(out dynResult))
				{
					isComplete = true;

					if (dynResult.UserData == null)
					{
						result += dynResult;
					}
					else
					{
						result += dynResult.UserData.Object;
						if (dynResult.UserData.Object == null)
						{
							result += " (" + dynResult.UserData.Object.GetType() + ")";
						}
					}

				}
				else
				{
					result = "";
				}

			}
			catch (Exception exception)
			{
				if (exception is InterpreterException interExcept)
				{
					PrintErrorAction?.Invoke(interExcept.DecoratedMessage);
				}
				else
				{
					PrintErrorAction?.Invoke(exception.Message);
				}

				Debug.Log(exception);
				Terminate();
				isComplete = true;
			}

			return isComplete;
		}

		public override void ProtectedSetSource(string source)
		{
			try
			{
				scriptEngine.SetCoroutine(source);
			}
			catch (Exception exception)
			{
				if (exception is InterpreterException interExcept)
				{
					PrintErrorAction?.Invoke(interExcept.DecoratedMessage);
				}
				else
				{
					PrintErrorAction?.Invoke(exception.Message);
				}

				Debug.Log(exception);
				Terminate();
			}
		}
	}
}
