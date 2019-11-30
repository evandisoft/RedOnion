using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MunOS.Executors
{
	/// <summary>
	/// Holds all executables of a given priority and can execute them
	/// with the given time limit.
	/// </summary>
	public abstract class PriorityExecutor
	{
		/// <summary>
		/// New executables to be added on next execute
		/// </summary>
		protected List<IExecutable> newExecutables = new List<IExecutable>();
		/// <summary>
		/// The total list of executables.
		/// </summary>
		public List<IExecutable> executables = new List<IExecutable>();
		/// <summary>
		/// The list of executables currently executing, or ones that did not
		/// finish last time.
		/// </summary>
		protected List<IExecutable> executeList = new List<IExecutable>();

		Stopwatch stopwatch = new Stopwatch();

		public void RegisterExecutable(IExecutable executable)
		{
			newExecutables.Add(executable);
		}

		public virtual void Execute(double timeLimitMicros)
		{
			stopwatch.Reset();
			stopwatch.Start();
			executables.AddRange(newExecutables);
			newExecutables.Clear();
			foreach (var e in executables)
			{
				if (!executeList.Contains(e))
				{
					executeList.Add(e);
				}
			}

			double remainingTime = timeLimitMicros;
			while (remainingTime > 0 && executeList.Count > 0)
			{
				for (int i = executeList.Count - 1; i >= 0; i--)
				{
					if (executeList[i].IsSleeping())
					{
						executeList.RemoveAt(i);

					}
				}
				if (executeList.Count == 0)
				{
					break;
				}
				double executeTime = remainingTime / executeList.Count;

				for (int i = executeList.Count - 1; i >= 0; i--)
				{
					if (executeList[i].Execute(executeTime))
					{
						executeList.RemoveAt(i);
					}
					remainingTime = timeLimitMicros - stopwatch.Elapsed.TotalMilliseconds;
					if (remainingTime < 0)
					{
						break;
					}
				}
			}

			stopwatch.Stop();
		}
	}
}
