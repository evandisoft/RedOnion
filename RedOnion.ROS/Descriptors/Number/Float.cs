using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		internal class OfFloat : Descriptor
		{
			internal OfFloat()
				: base("float", typeof(float), ExCode.Float, TypeCode.Single) { }
			public override object Box(ref Value self)
				=> self.num.Float;
			public override int GetHashCode(ref Value self)
				=> self.num.Float.GetHashCode();
			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.Float.ToString(format, provider);

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
				result = (float)args[0].ToDouble();
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
					self = new Value((char)self.num.Double);
					return true;
				case ExCode.Byte:
					self = new Value((byte)self.num.Double);
					return true;
				case ExCode.UShort:
					self = new Value((ushort)self.num.Double);
					return true;
				case ExCode.UInt:
					self = new Value((uint)self.num.Double);
					return true;
				case ExCode.ULong:
					self = new Value((ulong)self.num.Double);
					return true;
				case ExCode.SByte:
					self = new Value((sbyte)self.num.Double);
					return true;
				case ExCode.Short:
					self = new Value((short)self.num.Double);
					return true;
				case ExCode.Int:
					self = new Value((int)self.num.Double);
					return true;
				case ExCode.Long:
					self = new Value((long)self.num.Double);
					return true;
				case ExCode.Number:
				case ExCode.Float:
					return true;
				case ExCode.Double:
					self = new Value(self.num.Double);
					return true;
				case ExCode.Bool:
					var d = self.num.Double;
					self = new Value(!double.IsNaN(d) && d != 0.0);
					return true;
				}
				return false;
			}

			public override void Unary(ref Value self, OpCode op)
			{
				switch (op)
				{
				case OpCode.Plus:
					return;
				case OpCode.Neg:
					self.num.Float = -self.num.Float;
					return;
				case OpCode.Not:
					var d = self.num.Double;
					self = new Value(double.IsNaN(d) || d == 0.0);
					return;
				}
				UnaryError(op);
			}
			public override bool Equals(ref Value self, object obj)
			{
				if (!(obj is Value rhs))
					return self.num.Float.Equals(obj);
				if (rhs.desc == this)
					return self.num.Float == rhs.num.Float;
				var rtype = rhs.desc.Primitive;
				if (!rtype.IsNumberOrChar())
					return false;
				if (rtype != ExCode.Double)
					rhs.desc.Convert(ref rhs, Double);
				return self.num.Float == rhs.num.Double;
			}
			public override bool Binary(ref Value lhs, OpCode op, ref Value rhs)
			{
				var rtype = rhs.desc.Primitive;
				if (rtype != ExCode.Float)
				{
					if (rtype == ExCode.Double)
						return false;
					if (!rhs.desc.Convert(ref rhs, this))
						return false;
				}
				if (lhs.desc.Primitive != ExCode.Float && !lhs.desc.Convert(ref lhs, this))
					return false;
				switch (op)
				{
				case OpCode.Add:
					lhs.num.Float += rhs.num.Float;
					return true;
				case OpCode.Sub:
					lhs.num.Float -= rhs.num.Float;
					return true;
				case OpCode.Mul:
					lhs.num.Float *= rhs.num.Float;
					return true;
				case OpCode.Div:
					lhs.num.Float /= rhs.num.Float;
					return true;
				case OpCode.Power:
				case OpCode.BitXor:
					lhs.num.Float = (float)Math.Pow(lhs.num.Float, rhs.num.Float);
					return true;

				case OpCode.Equals:
					lhs = lhs.num.Float == rhs.num.Float;
					return true;
				case OpCode.Differ:
					lhs = lhs.num.Float != rhs.num.Float;
					return true;
				case OpCode.Less:
					lhs = lhs.num.Float < rhs.num.Float;
					return true;
				case OpCode.More:
					lhs = lhs.num.Float > rhs.num.Float;
					return true;
				case OpCode.LessEq:
					lhs = lhs.num.Float <= rhs.num.Float;
					return true;
				case OpCode.MoreEq:
					lhs = lhs.num.Float >= rhs.num.Float;
					return true;
				}
				return false;
			}
		}
	}
}
