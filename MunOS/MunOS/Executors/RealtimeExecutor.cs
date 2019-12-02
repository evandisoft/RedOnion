using System.Collections.Generic;
using System.Diagnostics;

namespace MunOS.Executors
{
	public class RealtimeExecutor : PriorityExecutor
	{
		public override void Execute(long tickLimit)
		{
			base.Execute(tickLimit);
		}
	}
}