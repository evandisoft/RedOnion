using System;
using System.Diagnostics;
using Kerbalua.Parsing;
using MoonSharp.Interpreter;
using MunOS.Core;
using MunOS.Core.Executors;
using MunOS.ProcessLayer;

namespace Kerbalua.Scripting
{
	public class KerbaluaThread:EngineThread
	{
		public KerbaluaProcess ScriptProcess => parentProcess as KerbaluaProcess;
		KerbaluaScript ScriptEngine => ScriptProcess.scriptEngine;
		DynValue coroutine;

		public KerbaluaThread(string source, string path, KerbaluaProcess parentProcess, string name = "") : base(source, path, parentProcess, name)
		{
			if (IncompleteLuaParsing.IsImplicitReturn(source))
			{
				source = "return " + source;
			}
			DynValue mainFunction = ScriptProcess.scriptEngine.DoString("return function () " + source + "\n end");

			coroutine = ScriptEngine.CreateCoroutine(mainFunction);
		}

		public override bool IsSleeping
		{
			get
			{
				if (sleepwatch.IsRunning)
				{
					if (sleepwatch.ElapsedMilliseconds < sleeptimeMillis)
					{
						return true;
					}

					sleepwatch.Reset();
					sleeptimeMillis=0;
				}
				return false;
			}
		}

		long sleeptimeMillis=0;
		Stopwatch sleepwatch=new Stopwatch();
		public void Sleep(double sleepSeconds)
		{
			sleeptimeMillis=(long)(sleepSeconds*1000);
			sleepwatch.Start();
			coroutine.Coroutine.AutoYieldCounter=0;
		}

		Stopwatch tickwatch=new Stopwatch();
		int perIterationCounter=100;
		protected override ExecStatus ProtectedExecute(long tickLimit)
		{
			if (coroutine == null)
			{
				throw new Exception("Coroutine not set in KerbaluaScript");
			}

			if (IsSleeping)
			{
				return ExecStatus.SLEEPING;
			}

			tickwatch.Reset();
			tickwatch.Start();

			DynValue retval=null;
			while (tickwatch.ElapsedTicks<tickLimit)
			{
				coroutine.Coroutine.AutoYieldCounter = perIterationCounter;
				retval = coroutine.Coroutine.Resume();
			}

			var state=coroutine.Coroutine.State;
			if (state == CoroutineState.Dead)
			{
				string retvalString=GetOutputString(retval);
				parentProcess.outputBuffer.AddReturnValue(retvalString);
				return ExecStatus.FINISHED;
			}
			if (state == CoroutineState.ForceSuspended)
			{
				if (IsSleeping)
				{
					return ExecStatus.SLEEPING;
				}
				return ExecStatus.INTERRUPTED;
			}
			if (state == CoroutineState.Suspended)
			{
				return ExecStatus.YIELDED;
			}

			throw new Exception("State of coroutine should not have been "+state);
		}

		public string GetOutputString(DynValue dynResult)
		{
			string result = "";

			if (dynResult.Type==DataType.String)
			{
				result = "\"" + dynResult.ToObject() + "\"";
				return result;
			}
			if (dynResult.Type == DataType.Nil || dynResult.Type== DataType.Void)
			{
				result = dynResult.ToString();
				return result;
			}

			result += dynResult.ToObject().ToString();

			if (dynResult.UserData==null)
			{
				return result;
			}

			// This is a static.
			if (dynResult.UserData.Object==null)
			{
				return result;
			}

			// This is a type
			if (dynResult.ToObject() is Type)
			{
				result += " (runtime type)";
				return result;
			}

			return result;
		}
	}
}
