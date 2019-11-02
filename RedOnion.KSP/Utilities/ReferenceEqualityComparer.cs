using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.KSP.Utilities
{
	/// <summary>
	/// Implementation of IEqualityComparer enforcing reference equality
	/// </summary>
	public class ReferenceEqualityComparer : IEqualityComparer<object>
	{
		bool IEqualityComparer<object>.Equals(object x, object y)
			=> ReferenceEquals(x, y);
		int IEqualityComparer<object>.GetHashCode(object obj)
			=> obj.GetHashCode();
	}
}
