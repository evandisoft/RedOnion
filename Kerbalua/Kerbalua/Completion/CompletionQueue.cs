using System;
using System.Collections.Generic;
using System.Text;

namespace Kerbalua.Completion
{
	public static class CompletionQueue
	{
		public static Queue<string> completionQueue=new Queue<string>();
		public static int size=10000;

		public static string Log(string str)
		{
			completionQueue.Enqueue(str);
			if (completionQueue.Count>size)
			{
				completionQueue.Dequeue();
			}
			return str;
		}

		public static string String()
		{
			StringBuilder sb=new StringBuilder();
			foreach(var str in completionQueue)
			{
				sb.Append(str);
				sb.Append("\n");
			}
			return sb.ToString();
		}
	}
}
