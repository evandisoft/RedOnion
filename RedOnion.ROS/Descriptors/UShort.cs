using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static readonly Descriptor UShort = new OfUShort();

		internal class OfUShort : Descriptor
		{
			internal OfUShort() : base("ushort", typeof(ushort), ExCode.UShort, TypeCode.UInt16) { }
			public override object Box(ref Value self) => self.num.UShort;

			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.UShort.ToString(format, provider);
		}
	}
}
