using System;
namespace Kerbalua.MoonSharp {
	public class ExecutionLimitException:Exception {
		public ExecutionLimitException(string message):base(message)
		{
		}
	}
}
