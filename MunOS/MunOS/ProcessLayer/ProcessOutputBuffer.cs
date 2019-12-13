using System;
using System.Text;

namespace MunOS.ProcessLayer
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

		public string GetString(out bool isNewOutput)
		{
			isNewOutput=newOutput;
			if (newOutput)
			{
				outputText=OutputBuffer.ToString();
			}
			newOutput = false;

			return outputText;
		}

		public void Clear()
		{
			OutputBuffer.Clear();
			newOutput = true;
		}

		public void AddString(string str)
		{
			OutputBuffer.Append(str);
			int diff = OutputBuffer.Length - BufferSizeLimit;
			if (diff>0)
			{
				OutputBuffer.Remove(0, diff);
			}
			newOutput = true;
		}

		public void AddText(string str)
		{
			AddString("\n" + str);
		}

		public void AddReturnValue(string str)
		{
			AddString("\nr> " + str);
		}

		public void AddOutput(string str)
		{
			AddString("\no> " + str);
		}

		public void AddError(string str)
		{
			AddString("\ne> " + str);
		}

		public void AddSourceString(string str)
		{
			AddString("\ni> " + str);
		}

		public void AddFileContent(string str)
		{
			AddString("\nf> " + str);
		}
	}
}
