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
		/// Execute for the specified timeLimitMicros.
		/// </summary>
		/// <returns>Whether this executable voluntarily yielded. If it did not voluntarily yield we may be able to give it
		/// more time later if the other processes finished early.</returns>
		/// <param name="tickLimit">Time limit micros.</param>
		ExecStatus Execute(long tickLimit);
		/// <returns><c>true</c>, if executable is sleeping, <c>false</c> otherwise.</returns>
		bool IsSleeping { get; }

		/// <summary>
		/// If MunOS encounters an exception while running Execute, this handler will be called and the process
		/// will be removed.
		/// </summary>
		/// <param name="e">E.</param>
		void HandleException(Exception e);
	}
}
