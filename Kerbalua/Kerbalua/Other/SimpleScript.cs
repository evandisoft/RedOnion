using System;
using UnityEngine;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.Interpreter.Interop;

namespace Kerbalua.Other {
	public class SimpleScript:Script {
		public SimpleScript(CoreModules coreModules) : base(coreModules)
		{
		}

		DynValue coroutine;
		public bool EvaluateWithCoroutine(string source, out DynValue result)
		{
			if (coroutine == null) {
				SetCoroutine(source);
			} 
			result = coroutine.Coroutine.Resume();
			//Debug.Log("result is " + result);
			//if (coroutine.Coroutine.State == CoroutineState.ForceSuspended) {
			//	Globals["suspended"] = result;
			//}

			bool isComplete = false;
			if (coroutine.Coroutine.State == CoroutineState.Dead) {
				//Debug.Log("It's dead jim " + result);

				//Debug.Log("Coroutine state is" + coroutine.Coroutine.State + ", result is " + result);
				isComplete = true;
				coroutine = null;
			} else {
				//Debug.Log("Coroutine state is" + coroutine.Coroutine.State + ", result is incomplete");
			}

			//DynValue result = base.DoString("return " + str);
			return isComplete;
		}

		void SetCoroutine(string source)
		{
			try {
				if (source.StartsWith("=")) {
					source = "return " + source.Substring(1);
				}
				DynValue mainFunction = base.DoString("return function () " + source + " end");
				//Debug.Log("Main function is "+mainFunction);
				coroutine = CreateCoroutine(mainFunction);
				//Debug.Log("Coroutine is " + coroutine);
			}
			catch(SyntaxErrorException e) {
				//Doesn't work in general
				//DynValue mainFunction = base.DoString("return function () return " + source + " end");
				//Debug.Log("Main function is "+mainFunction);
				//coroutine = CreateCoroutine(mainFunction);
				//Debug.Log(e);
				//Debug.Log("Coroutine is " + coroutine);
				throw e;
			}
			coroutine.Coroutine.AutoYieldCounter = 1000;
		}

		public void Terminate()
		{
			coroutine = null;
		}
	}
}
