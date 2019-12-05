namespace MunOS.Core.Executors
{
	public class IdleExecutor : PriorityExecutor
	{
		int numSkips = 0;

		public override void Execute(long tickLimit)
		{
			base.Execute(tickLimit);
			if (tickLimit < 0)
			{
				if (executeQueue.Count > 0)
				{
					if (numSkips < CoreExecMgr.MaxIdleSkips)
					{
						numSkips++;
					}
					else
					{
						numSkips = 0;
						ExecuteExecutable(executeQueue.Dequeue(), CoreExecMgr.IdleForceTicks);
					}
				}
				else
				{
					// if there is nothing in the queue, we should reset the skips
					numSkips=0;
				}
			}
		}
	}
}