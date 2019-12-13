using System;
using System.Globalization;

namespace RedOnion.Utilities
{
	/// <summary>
	/// The purpose of this helper is to allow overloads accepting
	/// both string and FormattableString.
	/// </summary>
	public readonly struct StringWrapper
	{
		public readonly string String;
		public StringWrapper(string it) => String = it;
		public override string ToString() => String;

		public static implicit operator StringWrapper(string it)
			=> new StringWrapper(it);

		// this is actually never used (even if you do not have overload for FormattableString),
		// but helps with overload resolution (by making the implicit conversion ambiguous for FormattableString).
		public static implicit operator StringWrapper(FormattableString it)
			=> new StringWrapper(it.ToString(CultureInfo.InvariantCulture));
	}
}
