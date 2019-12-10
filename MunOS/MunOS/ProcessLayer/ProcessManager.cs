using System;
using System.Collections.Generic;

namespace MunOS.ProcessLayer
{
	public class ProcessManager
	{
		static public void Initialize()
		{
			instance = new ProcessManager();
		}
		static public ProcessManager instance;
		/// <summary>
		/// Should be initialized by LiveReplMain prior to anything else being
		/// able to use it. Must be reinitialized on every scene change.
		/// </summary>
		/// <value>The instance.</value>
		static public ProcessManager Instance
		{
			get
			{
				if (instance==null)
				{
					throw new Exception(nameof(ProcessManager)+" was not initialized!");
				}

				return instance;
			}
		}

		public List<MunProcess> Processes=new List<MunProcess>();

		public void FixedUpdate()
		{
			MunThread.ResetStopwatch();
			foreach(var process in Processes)
			{
				process.FixedUpdate();
			}
		}
	}
}
