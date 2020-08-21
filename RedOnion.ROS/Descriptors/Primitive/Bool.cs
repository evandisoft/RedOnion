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
