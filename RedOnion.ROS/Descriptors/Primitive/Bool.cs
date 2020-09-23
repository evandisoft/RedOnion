using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		internal class OfBool : Descriptor
		{
			internal OfBool()
				: base("bool", typeof(bool), ExCode.Bool, TypeCode.Boolean) { }
			public override object Box(ref Value self)
				=> self.num.Bool;
			public override int GetHashCode(ref Value self)
				=> self.num.Bool.GetHashCode();
			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> self.num.Bool.ToString(provider);

			public override bool Call(ref Value result, object self, in Arguments args)
			{
				if (result.obj != (object)typeof(bool))
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

			public override void Unary(ref Value self, OpCode op)
			{
				switch(op)
				{
				case OpCode.Not:
					self.num.Bool = !self.num.Bool;
					return;
				}
				UnaryError(op);
			}

			public override bool Equals(ref Value self, object obj)
			{
				if (!(obj is Value rhs))
					return self.num.Bool.Equals(obj);
				if (rhs.desc == this)
					return self.num.Bool == rhs.num.Bool;
				var rtype = rhs.desc.Primitive;
				if (!rtype.IsNumberOrChar())
					return false;
				if (rtype != ExCode.Bool)
					rhs.desc.Convert(ref rhs, Bool);
				return self.num.Bool == rhs.num.Bool;
			}
		}
	}
}
