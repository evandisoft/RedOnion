using System;
namespace MunOS.Core
{
	/// <summary>
	/// A class to contain all information needed by MunOS for a given Executable
	/// </summary>
	public class ExecInfo
	{
		public readonly string name;
		public readonly long ID;
		public readonly IExecutable executable;
		public ExecInfo(long ID, string name, IExecutable executable)
		{
			this.ID=ID;
			this.name=name;
			this.executable=executable;
		}
	}
}
