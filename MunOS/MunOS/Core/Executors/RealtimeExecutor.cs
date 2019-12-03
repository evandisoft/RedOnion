using System.Collections.Generic;
using System.Diagnostics;

namespace MunOS.Core.Executors
{
	public class RealtimeExecutor : PriorityExecutor
	{
		public override void Execute(long tickLimit)
		{
			base.Execute(tickLimit);
		}
	}
}