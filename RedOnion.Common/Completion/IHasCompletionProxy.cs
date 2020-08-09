using System;
namespace RedOnion.Common.Completion
{
	public interface IHasCompletionProxy
	{
		/// <summary>
		/// An object that should be used for completion instead of the
		/// current object.
		/// </summary>
		/// <value>The completion proxy.</value>
		object CompletionProxy { get; }
	}
}
