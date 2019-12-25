using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunOS
{
	/// <summary>
	/// Thread / execution status.
	/// </summary>
	public enum MunStatus : sbyte
	{
		/// <summary>
		/// Thread was either never started or was interrupted by scheduler,
		/// before completing or yielding.
		/// </summary>
		Incomplete,
		/// <summary>
		/// Thread voluntarily yielded (paused its own execution until next update).
		/// </summary>
		Yielded,
		/// <summary>
		/// Thread is currently sleeping (like yielding but this state helps with scheduling - no time assigned).
		/// This may be used for threads that want to be executed periodically with some higher interval (like cron),
		/// or can quickly check the condition (without invoking unknown script - e.g. checking that some background operation finished).
		/// </summary>
		Sleeping,
		/// <summary>
		/// Thread is doing cleanup (e.g. executing final blocks) before termination.
		/// </summary>
		Terminating,

		/// <summary>
		/// Thread finished execution and can be removed or restarted.
		/// </summary>
		Finished = -1,
		/// <summary>
		/// Thread was terminated (externally or by unhandled exception) and will be removed.
		/// </summary>
		Terminated = -2,
	}

	public static class MunStatusExtensions
	{
		/// <summary>
		/// State is final (negative value) - can only change if resetting/recycling the thread, not by any other means.
		/// </summary>
		public static bool IsFinal(this MunStatus status)
			=> status < 0;
	}
}
