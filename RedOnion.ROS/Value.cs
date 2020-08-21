using RedOnion.ROS.Objects;
using RedOnion.Utilities;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// keep all static fields in DescriptorOf.cs (not to mess the order of initialization)

/*	The reason for using struct is to avoid memory allocation
 *	(and therefore frequent garbage collection),
 *	because these would get created very often being a class.
 *	Most values are instead held in Core.Arguments (ArgumentList vals),
 *	which uses Utilities.ListCore<Value>, which maintains Value[].
 *	Rarely returned from functions, ref Value argument used instead
 *	(first argument is most often reused for result output,
 *	just like in real CPUs some register like AX or R0 is used).
 */
namespace RedOnion.ROS
{
	/// <summary>
	/// Object that can provide descriptor for itself
	/// </summary>
	public interface ISelfDescribing
	{
		Descriptor Descriptor { get; }
	}

	/// <summary>
	/// Value with descriptor.
	/// </summary>
	[DebuggerDisplay("{DebugString}")]
	public struct Value : IFormattable /* TODO: IConvertible */
	{
		/// <summary>
		/// Culture settings for formatting (invariant by default).
		/// </summary>
		public static CultureInfo Culture
		{
			get => Descriptor.Culture;
			set => Descriptor.Culture = value;
		}
		public static string Format(FormattableString msg)
			=> msg.ToString(Culture);
		public static string Format(StringWrapper msg, params object[] args)
			=> args?.Length > 0 ? string.Format(Culture, msg.String, args) : msg.String;

		/// <summary>
		/// The descriptor - how the core interacts with the value
		/// or what type it represents if obj == null
		/// (except for null and void - these are type-less values)
		/// </summary>
		public Descriptor desc;
		/// <summary>
		/// The object unless primitive value (number, null, void or type).
		/// </summary>
		public object obj;
		/// <summary>
		/// Null for lvalues, string for property references,
		/// ValueBox with index for simple indexing, Value[] for complex indexing.
		/// </summary>
		public object idx;
		/// <summary>
		/// Numeric / extra data. Only internal / built-in descriptors can use this,
		/// standard descriptors (of objects) are not allowed to use this,
		/// because non-zero numeric data marks references (and possibly other things).
		/// </summary>
		public NumericData num;

		// helper to be used in `idx` for simple integer indexing (real index in num.Int)
		public static readonly object IntIndex = new object();

		// avoid static initialization that would reference Descriptor!
		// could be creating `Void` when `Descriptor.Void` is still null!
		public static Value Void => Descriptor.VoidValue;
		public static Value Null => Descriptor.NullValue;
		public static Value NaN => Descriptor.NaNValue;
		public static Value False => Descriptor.FalseValue;
		public static Value True => Descriptor.TrueValue;

		public Value(Descriptor it) : this(it ?? Descriptor.Null, it) { }
		public Value(Type type) : this(Descriptor.Of(type), null) { }
		public Value(object it) : this()
		{
			if (it == null)
			{
				desc = Descriptor.Null;
				return;
			}
			if (it is Value v)
			{
				desc = v.desc;
				obj = v.obj;
				num = v.num;
				return;
			}
			if (it is Descriptor d)
			{
				obj = desc = d;
				return;
			}
			if (it is ISelfDescribing sd)
			{
				obj = it;
				desc = sd.Descriptor;
				return;
			}
			if (it is Type t)
			{
				desc = Descriptor.Of(t);
				return;
			}
			desc = Descriptor.Of(it.GetType());
			if (!IsNumberOrChar)
			{
				obj = it;
				return;
			}
			var n = (IConvertible)it;
			switch ((OpCode)desc.Primitive)
			{
			case OpCode.Char:
			case OpCode.WideChar:
				num.Char = n.ToChar(Culture);
				return;

			case OpCode.Byte:
				num.Byte = n.ToByte(Culture);
				return;
			case OpCode.UShort:
				num.UShort = n.ToUInt16(Culture);
				return;
			case OpCode.UInt:
				num.UInt = n.ToUInt32(Culture);
				return;
			case OpCode.ULong:
				num.ULong = n.ToUInt64(Culture);
				return;
			case OpCode.SByte:
				num.SByte = n.ToSByte(Culture);
				return;
			case OpCode.Short:
				num.Short = n.ToInt16(Culture);
				return;
			case OpCode.Int:
				num.Int = n.ToInt32(Culture);
				return;
			case OpCode.Long:
				num.Long = n.ToInt64(Culture);
				return;
			case OpCode.Float:
				num.Float = (float)n.ToDouble(Culture);
				return;
			case OpCode.Double:
				num.Double = n.ToDouble(Culture);
				return;
			case OpCode.Bool:
				num.Bool = n.ToBoolean(Culture);
				return;
			}
		}

