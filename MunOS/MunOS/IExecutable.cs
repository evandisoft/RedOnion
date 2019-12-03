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

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:MunOS.IExecutable"/> is sleeping.
		/// </summary>
		/// <value><c>true</c> if is sleeping; otherwise, <c>false</c>.</value>
		bool IsSleeping { get; }

		/// <summary>
		/// If MunOS encounters an exception while running this executable's Execute, 
		/// this handler will be called and the process
		/// associated with this executable will be removed from the OS.
		/// </summary>
		/// <param name="e">E.</param>
		void HandleException(string name, long id, Exception e);

		/// <summary>
		/// Will be called if this executable's process is terminated by the ExecutionManager's
		/// Kill method.
		/// </summary>
		void OnTerminated(string name,long id);
	}
}
