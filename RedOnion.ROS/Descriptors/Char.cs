using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static readonly Descriptor Char = new OfChar();

		internal class OfChar : Descriptor
		{
			internal OfChar()
				: base("char", typeof(char), ExCode.WideChar, TypeCode.Char) { }
			public override object Box(ref Value self)
				=> self.num.Char;
			public override bool Equals(ref Value self, object obj)
				=> self.num.Char.Equals(obj);
			public override int GetHashCode(ref Value self)
				=> self.num.Char.GetHashCode();
			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.Char.ToString(provider);

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
					self = +self.num.Char;
					return true;
				case OpCode.Neg:
					self = -self.num.Char;
					return true;
				case OpCode.Flip:
					self = ~self.num.Char;
					return true;
				case OpCode.Not:
					self = new Value(self.num.Long == 0);
					return true;
				case OpCode.Inc:
					self.num.Char += (char)1;
					return true;
				case OpCode.Dec:
					self.num.Char -= (char)1;
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
