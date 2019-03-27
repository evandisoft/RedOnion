using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

// TODO: Support for enums (store the type in `ptr`)

namespace RedOnion.Script
{
	/// <summary>
	/// Method of delayed creation of objects (in BaseProps)
	/// </summary>
	/// <param name="engine">The engine to associate the object with</param>
	/// <returns>The object</returns>
	public delegate IObject CreateObject(IEngine engine);
	/// <summary>
	/// Getter for ValueKind.EasyProp
	/// </summary>
	/// <param name="self">The object the property belongs to</param>
	/// <returns>The value of the property</returns>
	public delegate Value PropertyGetter(IObject self);
	/// <summary>
	/// Setter for ValueKind.EasyProp
	/// </summary>
	/// <param name="self">The object the property belongs to</param>
	/// <returns>New value of the property</returns>
	public delegate void PropertySetter(IObject self, Value value);
	/// <summary>
	/// Getter for ValueKind.EasyProp (generic form)
	/// </summary>
	/// <param name="self">The object the property belongs to</param>
	/// <returns>The value of the property</returns>
	public delegate Value PropertyGetter<Obj>(Obj self) where Obj : IObject;
	/// <summary>
	/// Setter for ValueKind.EasyProp (generic form)
	/// </summary>
	/// <param name="self">The object the property belongs to</param>
	/// <returns>New value of the property</returns>
	public delegate void PropertySetter<Obj>(Obj self, Value value) where Obj : IObject;

	//todo: try matching this with System.TypeCode
	public enum ValueKind : ushort
	{
		/// <summary>
		/// Undefined value
		/// </summary>
		Undefined	= 0x0000,
		/// <summary>
		/// Standard object (ptr is IObject)
		/// </summary>
		Object		= 0x0001,
		/// <summary>
		/// Object with delayed creation (ptr is CreateObject)
		/// </summary>
		Create		= 0x0002,
		/// <summary>
		/// Standard property (ptr is IProperty, maybe IPropertyEx)
		/// </summary>
		Property	= 0x0003,
		/// <summary>
		/// Alternative property implementation using ptr=PropertyGetter, idx=PropertySetter
		/// </summary>
		EasyProp	= 0x0004,
		/// <summary>
		/// Identifier / property reference (name in idx, ptr is IProperties, usually IObject)
		/// </summary>
		Reference	= 0x0008,
		/// <summary>
		/// Reference to indexed value (ptr is IObject, idx is Value)
		/// </summary>
		IndexRef	= 0x0009,

		Byte		= 0x0140,// 8 bit unsigned
		UShort		= 0x0241,// 16 bit unsigned
		UInt		= 0x0442,// 32 bit unsigned
		ULong		= 0x0843,// 64 bit unsigned
		SByte		= 0x4144,// 8 bit signed
		Short		= 0x4245,// s16
		Int			= 0x4446,// s32
		Long		= 0x4847,// s64
		Float		= 0xC448,// f32
		Double		= 0xC849,// f64
		Bool		= 0x014B,// u1
		Char		= 0x026C,// char (marked as both number and string)
		String		= 0x002A,// string

		fStr		= 0x0020,// is string or char
		fNum		= 0x0040,// is number (primitive type - includes char and bool)
		fEnum		= 0x0080,// is enum (must also be marked as number), ptr is Type
		f64			= 0x1800,// is 64bit or more
		fFp			= 0x8000,// floating point
		fSig		= 0x4000,// signed
		mSz			= 0x3F00,// number size mask
	}

	[Flags]
	public enum ValueFlags : ushort
	{
		None		= 0,
		/// <summary>
		/// Cannot change type (ValueKind) even in writable properties
		/// </summary>
		StrongType	= 0x0001,
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct ValueData
	{
		[FieldOffset(0)]
		public long Long;
		[FieldOffset(0)]
		public double Double;

		public bool		Bool	=> Long != 0;
		public char		Char	=> (char)Long;
		public byte		Byte	=> (byte)Long;
		public ushort	UShort	=> (ushort)Long;
		public uint		UInt	=> (uint)Long;
		public ulong	ULong	=> (ulong)Long;
		public sbyte	SByte	=> (sbyte)Long;
		public short	Short	=> (short)Long;
		public int		Int		=> (int)Long;
		public float	Float	=> (float)Double;
	}

	[DebuggerDisplay("{kind}; ptr: {ptr}; idx: {idx}; long: {data.Long}; double: {data.Double}")]
	public partial struct Value
	{
		internal ValueKind kind;
		internal ValueFlags flag;
		internal object ptr;
		internal object idx;
		internal ValueData data;

		internal Value(ValueKind vtype)
		{
			kind = vtype;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
		}

		internal Value(ValueKind vtype, object value)
		{
			kind = vtype;
			flag = 0;
			ptr = value;
			idx = null;
			data = new ValueData();
		}

		internal Value(ValueKind vtype, object obj, object name)
		{
			kind = vtype;
			flag = 0;
			ptr = obj;
			idx = name;
			data = new ValueData();
		}

		internal Value(ValueKind vtype, long value)
		{
			kind = vtype;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Long = value;
		}

		internal Value(ValueKind vtype, double value)
		{
			kind = vtype;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Double = value;
		}

		public Value(Value value)
		{
			kind = value.Kind;
			flag = 0;
			ptr = value.ptr;
			idx = value.idx;
			data = value.data;
		}

