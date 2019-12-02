using System;
using MunOS.Executors;

namespace MunOS
{
	/// <summary>
	/// Interface for an object that can be executed for a given amount of microseconds
	/// </summary>
	public interface IExecutable
	{
		/// <summary>
		/// Execute for the specified tickLimit.
		/// </summary>
		/// <returns>The <see cref="ExecStatus"/> </returns>
		/// <param name="tickLimit">Limit on number of ticks this should execute for.</param>
		ExecStatus Execute(long tickLimit);
		/// <returns><c>true</c>, if executable is sleeping, <c>false</c> otherwise.</returns>
		bool IsSleeping { get; }

		/// <summary>
		/// If MunOS encounters an exception while running this executable's Execute, 
		/// this handler will be called and the process
		/// associated with this executable will be removed.
		/// </summary>
		/// <param name="e">E.</param>
		void HandleException(string name, long id, Exception e);

		/// <summary>
		/// Will be called when this executable's process is terminated by the OS
		/// </summary>
		void OnTerminated(string name,long id);
	}
}
