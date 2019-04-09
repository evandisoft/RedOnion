using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static readonly Descriptor Byte = new OfByte();

		internal class OfByte : Descriptor
		{
			internal OfByte()
				: base("byte", typeof(byte), ExCode.Byte, TypeCode.Byte) { }
			public override object Box(ref Value self)
				=> self.num.Byte;
			public override bool Equals(ref Value self, object obj)
				=> self.num.Byte.Equals(obj);
			public override int GetHashCode(ref Value self)
				=> self.num.Byte.GetHashCode();
			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.Byte.ToString(format, provider);

			public override bool Convert(ref Value self, Descriptor to)
			{
				switch (to.Primitive)
				{
				case ExCode.String:
					self = ToString(ref self, null, Value.Culture, false);
					return true;
				case ExCode.Char:
				case ExCode.WideChar:
					self = new Value(self.num.Char);
					return true;
				case ExCode.Number:
				case ExCode.Byte:
					return true;
				case ExCode.UShort:
					self = new Value(self.num.UShort);
					return true;
				case ExCode.UInt:
					self = new Value(self.num.UInt);
					return true;
				case ExCode.ULong:
					self = new Value(self.num.ULong);
					return true;
				case ExCode.SByte:
					self = new Value(self.num.SByte);
					return true;
				case ExCode.Short:
					self = new Value(self.num.Short);
					return true;
				case ExCode.Int:
					self = new Value(self.num.Int);
					return true;
				case ExCode.Long:
					self = new Value(self.num.Long);
					return true;
				case ExCode.Float:
					self = new Value((float)self.num.Long);
					return true;
				case ExCode.Double:
					self = new Value((double)self.num.Long);
					return true;
				case ExCode.Bool:
					self = new Value(self.num.Long != 0);
					return true;
				}
				return false;
			}

			public override bool Unary(ref Value self, OpCode op)
			{
				switch (op)
				{
				case OpCode.Plus:
					self = +self.num.Byte;
					return true;
				case OpCode.Neg:
					self = -self.num.Byte;
					return true;
				case OpCode.Flip:
					self = ~self.num.Byte;
					return true;
				case OpCode.Not:
					self = new Value(self.num.Long == 0);
					return true;
				case OpCode.Inc:
					self.num.Byte += 1;
					return true;
				case OpCode.Dec:
					self.num.Byte -= 1;
					return true;
				}
				return false;
			}
			public override bool Binary(ref Value lhs, OpCode op, ref Value rhs)
			{
				lhs.desc.Convert(ref lhs, Int);
				return lhs.desc.Binary(ref lhs, op, ref rhs);
			}
		}
	}
}
