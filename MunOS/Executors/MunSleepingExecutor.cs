using System;
using static RedOnion.Debugging.QueueLogger;

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

		protected override void Execute(MunThread thread, long tickLimit)
		{
			MunThread.Current = thread;
			try
			{
				var status = thread.Execute(tickLimit);
				thread.Status = status;

				switch (status)
				{
				case MunStatus.Sleeping:
					// nothing to do, it just stays where it is
					break;
				case MunStatus.Finished:
				{
					var next = thread.NextThread;
					if (next != null)
					{
						if (next == thread)
						{
							// restart the thread
							thread.Status = MunStatus.Incomplete;
							thread.OnRestart();
							break;
						}
						// this is probably init-chain
						thread.NextThread = null; // rather clean this in case Core.Schedule(thread) also implements the logic
						Core.Schedule(next);
					}
					goto default;
				}
				default:
					// hand it over to the core
					Core.Schedule(thread);
					break;
				}
			}
			catch (Exception ex)
			{
				thread.Exception = ex;
				var err = new MunEvent(Core, thread, ex);
				thread.OnError(err);
				if (!err.Handled)
				{
					Core.OnError(err);
					if (!err.Handled)
						Core.Kill(thread, hard: true);
				}
			}
			finally
			{
				MunThread.Current = null;
			}
		}
	}
}
