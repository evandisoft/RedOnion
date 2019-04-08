using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		internal static readonly Descriptor UInt = new OfUInt();

		internal class OfUInt : Descriptor
		{
			internal OfUInt() : base("uint", typeof(uint), ExCode.UInt, TypeCode.UInt32) { }
			public override object Box(ref Value self) => self.num.UInt;

			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.UInt.ToString(format, provider);

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
				case ExCode.Byte:
					self = new Value(self.num.Byte);
					return true;
				case ExCode.UShort:
					self = new Value(self.num.UShort);
					return true;
				case ExCode.Number:
				case ExCode.UInt:
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
					return true;
				case OpCode.Neg:
					self = new Value(-self.num.UInt);
					return true;
				case OpCode.Flip:
					self.num.UInt = ~self.num.UInt;
					return true;
				case OpCode.Not:
					self = new Value(self.num.Long == 0);
					return true;
				}
				return false;
			}
			public override bool Binary(ref Value lhs, OpCode op, ref Value rhs)
			{
				var rtype = rhs.desc.Primitive;
				if (rtype != ExCode.UInt)
				{
					if (rtype.Kind() != OpKind.Number)
						return false;
					if ((rtype & ExCode.fFp) != 0)
					{
						if (rtype != ExCode.Double)
							rhs.desc.Convert(ref rhs, Double);
						return false;
					}
					if (rtype.NumberSize() >= 4)
					{
						if (rtype != ExCode.ULong)
							rhs.desc.Convert(ref rhs, ULong);
						return false;
					}
				}
				if (lhs.desc.Primitive != ExCode.UInt && !lhs.desc.Convert(ref lhs, this))
					return false;
				switch (op)
				{
				case OpCode.BitOr:
					lhs.num.UInt |= rhs.num.UInt;
					return true;
				case OpCode.BitXor:
					lhs.num.UInt ^= rhs.num.UInt;
					return true;
				case OpCode.BitAnd:
					lhs.num.UInt &= rhs.num.UInt;
					return true;
				case OpCode.ShiftLeft:
					lhs.num.UInt <<= rhs.num.Int;
					return true;
				case OpCode.ShiftRight:
					lhs.num.UInt >>= rhs.num.Int;
					return true;
				case OpCode.Add:
					lhs.num.UInt += rhs.num.UInt;
					return true;
				case OpCode.Sub:
					lhs.num.UInt -= rhs.num.UInt;
					return true;
				case OpCode.Mul:
					lhs.num.UInt *= rhs.num.UInt;
					return true;
				case OpCode.Div:
					if (rhs.num.Long == 0)
						lhs = Value.NaN;
					else lhs.num.UInt /= rhs.num.UInt;
					return true;
				}
				return false;
			}
		}
	}
}
