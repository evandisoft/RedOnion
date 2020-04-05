using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		internal class OfLong : Descriptor
		{
			internal OfLong()
				: base("long", typeof(long), ExCode.Long, TypeCode.Int64) { }
			public override object Box(ref Value self)
				=> self.num.Long;
			public override int GetHashCode(ref Value self)
				=> self.num.Long.GetHashCode();
			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.Long.ToString(format, provider);

			public override bool Call(ref Value result, object self, Arguments args, bool create = false)
			{
				if (args.Length != 1 || (result.obj != null && result.obj != this))
					return false;
				result = args[0].ToLong();
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
					case ExCode.Int:
						self = new Value(self.num.Int);
						return true;
					case ExCode.Number:
					case ExCode.Long:
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
						self.num.Long = -self.num.Long;
						return true;
					case OpCode.Flip:
						self.num.Long = ~self.num.Long;
						return true;
					case OpCode.Not:
						self = new Value(self.num.Long == 0);
						return true;
				}
				return false;
			}
			public override bool Equals(ref Value self, object obj)
			{
				if (!(obj is Value rhs))
					return self.num.Long.Equals(obj);
				if (rhs.desc == this)
					return self.num.Long == rhs.num.Long;
				var rtype = rhs.desc.Primitive;
				if (!rtype.IsNumberOrChar())
					return false;
				if (rtype.IsFloatPoint())
				{
					if (rtype != ExCode.Double)
						rhs.desc.Convert(ref rhs, Double);
					return self.num.Long == rhs.num.Double;
				}
				return self.num.Long == rhs.num.Long;
			}
			public override bool Binary(ref Value lhs, OpCode op, ref Value rhs)
			{
				var rtype = rhs.desc.Primitive;
				if (rtype != ExCode.Long)
				{
					if (!rtype.IsNumberOrChar())
						return false;
					if (rtype.IsFloatPoint())
					{
						if (rtype != ExCode.Double)
							rhs.desc.Convert(ref rhs, Double);
						return false;
					}
					rhs.desc.Convert(ref rhs, Long);
				}
				if (lhs.desc.Primitive != ExCode.Long && !lhs.desc.Convert(ref lhs, this))
					return false;
				switch (op)
				{
					case OpCode.BitOr:
						lhs.num.Long |= rhs.num.Long;
						return true;
					case OpCode.BitXor:
						lhs.num.Long ^= rhs.num.Long;
						return true;
					case OpCode.BitAnd:
						lhs.num.Long &= rhs.num.Long;
						return true;
					case OpCode.ShiftLeft:
						lhs.num.Long <<= rhs.num.Int;
						return true;
					case OpCode.ShiftRight:
						lhs.num.Long >>= rhs.num.Int;
						return true;
					case OpCode.Add:
						lhs.num.Long += rhs.num.Long;
						return true;
					case OpCode.Sub:
						lhs.num.Long -= rhs.num.Long;
						return true;
					case OpCode.Mul:
						lhs.num.Long *= rhs.num.Long;
						return true;
					case OpCode.Div:
						if (rhs.num.Long == 0)
							lhs = Value.NaN;
						else lhs.num.Long /= rhs.num.Long;
						return true;
					case OpCode.Power:
						lhs = Math.Pow(lhs.num.Long, rhs.num.Long);
						return true;
					case OpCode.Equals:
						lhs = lhs.num.Long == rhs.num.Long;
						return true;
					case OpCode.Differ:
						lhs = lhs.num.Long != rhs.num.Long;
						return true;
					case OpCode.Less:
						lhs = lhs.num.Long < rhs.num.Long;
						return true;
					case OpCode.More:
						lhs = lhs.num.Long > rhs.num.Long;
						return true;
					case OpCode.LessEq:
						lhs = lhs.num.Long <= rhs.num.Long;
						return true;
					case OpCode.MoreEq:
						lhs = lhs.num.Long >= rhs.num.Long;
						return true;
				}
				return false;
			}
		}
	}
}
