using System.Collections.Generic;
using System.Diagnostics;

namespace MunOS.Executors
{
	public class MunRealtimeExecutor : MunExecutor
	{
		public MunRealtimeExecutor(MunCore core) : base(core, "Realtime")
		{
			limit = MunCore.DefaultRealtimeLimit;
			minTicks = MunCore.DefaultMinRealtimeTicks;
		}
	}
}
