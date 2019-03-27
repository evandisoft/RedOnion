using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RedOnion.Script
{
	public partial struct Value
	{
		/// <summary>
		/// Get native object (string, int, ...)
		/// </summary>
		public object Native
		{
			get
			{
				if ((kind & ValueKind.fEnum) != 0)
					return Enum.ToObject((Type)idx, Long);
				switch (Kind)
				{
				default:
					return null;
				case ValueKind.Object:
					if (ptr == null)
						return null;
					var obj = (IObject)ptr;
					if ((obj.Features & ObjectFeatures.Proxy) != 0)
						return obj.Target;
					return obj.Value.Native;
				case ValueKind.Reference:
				case ValueKind.IndexRef:
					return RValue.Native;
				case ValueKind.String:
					return ptr;
				case ValueKind.Char:
					return data.Char;
				case ValueKind.Bool:
					return data.Bool;
				case ValueKind.Byte:
					return data.Byte;
				case ValueKind.UShort:
					return data.UShort;
				case ValueKind.UInt:
					return data.UInt;
				case ValueKind.ULong:
					return data.ULong;
				case ValueKind.SByte:
					return data.SByte;
				case ValueKind.Short:
					return data.Short;
				case ValueKind.Int:
					return data.Int;
				case ValueKind.Long:
					return data.Long;
				case ValueKind.Float:
					return data.Float;
				case ValueKind.Double:
					return data.Double;
				}
			}
		}
		internal Value AdjustForEnum(Value from)
		{
			if ((from.Kind & ValueKind.fEnum) != 0)
			{
				kind |= ValueKind.fEnum;
				idx = from.idx;
			}
			return this;
		}

		public bool IsReference
			=> Kind == ValueKind.Reference
			|| Kind == ValueKind.IndexRef;

		/// <summary>
		/// Get right-value (unassignable, dereferenced)
		/// </summary>
		public Value RValue
		{
			get
			{
				switch (Kind)
				{
				default:
					return this;
				case ValueKind.Reference:
					return ((IProperties)ptr).Get((string)idx);
				case ValueKind.IndexRef:
					return ((IObject)ptr).IndexGet((Value)idx);
				}
			}
		}
		/// <summary>
		/// Get referenced object (if object or reference; null otherwise).
		/// Returns the object whose property is referenced if reference.
		/// Consider using Object or Engine.Box instead.
		/// </summary>
		public IObject RefObj => IsReference || Kind == ValueKind.Object ?
			ptr as IObject : null;
		/// <summary>
		/// Get referenced object if it is object (null otherwise).
		/// Consider using Engine.Box if that was desired.
		/// </summary>
		public IObject Object => RValue.RefObj;
		/// <summary>
		/// Set the value for references
		/// </summary>
		public bool Set(Value value)
		{
			switch (Kind)
			{
			default:
				return false;
			case ValueKind.Reference:
				return ((IProperties)ptr).Set((string)idx, value.RValue);
			case ValueKind.IndexRef:
				return ((IObject)ptr).IndexSet((Value)idx, value.RValue);
			}
		}
		/// <summary>
		/// Modify the value (compound assignment)
		/// </summary>
		public bool Modify(OpCode op, Value value)
		{
			value = value.RValue;
			if (Kind == ValueKind.Reference)
			{
				if (ptr is IObject obj)
					return obj.Modify((string)idx, op, value);
				var tmp = this;
				tmp.Modify(op, value);
				return ((IProperties)ptr).Set((string)idx, tmp);
			}
			if (Kind == ValueKind.IndexRef)
				return ((IObject)ptr).IndexModify((Value)idx, op, value);
			switch (op)
			{
			case OpCode.Assign:
				this = value;
				return true;
			case OpCode.OrAssign:
				this = this | value;
				return true;
			case OpCode.XorAssign:
				this = this ^ value;
				return true;
			case OpCode.AndAssign:
				this = this & value;
				return true;
			case OpCode.LshAssign:
				this = ShiftLeft(value);
				return true;
			case OpCode.RshAssign:
				this = ShiftRight(value);
				return true;
			case OpCode.AddAssign:
				this = this + value;
				return true;
			case OpCode.SubAssign:
				this = this - value;
				return true;
			case OpCode.MulAssign:
				this = this * value;
				return true;
			case OpCode.DivAssign:
				this = this / value;
				return true;
			case OpCode.ModAssign:
				this = this % value;
				return true;
			}
			return false;
		}

		public bool IsProperty
			=> Kind == ValueKind.Property
			|| Kind == ValueKind.EasyProp;
		/// <summary>
		/// Get value of property
		/// </summary>
		public Value Get(IObject self)
		{
			switch (Kind)
			{
			default:
				if (!self.Engine.HasOption(EngineOption.Silent))
					throw new InvalidOperationException();
				return new Value();
			case ValueKind.Property:
				return ((IProperty)ptr).Get(self);
			case ValueKind.EasyProp:
				if (ptr == null)
				{
					if (!self.Engine.HasOption(EngineOption.Silent))
						throw new InvalidOperationException(Name + " is write only");
					return new Value();
				}
				return ((PropertyGetter)ptr)(self);
			}
		}
		/// <summary>
		/// Set value of property
		/// </summary>
		public bool Set(IObject self, Value value)
		{
			switch (Kind)
			{
			case ValueKind.Property:
				if (((IProperty)ptr).Set(self, value))
					return true;
				break;
			case ValueKind.EasyProp:
				if (idx == null)
					break;
				((PropertySetter)idx)(self, value);
				return true;
			}
			if (!self.Engine.HasOption(EngineOption.Silent))
				throw new InvalidOperationException(Name + " is read only");
			return false;
		}
		/// <summary>
		/// Modify value of property
		/// </summary>
		public bool Modify(IObject self, OpCode op, Value value)
		{
			switch (Kind)
			{
			default:
				if (!self.Engine.HasOption(EngineOption.Silent))
					throw new InvalidOperationException(Name + " is read only");
				return false;
			case ValueKind.Property:
				var prop = (IProperty)ptr;
				if (prop is IPropertyEx ex)
					return ex.Modify(self, op, value);
				var tmp = prop.Get(self);
				tmp.Modify(op, value);
				return prop.Set(self, tmp);
			case ValueKind.EasyProp:
				if (ptr == null)
				{
					if (!self.Engine.HasOption(EngineOption.Silent))
						throw new InvalidOperationException(Name + " is write only");
					return new Value();
				}
				if (idx == null)
				{
					if (!self.Engine.HasOption(EngineOption.Silent))
						throw new InvalidOperationException(Name + " is read only");
					return false;
				}
				var it = ((PropertyGetter)ptr)(self);
				it.Modify(op, value);
				((PropertySetter)idx)(self, it);
				return true;
			}
		}

		/// <summary>
		/// Helper for compound assignment operators and increment/decrement
		/// </summary>
		public Value Self
		{
			get => RValue;
			set => Set(value);
		}

		/// <summary>
		/// Convert to number (numeric value) if something else
		/// </summary>
		public Value Number
		{
			get
			{
				switch (Kind)
				{
				default:
					return new Value();
				case ValueKind.Object:
					return ptr == null ? new Value() : ((IObject)ptr).Value.Number;
				case ValueKind.Reference:
					return ((IProperties)ptr).Get((string)idx).Number;
				case ValueKind.IndexRef:
					return ((IObject)ptr).IndexGet((Value)idx).Number;
				case ValueKind.String:
					if ((string)ptr == "")
						return new Value();
					return Double;
				case ValueKind.Char:
					return UShort;
				case ValueKind.Bool:
				case ValueKind.Byte:
				case ValueKind.UShort:
				case ValueKind.UInt:
				case ValueKind.ULong:
				case ValueKind.SByte:
				case ValueKind.Short:
				case ValueKind.Int:
				case ValueKind.Long:
				case ValueKind.Float:
				case ValueKind.Double:
					return this;
				}
			}
		}

		/// <summary>
		/// Culture settings for formatting (invariant by default)
		/// </summary>
		public static CultureInfo Culture = CultureInfo.InvariantCulture;

		public static implicit operator string(Value value)
			=> value.ToString();
		public string String => ToString();
		public override string ToString()
		{
			switch (Kind)
			{
			default:
				return "undefined";
			case ValueKind.Object:
				if (ptr == null)
					return "null";
				var obj = (IObject)ptr;
				if (obj.HasFeature(ObjectFeatures.Proxy))
				{
					var it = obj.Target;
					return it == null ? "null" : it.ToString();
				}
				return obj.Value.String;
			case ValueKind.Reference:
				return ((IProperties)ptr).Get((string)idx).String;
			case ValueKind.IndexRef:
				return ((IObject)ptr).IndexGet((Value)idx).String;
			case ValueKind.String:
				return (string)ptr;
			case ValueKind.Char:
				return data.Char.ToString(Culture);
			case ValueKind.Bool:
				return data.Bool ? "true" : "false";
			case ValueKind.Byte:
				return data.Byte.ToString(Culture);
			case ValueKind.UShort:
				return data.UShort.ToString(Culture);
			case ValueKind.UInt:
				return data.UInt.ToString(Culture);
			case ValueKind.ULong:
				return data.ULong.ToString(Culture);
			case ValueKind.SByte:
				return data.SByte.ToString(Culture);
			case ValueKind.Short:
				return data.Short.ToString(Culture);
			case ValueKind.Int:
				return data.Int.ToString(Culture);
			case ValueKind.Long:
				return data.Long.ToString(Culture);
			case ValueKind.Float:
				return data.Float.ToString(Culture);
			case ValueKind.Double:
				return data.Double.ToString(Culture);
			}
		}

		public string Name
		{
			get
			{
				switch (Kind)
				{
				default:
					return String;
				case ValueKind.Undefined:
					return "undefined";
				case ValueKind.Object:
					return ptr == null ? "null" : ((IObject)ptr).Name;
				case ValueKind.Reference:
					var props = (IProperties)ptr;
					if (props is IObject obj)
						return obj.Name + "." + idx;
					return (ptr == null ? "null." : "unknown.") + idx;
				case ValueKind.IndexRef:
					return string.Format(Culture,
						"{0}[{1}]", ptr == null ? "null" : ((IObject)ptr).Name, idx);
				}
			}
		}

		public ValueKind Kind => kind;
		/// <summary>
		/// Is string or char
		/// </summary>
		public bool IsString => (Kind & ValueKind.fStr) != 0;
		/// <summary>
		/// Is number (primitive type - includes char and bool)
		/// </summary>
		public bool IsNumber => (Kind & ValueKind.fNum) != 0;
		/// <summary>
		/// Is enum (handled like primitive but converted to enum when converting to Native)
		/// </summary>
		public bool IsEnum => (Kind & ValueKind.fEnum) != 0;
		/// <summary>
		/// Is 64bit or more (long, ulong and double, more bits not supported yet)
		/// </summary>
		public bool Is64 => (Kind & ValueKind.f64) != 0;
		/// <summary>
		/// Number of bytes the number / primitive type uses (zero if not primitive type)
		/// </summary>
		public byte NumberSize => (byte)(((ushort)(Kind & ValueKind.mSz)) >> 8);
		/// <summary>
		/// Is signed number type (double, float, int, long, short or sbyte)
		/// </summary>
		public bool Signed => (Kind & ValueKind.fSig) != 0;
		/// <summary>
		/// Is foating point number (double or float)
		/// </summary>
		public bool IsFloatigPoint => (Kind & ValueKind.fFp) != 0;
		/// <summary>
		/// Is floating point number with not-a-number value
		/// </summary>
		public bool IsNaN => (Kind & ValueKind.fFp) != 0 && double.IsNaN(data.Double);

		public static implicit operator bool(Value value)
			=> value.Bool;
		public bool Bool => IsNumber ? IsFloatigPoint ?
			data.Double != 0 && !double.IsNaN(data.Double) :
			data.Long != 0 :
			Kind == ValueKind.Object ? ptr != null :
			Kind == ValueKind.String ? ptr != null && ((string)ptr).Length > 0 :
			Kind == ValueKind.Reference ? ((IProperties)ptr).Get((string)idx).Bool :
			Kind == ValueKind.IndexRef ? ((IObject)ptr).IndexGet((Value)idx).Bool :
			false;

		public static implicit operator char(Value value)
			=> value.Char;
		public char Char
		{
			get
			{
				if (Kind == ValueKind.String)
				{
					var s = ptr as string;
					return s == null || s.Length == 0 ? '\0' : s[0];
				}
				return IsNumber ? IsFloatigPoint ?
					(char)data.Double : (char)data.Long :
					Kind == ValueKind.Reference ? ((IProperties)ptr).Get((string)idx).Char :
					Kind == ValueKind.IndexRef ? ((IObject)ptr).IndexGet((Value)idx).Char :
					'\0';
			}
		}

		public static implicit operator double(Value value)
			=> value.Double;
		public double Double
		{
			get
			{
				if (Kind == ValueKind.String)
				{
					if (ptr != null && double.TryParse((string)ptr,
						NumberStyles.Float, Culture,
						out var v))
						return v;
					return double.NaN;
				}
				return IsNumber ? IsFloatigPoint ?
					data.Double : data.Long :
					Kind == ValueKind.Reference ? ((IProperties)ptr).Get((string)idx).Double :
					Kind == ValueKind.IndexRef ? ((IObject)ptr).IndexGet((Value)idx).Double :
					double.NaN;
			}
		}

		public static implicit operator long(Value value)
			=> value.Long;
		public long Long
		{
			get
			{
				if (Kind == ValueKind.String)
				{
					if (ptr != null && long.TryParse((string)ptr,
						NumberStyles.Number, Culture,
						out var v))
						return v;
					return 0;
				}
				return IsNumber ? IsFloatigPoint ?
					(long)data.Double : data.Long :
					Kind == ValueKind.Reference ? ((IProperties)ptr).Get((string)idx).Long :
					Kind == ValueKind.IndexRef ? ((IObject)ptr).IndexGet((Value)idx).Long :
					0;
			}
		}

		public static implicit operator ulong(Value value)
			=> value.ULong;
		public ulong ULong
		{
			get
			{
				if (Kind == ValueKind.String)
				{
					if (ptr != null && ulong.TryParse((string)ptr,
						NumberStyles.Number, Culture,
						out var v))
						return v;
					return 0;
				}
				return IsNumber ? IsFloatigPoint ?
					(ulong)data.Double : (ulong)data.Long :
					Kind == ValueKind.Reference ? ((IProperties)ptr).Get((string)idx).ULong :
					Kind == ValueKind.IndexRef ? ((IObject)ptr).IndexGet((Value)idx).ULong :
					0;
			}
		}

		public float	Float	=> (float)	Double;
		public int		Int		=> (int)	Long;
		public uint		UInt	=> (uint)	ULong;
		public short	Short	=> (short)	Long;
		public ushort	UShort	=> (ushort)	ULong;
		public sbyte	SByte	=> (sbyte)	Long;
		public byte		Byte	=> (byte)	ULong;

		public static Value FromPrimitive(object value)
		{
			if (value == null)
				return new Value((IObject)null);
			if (value is string || value is StringBuilder)
				return value.ToString();
			if (value is Enum)
			{
				var type = value.GetType();
				var result = Value.FromPrimitive(
					Convert.ChangeType(value,
					Enum.GetUnderlyingType(type)));
				result.kind |= ValueKind.fEnum;
				result.idx = type;
				return result;
			}
			if (value is bool bval)
				return bval;
			if (value is int ival)
				return ival;
			if (value is float fval)
				return fval;
			if (value is double dval)
				return dval;
			if (value is uint uval)
				return uval;
			if (value is long i64)
				return i64;
			if (value is ulong u64)
				return u64;
			if (value is short i16)
				return i16;
			if (value is ushort u16)
				return u16;
			if (value is byte u8)
				return u8;
			if (value is sbyte i8)
				return i8;
			if (value is char c)
				return c;
			return new Value();
		}

		public ICallable ToCallable()
		{
			var obj = RValue.Object;
			if (obj == null || !obj.HasFeature(ObjectFeatures.Function))
				throw new InvalidOperationException(Name + " is not a function");
			return obj;
		}
	}
}
