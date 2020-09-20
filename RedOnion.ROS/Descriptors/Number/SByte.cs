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
			public override int GetHashCode(ref Value self)
				=> self.num.SByte.GetHashCode();
			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.SByte.ToString(format, provider);

			public override bool Call(ref Value result, object self, in Arguments args)
			{
				if (result.obj != this)
					return false;
				if (args.Length != 1)
				{
					if (args.Length == 0)
					{
						result = new Value(this, null);
						return true;
					}
					return false;
				}
				var it = args[0];
				if (!it.desc.Convert(ref it, this))
					return false;
				result = it;
				return true;
			}

			public override bool Convert(ref Value self, Descriptor to, CallFlags flags = CallFlags.Convert)
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

			public override void Unary(ref Value self, OpCode op)
			{
				switch (op)
				{
				case OpCode.Plus:
					self = +self.num.SByte;
					return;
				case OpCode.Neg:
					self = -self.num.SByte;
					return;
				case OpCode.Flip:
					self = ~self.num.SByte;
					return;
				case OpCode.Not:
					self = new Value(self.num.Long == 0);
					return;
				case OpCode.Inc:
					self.num.SByte += 1;
					return;
				case OpCode.Dec:
					self.num.SByte -= 1;
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
