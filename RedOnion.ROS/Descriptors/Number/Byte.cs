using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		internal class OfByte : Descriptor
		{
			internal OfByte()
				: base("byte", typeof(byte), ExCode.Byte, TypeCode.Byte) { }
			public override object Box(ref Value self)
				=> self.num.Byte;
			public override int GetHashCode(ref Value self)
				=> self.num.Byte.GetHashCode();
			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.Byte.ToString(format, provider);

			public override bool Call(ref Value result, object self, Arguments args, bool create = false)
			{
				if (args.Length != 1 || (result.obj != null && result.obj != this))
					return false;
				var it = args[0];
				if (!it.desc.Convert(ref it, this))
					return false;
				result = it;
				return true;
			}

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

			public override void Unary(ref Value self, OpCode op)
			{
				switch (op)
				{
				case OpCode.Plus:
					self = +self.num.Byte;
					return;
				case OpCode.Neg:
					self = -self.num.Byte;
					return;
				case OpCode.Flip:
					self = ~self.num.Byte;
					return;
				case OpCode.Not:
					self = new Value(self.num.Long == 0);
					return;
				case OpCode.Inc:
					self.num.Byte += 1;
					return;
				case OpCode.Dec:
					self.num.Byte -= 1;
					return;
				}
				UnaryError(op);
			}
			public override bool Equals(ref Value self, object obj)
			{
				if (!(obj is Value rhs))
					return self.num.Int.Equals(obj);
				if (rhs.desc == this)
					return self.num.Int == rhs.num.Int;
				var rtype = rhs.desc.Primitive;
				if (!rtype.IsNumberOrChar())
					return false;
				if (rtype.IsFloatPoint())
				{
					if (rtype != ExCode.Double)
						rhs.desc.Convert(ref rhs, Double);
					return self.num.Int == rhs.num.Double;
				}
				return rtype.IsSigned()
					? self.num.Long == rhs.num.Long
					: (ulong)self.num.Long == rhs.num.ULong;
			}
			public override bool Binary(ref Value lhs, OpCode op, ref Value rhs)
			{
				lhs.desc.Convert(ref lhs, Int);
				return lhs.desc.Binary(ref lhs, op, ref rhs);
			}
		}
	}
}
