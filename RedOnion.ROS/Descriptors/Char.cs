using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static readonly Descriptor Char = new OfChar();

		internal class OfChar : Descriptor
		{
			internal OfChar() : base("char", typeof(char), ExCode.WideChar, TypeCode.Char) { }
			public override object Box(ref Value self) => self.num.Char;

			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.Char.ToString(provider);
		}
	}
}
