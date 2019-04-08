using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static readonly Descriptor Byte = new OfByte();

		internal class OfByte : Descriptor
		{
			internal OfByte() : base("byte", typeof(byte), ExCode.Byte, TypeCode.Byte) { }
			public override object Box(ref Value self) => self.num.Byte;

			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.Byte.ToString(format, provider);
		}
	}
}
