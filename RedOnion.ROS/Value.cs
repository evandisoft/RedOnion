using RedOnion.ROS.Objects;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RedOnion.ROS
{
	/// <summary>
	/// Value with descriptor.
	/// </summary>
	[DebuggerDisplay("{DebugString}")]
	public struct Value : IFormattable /* TODO: IConvertible */
	{
		/// <summary>
		/// Culture settings for formatting (invariant by default).
		/// </summary>
		public static CultureInfo Culture = CultureInfo.InvariantCulture;

		/// <summary>
		/// The descriptor for the object
		/// </summary>
		public Descriptor desc;
		/// <summary>
		/// The object itself (if any)
		/// </summary>
		public object obj;
		/// <summary>
		/// Numeric data
		/// </summary>
		internal NumericData num;

		/// <summary>
		/// The descriptor for the object
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Descriptor Descriptor => desc;

		/// <summary>
		/// The object or boxed value
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public object Object => desc.Box(ref this);

		public static readonly Value Void = new Value(Descriptor.Void, null);
		public static readonly Value Null = new Value(Descriptor.Null, null);
		public static readonly Value NaN = new Value(double.NaN);
		public static readonly Value False = new Value(false);
		public static readonly Value True = new Value(false);

		public Value(UserObject it) : this(it, it) {}
		public Value(Descriptor descriptor, object it)
		{
			desc = descriptor;
			obj = it;
			num = new NumericData();
		}
		internal Value(Descriptor descriptor, NumericData it)
		{
			desc = descriptor;
			obj = null;
			num = it;
		}
		internal Value(Descriptor descriptor, object it, NumericData data)
		{
			desc = descriptor;
			obj = it;
			num = data;
		}

		public Value(string s)
			: this(Descriptor.String, s) { }
		public override string ToString()
			=> DebugString;
		public string ToString(string format, IFormatProvider provider)
			=> desc.ToString(ref this, format, provider, true);

		public Value(double v) : this(Descriptor.Double, null) => num.Double = v;
		public Value(float v) : this(Descriptor.Float, null) => num.Float = v;
		public Value(int v) : this(Descriptor.Int, null) => num.Int = v;
		public Value(uint v) : this(Descriptor.UInt, null) => num.UInt = v;
		public Value(long v) : this(Descriptor.Long, null) => num.Long = v;
		public Value(char v) : this(Descriptor.Char, null) => num.Char = v;
		public Value(bool v) : this(Descriptor.Bool, null) => num.Bool = v;
		public Value(byte v) : this(Descriptor.Byte, null) => num.Byte = v;
		public Value(sbyte v) : this(Descriptor.SByte, null) => num.SByte = v;
		public Value(short v) : this(Descriptor.Short, null) => num.Short = v;
		public Value(ushort v) : this(Descriptor.UShort, null) => num.UShort = v;
		public Value(ulong v) : this(Descriptor.ULong, null) => num.ULong = v;

		public static implicit operator Value(string s) => new Value(s);
		public static implicit operator Value(double v) => new Value(v);
		public static implicit operator Value(float v) => new Value(v);
		public static implicit operator Value(int v) => new Value(v);
		public static implicit operator Value(uint v) => new Value(v);
		public static implicit operator Value(long v) => new Value(v);
		public static implicit operator Value(char v) => new Value(v);
		public static implicit operator Value(bool v) => new Value(v);
		public static implicit operator Value(byte v) => new Value(v);
		public static implicit operator Value(sbyte v) => new Value(v);
		public static implicit operator Value(short v) => new Value(v);
		public static implicit operator Value(ushort v) => new Value(v);
		public static implicit operator Value(ulong v) => new Value(v);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string Name => desc.Name;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsNumber => desc.Primitive > ExCode.String;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsInt => desc.Primitive == ExCode.Int;

		public int ToInt()
		{
			var type = desc.Primitive;
			if (type == ExCode.Int)
				return num.Int;
			if (type.Kind() == OpKind.Number)
				return type == ExCode.Double || type == ExCode.Float
					? (int)num.Double : (int)num.Long;
			var it = this;
			if (it.desc.Convert(ref it, Descriptor.Int))
				return it.num.Int;
			throw InvalidOperation("Could not convert {0} to int", this);
		}

		/// <summary>
		/// Numeric data
		/// </summary>
		[StructLayout(LayoutKind.Explicit)]
		[DebuggerDisplay("{Double}|{Long}|{HighInt}|{Int}")]
		internal struct NumericData
		{
			[FieldOffset(0)]
			public long Long;
			[FieldOffset(0)]
			public double Double;

			public float Float
			{
				get => (float)Double;
				set => Double = value;
			}
			public int Int
			{
				get => (int)Long;
				set => Long = value;
			}
			public uint UInt
			{
				get => (uint)Long;
				set => Long = value;
			}
			public bool Bool
			{
				get => Long != 0;
				set => Long = value ? 1 : 0;
			}
			public byte Byte
			{
				get => (byte)Long;
				set => Long = value;
			}
			public sbyte SByte
			{
				get => (sbyte)Long;
				set => Long = value;
			}
			public short Short
			{
				get => (short)Long;
				set => Long = value;
			}
			public ushort UShort
			{
				get => (ushort)Long;
				set => Long = value;
			}
			public ulong ULong
			{
				get => (ulong)Long;
				set => Long = (long)value;
			}
			public char Char
			{
				get => (char)Long;
				set => Long = value;
			}

			public NumericData(long v) : this() => Long = v;
			public NumericData(double v) : this() => Double = v;
			public NumericData(float v) : this() => Double = v;
			public NumericData(int v) : this() => Long = v;
			public NumericData(uint v) : this() => Long = v;
			public NumericData(bool v) : this() => Long = v ? 1 : 0;
			public NumericData(byte v) : this() => Long = v;
			public NumericData(sbyte v) : this() => Long = v;
			public NumericData(short v) : this() => Long = v;
			public NumericData(ushort v) : this() => Long = v;
			public NumericData(ulong v) : this() => Long = (long)v;
			public NumericData(char v) : this() => Long = v;

			internal NumericData(uint lo, int hi) : this() => Long = lo | ((long)hi << 32);
			internal NumericData(int lo, int hi) : this((uint)lo, hi) { }
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			internal int HighInt
			{
				get => (int)(Long >> 32);
				set => Long = (uint)Long | ((long)value << 32);
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal bool IsReference => num.HighInt != 0 && (OpCode)desc.Primitive <= OpCode.String;
		internal void SetRef(int idx) => num.Long = (uint)idx | ((long)~idx << 32);
		internal void SetRef(Context ctx, int idx)
		{
			obj = desc = ctx;
			num.Long = (uint)idx | ((long)~idx << 32);
		}

		static internal InvalidOperationException InvalidOperation(string msg, params object[] args)
			=> new InvalidOperationException(string.Format(Culture, msg, args));

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebugString
		{
			get
			{
				if (IsReference)
				{
					var name = desc.NameOf(obj, num.Int);
					return string.Format(Culture, name == null || name.Length == 0 || name[0] == '#'
						? "'{0}'#{1}" : "'{0}'.{2}", desc.Name, num.Int, name);
				}
				return obj == desc ? desc.Name : string.Format(Culture,
					"{0}: {1}", desc.Name, desc.ToString(ref this, null, Culture, true));
			}
		}
	}
}
