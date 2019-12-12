using System;
using System.Diagnostics;
using Kerbalua.Parsing;
using MoonSharp.Interpreter;
using MunOS.Core;
using MunOS.ProcessLayer;
using static MunOS.Debugging.QueueLogger;

namespace Kerbalua.Scripting
{
	public class KerbaluaThread:EngineThread
	{
		public KerbaluaProcess ScriptProcess => parentProcess as KerbaluaProcess;
		KerbaluaScript ScriptEngine => ScriptProcess.ScriptEngine;
		DynValue coroutine;

		public DynValue ReturnValue { get; private set; }

		/// <summary>
		/// This can throw an errorif the script syntax is incorrect.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="path">Path.</param>
		/// <param name="parentProcess">Parent process.</param>
		public KerbaluaThread(string source, string path, KerbaluaProcess parentProcess) : base(source, path, parentProcess)
		{
			var mainFunction=CreateFunction(source);
			coroutine = ScriptEngine.CreateCoroutine(mainFunction);
		}

		DynValue CreateFunction(string scriptSource)
		{
			if (IncompleteLuaParsing.IsImplicitReturn(scriptSource))
			{
				scriptSource = "return " + scriptSource;
			}
			DynValue mainFunction = ScriptProcess.ScriptEngine.DoString("return function () " + scriptSource + "\n end");
			return mainFunction;
		}

		public KerbaluaThread(Closure f,KerbaluaProcess parentProcess) : base("fn", "", parentProcess)
		{
			coroutine = ScriptEngine.CreateCoroutine(f);
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

		Stopwatch tickwatch=new Stopwatch();
		int perIterationCounter=100;
		protected override ExecStatus ProtectedExecute(long tickLimit)
		{
			if (IsSleeping)
			{
				return ExecStatus.SLEEPING;
			}

			tickwatch.Reset();
			tickwatch.Start();

			//MunLogger.Log("tick limit is "+tickLimit);
			DynValue retval=null;
			CoroutineState state=CoroutineState.ForceSuspended;
			while (tickwatch.ElapsedTicks<tickLimit && state==CoroutineState.ForceSuspended)
			{
				coroutine.Coroutine.AutoYieldCounter = perIterationCounter;
				retval = coroutine.Coroutine.Resume();
				state=coroutine.Coroutine.State;
			}

			//MunLogger.Log("state was "+state);

			if (state == CoroutineState.Dead)
			{
				ReturnValue=retval;
				return ExecStatus.FINISHED;
			}
			if (state == CoroutineState.ForceSuspended)
			{
				return ExecStatus.INTERRUPTED;
			}
			if (state == CoroutineState.Suspended)
			{
				// if it is suspended we assume sleep (which is just a renamed yield)
				// was called. The argument passed to yield 
				// as the amount of time to sleep.
				if (retval.Tuple!=null) 
				{
					throw new Exception("Too many arguments to sleep");
				}
				if (retval.IsNil())
				{
					return ExecStatus.YIELDED;
				}
				if (retval.Type!=DataType.Number)
				{
					throw new Exception("Argument to sleep must be a number");
				}
				double sleepSeconds=retval.Number;
				sleeptimeMillis=(long)(sleepSeconds*1000);
				sleepwatch.Start();

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
