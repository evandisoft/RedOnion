using System;
using System.Collections.Generic;
using System.Text;

namespace MunOS
{
	public class ProcessOutputBuffer
	{
		public ProcessOutputBuffer(int bufferSizeLimit = 10000)
		{
			BufferSizeLimit = bufferSizeLimit;
		}

		public int BufferSizeLimit = 10000;

		public bool FreshOutput { get; private set; }

		StringBuilder OutputBuffer = new StringBuilder();

		public string GetString()
		{
			FreshOutput = false;
			return OutputBuffer.ToString();
		}

		public void Print(string str)
		{
			OutputBuffer.Append(str);
			int diff = OutputBuffer.Length - BufferSizeLimit;
			if (diff>0)
			{
				OutputBuffer.Remove(0, diff);
			}
			FreshOutput = true;
		}
	}
}
