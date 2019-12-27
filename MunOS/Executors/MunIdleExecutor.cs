namespace MunOS.Executors
{
	public class MunIdleExecutor : MunExecutor
	{
		public MunIdleExecutor(MunCore core) : base(core, "Idle")
		{
			maxSkips = MunCore.DefaultMaxIdleSkips;
			limit = MunCore.DefaultIdleLimit;
			minTicks = MunCore.DefaultMinIdleTicks;
		}
	}
}
