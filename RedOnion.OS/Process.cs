using System;
namespace RedOnion.OS
{
	abstract public class Process
	{
		/// <summary>
		/// Run for the time allotted.
		/// </summary>
		/// <returns><c>true</c>, if process is finished, <c>false</c> otherwise.</returns>
		/// <param name="timeAllotted">Time allotted.</param>
		public abstract bool FixedUpdate(float timeAllotted);


	}
}
