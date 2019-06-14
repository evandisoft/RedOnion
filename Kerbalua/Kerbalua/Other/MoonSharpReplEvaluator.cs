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
			scriptEngine.PrintErrorAction = (str)=>PrintErrorAction?.Invoke(str);
			
			scriptEngine.Options.ScriptLoader = new FileSystemScriptLoader();
			((ScriptLoaderBase)scriptEngine.Options.ScriptLoader).IgnoreLuaPathGlobal = true;
			((ScriptLoaderBase)scriptEngine.Options.ScriptLoader).ModulePaths = new string[] { Settings.BaseScriptsPath+"/?.lua" };
		}

		public override void ResetEngine()
		{
			InternalResetEngine();
			base.ResetEngine();
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

					if (dynResult.Type==DataType.String)
					{
						result = "\"" + dynResult.ToObject() + "\"";
					}
					else if (dynResult.Type == DataType.Nil || dynResult.Type== DataType.Void)
					{
						result = dynResult.ToString();
					}
					else
					{
						result += dynResult.ToObject().ToString();
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

		protected override void ProtectedSetSource(string source, string path)
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

		public override void FixedUpdate()
		{
			// do nothing, LUA/MoonSharp Engine does not use events
		}
	}
}
