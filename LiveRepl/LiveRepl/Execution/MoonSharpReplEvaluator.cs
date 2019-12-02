using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;
using Kerbalua.Scripting;
using Kerbalua.Completion;
using MoonSharp.Interpreter.Loaders;
using RedOnion.KSP.MoonSharp;
using RedOnion.KSP.Autopilot;
using RedOnion.KSP.Settings;
using System.IO;

namespace LiveRepl.Execution
{
    public class MoonSharpReplEvaluator:ReplEvaluator
    {
		KerbaluaScript scriptEngine;

		public override string Extension => ".lua";

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
				return MoonSharpIntellisense.GetCompletions(scriptEngine.Globals, source, cursorPos, out replaceStart, out replaceEnd);
			} catch (Exception e) {
				Debug.Log(e);
				replaceStart = replaceEnd = cursorPos;
				return new List<string>();
			}
		}


		void InternalResetEngine()
		{
			KerbaluaScript.Initialize();
			scriptEngine = KerbaluaScript.Instance;
			scriptEngine.Options.DebugPrint = (string str) => {
				PrintAction?.Invoke(str);
			};
			scriptEngine.PrintErrorAction = (str)=>PrintErrorAction?.Invoke(str);
			
			scriptEngine.Options.ScriptLoader = new FileSystemScriptLoader();
			((ScriptLoaderBase)scriptEngine.Options.ScriptLoader).IgnoreLuaPathGlobal = true;
			((ScriptLoaderBase)scriptEngine.Options.ScriptLoader).ModulePaths = new string[] { SavedSettings.BaseScriptsPath+"/?.lua" };
		
		 }

		public override void ResetEngine()
		{
			Terminate();
			InternalResetEngine();
			base.ResetEngine();
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
						return isComplete;
					}
					else if (dynResult.Type == DataType.Nil || dynResult.Type== DataType.Void)
					{
						result = dynResult.ToString();
						return isComplete;
					}

					result += dynResult.ToObject().ToString();

					if (dynResult.UserData==null)
					{
						return isComplete;
					}

					// This is a static.
					if (dynResult.UserData.Object==null)
					{
						return isComplete;
					}

					// This is a type
					if (dynResult.ToObject() is Type)
					{
						result += " (runtime type)";
						return isComplete;
					}

					return isComplete;
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

		public override IList<string> GetDisplayableCompletions(string source, int cursorPos, out int replaceStart, out int replaceEnd)
		{

			return GetCompletions(source, cursorPos, out replaceStart, out replaceEnd);
		}

		public override string GetImportString(string scriptname)
		{
			string basename = Path.GetFileNameWithoutExtension(scriptname);

			return "require(\""+basename+"\")";
		}
	}
}
