using System;

namespace MunOS.ProcessLayer
{
	public abstract class EngineThread : MunThread
	{
		public readonly string source;
		public readonly string path;
		protected EngineThread(string source, string path, MunProcess parentProcess) : base(parentProcess)
		{
			this.source=source;
			this.path=path;
		}
	}
}
