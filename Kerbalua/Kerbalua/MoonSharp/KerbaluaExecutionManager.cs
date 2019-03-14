using System;
using System.Diagnostics;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;
using UnityEngine;

namespace Kerbalua.MoonSharp {
	public class KerbaluaExecutionManager:IDebugger {
		Stopwatch stopWatch = new Stopwatch();
		int timeLimit;

		public KerbaluaExecutionManager(int timeLimit=100)
		{
			this.timeLimit= timeLimit;
			debuggerAction.Action = DebuggerAction.ActionType.StepIn;
		}

		DebuggerAction debuggerAction = new DebuggerAction();
		int i = 0;
		public DebuggerAction GetAction(int ip, SourceRef sourceref)
		{
			if (!stopWatch.IsRunning) {
				UnityEngine.Debug.Log("Starting stopwatch");
				stopWatch.Start();
			}

			//Console.WriteLine(ip + "," + sourceref);
			if (sourceref != null) {
				//Console.WriteLine(sourceCode.Lines[sourceref.ToLine]);
			}
			//debugService.OwnerScript.
			if (stopWatch.ElapsedMilliseconds>timeLimit) {
				stopWatch.Reset();
				UnityEngine.Debug.Log("Throwing exception");

				throw new ExecutionLimitException("Execution timed out");
			}
			return debuggerAction;
		}

		public DebuggerCaps GetDebuggerCaps()
		{
			return DebuggerCaps.HasLineBasedBreakpoints | DebuggerCaps.CanDebugSourceCode;
		}

		public List<DynamicExpression> GetWatchItems()
		{
			return new List<DynamicExpression>();
		}

		public bool IsPauseRequested()
		{
			return false;
		}

		public void RefreshBreakpoints(IEnumerable<SourceRef> refs)
		{

		}

		string[] byteCode;
		public void SetByteCode(string[] byteCode)
		{
			this.byteCode = byteCode;
		}

		DebugService debugService;
		public void SetDebugService(DebugService debugService)
		{
			this.debugService = debugService;
		}

		SourceCode sourceCode;
		public void SetSourceCode(SourceCode sourceCode)
		{
			this.sourceCode = sourceCode;
		}

		public void SignalExecutionEnded()
		{
			UnityEngine.Debug.Log("Execution Ended");
			stopWatch.Reset();
		}

		public bool SignalRuntimeException(ScriptRuntimeException ex)
		{
			stopWatch.Reset();
			throw ex;
		}

		public void Update(WatchType watchType, IEnumerable<WatchItem> items)
		{

		}
	}
}
