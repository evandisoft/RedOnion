using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace RedOnion.Script
{
	public enum ValueKind
	{
		Undefined	= 0x0000,// Undefined value
		Object		= 0x0001,// Engine object (ptr is IObject)
		Create		= 0x0002,// Lazy-create object (ptr is Create) - in ptr.ro only
		Property	= 0x0003,// Property (ptr is iProp) - in ptr.ro only
		Reference	= 0x0008,// Identifier / property reference (in str, ptr is IProperties)
		Byte		= 0x0110,// 8 bit unsigned
		UShort		= 0x0211,// 16 bit unsigned
		UInt		= 0x0412,// 32 bit unsigned
		ULong		= 0x0813,// 64 bit unsigned
		SByte		= 0x4114,// 8 bit signed
		Short		= 0x4215,// s16
		Int			= 0x4416,// s32
		Long		= 0x4817,// s64
		Float		= 0xC418,// f32
		Double		= 0xC819,// f64
		Bool		= 0x011B,// u1
		String		= 0x0029,// string
		Char		= 0x023A,// char

		fStr		= 0x0020,// is string or char
		fNum		= 0x0010,// is number
		f64			= 0x1800,// is 64bit or more
		fFp			= 0x8000,// floating point
		fSig		= 0x4000,// signed
		mSz			= 0x3F00,// number size mask
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct ValueData
	{
		[FieldOffset(0)]
		public long Long;
		[FieldOffset(0)]
		public double Double;

		public bool Bool => Long != 0;
		public char Char => (char)Long;
		public byte Byte => (byte)Long;
		public ushort UShort => (ushort)Long;
		public uint UInt => (uint)Long;
		public ulong ULong => (ulong)Long;
		public sbyte SByte => (sbyte)Long;
		public short Short => (short)Long;
		public int Int => (int)Long;
		public float Float => (float)Double;
	}

	[DebuggerDisplay("{type}; ptr: {ptr}; str: {str}; long: {data.Long}; double: {data.Double}")]
	public partial struct Value
	{
		private ValueKind type;
		internal Object ptr;
		internal string str;
		internal ValueData data;

		internal Value(ValueKind vtype)
		{
			type = vtype;
			ptr = null;
			str = null;
			data = new ValueData();
		}

		internal Value(ValueKind vtype, object value)
		{
			type = vtype;
			ptr = value;
			str = null;
			data = new ValueData();
		}

		internal Value(ValueKind vtype, object obj, string name)
		{
			type = vtype;
			ptr = obj;
			str = name;
			data = new ValueData();
		}

		internal Value(ValueKind vtype, long value)
		{
			type = vtype;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Long = value;
		}

		internal Value(ValueKind vtype, double value)
		{
			type = vtype;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Double = value;
		}

		public Value(Value value)
		{
			type = value.Type;
			ptr = value.ptr;
			str = value.str;
			data = value.data;
		}

		public Value(IObject obj)
		{
			type = ValueKind.Object;
			ptr = obj;
			str = null;
			data = new ValueData();
		}

		public Value(IProperty prop)
		{
			type = ValueKind.Property;
			ptr = prop;
			str = null;
			data = new ValueData();
		}

		public Value(IProperties obj, string name)
		{
			type = ValueKind.Reference;
			ptr = obj;
			str = name;
			data = new ValueData();
		}

		public static implicit operator Value(string value)
		{
			return new Value(value);
		}

		public Value(string value)
		{
			type = ValueKind.String;
			ptr = null;
			str = value;
			data = new ValueData();
		}

		public static implicit operator Value(char value)
		{
			return new Value(value);
		}

		public Value(char value)
		{
			type = ValueKind.Char;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(bool value)
		{
			return new Value(value);
		}

		public Value(bool value)
		{
			type = ValueKind.Bool;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Long = value ? 1 : 0;
		}

		public static implicit operator Value(byte value)
		{
			return new Value(value);
		}

		public Value(byte value)
		{
			type = ValueKind.Byte;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(ushort value)
		{
			return new Value(value);
		}

		public Value(ushort value)
		{
			type = ValueKind.UShort;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(uint value)
		{
			return new Value(value);
		}

		public Value(uint value)
		{
			type = ValueKind.UInt;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(ulong value)
		{
			return new Value(value);
		}

		public Value(ulong value)
		{
			type = ValueKind.ULong;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Long = (long)value;
		}

		public static implicit operator Value(sbyte value)
		{
			return new Value(value);
		}

		public Value(sbyte value)
		{
			type = ValueKind.SByte;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(short value)
		{
			return new Value(value);
		}

		public Value(short value)
		{
			type = ValueKind.Short;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(int value)
		{
			return new Value(value);
		}

		public Value(int value)
		{
			type = ValueKind.Int;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(long value)
		{
			return new Value(value);
		}

		public Value(long value)
		{
			type = ValueKind.Long;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(float value)
		{
			return new Value(value);
		}

		public Value(float value)
		{
			type = ValueKind.Float;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Double = value;
		}

		public static implicit operator Value(double value)
		{
			return new Value(value);
		}

		public Value(double value)
		{
			type = ValueKind.Double;
			ptr = null;
			str = null;
			data = new ValueData();
			data.Double = value;
		}
	}
}
