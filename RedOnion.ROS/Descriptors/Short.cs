using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static readonly Descriptor Short = new OfShort();

		internal class OfShort : Descriptor
		{
			internal OfShort() : base("short", typeof(short), ExCode.Short, TypeCode.Int16) { }
			public override object Box(ref Value self) => self.num.Short;

			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.Short.ToString(format, provider);
		}
	}
}
