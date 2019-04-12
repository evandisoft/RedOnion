using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		internal class OfSByte : Descriptor
		{
			internal OfSByte()
				: base("sbyte", typeof(sbyte), ExCode.SByte, TypeCode.SByte) { }
			public override object Box(ref Value self)
				=> self.num.SByte;
			public override bool Equals(ref Value self, object obj)
				=> self.num.SByte.Equals(obj);
			public override int GetHashCode(ref Value self)
				=> self.num.SByte.GetHashCode();
			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.SByte.ToString(format, provider);

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
				case ExCode.Number:
				case ExCode.SByte:
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
					self = +self.num.SByte;
					return true;
				case OpCode.Neg:
					self = -self.num.SByte;
					return true;
				case OpCode.Flip:
					self = ~self.num.SByte;
					return true;
				case OpCode.Not:
					self = new Value(self.num.Long == 0);
					return true;
				case OpCode.Inc:
					self.num.SByte += 1;
					return true;
				case OpCode.Dec:
					self.num.SByte -= 1;
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