		public Value(Descriptor descriptor, object it)
		{
			desc = descriptor;
			obj = it;
			idx = null;
			num = new NumericData();
		}
		internal Value(Descriptor descriptor, object it, string name)
		{
			desc = descriptor;
			obj = it;
			idx = name;
			num = new NumericData();
		}
		internal Value(Descriptor descriptor, object it, int i)
		{
			desc = descriptor;
			obj = it;
			idx = IntIndex;
			num = new NumericData(i);
		}
		internal Value(Descriptor descriptor, NumericData it)
		{
			desc = descriptor;
			obj = null;
			idx = null;
			num = it;
		}
		internal Value(Descriptor descriptor, object it, NumericData data)
		{
			desc = descriptor;
			obj = it;
			idx = null;
			num = data;
		}

		public Value(string s)
			: this(s == null ? Descriptor.Null : Descriptor.String, s) { }
		public override string ToString()
			=> DebugString;
		public string ToString(string format, IFormatProvider provider)
			=> desc.ToString(ref this, format, provider, true);
		public string ToStr()
			=> desc.ToString(ref this, null, Culture, false);

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

		public static bool operator ==(Value lhs, Value rhs)
			=> lhs.desc.Equals(ref lhs, rhs);
		public static bool operator !=(Value lhs, Value rhs)
			=> !lhs.desc.Equals(ref lhs, rhs);
		public static bool operator ==(Value lhs, object rhs)
			=> lhs.desc.Equals(ref lhs, rhs);
		public static bool operator !=(Value lhs, object rhs)
			=> !lhs.desc.Equals(ref lhs, rhs);
		public static bool operator ==(object lhs, Value rhs)
			=> rhs.desc.Equals(ref rhs, lhs);
		public static bool operator !=(object lhs, Value rhs)
			=> !rhs.desc.Equals(ref rhs, lhs);
		public override bool Equals(object obj)
			=> desc.Equals(ref this, obj);
		public override int GetHashCode()
			=> desc.GetHashCode(ref this);

		public string Name => desc.Name;
		public object Box() => desc.Box(ref this);
		public Value(Action action) : this(Descriptor.Actions[0], action) { }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsVoid => desc.Primitive == ExCode.Void;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsNull => desc.Primitive == ExCode.Null;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsFunction => desc.Primitive == ExCode.Function;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsString => desc.Primitive == ExCode.String;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsStringOrChar => desc.IsStringOrChar;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsNumber => desc.IsNumber;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsNumberOrChar => desc.IsNumberOrChar;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsFpNumber => desc.IsFpNumber;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsIntegral => desc.IsIntegral;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsInt => desc.Primitive == ExCode.Int;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsReference => idx != null;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsIntIndex => ReferenceEquals(idx, IntIndex);

