using System;
using System.Diagnostics;
using Kerbalua.Parsing;
using Kerbalua.Scripting;
using MunSharp.Interpreter;
using MunOS;
using static RedOnion.Debugging.QueueLogger;

namespace RedOnion.KSP.Kerbalua
{
	public class KerbaluaThread:MunThread
	{
		public KerbaluaProcess ScriptProcess => (KerbaluaProcess)Process;
		KerbaluaScript ScriptEngine => ScriptProcess.ScriptEngine;
		DynValue coroutine;
		string source, path;

		public DynValue ReturnValue { get; private set; }

		public KerbaluaThread(KerbaluaProcess process, MunPriority priority, string source, string path, bool start = true)
			: base(process.Core, process, priority, path, start)
		{
			this.source = source;
			this.path = path;
		}
		public KerbaluaThread(KerbaluaProcess process, MunPriority priority, Closure f, bool start = true)
			: base(process.Core, process, priority, "fn", start)
		{
			coroutine = ScriptEngine.CreateCoroutine(f);
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

		public void Sleep(double seconds)
		{
			sleeptimeMillis=(long)(seconds*1000);
			sleepwatch.Reset();
			sleepwatch.Start();
			Status=MunStatus.Sleeping;
		}

		long sleeptimeMillis=0;
		Stopwatch sleepwatch=new Stopwatch();

		Stopwatch tickwatch=new Stopwatch();
		int perIterationCounter=100;
		protected override MunStatus Execute(long tickLimit)
		{
			if (coroutine == null)
			{
				coroutine = ScriptEngine.CreateCoroutine(CreateFunction(source));
			}

			if (Status == MunStatus.Sleeping)
			{
				if (sleepwatch.IsRunning)
				{
					if (sleepwatch.ElapsedMilliseconds < sleeptimeMillis)
					{
						return MunStatus.Sleeping;
					}

					sleepwatch.Reset();
					sleeptimeMillis=0;
				}

				return MunStatus.Incomplete;
			}

			tickwatch.Reset();
			tickwatch.Start();

			//MunLogger.Log("tick limit is "+tickLimit);
			DynValue retval=null;
			CoroutineState state=CoroutineState.ForceSuspended;
			// We want this to run at least once, to always give anything that woke up a guarantee
			// of at least perIterationCounter instructions
			do
			{
				coroutine.Coroutine.AutoYieldCounter = perIterationCounter;
				retval = coroutine.Coroutine.Resume();
				state=coroutine.Coroutine.State;
			} while (tickwatch.ElapsedTicks<tickLimit && state==CoroutineState.ForceSuspended);

			//MunLogger.Log("state was "+state);

			if (state == CoroutineState.Dead)
			{
				ReturnValue=retval;
				return MunStatus.Finished;
			}
			if (state == CoroutineState.ForceSuspended)
			{
				return MunStatus.Incomplete;
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
					return MunStatus.Yielded;
				}
				if (retval.Type!=DataType.Number)
				{
					throw new Exception("Argument to sleep must be a number");
				}
				double sleepSeconds=retval.Number;
				sleeptimeMillis=(long)(sleepSeconds*1000);
				sleepwatch.Start();
				

				return MunStatus.Sleeping;
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
