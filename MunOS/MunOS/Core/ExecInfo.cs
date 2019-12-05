using System;
namespace MunOS.Core
{
	/// <summary>
	/// A class to contain all information needed by MunOS for a given Executable
	/// </summary>
	public class ExecInfo
	{
		public readonly string name;
		/// <summary>
		/// This ID is only used for associating with an ID during runtime.
		/// </summary>
		public readonly long ID;
		public readonly IExecutable executable;
		public bool terminated;
		public ExecInfo(long ID, IExecutable executable)
		{
			this.ID=ID;
			this.executable=executable;
		}
	}
}
