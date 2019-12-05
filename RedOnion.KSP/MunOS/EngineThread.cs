using System;
using MunOS.Core.Executors;
using MunOS.Processing;

namespace RedOnion.KSP.MunOS
{
	public abstract class EngineThread : MunThread
	{
		protected ScriptOutputHandler outputHandler;

		protected EngineThread(ScriptOutputHandler outputHandler, string name = "") : base(name)
		{
			this.outputHandler=outputHandler;
		}
	}
}
