using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		/// <summary>
		/// Represents empty reference
		/// and will cause errors on attemt to use it in most operations.
		/// </summary>
		internal class OfNull : Descriptor
		{
			internal OfNull()
				: base("null", typeof(object), ExCode.Null, TypeCode.Empty) { }
			public override bool Equals(ref Value self, object obj)
				=> obj == null || obj is Value val && val.desc.Primitive == ExCode.Null;
			public override int GetHashCode(ref Value self)
				=> ~0;
			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> debug ? "null" : "";
			public override bool Binary(ref Value lhs, OpCode op, ref Value rhs)
			{
				switch (op)
				{
				case OpCode.Equals:
					lhs = lhs.desc.Primitive == ExCode.Null && rhs.desc.Primitive == ExCode.Null;
					return true;
				case OpCode.Differ:
					lhs = lhs.desc.Primitive != ExCode.Null || rhs.desc.Primitive != ExCode.Null;
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Used in functions when you reference this which is null.
		/// </summary>
		internal class OfNullSelf : OfNull
		{
		}
	}
}
