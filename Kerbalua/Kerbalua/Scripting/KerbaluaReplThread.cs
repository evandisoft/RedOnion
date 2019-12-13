using System;
using MunOS.Core;

namespace Kerbalua.Scripting
{
	public class KerbaluaReplThread : KerbaluaThread
	{
		public KerbaluaReplThread(string source, string path, KerbaluaProcess parentProcess) : base(source, path, parentProcess)
		{
		}

		protected override ExecStatus ProtectedExecute(long tickLimit)
		{
			var status=base.ProtectedExecute(tickLimit);
			if (status==ExecStatus.FINISHED)
			{
				if (ReturnValue==null)
				{
					throw new Exception("For thread "+ID+" KerbaluaReplThread had ReturnValue of null");
				}

				string retvalString=GetOutputString(ReturnValue);
				parentProcess.outputBuffer.AddReturnValue(retvalString);
			}

			return status;
		}
	}
}
