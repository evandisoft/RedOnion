using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Bee.Run
{
	public enum Vtype
	{
		/// <summary>
		/// undefined value
		/// </summary>
		[Alias("undefined")]
		Undef = 0x0000,
		/// <summary>
		/// engine object (ptr is iObject)
		/// </summary>
		Object = 0x0001,
		/// <summary>
		/// lazy-create object (ptr is create) - in ptr.ro only
		/// </summary>
		Create = 0x0002,
		/// <summary>
		/// property (ptr is iProp) - in ptr.ro only
		/// </summary>
		[Alias("property")]
		Prop = 0x0003,
		/// <summary>
		/// identifier / property reference (in str, ptr is iProps)
		/// </summary>
		[Alias("reference")]
		Ident = 0x0008,
		/// <summary>
		/// u8
		/// </summary>
		Byte = 0x0110,
		/// <summary>
		/// u16
		/// </summary>
		Ushort = 0x0211,
		/// <summary>
		/// u32
		/// </summary>
		Uint = 0x0412,
		/// <summary>
		/// u64
		/// </summary>
		Ulong = 0x0813,
		/// <summary>
		/// s8
		/// </summary>
		Sbyte = 0x4114,
		/// <summary>
		/// s16
		/// </summary>
		Short = 0x4215,
		/// <summary>
		/// s32
		/// </summary>
		Int = 0x4416,
		/// <summary>
		/// s64
		/// </summary>
		Long = 0x4817,
		/// <summary>
		/// f32
		/// </summary>
		Float = 0xC418,
		/// <summary>
		/// f64
		/// </summary>
		Double = 0xC819,
		/// <summary>
		/// u1
		/// </summary>
		Bool = 0x011B,
		/// <summary>
		/// string
		/// </summary>
		String = 0x0029,
		/// <summary>
		/// char
		/// </summary>
		Char = 0x023A,
		/// <summary>
		/// is string or char
		/// </summary>
		FStr = 0x0020,
		/// <summary>
		/// is number
		/// </summary>
		FNum = 0x0010,
		/// <summary>
		/// is 64bit or more
		/// </summary>
		F64 = 0x1800,
		/// <summary>
		/// floating point
		/// </summary>
		FFp = 0x8000,
		/// <summary>
		/// signed
		/// </summary>
		FSig = 0x4000,
		/// <summary>
		/// number size mask
		/// </summary>
		MSz = 0x3F00,
	}//Vtype
	
	[StructLayout(LayoutKind.Explicit)]
	internal struct Vdata
	{
		[FieldOffset(0)]
		public long Long;
		[FieldOffset(0)]
		public double Double;
		public bool Bool
		{
			get
			{
				return this.Long != 0;
			}
		}//Bool
		
		public char Char
		{
			get
			{
				return ((char)this.Long);
			}
		}//Char
		
		public byte Byte
		{
			get
			{
				return unchecked((byte)this.Long);
			}
		}//Byte
		
		public ushort Ushort
		{
			get
			{
				return ((ushort)this.Long);
			}
		}//Ushort
		
		public uint Uint
		{
			get
			{
				return ((uint)this.Long);
			}
		}//Uint
		
		public ulong Ulong
		{
			get
			{
				return ((ulong)this.Long);
			}
		}//Ulong
		
		public sbyte Sbyte
		{
			get
			{
				return ((sbyte)this.Long);
			}
		}//Sbyte
		
		public short Short
		{
			get
			{
				return ((short)this.Long);
			}
		}//Short
		
		public int Int
		{
			get
			{
				return ((int)this.Long);
			}
		}//Int
		
		public float Float
		{
			get
			{
				return ((float)this.Double);
			}
		}//Float
	}//Vdata
	
	[DebuggerDisplay("{Vt}; Ptr: {Ptr}; Str: {Str}; Long: {Dta.Long}; Double: {Dta.Double}")]
	public partial struct Value
	{
		private Vtype Vt;
		internal Object Ptr;
		internal string Str;
		internal Vdata Dta;
		internal Value( Vtype vtype )
		{
			Vt = vtype;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
		}//.ctor
		
		internal Value( Vtype vtype, Object @object )
		{
			Vt = vtype;
			Ptr = @object;
			Str = null;
			Dta = new Vdata();
		}//.ctor
		
		internal Value( Vtype vtype, Object @object, string @string )
		{
			Vt = vtype;
			Ptr = @object;
			Str = @string;
			Dta = new Vdata();
		}//.ctor
		
		internal Value( Vtype vtype, long @long )
		{
			Vt = vtype;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Long = @long;
		}//.ctor
		
		internal Value( Vtype vtype, double @double )
		{
			Vt = vtype;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Double = @double;
		}//.ctor
		
		public Value( Value value )
		{
			Vt = value.Vt;
			Ptr = value.Ptr;
			Str = value.Str;
			Dta = value.Dta;
		}//.ctor
		
		public Value( IObject obj )
		{
			Vt = Vtype.Object;
			Ptr = obj;
			Str = null;
			Dta = new Vdata();
		}//.ctor
		
		public Value( IProp prop )
		{
			Vt = Vtype.Prop;
			Ptr = prop;
			Str = null;
			Dta = new Vdata();
		}//.ctor
		
		public Value( IProps obj, string @string )
		{
			Vt = Vtype.Ident;
			Ptr = obj;
			Str = @string;
			Dta = new Vdata();
		}//.ctor
		
		public static implicit operator Value( string @string )
		{
			return new Value( @string );
		}//value
		
		public Value( string @string )
		{
			Vt = Vtype.String;
			Ptr = null;
			Str = @string;
			Dta = new Vdata();
		}//.ctor
		
		public static implicit operator Value( char @char )
		{
			return new Value( @char );
		}//value
		
		public Value( char @char )
		{
			Vt = Vtype.Char;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Long = @char;
		}//.ctor
		
		public static implicit operator Value( bool @bool )
		{
			return new Value( @bool );
		}//value
		
		public Value( bool @bool )
		{
			Vt = Vtype.Bool;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Long = @bool ? 1 : 0;
		}//.ctor
		
		public static implicit operator Value( byte @byte )
		{
			return new Value( @byte );
		}//value
		
		public Value( byte @byte )
		{
			Vt = Vtype.Byte;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Long = @byte;
		}//.ctor
		
		public static implicit operator Value( ushort @ushort )
		{
			return new Value( @ushort );
		}//value
		
		public Value( ushort @ushort )
		{
			Vt = Vtype.Ushort;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Long = @ushort;
		}//.ctor
		
		public static implicit operator Value( uint @uint )
		{
			return new Value( @uint );
		}//value
		
		public Value( uint @uint )
		{
			Vt = Vtype.Uint;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Long = @uint;
		}//.ctor
		
		public static implicit operator Value( ulong @ulong )
		{
			return new Value( @ulong );
		}//value
		
		public Value( ulong @ulong )
		{
			Vt = Vtype.Ulong;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Long = ((long)@ulong);
		}//.ctor
		
		public static implicit operator Value( sbyte @sbyte )
		{
			return new Value( @sbyte );
		}//value
		
		public Value( sbyte @sbyte )
		{
			Vt = Vtype.Sbyte;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Long = @sbyte;
		}//.ctor
		
		public static implicit operator Value( short @short )
		{
			return new Value( @short );
		}//value
		
		public Value( short @short )
		{
			Vt = Vtype.Short;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Long = @short;
		}//.ctor
		
		public static implicit operator Value( int @int )
		{
			return new Value( @int );
		}//value
		
		public Value( int @int )
		{
			Vt = Vtype.Int;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Long = @int;
		}//.ctor
		
		public static implicit operator Value( long @long )
		{
			return new Value( @long );
		}//value
		
		public Value( long @long )
		{
			Vt = Vtype.Long;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Long = @long;
		}//.ctor
		
		public static implicit operator Value( float @float )
		{
			return new Value( @float );
		}//value
		
		public Value( float @float )
		{
			Vt = Vtype.Float;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Double = @float;
		}//.ctor
		
		public static implicit operator Value( double @double )
		{
			return new Value( @double );
		}//value
		
		public Value( double @double )
		{
			Vt = Vtype.Double;
			Ptr = null;
			Str = null;
			Dta = new Vdata();
			Dta.Double = @double;
		}//.ctor
	}//Value
}//Bee.Run
