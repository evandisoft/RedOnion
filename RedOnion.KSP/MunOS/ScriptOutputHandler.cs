using System;
namespace MunOS.Processing
{
	public class ScriptOutputHandler
	{
		ProcessOutputBuffer OutputBuffer;

		public ScriptOutputHandler(ProcessOutputBuffer processOutputBuffer)
		{
			OutputBuffer=processOutputBuffer;
		}

		public void AddText(string str)
		{
			OutputBuffer.AddOutput("\n" + str);
		}

		public void AddReturnValue(string str)
		{
			OutputBuffer.AddOutput("\nr> " + str);
		}

		public void AddOutput(string str)
		{
			OutputBuffer.AddOutput("\no> " + str);
		}

		public void AddError(string str)
		{
			OutputBuffer.AddOutput("\ne> " + str);
		}

		public void AddSourceString(string str)
		{
			OutputBuffer.AddOutput("\ni> " + str);
		}

		public void AddFileContent(string str)
		{
			OutputBuffer.AddOutput("\nf> " + str);
		}
	}
}