		public Value(IProperty prop)
		{
			kind = ValueKind.Property;
			flag = 0;
			ptr = prop;
			idx = null;
			data = new ValueData();
		}
		public Value(PropertyGetter getter, PropertySetter setter)
		{
			kind = ValueKind.EasyProp;
			flag = 0;
			ptr = getter;
			idx = setter;
			data = new ValueData();
		}
		public static Value Property<Obj>(
			PropertyGetter<Obj> getter,
			PropertySetter<Obj> setter)
			where Obj : IObject
			=> new Value(getter == null ? (PropertyGetter)null : obj => getter((Obj)obj),
				setter == null ? (PropertySetter)null : (obj, value) => setter((Obj)obj, value));

		public static Value ReadOnly(PropertyGetter getter)
			=> new Value(getter, (PropertySetter)null);
		public static Value ReadOnly<Obj>(
			PropertyGetter<Obj> getter)
			where Obj : IObject
			=> new Value(obj => getter((Obj)obj), (PropertySetter)null);
		public static Value WriteOnly(PropertySetter setter)
			=> new Value((PropertyGetter)null, setter);
		public static Value WriteOnly<Obj>(
			PropertySetter<Obj> setter)
			where Obj : IObject
			=> new Value((PropertyGetter)null, (obj, value) => setter((Obj)obj, value));

		public Value(IProperties obj, string name)
		{
			kind = ValueKind.Reference;
			flag = 0;
			ptr = obj;
			idx = name;
			data = new ValueData();
		}
		public static Value IndexRef(IObject obj, Value index)
			=> new Value(ValueKind.IndexRef, obj, index);

		public static implicit operator Value(BasicObjects.BasicObject obj)
			=> Value.FromObject(obj);
		public static implicit operator Value(BasicObjects.SimpleObject obj)
			=> Value.FromObject(obj);
		public static Value FromObject(IObject obj)
			=> new Value(obj);
		public Value(IObject obj)
		{
			kind = ValueKind.Object;
			flag = 0;
			ptr = obj;
			idx = null;
			data = new ValueData();
		}
		public Value(BasicObjects.BasicObject obj)
		{
			kind = ValueKind.Object;
			flag = 0;
			ptr = obj;
			idx = null;
			data = new ValueData();
		}
		public Value(BasicObjects.SimpleObject obj)
		{
			kind = ValueKind.Object;
			flag = 0;
			ptr = obj;
			idx = null;
			data = new ValueData();
		}

		public static implicit operator Value(CreateObject create)
			=> new Value(create);
		public Value(CreateObject create)
		{
			kind = ValueKind.Create;
			flag = 0;
			ptr = create;
			idx = null;
			data = new ValueData();
		}

		public static implicit operator Value(string value)
			=> new Value(value);
		public Value(string value)
		{
			kind = ValueKind.String;
			flag = 0;
			ptr = value;
			idx = null;
			data = new ValueData();
		}

		public static implicit operator Value(char value)
			=> new Value(value);
		public Value(char value)
		{
			kind = ValueKind.Char;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(bool value)
			=> new Value(value);
		public Value(bool value)
		{
			kind = ValueKind.Bool;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Long = value ? 1 : 0;
		}

		public static implicit operator Value(byte value)
			=> new Value(value);
		public Value(byte value)
		{
			kind = ValueKind.Byte;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(ushort value)
		{
			return new Value(value);
		}

		public Value(ushort value)
		{
			kind = ValueKind.UShort;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(uint value)
		{
			return new Value(value);
		}

		public Value(uint value)
		{
			kind = ValueKind.UInt;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(ulong value)
		{
			return new Value(value);
		}

		public Value(ulong value)
		{
			kind = ValueKind.ULong;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Long = (long)value;
		}

		public static implicit operator Value(sbyte value)
		{
			return new Value(value);
		}

		public Value(sbyte value)
		{
			kind = ValueKind.SByte;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(short value)
		{
			return new Value(value);
		}

		public Value(short value)
		{
			kind = ValueKind.Short;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(int value)
		{
			return new Value(value);
		}

		public Value(int value)
		{
			kind = ValueKind.Int;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(long value)
		{
			return new Value(value);
		}

		public Value(long value)
		{
			kind = ValueKind.Long;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Long = value;
		}

		public static implicit operator Value(float value)
		{
			return new Value(value);
		}

		public Value(float value)
		{
			kind = ValueKind.Float;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Double = value;
		}

		public static implicit operator Value(double value)
		{
			return new Value(value);
		}

		public Value(double value)
		{
			kind = ValueKind.Double;
			flag = 0;
			ptr = null;
			idx = null;
			data = new ValueData();
			data.Double = value;
		}

		public static Value Method<Obj>(Action<Obj> action) where Obj: IObject
			=> new Value(engine => new BasicObjects.SimpleMethod0<Obj>(engine, action));
		public static Value Method<Obj>(Action<Obj, Value> action) where Obj : IObject
			=> new Value(engine => new BasicObjects.SimpleMethod1<Obj>(engine, action));
		public static Value Method<Obj>(Action<Obj, Value, Value> action) where Obj : IObject
			=> new Value(engine => new BasicObjects.SimpleMethod2<Obj>(engine, action));

		public static Value Func<Obj>(Func<Obj, Value> func) where Obj : IObject
			=> new Value(engine => new BasicObjects.SimpleFunction0<Obj>(engine, func));
		public static Value Func<Obj>(Func<Obj, Value, Value> func) where Obj : IObject
			=> new Value(engine => new BasicObjects.SimpleFunction1<Obj>(engine, func));
		public static Value Func<Obj>(Func<Obj, Value, Value, Value> func) where Obj : IObject
			=> new Value(engine => new BasicObjects.SimpleFunction2<Obj>(engine, func));
	}
}
