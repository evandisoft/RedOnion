using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static readonly Descriptor Int = new OfInt();

		internal class OfInt : Descriptor
		{
			internal OfInt() : base("int", typeof(int), ExCode.Int, TypeCode.Int32) { }
			public override object Box(ref Value self) => self.num.Int;

			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.Int.ToString(format, provider);

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
				case ExCode.Number:
				case ExCode.Int:
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
					var v = self.num.Int;
					if (v == int.MinValue)
					{
						self = new Value(-(long)v);
						return true;
					}
					self.num.Int = -v;
					return true;
				case OpCode.Flip:
					self.num.Int = ~self.num.Int;
					return true;
				case OpCode.Not:
					self = new Value(self.num.Long == 0);
					return true;
				case OpCode.Inc:
					self.num.Int += 1;
					return true;
				case OpCode.Dec:
					self.num.Int -= 1;
					return true;
				}
				return false;
			}
			public override bool Binary(ref Value lhs, OpCode op, ref Value rhs)
			{
				var rtype = rhs.desc.Primitive;
				if (rtype != ExCode.Int)
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
						if (rtype != ExCode.Long)
							rhs.desc.Convert(ref rhs, Long);
						return false;
					}
				}
				if (lhs.desc.Primitive != ExCode.Int && !lhs.desc.Convert(ref lhs, this))
					return false;
				switch (op)
				{
				case OpCode.BitOr:
					lhs.num.Int |= rhs.num.Int;
					return true;
				case OpCode.BitXor:
					lhs.num.Int ^= rhs.num.Int;
					return true;
				case OpCode.BitAnd:
					lhs.num.Int &= rhs.num.Int;
					return true;
				case OpCode.ShiftLeft:
					lhs.num.Int <<= rhs.num.Int;
					return true;
				case OpCode.ShiftRight:
					lhs.num.Int >>= rhs.num.Int;
					return true;
				case OpCode.Add:
					lhs.num.Int += rhs.num.Int;
					return true;
				case OpCode.Sub:
					lhs.num.Int -= rhs.num.Int;
					return true;
				case OpCode.Mul:
					lhs.num.Int *= rhs.num.Int;
					return true;
				case OpCode.Div:
					if (rhs.num.Long == 0)
						lhs = Value.NaN;
					else lhs.num.Int /= rhs.num.Int;
					return true;
				}
				return false;
			}
		}
	}
}
