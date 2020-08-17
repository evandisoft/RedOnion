using System;
namespace RedOnion.Common.Completion
{
	/// <summary>
	/// Container to hold a type so it is treated as a static class for completion purposes.
	/// Without this, there is no way to tell the completion engine to use the type as
	/// a static, rather than as a type (which leads to reflection completion)
	/// </summary>
	public class Static
	{
		public readonly Type type;

		public Static(Type type)
		{
			this.type=type;
		}
	}
}
