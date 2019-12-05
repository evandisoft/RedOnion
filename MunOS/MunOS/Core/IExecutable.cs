using System;
using MunOS.Core;
using MunOS.Core.Executors;

namespace MunOS.Core
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
		/// Gets a value indicating whether this <see cref="IExecutable"/> is sleeping.
		/// </summary>
		/// <value><c>true</c> if is sleeping; otherwise, <c>false</c>.</value>
		bool IsSleeping { get; }

		/// <summary>
		/// If <see cref="CoreExecMgr"/> encounters an exception while running this executable's Execute, 
		/// this handler will be called and the process
		/// associated with this executable will be removed from the OS.
		/// </summary>
		/// <param name="e">E.</param>
		void HandleException(Exception e);

		/// <summary>
		/// Will be called if this executable's process is terminated by the ExecutionManager's
		/// Kill method.
		/// </summary>
		void OnTerminated();
	}
}
