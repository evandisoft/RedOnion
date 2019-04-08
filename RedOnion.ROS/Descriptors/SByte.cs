using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static readonly Descriptor SByte = new OfSByte();

		internal class OfSByte : Descriptor
		{
			internal OfSByte() : base("sbyte", typeof(sbyte), ExCode.SByte, TypeCode.SByte) { }
			public override object Box(ref Value self) => self.num.SByte;

			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.SByte.ToString(format, provider);
		}
	}
}
