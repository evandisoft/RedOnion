using System;
using System.Text;

namespace Kerbalua.Utility {
	public class Misc {
		static public string ReverseString(string stringToReverse)
		{
			var toBuilder = new StringBuilder();
			for (int i = stringToReverse.Length - 1;i >= 0;i--) {
				toBuilder.Append(stringToReverse[i]);
			}
			return toBuilder.ToString();
		}
	}
}
