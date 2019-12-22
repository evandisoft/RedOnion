using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunOS
{
	public class MunEvent
	{
		public MunCore Core { get; }
		public MunProcess Process { get; }
		public MunThread Thread { get; }
		public Exception Exception { get; }
		public bool Handled { get; set; }

		public MunEvent(MunCore core, Exception ex)
		{
			Core = core;
			Exception = ex;
		}

		public MunEvent(MunCore core, MunThread thread, Exception ex = null)
		{
			Core = core;
			Process = thread?.Process;
			Thread = thread;
			Exception = ex;
		}

		public MunEvent(MunCore core, MunProcess process, Exception ex = null)
		{
			Core = core;
			Process = process;
			Exception = ex;
		}
	}
}
