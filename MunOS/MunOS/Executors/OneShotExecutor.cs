namespace MunOS.Executors
{
	public class OneShotExecutor : PriorityExecutor
	{
		int numSkips = 0;

		public override void Execute(double timeLimitMicros)
		{
			base.Execute(timeLimitMicros);
			if (timeLimitMicros < 0)
			{
				if (executeList.Count > 0)
				{
					if (numSkips < ExecutionManager.MaxOneShotSkips)
					{
						numSkips++;
					}
					else
					{
						numSkips = 0;
						executeList[0].Execute(ExecutionManager.OneShotForceExecuteTime);
						executeList.RemoveAt(0);
					}
				}
			}
		}
	}
}