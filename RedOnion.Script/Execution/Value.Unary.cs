using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script
{
	public partial struct Value
	{
		public static Value operator !(Value value)
			=> !value.Bool;

		public static Value operator +(Value value)
		{
		again:
			if (value.IsNumber)
				return value;
			if (value.Type == ValueKind.Reference)
			{
				value = ((IProperties)value.ptr).Get(value.str);
				goto again;
			}
			return new Value();
		}

		public static Value operator -(Value value)
		{
		again:
			switch (value.Type)
			{
			default:
				return new Value();
			case ValueKind.Reference:
				value = ((IProperties)value.ptr).Get(value.str);
				goto again;
			case ValueKind.Char:
				return new Value((char)0) - value.data.Char;
			case ValueKind.Bool:
				return new Value(value.data.Bool ? -1 : 0);
			case ValueKind.Byte:
				return new Value((byte)0) - value.data.Byte;
			case ValueKind.UShort:
				return new Value((ushort)0) - value.data.UShort;
			case ValueKind.UInt:
				return new Value((uint)0) - value.data.UInt;
			case ValueKind.ULong:
				return new Value((ulong)0) - value.data.ULong;
			case ValueKind.SByte:
				return new Value((sbyte)0) - value.data.SByte;
			case ValueKind.Short:
				return new Value((short)0) - value.data.Short;
			case ValueKind.Int:
				return new Value((int)0) - value.data.Int;
			case ValueKind.Long:
				return new Value((long)0) - value.data.Long;
			case ValueKind.Float:
				return new Value((float)0) - value.data.Float;
			case ValueKind.Double:
				return new Value((double)0) - value.data.Double;
			}
		}

		public static Value operator ~(Value value)
		{
		again:
			switch (value.Type)
			{
			default:
				return new Value();
			case ValueKind.Reference:
				value = ((IProperties)value.ptr).Get(value.str);
				goto again;
			case ValueKind.Char:
				return new Value(~value.data.Char);
			case ValueKind.Bool:
				return new Value(!value.data.Bool);
			case ValueKind.Byte:
				return new Value(~value.data.Byte);
			case ValueKind.UShort:
				return new Value(~value.data.UShort);
			case ValueKind.UInt:
				return new Value(~value.data.UInt);
			case ValueKind.ULong:
				return new Value(~value.data.ULong);
			case ValueKind.SByte:
				return new Value(~value.data.SByte);
			case ValueKind.Short:
				return new Value(~value.data.Short);
			case ValueKind.Int:
				return new Value(~value.data.Int);
			case ValueKind.Long:
				return new Value(~value.data.Long);
			case ValueKind.Float:
				return new Value(~(long)value.data.Float);
			case ValueKind.Double:
				return new Value(~(long)value.data.Double);
			}
		}

		public static Value operator ++(Value value)
		{
		again:
			switch (value.Type)
			{
			default:
				return new Value();
			case ValueKind.Reference:
				value = ((IProperties)value.ptr).Get(value.str);
				goto again;
			case ValueKind.Char:
				return new Value((char)(value.data.Char + 1));
			case ValueKind.Bool:
				return new Value(!value.data.Bool);
			case ValueKind.Byte:
				return new Value((byte)(value.data.Byte + 1u));
			case ValueKind.UShort:
				return new Value((ushort)(value.data.UShort + 1u));
			case ValueKind.UInt:
				return new Value(value.data.UInt + 1u);
			case ValueKind.ULong:
				return new Value(value.data.ULong + 1u);
			case ValueKind.SByte:
				return new Value((sbyte)(value.data.SByte + 1));
			case ValueKind.Short:
				return new Value((short)(value.data.Short + 1));
			case ValueKind.Int:
				return new Value(value.data.Int + 1);
			case ValueKind.Long:
				return new Value(value.data.Long + 1);
			case ValueKind.Float:
				return new Value(value.data.Float + 1f);
			case ValueKind.Double:
				return new Value(value.data.Double + 1.0);
			}
		}

		public static Value operator --(Value value)
		{
		again:
			switch (value.Type)
			{
			default:
				return new Value();
			case ValueKind.Reference:
				value = ((IProperties)value.ptr).Get(value.str);
				goto again;
			case ValueKind.Char:
				return new Value((char)(value.data.Char - 1));
			case ValueKind.Bool:
				return new Value(!value.data.Bool);
			case ValueKind.Byte:
				return new Value(unchecked((byte)(value.data.Byte - 1u)));
			case ValueKind.UShort:
				return new Value((ushort)(value.data.UShort - 1u));
			case ValueKind.UInt:
				return new Value(value.data.UInt - 1u);
			case ValueKind.ULong:
				return new Value(value.data.ULong - 1u);
			case ValueKind.SByte:
				return new Value((sbyte)(value.data.SByte - 1));
			case ValueKind.Short:
				return new Value((short)(value.data.Short - 1));
			case ValueKind.Int:
				return new Value(value.data.Int - 1);
			case ValueKind.Long:
				return new Value(value.data.Long - 1);
			case ValueKind.Float:
				return new Value(value.data.Float - 1f);
			case ValueKind.Double:
				return new Value(value.data.Double - 1.0);
			}
		}
	}
}
