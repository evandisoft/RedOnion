using System;

namespace RedOnion.KSP.Attributes
{
	public class SafeProps : Attribute { }

	/// <summary>
	/// Marker for potentially dangerous API.
	/// </summary>
	public class UnsafeAttribute : Attribute { }
}
