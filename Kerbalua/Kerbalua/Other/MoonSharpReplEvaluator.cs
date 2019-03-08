using System;
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

		public MoonSharpReplEvaluator(CoreModules coreModules)
		{
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
	}
}
