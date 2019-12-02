using System;
namespace MunOS
{
	public class Process
	{
		static long NextID=0;
		public readonly string name;
		public readonly long ID=NextID++;
		public readonly IExecutable executable;
		public bool terminated;
		public Process(string name, IExecutable executable)
		{
			this.name=name;
			this.executable=executable;
		}
	}
}
