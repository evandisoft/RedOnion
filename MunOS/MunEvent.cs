using System;
using System.Diagnostics;
using System.Reflection;
using static RedOnion.Debugging.QueueLogger;

namespace MunOS
{
	/// <summary>
	/// Designed mainly for error handling (e.g. MunCore.OnError).
	/// </summary>
	public class MunEvent
	{
		/// <summary>
		/// The core in which the error/event occured (should never be null).
		/// </summary>
		public MunCore Core { get; }
		/// <summary>
		/// The process in which the error/event occured (can be null).
		/// </summary>
		public MunProcess Process { get; }
		/// <summary>
		/// The thread in which the error/event occured (can be null).
		/// </summary>
		public MunThread Thread { get; }
		/// <summary>
		/// The exception that was the source of the error (can be null).
		/// </summary>
		public Exception Exception { get; }
		/// <summary>
		/// Stack frame where the error/event occured (should never be null).
		/// </summary>
		public StackFrame StackFrame { get; set; }
		/// <summary>
		/// Method or constructor info where the error/event occured.
		/// </summary>
		public MethodBase Method => StackFrame.GetMethod();

		/// <summary>
		/// Signal that the error was handled (and should not be propagated further).
		/// </summary>
		public bool Handled { get; set; }
		/// <summary>
		/// The error was already printed/reported to the user (but may still be unhandled).
		/// </summary>
		public bool Printed { get; set; }

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
