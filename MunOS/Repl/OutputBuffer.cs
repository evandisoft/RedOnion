using System;
using System.Text;

namespace MunOS.Repl
{
	// TODO: add line number limit

	public class OutputBuffer
	{
		// TODO: apply the limit when changing
		public int CharLimit { get; set; }

		public OutputBuffer(int charLimit = 10000)
		{
			CharLimit = charLimit;
		}

		// TODO: list of lines (or entries - may track groups of lines)
		StringBuilder builder = new StringBuilder();
		string outputText;
		bool newOutput;

		public string GetString(out bool isNewOutput)
		{
			isNewOutput=newOutput;
			if (newOutput)
			{
				outputText=builder.ToString();
			}
			newOutput = false;

			return outputText;
		}

		public void Clear()
		{
			builder.Clear();
			newOutput = true;
		}

		public void AddString(string str)
		{
			// TODO: remove whole lines
			builder.Append(str);
			int diff = builder.Length - CharLimit;
			if (diff>0)
			{
				builder.Remove(0, diff);
			}
			newOutput = true;
		}

		public void AddText(string str)
			=> AddString("\n" + str);
		public void AddReturnValue(string str)
			=> AddString("\nr> " + str);
		public void AddOutput(string str)
			=> AddString("\no> " + str);
		public void AddError(string str)
			=> AddString("\ne> " + str);
		public void AddSourceString(string str)
			=> AddString("\ni> " + str);
		public void AddFileContent(string str)
			=> AddString("\nf> " + str);
	}
}
