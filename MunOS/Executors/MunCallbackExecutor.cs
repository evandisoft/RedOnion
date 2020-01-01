namespace MunOS.Executors
{
	public class MunCallbackExecutor : MunExecutor
	{
		public MunCallbackExecutor(MunCore core) : base(core, "Callback")
		{
			maxSkips = MunCore.DefaultMaxCallbackSkips;
			limit = MunCore.DefaultCallbackLimit;
			minTicks = MunCore.DefaultMinCallbackTicks;
		}
	}
}
