using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Bee.Run
{
	public partial struct Value
	{
		/// <summary>
		/// get native object (string, int, ...)
		/// </summary>
		public Object Native
		{
			get
			{
				switch( Vt )
				{
				default:
					return null;
				case Vtype.Object:
					return Ptr == null ? null : ((IObject)Ptr).Value.Native;
				case Vtype.Ident:
					return ((IProps)Ptr).Get( Str ).Native;
				case Vtype.String:
					return Str;
				case Vtype.Char:
					return Dta.Char;
				case Vtype.Bool:
					return Dta.Bool;
				case Vtype.Byte:
					return Dta.Byte;
				case Vtype.Ushort:
					return Dta.Ushort;
				case Vtype.Uint:
					return Dta.Uint;
				case Vtype.Ulong:
					return Dta.Ulong;
				case Vtype.Sbyte:
					return Dta.Sbyte;
				case Vtype.Short:
					return Dta.Short;
				case Vtype.Int:
					return Dta.Int;
				case Vtype.Long:
					return Dta.Long;
				case Vtype.Float:
					return Dta.Float;
				case Vtype.Double:
					return Dta.Double;
				}
			}
		}//Native
		
		/// <summary>
		/// get right-value (unassignable, dereferenced)
		/// </summary>
		public Value Rvalue
		{
			get
			{
				return Vt == Vtype.Ident ? ((IProps)Ptr).Get( Str ) : this;
			}
		}//Rvalue
		
		/// <summary>
		/// get referenced object (if object or reference; null otherwise)
		/// </summary>
		public IObject Refobj
		{
			get
			{
				return Vt == Vtype.Ident ? Ptr as IObject : Vt == Vtype.Object ? ((IObject)Ptr) : null;
			}
		}//Refobj
		
		/// <summary>
		/// set the value for references
		/// </summary>
		public void Set( Value value )
		{
			if( Vt == Vtype.Ident )
			{
				((IProps)Ptr).Set( Str, value.Vt == Vtype.Ident ? ((IProps)value.Ptr).Get( value.Str ) : value );
			}
		}//Set
		
		/// <summary>
		/// helper for compound assignment operators and increment/decrement
		/// (gets @rvalue, sets value if reference - uses @set method)
		/// </summary>
		public Value Self
		{
			get
			{
				return Rvalue;
			}
			set
			{
				Set( value );
			}
		}//Self
		
		/// <summary>
		/// convert to number (numeric value) if something else
		/// @note undefined (vtype.undef) is also valid result (e.g. for empty string)
		/// </summary>
		public Value Number
		{
			get
			{
				switch( Vt )
				{
				default:
					return new Value();
				case Vtype.Object:
					return Ptr == null ? new Value() : ((IObject)Ptr).Value.Number;
				case Vtype.Ident:
					return ((IProps)Ptr).Get( Str ).Number;
				case Vtype.String:
					if( Str == "" )
					{
						return new Value();
					}
					return this.Double;
				case Vtype.Char:
					return this.Ushort;
				case Vtype.Bool:
					return this.Byte;
				case Vtype.Byte:
				case Vtype.Ushort:
				case Vtype.Uint:
				case Vtype.Ulong:
				case Vtype.Sbyte:
				case Vtype.Short:
				case Vtype.Int:
				case Vtype.Long:
				case Vtype.Float:
				case Vtype.Double:
					return this;
				}
			}
		}//Number
		
		public static CultureInfo Culture = CultureInfo.InvariantCulture;
		public static implicit operator string( Value value )
		{
			return value.ToString();
		}//string
		
		public String String
		{
			get
			{
				return ToString();
			}
		}//String
		
		public override string ToString(  )
		{
			switch( Vt )
			{
			default:
				return "undefined";
			case Vtype.Object:
				return Ptr == null ? "null" : ((IObject)Ptr).Value.String;
			case Vtype.Ident:
				return ((IProps)Ptr).Get( Str ).String;
			case Vtype.String:
				return Str;
			case Vtype.Char:
				return Dta.Char.ToString( Culture );
			case Vtype.Bool:
				return Dta.Bool ? "true" : "false";
			case Vtype.Byte:
				return Dta.Byte.ToString( Culture );
			case Vtype.Ushort:
				return Dta.Ushort.ToString( Culture );
			case Vtype.Uint:
				return Dta.Uint.ToString( Culture );
			case Vtype.Ulong:
				return Dta.Ulong.ToString( Culture );
			case Vtype.Sbyte:
				return Dta.Sbyte.ToString( Culture );
			case Vtype.Short:
				return Dta.Short.ToString( Culture );
			case Vtype.Int:
				return Dta.Int.ToString( Culture );
			case Vtype.Long:
				return Dta.Long.ToString( Culture );
			case Vtype.Float:
				return Dta.Float.ToString( Culture );
			case Vtype.Double:
				return Dta.Double.ToString( Culture );
			}
		}//ToString
		
		public Vtype Type
		{
			get
			{
				return Vt;
			}
		}//Type
		
		public bool Isstr
		{
			get
			{
				return (Vt & Vtype.FNum) != 0;
			}
		}//Isstr
		
		public bool Isnum
		{
			get
			{
				return (Vt & Vtype.FNum) != 0;
			}
		}//Isnum
		
		public bool Is64
		{
			get
			{
				return (Vt & Vtype.F64) != 0;
			}
		}//Is64
		
		public byte Numsz
		{
			get
			{
				return unchecked((byte)(((ushort)(Vt & Vtype.MSz)) >> 8));
			}
		}//Numsz
		
		public bool Signed
		{
			get
			{
				return (Vt & Vtype.FSig) != 0;
			}
		}//Signed
		
		public bool Isfp
		{
			get
			{
				return (Vt & Vtype.FFp) != 0;
			}
		}//Isfp
		
		public bool Isnan
		{
			get
			{
				return ((Vt & Vtype.FFp) != 0) && System.Double.IsNaN( Dta.Double );
			}
		}//Isnan
		
		public static implicit operator bool( Value value )
		{
			return value.Bool;
		}//bool
		
		public bool Bool
		{
			get
			{
				return Isnum ? Isfp ? (Dta.Double != 0) && (!System.Double.IsNaN( Dta.Double )) : Dta.Long != 0 : Vt == Vtype.Object ? Ptr != null : Vt == Vtype.String ? (Ptr != null) && (((string)Ptr).Length > 0) : Vt == Vtype.Ident ? ((IProps)Ptr).Get( Str ).Bool : false;
			}
		}//Bool
		
		public static implicit operator char( Value value )
		{
			return value.Char;
		}//char
		
		public char Char
		{
			get
			{
				if( Vt == Vtype.String )
				{
					var s = Ptr as System.String;
					return (s == null) || (s.Length == 0) ? '\0' : s[0];
				}
				return Isnum ? Isfp ? ((char)Dta.Double) : ((char)Dta.Long) : Vt == Vtype.Ident ? ((IProps)Ptr).Get( Str ).Char : '\0';
			}
		}//Char
		
		public static implicit operator double( Value value )
		{
			return value.Double;
		}//double
		
		public double Double
		{
			get
			{
				if( Vt == Vtype.String )
				{
					double v;
					if( (Ptr != null) && double.TryParse( ((string)Ptr), NumberStyles.Float, CultureInfo.InvariantCulture, out v ) )
					{
						return v;
					}
					return System.Double.NaN;
				}
				return Isnum ? Isfp ? Dta.Double : Dta.Long : Vt == Vtype.Ident ? ((IProps)Ptr).Get( Str ).Double : System.Double.NaN;
			}
		}//Double
		
		public static implicit operator long( Value value )
		{
			return value.Long;
		}//long
		
		public long Long
		{
			get
			{
				if( Vt == Vtype.String )
				{
					long v;
					if( (Ptr != null) && long.TryParse( ((string)Ptr), NumberStyles.Number, CultureInfo.InvariantCulture, out v ) )
					{
						return v;
					}
					return 0;
				}
				return Isnum ? Isfp ? ((long)Dta.Double) : Dta.Long : Vt == Vtype.Ident ? ((IProps)Ptr).Get( Str ).Long : 0;
			}
		}//Long
		
		public static implicit operator ulong( Value value )
		{
			return value.Ulong;
		}//ulong
		
		public ulong Ulong
		{
			get
			{
				if( Vt == Vtype.String )
				{
					ulong v;
					if( (Ptr != null) && ulong.TryParse( ((string)Ptr), NumberStyles.Number, CultureInfo.InvariantCulture, out v ) )
					{
						return v;
					}
					return 0;
				}
				return Isnum ? Isfp ? ((ulong)Dta.Double) : ((ulong)Dta.Long) : Vt == Vtype.Ident ? ((IProps)Ptr).Get( Str ).Ulong : 0;
			}
		}//Ulong
		
		public float Float
		{
			get
			{
				return ((float)this.Double);
			}
		}//Float
		
		public int Int
		{
			get
			{
				return ((int)this.Long);
			}
		}//Int
		
		public uint Uint
		{
			get
			{
				return ((uint)this.Ulong);
			}
		}//Uint
		
		public short Short
		{
			get
			{
				return ((short)this.Long);
			}
		}//Short
		
		public ushort Ushort
		{
			get
			{
				return ((ushort)this.Ulong);
			}
		}//Ushort
		
		public sbyte Sbyte
		{
			get
			{
				return ((sbyte)this.Long);
			}
		}//Sbyte
		
		public byte Byte
		{
			get
			{
				return unchecked((byte)this.Ulong);
			}
		}//Byte
	}//Value
}//Bee.Run
