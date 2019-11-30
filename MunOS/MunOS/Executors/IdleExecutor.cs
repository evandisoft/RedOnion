namespace MunOS.Executors
{
	public class IdleExecutor : PriorityExecutor
	{
		int numSkips = 0;

		public override void Execute(double timeLimitMicros)
		{
			base.Execute(timeLimitMicros);
			if (timeLimitMicros < 0)
			{
				if (executeList.Count > 0)
				{
					if (numSkips < ExecutionManager.MaxIdleSkips)
					{
						numSkips++;
					}
					else
					{
						numSkips = 0;
						for (int i = executeList.Count; i >= 0; i--)
						{
							if (executeList[i].IsSleeping())
							{
								executeList.RemoveAt(i);
							}
							else
							{
								executeList[i].Execute(ExecutionManager.IdleForceExecuteTime);
								executeList.RemoveAt(i);
								break;
							}
						}
					}
				}
			}
		}
	}
}