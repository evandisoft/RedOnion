using System;
using MunOS;

namespace Kerbalua.Scripting
{
	public class KerbaluaReplThread : KerbaluaThread
	{
		public KerbaluaReplThread(KerbaluaProcess process, string source, string path)
			: base(process, MunPriority.Main, source, path) { }

		protected override MunStatus Execute(long tickLimit)
		{
			var status=base.Execute(tickLimit);
			if (status==MunStatus.Finished)
			{
				if (ReturnValue==null)
				{
					throw new Exception("For thread "+ID+" KerbaluaReplThread had ReturnValue of null");
				}

				string retvalString=GetOutputString(ReturnValue);
				Process.OutputBuffer.AddReturnValue(retvalString);
			}

			return status;
		}
	}
}
