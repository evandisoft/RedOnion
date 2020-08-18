using System;

namespace MunSharp.Interpreter.Diagnostics.PerformanceCounters
{
	internal interface IPerformanceStopwatch
	{
		IDisposable Start();
		PerformanceResult GetResult();
	}
}
