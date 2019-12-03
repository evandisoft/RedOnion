using System;
namespace MunOS.Core
{
	/// <summary>
	/// A class to contain all information needed by MunOS for a given Executable
	/// </summary>
	public class ExecInfo
	{
		static long NextID=0;
		public readonly string name;
		public readonly long ID=NextID++;
		public readonly IExecutable executable;
		public ExecInfo(string name, IExecutable executable)
		{
			this.name=name;
			this.executable=executable;
		}
	}
}
