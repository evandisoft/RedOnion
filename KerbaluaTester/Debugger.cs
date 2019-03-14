using System;
using System.Threading;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;

namespace KerbaluaTester {
	public class Debugger:IDebugger {
		Action allowMainThreadAndWaitForItToPause;

		public Debugger(Action waitForMainThread)
		{
			this.allowMainThreadAndWaitForItToPause = waitForMainThread;
		}
		Random random = new Random();
		int i = 0;
		public DebuggerAction GetAction(int ip, SourceRef sourceref)
		{
			Console.WriteLine(ip + "," + sourceref);
			if (sourceref!=null) {
				Console.WriteLine(sourceCode.Lines[sourceref.ToLine]);
			}
			DebuggerAction debuggerAction = new DebuggerAction();
			//if (ip > 106)
			//debuggerAction.Action = DebuggerAction.ActionType.
			//Thread.Sleep(1000);
			//Thread.Sleep((int)(random.NextDouble() * 1000));
			allowMainThreadAndWaitForItToPause();
			//debugService.OwnerScript.
			if (++i> 40){
				throw new Exception("I just don't want to go on");
			}
			return debuggerAction;
		}

		public DebuggerCaps GetDebuggerCaps()
		{
			return DebuggerCaps.HasLineBasedBreakpoints|DebuggerCaps.CanDebugSourceCode;
		}

		public List<DynamicExpression> GetWatchItems()
		{
			return new List<DynamicExpression>();
		}

		public bool IsPauseRequested()
		{
			return true;
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
			Console.WriteLine("Execution Ended");
		}

		public bool SignalRuntimeException(ScriptRuntimeException ex)
		{
			throw new NotImplementedException();
		}

		public void Update(WatchType watchType, IEnumerable<WatchItem> items)
		{

		}
	}
}
