using System;
using System.Collections.Generic;

namespace MunOS.Repl
{
	public class ReplHistory
	{
		public ReplHistory(int maxHistorySize = 1000)
		{
			MaxHistorySize = maxHistorySize;
		}
		public int MaxHistorySize = 1000;

		LinkedListNode<string> currentItem = null;
		protected LinkedList<string> historyList = new LinkedList<string>();

		public string Current()
		{
			if (currentItem == null)
			{
				return "";
			}

			return currentItem.Value;
		}

		public string Up()
		{
			if (currentItem == null)
			{
				currentItem = historyList.First;
			}
			else if (currentItem != historyList.Last)
			{
				currentItem = currentItem.Next;
			}

			return currentItem.Value;
		}

		public string Down()
		{
			if (currentItem == null)
			{
				return "";
			}

			if (currentItem != historyList.First)
			{
				currentItem = currentItem.Previous;
			}

			return currentItem.Value;
		}

		public void Add(string str)
		{
			if (!(historyList.Count > 0 && str == historyList.First.Value))
			{
				historyList.AddFirst(str);
			}

			while (historyList.Count > MaxHistorySize)
			{
				historyList.RemoveLast();
			}

			currentItem = null;
		}
	}
}
