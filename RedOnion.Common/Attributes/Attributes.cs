using System;

namespace RedOnion.Attributes
{
	/// <summary>
	/// Marker for potentially dangerous API.
	/// </summary>
	public class UnsafeAttribute : Attribute { }

	public class SafeProps : Attribute { }
}