		public void Dereference()
		{
			if (IsReference)
				desc.Get(ref this);
		}

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
			throw new InvalidOperation("Could not convert {0} to int", this);
		}
		public double ToDouble()
		{
			var type = desc.Primitive;
			if (type == ExCode.Double || type == ExCode.Float)
				return num.Double;
			if (type.Kind() == OpKind.Number)
				return num.Long;
			var it = this;
			if (it.desc.Convert(ref it, Descriptor.Double))
				return it.num.Double;
			throw new InvalidOperation("Could not convert {0} to double", this);
		}
		public uint ToUInt()
		{
			var type = desc.Primitive;
			if (type == ExCode.UInt)
				return num.UInt;
			if (type.Kind() == OpKind.Number)
				return type == ExCode.Double || type == ExCode.Float
					? (uint)num.Double : (uint)num.Long;
			var it = this;
			if (it.desc.Convert(ref it, Descriptor.UInt))
				return it.num.UInt;
			throw new InvalidOperation("Could not convert {0} to uint", this);
		}
		public long ToLong()
		{
			var type = desc.Primitive;
			if (type == ExCode.Long)
				return num.Long;
			if (type.Kind() == OpKind.Number)
				return type == ExCode.Double || type == ExCode.Float
					? (long)num.Double : num.Long;
			var it = this;
			if (it.desc.Convert(ref it, Descriptor.Long))
				return it.num.Long;
			throw new InvalidOperation("Could not convert {0} to long", this);
		}
		public ulong ToULong()
		{
			var type = desc.Primitive;
			if (type == ExCode.ULong)
				return num.ULong;
			if (type.Kind() == OpKind.Number)
				return type == ExCode.Double || type == ExCode.Float
					? (ulong)num.Double : num.ULong;
			var it = this;
			if (it.desc.Convert(ref it, Descriptor.ULong))
				return it.num.ULong;
			throw new InvalidOperation("Could not convert {0} to ulong", this);
		}
		public bool ToBool()
		{
			var type = desc.Primitive;
			if (type == ExCode.Bool)
				return num.Long != 0;
			if (type.Kind() == OpKind.Number)
				return type == ExCode.Double || type == ExCode.Float
					? !double.IsNaN(num.Double) && num.Double != 0.0 : num.Long != 0;
			var it = this;
			if (it.desc.Convert(ref it, Descriptor.Bool))
				return it.num.Long != 0;
			throw new InvalidOperation("Could not convert {0} to boolean", this);
		}
		public char ToChar()
		{
			var type = desc.Primitive;
			if (type == ExCode.WideChar)
				return num.Char;
			var it = this;
			if (it.desc.Convert(ref it, Descriptor.Char))
				return it.num.Char;
			throw new InvalidOperation("Could not convert {0} to char", this);
		}

		public T ToType<T>() => (T)ToType(typeof(T));
		public object ToType(Type type)
		{
			if (type == typeof(Value))
				return this;
			if (type.IsPrimitive)
			{
				if (type == typeof(int))
					return ToInt();
				if (type == typeof(string))
					return ToStr();
				if (type == typeof(double))
					return ToDouble();
				if (type == typeof(float))
					return (float)ToDouble();
				if (type == typeof(uint))
					return ToUInt();
				if (type == typeof(long))
					return ToLong();
				if (type == typeof(ulong))
					return ToULong();
				if (type == typeof(bool))
					return ToBool();
				if (type == typeof(char))
					return ToChar();
				if (type == typeof(byte))
					return (byte)ToUInt();
				if (type == typeof(sbyte))
					return (sbyte)ToInt();
				if (type == typeof(short))
					return (short)ToInt();
				if (type == typeof(ushort))
					return (ushort)ToUInt();
			}
			var it = obj;
			if (it != null && !type.IsAssignableFrom(it.GetType()))
			{
				var tmp = this;
				desc.Convert(ref tmp, Descriptor.Of(type));
				it = tmp.obj;
				if (type.IsSubclassOf(typeof(Delegate)) && it.GetType() != type)
					it = Delegate.CreateDelegate(type, it, "Invoke");
			}
			return it;
		}

		/// <summary>
		/// Numeric data
		/// </summary>
		[StructLayout(LayoutKind.Explicit)]
		[DebuggerDisplay("{Double}|{Long}|{HighInt}|{Int}")]
		public struct NumericData
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
		private string DebugString
		{
			get
			{
				if (desc == null)
					return obj == null ? "null" : obj.ToString();
				if (IsReference)
				{
					if (ReferenceEquals(idx, IntIndex))
						return string.Format(Culture, "{0}[{1}]", desc.Name, num.Int);
					if (idx is string name)
						return string.Format(Culture, "{0}.{1}", desc.Name, name);
				}
				return string.Format(Culture, "{0}: {1}",
					desc.Name, desc.ToString(ref this, null, Culture, true));
			}
		}
	}
}
