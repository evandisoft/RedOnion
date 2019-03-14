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

		protected bool markedForTermination = false;
		DynValue coroutine;
		public bool EvaluateWithCoroutine(string str, out DynValue result)
		{
			if (markedForTermination) {
				markedForTermination = false;
				coroutine = null;
				result = DynValue.NewString("(Execution Terminated)");
				
				return true;
			}


			try {
				if (coroutine == null) {
					DynValue mainFunction = base.DoString("return function () " + str + " end");
					//Debug.Log("Main function is "+mainFunction);
					coroutine = CreateCoroutine(mainFunction);
					//Debug.Log("Coroutine is " + coroutine);
					coroutine.Coroutine.AutoYieldCounter = 1000;
					//Debug.Log("AutoYield is " + coroutine.Coroutine.AutoYieldCounter);
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
			} catch (SyntaxErrorException e) {
				//DynValue result = base.DoString(str);
				//return result;
				Debug.Log(e);
				result = DynValue.NewString("(Execution Terminated)");
				return true;
			}
		}

		public void Terminate()
		{
			markedForTermination = true;
		}
	}
}
