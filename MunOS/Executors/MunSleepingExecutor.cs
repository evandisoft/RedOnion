using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunOS.Executors
{
	public class MunSleepingExecutor : MunExecutor
	{
		public MunSleepingExecutor(MunCore core) : base(core, "Sleeping") {}
		public override void Execute(long startTicks)
		{
			if (Count == 0)
				return;
			do Execute(GetNext(), 0);
			while (!AtFirst); // always do full round
		}
	}
}
