namespace MunOS.Executors
{
	public class OneShotExecutor : PriorityExecutor
	{
		int numSkips = 0;

		public override void Execute(long tickLimit)
		{
			base.Execute(tickLimit);
			if (tickLimit < 0)
			{
				if (executeQueue.Count > 0)
				{
					if (numSkips < ExecutionManager.MaxOneShotSkips)
					{
						numSkips++;
					}
					else
					{
						numSkips = 0;
						ExecuteProcess(executeQueue.Dequeue(), ExecutionManager.OneshotForceTicks);
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