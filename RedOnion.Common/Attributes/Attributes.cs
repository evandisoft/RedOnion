using System;

namespace RedOnion.Attributes
{
	/// <summary>Marks potentially dangerous API.</summary>
	public class UnsafeAttribute : Attribute { }

	/// <summary>Marks unfinished API.</summary>
	public class WorkInProgressAttribute : Attribute { }

	/// <summary>Marks classes with properies without side-effects (usually script-namespace).</summary>
	public class SafeProps : Attribute { }
}
