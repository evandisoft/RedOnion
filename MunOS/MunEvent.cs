using System;
using System.Diagnostics;
using System.Reflection;
using static RedOnion.Debugging.QueueLogger;

namespace MunOS
{
	public class MunEvent
	{
		public MunCore Core { get; }
		public MunProcess Process { get; }
		public MunThread Thread { get; }
		public Exception Exception { get; }

		public StackFrame StackFrame { get; set; }
		public MethodBase Method => StackFrame.GetMethod();

		public bool Handled { get; set; }

		public MunEvent(MunCore core, Exception ex)
		{
			Core = core;
			Exception = ex;
			StackFrame = new StackFrame(1);
			MunLogger.DebugLog($"MunEvent: {ex.GetType().Name} in {Method.DeclaringType.Name}.{Method.Name}");
		}

		public MunEvent(MunCore core, MunThread thread, Exception ex = null)
		{
			Core = core;
			Process = thread?.Process;
			Thread = thread;
			Exception = ex;
			StackFrame = new StackFrame(1);
			MunLogger.DebugLog($@"MunEvent: thread#{thread?.ID ?? MunID.Zero}, exception: {(
				ex?.GetType().Name ?? "none")} in {Method.DeclaringType.Name}.{Method.Name}");
		}

		public MunEvent(MunCore core, MunProcess process, Exception ex = null)
		{
			Core = core;
			Process = process;
			Exception = ex;
			StackFrame = new StackFrame(1);
			MunLogger.DebugLog($@"MunEvent: process#{process?.ID ?? MunID.Zero}, exception: {(
				ex?.GetType().Name ?? "none")} in {Method.DeclaringType.Name}.{Method.Name}");
		}

		public override string ToString()
			=> FormattableString.Invariant($@"MunEvent: process#{Process?.ID ?? MunID.Zero
				}, thread#{Thread?.ID ?? MunID.Zero}, exception: {(
				Exception?.GetType().Name ?? "none")
				} in {Method.DeclaringType.Name}.{Method.Name}");
	}
}
