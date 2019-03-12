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

		protected override string ProtectedEvaluate(string source)
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

		/// <summary>
		/// TODO: NOT CURRENTLY IMPLEMENTED
		/// </summary>
		/// <returns>The completions.</returns>
		/// <param name="source">Source.</param>
		/// <param name="cursorPos">Cursor position.</param>
		public override List<string> GetCompletions(string source, int cursorPos)
		{
			List<string> NOT_IMPLEMENTED_COMPLETIONS = new List<string>();
			NOT_IMPLEMENTED_COMPLETIONS.Add("MoonSharp");
			NOT_IMPLEMENTED_COMPLETIONS.Add("intellisense");
			NOT_IMPLEMENTED_COMPLETIONS.Add("is");
			NOT_IMPLEMENTED_COMPLETIONS.Add("not");
			NOT_IMPLEMENTED_COMPLETIONS.Add("currently");
			NOT_IMPLEMENTED_COMPLETIONS.Add("implemented");
			for(int i = 0;i < 100;i++) {
				NOT_IMPLEMENTED_COMPLETIONS.Add("test-string #"+i);
			}
			return NOT_IMPLEMENTED_COMPLETIONS;
		}

		/// <summary>
		/// TODO: NOT CURRENTLY IMPLEMENTED
		/// </summary>
		/// <returns>The partial completion.</returns>
		/// <param name="source">Source.</param>
		/// <param name="cursorPos">Cursor position.</param>
		public override string GetPartialCompletion(string source, int cursorPos)
		{
			return "";
		}

		public override void ResetEngine()
		{
			scriptEngine = new SimpleScript(coreModules);
		}
	}
}
