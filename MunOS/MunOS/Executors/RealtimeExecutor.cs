using System.Collections.Generic;
using System.Diagnostics;

namespace MunOS.Executors
{
	public class RealtimeExecutor : PriorityExecutor
	{
		Stopwatch stopwatch = new Stopwatch();

		public override void Execute(double timeLimitMicros)
		{
			base.Execute(timeLimitMicros);
		}
	}
}