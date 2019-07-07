using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static readonly Descriptor Bool = new OfBool();

		internal class OfBool : Descriptor
		{
			internal OfBool()
				: base("bool", typeof(bool), ExCode.Bool, TypeCode.Boolean) { }
			public override object Box(ref Value self)
				=> self.num.Bool;
			public override bool Equals(ref Value self, object obj)
				=> self.num.Bool.Equals(obj);
			public override int GetHashCode(ref Value self)
				=> self.num.Bool.GetHashCode();
			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.Bool.ToString(provider);

			public override bool Unary(ref Value self, OpCode op)
			{
				switch(op)
				{
				case OpCode.Not:
					self.num.Bool = !self.num.Bool;
					return true;
				}
				return false;
			}
		}
	}
}
