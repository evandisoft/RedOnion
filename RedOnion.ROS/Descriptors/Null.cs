using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static readonly Descriptor Null = new OfNull();

		/// <summary>
		/// Represents empty reference
		/// and will cause errors on attemt to use it in most operations.
		/// </summary>
		internal class OfNull : Descriptor
		{
			internal OfNull()
				: base("null", typeof(object), ExCode.Null, TypeCode.Empty) { }
			public override bool Equals(ref Value self, object obj)
				=> obj == null;
			public override int GetHashCode(ref Value self)
				=> ~0;
			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> debug ? "null" : "";
		}
	}
}
