using System;
using MunOS.Core.Executors;
using MunOS.Processing;

namespace RedOnion.KSP.MunOS
{
	public abstract class EngineThread : MunThread
	{
		protected ScriptOutputHandler outputHandler;

		protected EngineThread(MunProcess parentProcess,ScriptOutputHandler outputHandler, string name = "") : base(parentProcess, name)
		{
			this.outputHandler=outputHandler;
		}
	}
}
