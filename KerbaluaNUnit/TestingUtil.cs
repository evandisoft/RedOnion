using System;
using System.Collections.Generic;
using System.Text;

namespace KerbaluaNUnit
{
	public class TestingUtil
	{
		static public string ListToString<T>(IList<T> ts)
		{
			var sb = new StringBuilder();
			foreach (var t in ts)
			{
				sb.Append(t.ToString() + "\n");
			}
			return sb.ToString();
		}

		static public void PrintAll<T>(IList<T> ts)
		{
			Console.WriteLine(ListToString(ts));
		}
	}
}
