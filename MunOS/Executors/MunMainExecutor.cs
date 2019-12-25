namespace MunOS.Executors
{
	public class MunMainExecutor : MunExecutor
	{
		public MunMainExecutor(MunCore core) : base(core, "Main")
		{
			limit = 1.0;
			minTicks = MunCore.DefaultMinMainTicks;
		}
	}
}
