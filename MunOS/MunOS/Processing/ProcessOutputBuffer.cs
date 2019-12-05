using System;
using System.Text;

namespace MunOS.Processing
{
	public class ProcessOutputBuffer
	{
		public ProcessOutputBuffer(int bufferSizeLimit = 10000)
		{
			BufferSizeLimit = bufferSizeLimit;
		}

		public int BufferSizeLimit = 10000;

		StringBuilder OutputBuffer = new StringBuilder();
		private bool newOutput;
		string outputText;

		public string GetString()
		{
			if (newOutput)
			{
				outputText=OutputBuffer.ToString();
			}
			newOutput = false;

			return outputText;
		}

		public void AddOutput(string str)
		{
			OutputBuffer.Append(str);
			int diff = OutputBuffer.Length - BufferSizeLimit;
			if (diff>0)
			{
				OutputBuffer.Remove(0, diff);
			}
			newOutput = true;
		}
	}
}
