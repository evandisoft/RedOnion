using System;

namespace MunOS.ProcessLayer
{
	public abstract class EngineThread : MunThread
	{
		// ROS is recycling main REPL thread and needs to change this
		public /*readonly*/ string source;
		// REPL thread has path = null so this can stay readonly
		public readonly string path;
		protected EngineThread(string source, string path, MunProcess parentProcess) : base(parentProcess)
		{
			this.source=source;
			this.path=path;
		}
	}
}
