using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.Interpreter.Interop;
using UnityEngine;
using RedOnion.Script;

namespace Kerbalua.Other
{
    public class MoonSharpReplEvaluator:ReplEvaluator
    {
		SimpleScript scriptEngine;
		CoreModules coreModules;

		public MoonSharpReplEvaluator(CoreModules coreModules)
		{
			this.coreModules = coreModules;
			scriptEngine = new SimpleScript(coreModules);
		}

		public override string Evaluate(string source)
		{
			string output="";
			DynValue result;
			try {
				result = scriptEngine.DoString(source);
				output= "\n";
				if (result.UserData == null) {
					output += result;
				} else {
					output += result.UserData.Object;
					if (result.UserData.Object == null) {
						output += " (" + result.UserData.Object.GetType() + ")";
					}
				}
			} catch (Exception exception) {
				Debug.Log(exception);
			}

			return output;
		}

		public override List<string> GetCompletions(string source, int cursorPos)
		{
			throw new NotImplementedException();
		}

		public override string GetPartialCompletion(string source, int cursorPos)
		{
			throw new NotImplementedException();
		}

		public override void ResetEngine()
		{
			scriptEngine = new SimpleScript(coreModules);
		}
	}
}
