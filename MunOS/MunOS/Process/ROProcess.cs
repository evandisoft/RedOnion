using System;
using System.Collections.Generic;

namespace MunOS
{
	abstract public class ROProcess
	{
		/// <summary>
		/// Run for the time allotted.
		/// </summary>
		/// <returns><c>true</c>, if process is finished, <c>false</c> otherwise.</returns>
		/// <param name="timeAllotted">Time allotted.</param>
		public bool FixedUpdate(float timeAllotted)
		{
			return ProtectedFixedUpdate(timeAllotted);
		}
		protected abstract bool ProtectedFixedUpdate(float timeAllotted);

		/// <summary>
		/// Output is stored here so that it can be stored even when the
		/// process is not connected to a console.
		/// </summary>
		public ProcessOutputBuffer OutputBuffer = new ProcessOutputBuffer();

		public abstract void ReceiveInput(string str);
		public abstract bool IsWaitingForInput();
	}
}
